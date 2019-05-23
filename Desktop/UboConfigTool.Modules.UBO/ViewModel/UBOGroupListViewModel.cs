using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.ServiceLocation;
using RestSharp;
using RestSharp.Deserializers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using UboConfigTool.Controls;
using UboConfigTool.Infrastructure;
using UboConfigTool.Infrastructure.Events;
using UboConfigTool.Infrastructure.Interfaces;
using UboConfigTool.Infrastructure.Models;
using UboConfigTool.Infrastructure.Repository;

namespace UboConfigTool.Modules.UBO.ViewModel
{
    [Export( typeof( UBOGroupListViewModel ) )]
    [PartCreationPolicy( CreationPolicy.Shared )]
    public class UBOGroupListViewModel : NotificationObject
    {
        private readonly IUBODataService _UBOGrpRepository;
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventArregator;
        [ImportingConstructor]
        public UBOGroupListViewModel( IRegionManager regionManager, IUBODataService uboGrpRepository, IEventAggregator eventAgrregator )
        {
            if( regionManager == null )
            {
                throw new ArgumentNullException( "RegionManager" );
            }

            if( uboGrpRepository == null )
            {
                throw new ArgumentNullException( "UBOGroupRepository" );
            }

            if( eventAgrregator == null )
            {
                throw new ArgumentNullException( "EventArregator" );
            }

            _regionManager = regionManager;
            _UBOGrpRepository = uboGrpRepository;
            _eventArregator = eventAgrregator;

            _UBOGrpRepository.DataBeginLoadingEvent += _UBOGrpRepository_DataBeginLoadingEvent;
            uboGrpRepository.DataEndLoadingEvent += uboGrpRepository_DataEndLoadingEvent;

            _addUBOGroupCommand = new DelegateCommand( AddUBOGroup );
            _keywordSearchCommand = new DelegateCommand<object>( KeywordSearch );
            _uboGroupSelectionChangedCommand = new DelegateCommand<object>( UBOGroupSelectionChanged );

            InitData();
        }

        void uboGrpRepository_DataEndLoadingEvent()
        {
            Application.Current.Dispatcher.BeginInvoke( DispatcherPriority.Normal, new Action( () =>
            {
                IsDataInLoading = false;
                InitData();
            } ) );
        }

        void _UBOGrpRepository_DataBeginLoadingEvent()
        {
            Application.Current.Dispatcher.BeginInvoke( DispatcherPriority.Normal, new Action( () =>
            {
                ClearData();
                IsDataInLoading = true;
            } ) );
        }

        private void AddUBOGroup()
        {
            //TODO: via event to let shell to init the 
            //this._regionManager.RequestNavigate( RegionNames.SecondaryRegion, new Uri( "NewUserWizardView", UriKind.Relative ) );
            this._eventArregator.GetEvent<AddUBOGroupEvent>().Publish( true );
        }

        private ObservableCollection<UBOGroupViewModel> _uboGrpVMs = new ObservableCollection<UBOGroupViewModel>();
        public ObservableCollection<UBOGroupViewModel> UBOGroupVMs
        {
            get
            {
                return _uboGrpVMs;
            }
        }

        public UBOGroupViewModel CurrentUBOGroup
        {
            get;
            set;
        }

        public UBOViewModel CurrentEdittingUBO
        {
            get;
            set;
        }

        private readonly ICommand _addUBOGroupCommand;
        public ICommand AddUBOGroupCommand
        {
            get
            {
                return _addUBOGroupCommand;
            }
        }

        private readonly ICommand _keywordSearchCommand;
        public ICommand KeywordSearchCommand
        {
            get
            {
                return _keywordSearchCommand;
            }
        }

        private readonly ICommand _uboGroupSelectionChangedCommand;
        public ICommand UBOGroupSelectionChangedCommand
        {
            get
            {
                return _uboGroupSelectionChangedCommand;
            }
        }

        private bool _isDataInLoading = false;
        public bool IsDataInLoading
        {
            get
            {
                return this._isDataInLoading;
            }
            set
            {
                this._isDataInLoading = value;
                RaisePropertyChanged( () => IsDataInLoading );
            }
        }

        public void ClearData()
        {
            _uboGrpVMs.Clear();
        }

        public void InitData()
        {
            _uboGrpVMs.Clear();
            List<UBOGroup> grps = new List<UBOGroup>( _UBOGrpRepository.GetAllUBOs() );
            grps.ForEach( uboGrp =>
                {
                    _uboGrpVMs.Add( new UBOGroupViewModel( uboGrp, this ) );
                } );
        }

        public bool DeleteUBO( int uboId )
        {
            bool bSuc = _UBOGrpRepository.DeleteUBO( uboId );
            if( bSuc )
                Reloadata();
            return bSuc;
        }

        public void Reloadata()
        {
            if( !IsDataInLoading )
                this._UBOGrpRepository.ReloadData();
        }

        private void KeywordSearch( object keyWord )
        {
            if( string.IsNullOrEmpty( keyWord.ToString() ) )
            {
                this.InitData();
                return;
            }

            _uboGrpVMs.Clear();
            List<UBOGroup> grps = new List<UBOGroup>( _UBOGrpRepository.GetAllUBOs() );
            grps.ForEach( uboGrp =>
            {
                if( uboGrp.Name.Contains( keyWord.ToString() ) )
                    _uboGrpVMs.Add( new UBOGroupViewModel( uboGrp, this ) );
            } );
        }

        private void UBOGroupSelectionChanged( object grpId )
        {
            if( grpId == null )
                return;

            int uboGrpSelectedId = (int)grpId;
            if( uboGrpSelectedId == UBOGroupSelComboBoxCtl.ALL_UBO_GROUPS )
            {
                this.InitData();
                return;
            }

            _uboGrpVMs.Clear();
            List<UBOGroup> grps = new List<UBOGroup>( _UBOGrpRepository.GetAllUBOs() );
            grps.ForEach( uboGrp =>
            {
                if( uboGrp.Id == uboGrpSelectedId )
                    _uboGrpVMs.Add( new UBOGroupViewModel( uboGrp, this ) );
            } );
        }
    }
}
