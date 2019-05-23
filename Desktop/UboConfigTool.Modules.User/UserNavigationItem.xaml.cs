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
using UboConfigTool.Modules.User.Services;

namespace UboConfigTool.Modules.User
{
    /// <summary>
    /// UserNavigationItem.xaml 的交互逻辑
    /// </summary>
    [Export]
    public partial class UserNavigationItem : UserControl, IPartImportsSatisfiedNotification
    {
        private static Uri settingsViewUri = new Uri("SettingsView", UriKind.Relative);
        private readonly List<string> MAIN_REGION_VIEW_URIS = new List<string>
        {
            "SettingsView"
        };

        [Import]
        public IRegionManager regionManager;

        public UserNavigationItem()
        {
            InitializeComponent();
        }

        void IPartImportsSatisfiedNotification.OnImportsSatisfied()
        {
            IRegion mainContentRegion = this.regionManager.Regions[RegionNames.MainContentRegion];
            if (mainContentRegion != null && mainContentRegion.NavigationService != null)
            {
                mainContentRegion.NavigationService.Navigated += this.MainContentRegion_Navigated;
                mainContentRegion.NavigationService.Navigating += NavigationService_Navigating;
            }
        }

        void NavigationService_Navigating( object sender, RegionNavigationEventArgs e )
        {
            if( MAIN_REGION_VIEW_URIS.Contains( e.Uri.ToString() ) )
            {
                IUserService userService = ServiceLocator.Current.GetInstance<IUserService>();
                if( userService == null )
                    return;
                userService.ReloadData();
            }
        }

        public void MainContentRegion_Navigated(object sender, RegionNavigationEventArgs e)
        {
            this.UpdateNavigationButtonState(e.Uri);
        }

        private void UpdateNavigationButtonState(Uri uri)
        {
            this.SettingButton.IsChecked = ( uri == settingsViewUri );
        }

        private void UserButton_Click(object sender, RoutedEventArgs e)
        {
            this.regionManager.RequestNavigate(RegionNames.MainContentRegion, settingsViewUri);
        }
    }
}
