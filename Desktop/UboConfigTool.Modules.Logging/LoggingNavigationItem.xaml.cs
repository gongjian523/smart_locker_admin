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
using UboConfigTool.Modules.Logging.Service;

namespace UboConfigTool.Modules.Logging
{
    /// <summary>
    /// LoggingNavigationItem.xaml 的交互逻辑
    /// </summary>
    [Export]
    [ViewSortHint("05")]
    public partial class LoggingNavigationItem : UserControl, IPartImportsSatisfiedNotification
    {
        private static Uri loggingViewUri = new Uri("LoggingView", UriKind.Relative);
        private readonly List<string> MAIN_REGION_VIEW_URIS = new List<string>
        {
            "LoggingView"
        };

        [Import]
        IRegionManager regionManager;

        public LoggingNavigationItem()
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
                ILoggingService logService = ServiceLocator.Current.GetInstance<ILoggingService>();
                if( logService == null )
                    return;
                logService.LoadDeviceLogData();
            }
        }

        public void MainContentRegion_Navigated(object sender, RegionNavigationEventArgs e)
        {
            this.UpdateNavigationButtonState(e.Uri);
        }

        private void UpdateNavigationButtonState(Uri uri)
        {
            this.LoggingButton.IsChecked = (uri == loggingViewUri);
        }

        private void LoggingButton_Click(object sender, RoutedEventArgs e)
        {
            this.regionManager.RequestNavigate(RegionNames.MainContentRegion, loggingViewUri);
        }
    }
}
