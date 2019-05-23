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
    /// Interaction logic for TextInputDialog.xaml
    /// </summary>
    [Export( "TextInputDialog" )]
    public partial class TextInputDialog : UserControl
    {
        public TextInputDialog()
        {
            InitializeComponent();
            this.Loaded += TextInputDialog_Loaded;
            this.Unloaded += TextInputDialog_Unloaded;
        }

        void TextInputDialog_Unloaded( object sender, RoutedEventArgs e )
        {

        }

        void TextInputDialog_Loaded( object sender, RoutedEventArgs e )
        {
            IRegionManager regionMgr = ServiceLocator.Current.GetInstance<IRegionManager>();
            if( regionMgr != null )
                this.DataContext = regionMgr.Regions[RegionNames.SecondaryRegion].Context;
        }
    }
}
