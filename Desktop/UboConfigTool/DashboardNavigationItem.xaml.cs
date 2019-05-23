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
using UboConfigTool.Services;

namespace UboConfigTool
{
    /// <summary>
    /// Interaction logic for DashboardNavigationItem.xaml
    /// </summary>
    [Export]
    [ViewSortHint( "01" )]
    public partial class DashboardNavigationItem : UserControl, IPartImportsSatisfiedNotification
    {
        private static Uri _DashboardViewUri = new Uri( "DashboardView", UriKind.Relative );
        private readonly List<string> MAIN_REGION_VIEW_URIS = new List<string>
        {
            "DashboardView"
        };

        [Import]
        public IRegionManager regionManager;

        public DashboardNavigationItem()
        {
            InitializeComponent();
        }

        void IPartImportsSatisfiedNotification.OnImportsSatisfied()
        {
            IRegion mainContentRegion = this.regionManager.Regions[RegionNames.MainContentRegion];
            if( mainContentRegion != null && mainContentRegion.NavigationService != null )
            {
                mainContentRegion.NavigationService.Navigated += this.MainContentRegion_Navigated;
                mainContentRegion.NavigationService.Navigating += NavigationService_Navigating;
            }
        }

        void NavigationService_Navigating( object sender, RegionNavigationEventArgs e )
        {
            if( MAIN_REGION_VIEW_URIS.Contains( e.Uri.ToString() ) )
            {
                IDashboardService dashBoardService = ServiceLocator.Current.GetInstance<IDashboardService>();
                if( dashBoardService == null )
                    return;
                dashBoardService.RefreshData();
            }
        }

        public void MainContentRegion_Navigated( object sender, RegionNavigationEventArgs e )
        {
            this.UpdateNavigationButtonState( e.Uri );
        }

        private void UpdateNavigationButtonState( Uri uri )
        {
            this.DashboardButton.IsChecked = ( uri == _DashboardViewUri );
        }

        private void DashboardButton_Click( object sender, RoutedEventArgs e )
        {
            this.regionManager.RequestNavigate( RegionNames.MainContentRegion, _DashboardViewUri );
        }
    }
}
