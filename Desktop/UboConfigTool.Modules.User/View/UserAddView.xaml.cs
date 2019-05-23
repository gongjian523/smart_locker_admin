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
using UboConfigTool.Modules.User.ViewModel;

namespace UboConfigTool.Modules.User.View
{
    /// <summary>
    /// Interaction logic for UserAddView.xaml
    /// </summary>
    [Export( "UserAddView" )]
    public partial class UserAddView : UserControl
    {
        public UserAddView()
        {
            InitializeComponent();

            this.Loaded += UserAddView_Loaded;
        }

        void UserAddView_Loaded( object sender, RoutedEventArgs e )
        {
            IRegionManager regionMgr = ServiceLocator.Current.GetInstance<IRegionManager>();
            if( regionMgr != null )
                this.DataContext = regionMgr.Regions[RegionNames.SecondaryRegion].Context;
        }
    }
}
