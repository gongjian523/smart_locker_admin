using MahApps.Metro.Controls.Dialogs;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.Regions.Behaviors;
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
using UboConfigTool.Controllers;
using UboConfigTool.Infrastructure;
using UboConfigTool.Infrastructure.Events;
using UboConfigTool.ViewModel;

namespace UboConfigTool.View
{
    /// <summary>
    /// Interaction logic for NewUserWizardView.xaml
    /// </summary>
    [Export( "NewUserWizardView" )]
    public partial class NewUserWizardView
    {
        [Import]
        public IRegionManager _regionManager;

        [Import]
        public IEventAggregator _eventAggregator;

        public NewUserWizardView()
        {
            InitializeComponent();
            this.Loaded += NewUserWizardView_Loaded;
        }

        void NewUserWizardView_Loaded( object sender, RoutedEventArgs e )
        {
            this.DataContext = _regionManager.Regions[RegionNames.SecondaryRegion].Context as NewUserWizardViewModel;
            this._regionManager.RequestNavigate( RegionNames.NewUserWizardRegion, new Uri( "NewUserWizard_CreateUBOGroup", UriKind.Relative ) );
        }
    }
}
