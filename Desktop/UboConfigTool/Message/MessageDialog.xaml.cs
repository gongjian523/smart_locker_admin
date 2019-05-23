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

namespace UboConfigTool.Message
{
    /// <summary>
    /// Interaction logic for MessageDialog.xaml
    /// </summary>
    [Export( "MessageDialog" )]
    public partial class MessageDialog : UserControl
    {
        public MessageDialog( )
        {
            InitializeComponent();
            this.Loaded += MessageDialog_Loaded;
            this.Unloaded += MessageDialog_Unloaded;
        }

        void MessageDialog_Loaded( object sender, RoutedEventArgs e )
        {
            IRegionManager regionMgr = ServiceLocator.Current.GetInstance<IRegionManager>();
            if( regionMgr != null )
                this.DataContext = regionMgr.Regions[RegionNames.SecondaryRegion].Context;
        }

        void MessageDialog_Unloaded( object sender, RoutedEventArgs e )
        {

        }
    }
}
