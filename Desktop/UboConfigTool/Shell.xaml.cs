using MahApps.Metro.Controls;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
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
using System.Windows.Shapes;
using UboConfigTool.Infrastructure;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Practices.Prism.Events;
using UboConfigTool.Infrastructure.Events;
using System.Windows.Media.Animation;
using System.Threading.Tasks;
using UboConfigTool.View;
using System.Collections.Specialized;
using Microsoft.Practices.ServiceLocation;
using UboConfigTool.Infrastructure.Interfaces;
using UboConfigTool.ViewModel;
using UboConfigTool.Controllers;
using UboConfigTool.Services;

namespace UboConfigTool
{
    /// <summary>
    /// ShellNew.xaml 的交互逻辑
    /// </summary>
    [Export]
    public partial class Shell : MetroWindow
    {
        private static Uri dashboardViewUri = new Uri( "DashboardView", UriKind.Relative );
        private static Uri settingsViewUri = new Uri( "SettingsView", UriKind.Relative );

        [Import( AllowRecomposition = false )]
        public IModuleManager ModuleManager;

        [Import( AllowRecomposition = false )]
        public IRegionManager RegionManager;

        [Import( AllowRecomposition = false )]
        public IEventAggregator EventAggregater;

        public Shell()
        {
            InitializeComponent();

            this.Loaded += Shell_Loaded;

            this.RegionManager.RegisterViewWithRegion( RegionNames.MainNavigationBar, typeof( DashboardNavigationItem ) );
        }

        void Shell_Loaded( object sender, RoutedEventArgs e )
        {
            this.EventAggregater.GetEvent<UserLoginEvent>().Subscribe( this.UserLogIn, ThreadOption.UIThread );
            this.EventAggregater.GetEvent<PopupDialogBeginEvent>().Subscribe( this.PopupDialogBegin, ThreadOption.UIThread );
            this.EventAggregater.GetEvent<PopupDialogEndEvent>().Subscribe( this.PopupDialogEnd, ThreadOption.UIThread );
            this.EventAggregater.GetEvent<AddUBOGroupEvent>().Subscribe( this.AddUBOGroup, ThreadOption.UIThread );
        }

        private void PopupDialogEnd( bool obj )
        {
            Storyboard userGuidAnimation = this.FindResource( "userGuidCloseAnimation" ) as Storyboard;
            if( userGuidAnimation != null )
            {
                userGuidAnimation.Begin();
            }
        }

        private void PopupDialogBegin( bool obj )
        {
            Storyboard userGuidAnimation = this.FindResource( "userGuidAnimation" ) as Storyboard;
            if( userGuidAnimation != null )
            {
                userGuidAnimation.Begin();
            }
        }

        private void UserLogIn( bool bSuc )
        {
            if( !bSuc )
            {
                return;
            }

            Storyboard inTransitionAnimation = this.FindResource( "InTransition" ) as Storyboard;
            if( inTransitionAnimation != null )
            {
                inTransitionAnimation.Completed += inTransitionAnimation_Completed;
                inTransitionAnimation.Begin();
            }

            this.RegionManager.RequestNavigate( RegionNames.MainContentRegion, dashboardViewUri );
        }

        private void inTransitionAnimation_Completed( object sender, EventArgs e )
        {
            NavigateToUserWizardPopup( WizardType.NewUser );
        }

        private void AddUBOGroup( bool obj )
        {
            NavigateToUserWizardPopup( WizardType.NewUBOGroup );
        }

        private void NavigateToUserWizardPopup( WizardType type )
        {
            if( type == WizardType.NewUser )
            {
                IDashboardService dataService = ServiceLocator.Current.GetInstance<IDashboardService>();
                if( dataService == null
                    || dataService.GetDashboardSummaryInfo() == null
                    || dataService.GetDashboardSummaryInfo().UBOCount > 0 )
                    return;
            }

            NewUserWizardViewModel wizardVM = new NewUserWizardViewModel( new NewUserWizardController( RegionManager ), type );
            var regionContextBak = RegionManager.Regions[RegionNames.SecondaryRegion].Context;
            RegionManager.Regions[RegionNames.SecondaryRegion].Context = wizardVM;
            this.RegionManager.RequestNavigate( RegionNames.SecondaryRegion, new Uri( "NewUserWizardView", UriKind.Relative ) );
            RegionManager.Regions[RegionNames.SecondaryRegion].Context = regionContextBak;
        }
    }
}
