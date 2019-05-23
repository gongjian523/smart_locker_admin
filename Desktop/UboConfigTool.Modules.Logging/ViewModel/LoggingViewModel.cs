using MahApps.Metro.Controls;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UboConfigTool.Infrastructure;
using UboConfigTool.Infrastructure.Events;
using UboConfigTool.Modules.Logging.Model;
using UboConfigTool.Modules.Logging.Service;
using UboConfigTool.Modules.Logging.View;

namespace UboConfigTool.Modules.Logging.ViewModel
{
    public enum LogViewDisplayType
    {
        None = 0,
        DeviceLog = 1,
        SystemLog = 2
    }

    [Export( typeof( LoggingViewModel ) )]
    [PartCreationPolicy( CreationPolicy.Shared )]
    public class LoggingViewModel : NotificationObject, IPartImportsSatisfiedNotification
    {
        private static Uri deviceFilterViewUri = new Uri( "DeviceLogFilterView", UriKind.Relative );

        private readonly ILoggingService _loggingService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;

        [ImportingConstructor]
        public LoggingViewModel( ILoggingService loggingService, IEventAggregator eventAggregator, IRegionManager regionManager )
        {
            if( loggingService == null )
            {
                throw new ArgumentNullException( "loggingService" );
            }

            this._loggingService = loggingService;
            this._eventAggregator = eventAggregator;
            this._regionManager = regionManager;

            _deviceLogKeywordSearchCommand = new DelegateCommand<object>( DeviceLogKeywordSearchCommandExcute );
            _flyoutCommand = new DelegateCommand<object>( FlyoutOpenCloseCommand );
            _queryCommand = new DelegateCommand( QueryDeviceLog, CanQueryDeviceLog );
            _currentLogViewType = LogViewDisplayType.DeviceLog;

            this._loggingService.LogDataBeginLoadingEvent += _loggingService_LogDataBeginLoadingEvent;
            this._loggingService.LogDataEndLoadingEvent += _loggingService_LogDataEndLoadingEvent;

            this.LogDataInitializedCommand = new DelegateCommand( LogDataInitialized );
            _prePageCommand = new DelegateCommand( PrePageOfLogs, CanExcutePrePage );
            _nextPageCommand = new DelegateCommand( NextPageOfLogs, CanExcuteNextPage );
            _currentPageChangedCommand = new DelegateCommand( CurrentPageChanged );
        }

        private void CurrentPageChanged()
        {
            if( CurrentLogViewType == LogViewDisplayType.DeviceLog )
                _loggingService.SearchDeviceLogData( CurrentPageIndex, "", "", "", "" );
            else
                _loggingService.SearchSystemLogData( CurrentPageIndex, "", "", "", "" );
        }

        private bool CanExcuteNextPage()
        {
            return CurrentPageIndex < TotalPageNumber;
        }

        private void NextPageOfLogs()
        {
            CurrentPageIndex++;
            if( CurrentLogViewType == LogViewDisplayType.DeviceLog )
                _loggingService.SearchDeviceLogData( CurrentPageIndex, "", "", "", "", "", "" );
            else
                _loggingService.SearchSystemLogData( CurrentPageIndex, "", "", "", "" );

        }

        private bool CanExcutePrePage()
        {
            return CurrentPageIndex > 1;
        }

        private void PrePageOfLogs()
        {
            CurrentPageIndex--;

            if( CurrentLogViewType == LogViewDisplayType.DeviceLog )
                _loggingService.SearchDeviceLogData( CurrentPageIndex, "", "", "", "" );
            else
                _loggingService.SearchSystemLogData( CurrentPageIndex, "", "", "", "" );
        }

        public void LogDataInitialized()
        {
            CurrentPageIndex = 0;

            if( CurrentLogViewType == LogViewDisplayType.DeviceLog )
                this._loggingService.LoadDeviceLogData();
            else
                this._loggingService.LoadSystemLogData();
        }

