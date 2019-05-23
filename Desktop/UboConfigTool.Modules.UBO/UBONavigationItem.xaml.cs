using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

namespace UboConfigTool.Modules.UBO
{
    /// <summary>
    /// UBONavigationItem.xaml 的交互逻辑
    /// </summary>
    [Export]
    [ViewSortHint("02")]
    public partial class UBONavigationItem : UserControl, IPartImportsSatisfiedNotification
    {
        private static Uri uboGroupViewUri = new Uri( "UBOGroupListView", UriKind.Relative );
        private readonly List<string> MAIN_REGION_VIEW_URIS = new List<string>
        {
            "UBOGroupListView",
            "UBOGroupView",
            "UBOView"
        };

        [Import]
        public IRegionManager regionManager;

        public UBONavigationItem()
        {
            InitializeComponent();
        }

        void IPartImportsSatisfiedNotification.OnImportsSatisfied()
        {
            IRegion mainContentRegion = this.regionManager.Regions[RegionNames.MainContentRegion];
            if (mainContentRegion != null && mainContentRegion.NavigationService != null)
            {
                mainContentRegion.NavigationService.Navigated += this.MainContentRegion_Navigated;
                mainContentRegion.NavigationService.Navigating += this.MainContentRegion_Navigating;
            }
        }

        private void MainContentRegion_Navigating( object sender, RegionNavigationEventArgs e )
        {
            if( MAIN_REGION_VIEW_URIS.Contains( e.Uri.ToString() ) )
            {
                IUBODataService uboDataService = ServiceLocator.Current.GetInstance<IUBODataService>();
                if( uboDataService == null )
                    return;
                uboDataService.ReloadData();
            }
        }

        public void MainContentRegion_Navigated(object sender, RegionNavigationEventArgs e)
        {
            this.UpdateNavigationButtonState(e.Uri);
        }

        private void UpdateNavigationButtonState(Uri uri)
        {
            this.UBOButton.IsChecked = (uri == uboGroupViewUri);
        }

        private void UBOButton_Click(object sender, RoutedEventArgs e)
        {
           this.regionManager.RequestNavigate(RegionNames.MainContentRegion, uboGroupViewUri);
        }
    }
}
