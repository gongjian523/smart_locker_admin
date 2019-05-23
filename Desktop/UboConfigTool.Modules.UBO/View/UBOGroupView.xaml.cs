using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UboConfigTool.Infrastructure;
using UboConfigTool.Infrastructure.Interfaces;
using UboConfigTool.Modules.UBO.ViewModel;

namespace UboConfigTool.Modules.UBO.View
{
    /// <summary>
    /// Interaction logic for UBOGroupView.xaml
    /// </summary>
    [Export("UBOGroupView")]
    public partial class UBOGroupView : UserControl, INavigationAware
    {
        public UBOGroupView()
        {
            InitializeComponent();
        }

        private void Button_Click( object sender, RoutedEventArgs e )
        {
            IRegionManager regionMgr = ServiceLocator.Current.GetInstance<IRegionManager>();
            if( regionMgr != null )
            {
                regionMgr.RequestNavigate( RegionNames.MainContentRegion, new Uri( "UBOGroupListView", UriKind.Relative ) );
            }
        }

        public bool IsNavigationTarget( NavigationContext navigationContext )
        {
            return true;
        }

        public void OnNavigatedFrom( NavigationContext navigationContext )
        {
            
        }

        public void OnNavigatedTo( NavigationContext navigationContext )
        {
            UBOGroupListViewModel uboGrpLstVM = ServiceLocator.Current.GetInstance<UBOGroupListViewModel>();
            if( uboGrpLstVM != null &&　uboGrpLstVM.CurrentUBOGroup != null)
            {
                this.DataContext = uboGrpLstVM.CurrentUBOGroup;
            }
        }
    }
}