        private void UpdatePagingData( LogDataParcel<DeviceLogItem> logdata )
        {
            DeviceLogs = new ObservableCollection<DeviceLogItem>( logdata.logs );
            if( DeviceLogs.Count > 0 && CurrentPageIndex == 0 )
                CurrentPageIndex = 1;

            TotalPageNumber = logdata.total_pages;

            PrePageCommand.RaiseCanExecuteChanged();
            NextPageCommand.RaiseCanExecuteChanged();
        }

        private void UpdatePagingSystemLogData( LogDataParcel<SystemLogItem> logdata )
        {
            SystemLogs = new ObservableCollection<SystemLogItem>( logdata.logs );
            if( SystemLogs.Count > 0 && CurrentPageIndex == 0 )
                CurrentPageIndex = 1;

            TotalPageNumber = logdata.total_pages;

            PrePageCommand.RaiseCanExecuteChanged();
            NextPageCommand.RaiseCanExecuteChanged();
        }

        private void _loggingService_LogDataEndLoadingEvent()
        {
            if( CurrentLogViewType == LogViewDisplayType.DeviceLog )
                UpdatePagingData( _loggingService.DeviceLogs );
            else
                UpdatePagingSystemLogData( _loggingService.SystemLogs );

            InDataLoading = false;
        }

        private void _loggingService_LogDataBeginLoadingEvent()
        {
            InDataLoading = true;
        }

        private void DeviceLogKeywordSearchCommandExcute( object keyword )
        {
            string strFromDate =  FromDate.HasValue ? string.Format( "{0:yyyy-MM-dd}", FromDate ) : "";
            string strToDate =  ToDate.HasValue ? string.Format( "{0:yyyy-MM-dd}", ToDate ) : "";

            if( CurrentLogViewType == LogViewDisplayType.DeviceLog )
                _loggingService.SearchDeviceLogData( 1, strFromDate, strToDate, keyword.ToString(), "" );
            else
                _loggingService.SearchSystemLogData( 1, strFromDate, strToDate, keyword.ToString(), "" );

            if( TotalPageNumber > 0 )
                CurrentPageIndex = 1;
            else
                CurrentPageIndex = 0;
        }

        private void FlyoutOpenCloseCommand( object keyword )
        {
            foreach( Control controlItem in ( Application.Current.MainWindow as MetroWindow ).Flyouts.Items )
            {
                if( controlItem is DeviceLogFilterView && CurrentLogViewType == LogViewDisplayType.DeviceLog )
                {
                    ( controlItem as DeviceLogFilterView ).IsOpen = !( controlItem as DeviceLogFilterView ).IsOpen;
                }
                else if( controlItem is SystemLogFilterView && CurrentLogViewType == LogViewDisplayType.SystemLog )
                {
                    ( controlItem as SystemLogFilterView ).IsOpen = !( controlItem as SystemLogFilterView ).IsOpen;
                }
                else
                {
                    ( controlItem as Flyout ).IsOpen = false;
                }
            }
        }

        private bool CanQueryDeviceLog()
        {
            return true;
        }

        private void QueryDeviceLog()
        {
            string strFromDate =  FromDate.HasValue ? string.Format( "{0:yyyy-MM-dd}", FromDate ) : "";
            string strToDate =  ToDate.HasValue ? string.Format( "{0:yyyy-MM-dd}", ToDate ) : "";

            _loggingService.SearchDeviceLogData( 1, strFromDate, strToDate, "", "" );

            if( TotalPageNumber > 0 )
                CurrentPageIndex = 1;
            else
                CurrentPageIndex = 0;

            FlyoutOpenCloseCommand( null );
        }

        private ObservableCollection<DeviceLogItem> _deviceLogs;
        public ObservableCollection<DeviceLogItem> DeviceLogs
        {
            get
            {
                return _deviceLogs;
            }
            private set
            {
                if( value != null )
                {
                    this._deviceLogs = value;
                    this.RaisePropertyChanged( () => this.DeviceLogs );
                }
            }
        }

        private ObservableCollection<SystemLogItem> _systemLogs;
        public ObservableCollection<SystemLogItem> SystemLogs
        {
            get
            {
                return _systemLogs;
            }
            private set
            {
                if( value != null )
                {
                    this._systemLogs = value;
                    this.RaisePropertyChanged( () => SystemLogs );
                }
            }
        }

