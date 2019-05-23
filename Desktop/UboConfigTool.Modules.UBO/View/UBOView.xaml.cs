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
    /// Interaction logic for UBODetailView.xaml
    /// </summary>
    [Export( "UBOView" )]
    public partial class UBOView : UserControl, INavigationAware 
    {
        public UBOView()
        {
            InitializeComponent();
        }

        public void OnNavigatedTo( Microsoft.Practices.Prism.Regions.NavigationContext navigationContext )
        {
            UBOGroupListViewModel uboGrpLstVM = ServiceLocator.Current.GetInstance<UBOGroupListViewModel>();
            if( uboGrpLstVM != null && uboGrpLstVM.CurrentUBOGroup != null )
            {
                this.DataContext = uboGrpLstVM.CurrentEdittingUBO;
                txbTitle.Text = "UBO群组 > " + uboGrpLstVM.CurrentUBOGroup.GroupName + " > " + uboGrpLstVM.CurrentEdittingUBO.Name;
            }
        }

        public bool IsNavigationTarget( Microsoft.Practices.Prism.Regions.NavigationContext navigationContext )
        {
            return true;
        }

        public void OnNavigatedFrom( Microsoft.Practices.Prism.Regions.NavigationContext navigationContext )
        {

        }

        private void Button_Click( object sender, RoutedEventArgs e )
        {
            IRegionManager regionMgr = ServiceLocator.Current.GetInstance<IRegionManager>();
            if( regionMgr != null )
            {
                regionMgr.RequestNavigate( RegionNames.MainContentRegion, new Uri( "UBOGroupView", UriKind.Relative ) );
            }
        }
    }
}
