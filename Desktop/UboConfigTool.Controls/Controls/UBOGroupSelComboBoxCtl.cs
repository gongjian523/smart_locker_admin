using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using UboConfigTool.Infrastructure.Interfaces;
using UboConfigTool.Infrastructure.Models;

namespace UboConfigTool.Controls
{
    public class UBOGroupSelComboBoxCtl : ComboBox
    {
        public static int ALL_UBO_GROUPS = 9999;
        private List<KeyValuePair<int, string>> _uboGroups = new List<KeyValuePair<int, string>>();
        private IUBODataService _uboDataService;

        public UBOGroupSelComboBoxCtl()
        {
            this.Loaded += UBOGroupSelComboBoxCtl_Loaded;
        }

        public ObservableCollection<KeyValuePair<int, string>> UBOGroupList
        {
            get
            {
                return (ObservableCollection<KeyValuePair<int, string>>)this.GetValue( UBOGroupListProperty );
            }
            set
            {
                this.SetValue( UBOGroupListProperty, value );
            }
        }

        public static readonly DependencyProperty UBOGroupListProperty =
	DependencyProperty.Register( "UBOGroupList", typeof( ObservableCollection<KeyValuePair<int, string>> ), typeof( UBOGroupSelComboBoxCtl ), new FrameworkPropertyMetadata( null, null ) );

        void UBOGroupSelComboBoxCtl_Loaded( object sender, System.Windows.RoutedEventArgs e )
        {
            this.Style = this.TryFindResource( "MetroComboBox" ) as Style;
            UBOGroupList = new ObservableCollection<KeyValuePair<int, string>>();
            this.DataContext = this;

            try
            {
                _uboDataService = ServiceLocator.Current.GetInstance<IUBODataService>();
                if( _uboDataService == null )
                    return;
                InitBindingData();
                _uboDataService.DataEndLoadingEvent -= dataService_DataEndLoadingEvent;
                _uboDataService.DataEndLoadingEvent += dataService_DataEndLoadingEvent;
            }
            catch
            {
            }
        }

        private void dataService_DataEndLoadingEvent()
        {
            Application.Current.Dispatcher.BeginInvoke( DispatcherPriority.Normal, new Action( () =>
            {
                InitBindingData();
            } ) );

        }

        private void InitBindingData()
        {
            if( _uboDataService == null )
                return;

            UBOGroupList.Clear();
            UBOGroupList.Add( new KeyValuePair<int, string>( ALL_UBO_GROUPS, "全部UBO群组" ) );
            _uboDataService.GetAllUBOs().ToList<UBOGroup>().ForEach( uboGrp =>
            {
                UBOGroupList.Add( new KeyValuePair<int, string>( uboGrp.Id, uboGrp.Name ) );
            } );
            this.SelectedIndex = 0;
        }
    }
}