        private bool _inDataLoading = false;
        public bool InDataLoading
        {
            get
            {
                return _inDataLoading;
            }
            private set
            {
                this._inDataLoading = value;
                this.RaisePropertyChanged( () => InDataLoading );
            }
        }

        public ICommand LogDataInitializedCommand
        {
            get;
            private set;
        }

        private ICommand  _deviceLogKeywordSearchCommand;
        public ICommand DeviceLogKeywordSearchCommand
        {
            get
            {
                return _deviceLogKeywordSearchCommand;
            }
        }

        private ICommand _flyoutCommand;
        public ICommand FlyoutCommand
        {
            get
            {
                return _flyoutCommand;
            }
        }

        private DateTime? _fromDate;
        public DateTime? FromDate
        {
            get
            {
                return _fromDate;
            }
            set
            {
                this._fromDate = value;
                this.RaisePropertyChanged( () => FromDate );
            }
        }

        private DateTime? _toDate;
        public DateTime? ToDate
        {
            get
            {
                return _toDate;
            }
            set
            {
                this._toDate = value;
                this.RaisePropertyChanged( () => ToDate );
            }
        }

        private int _currentPageIndex = 0;
        public int CurrentPageIndex
        {
            get
            {
                return _currentPageIndex;
            }
            set
            {
                if( this._currentPageIndex != value )
                {
                    this._currentPageIndex = value;
                    this.RaisePropertyChanged( () => CurrentPageIndex );
                }
            }
        }

        private int _totalPageNumber = 0;
        public int TotalPageNumber
        {
            get
            {
                return _totalPageNumber;
            }
            private set
            {
                this._totalPageNumber = value;
                this.RaisePropertyChanged( () => TotalPageNumber );
            }
        }

        private string _searchingKeyword = string.Empty;
        public string SearchingKeyword
        {
            get
            {
                return _searchingKeyword;
            }
            set
            {
                if( this._searchingKeyword != value )
                {
                    this._searchingKeyword = value;
                    this.RaisePropertyChanged( () => SearchingKeyword );
                }
            }
        }

        private DelegateCommand _currentPageChangedCommand;
        public DelegateCommand CurrentPageChangedCommand
        {
            get
            {
                return _currentPageChangedCommand;
            }
        }

        private DelegateCommand _prePageCommand;
        public DelegateCommand PrePageCommand
        {
            get
            {
                return _prePageCommand;
            }
        }

        private DelegateCommand _nextPageCommand;
        public DelegateCommand NextPageCommand
        {
            get
            {
                return _nextPageCommand;
            }
        }

        private ICommand _queryCommand;
        public ICommand QueryCommand
        {
            get
            {
                return _queryCommand;
            }
        }

        private LogViewDisplayType _currentLogViewType = LogViewDisplayType.None;
        public LogViewDisplayType CurrentLogViewType
        {
            get
            {
                return _currentLogViewType;
            }
            set
            {
                this._currentLogViewType = value;
                this.RaisePropertyChanged( () => CurrentLogViewType );
                CloseFlyout();
                LogDataInitialized();
            }
        }

        public void CloseFlyout()
        {
            foreach( Control controlItem in ( Application.Current.MainWindow as MetroWindow ).Flyouts.Items )
            {
                ( controlItem as Flyout ).IsOpen = false;
            }
        }

        public void OnImportsSatisfied()
        {
            IRegion mainContentRegion = this._regionManager.Regions[RegionNames.MainContentRegion];
            if( mainContentRegion != null && mainContentRegion.NavigationService != null )
            {
                mainContentRegion.NavigationService.Navigated += this.MainContentRegion_Navigated;
            }
        }

        private void MainContentRegion_Navigated( object sender, RegionNavigationEventArgs e )
        {
            Uri loggingViewUri = new Uri( "LoggingView", UriKind.Relative );
            if( e.Uri == loggingViewUri )
                return;

            foreach( Control controlItem in ( Application.Current.MainWindow as MetroWindow ).Flyouts.Items )
            {
                if( controlItem is DeviceLogFilterView || controlItem is SystemLogFilterView )
                {
                    ( controlItem as Flyout ).IsOpen = false;
                }
            }
        }
    }
}
