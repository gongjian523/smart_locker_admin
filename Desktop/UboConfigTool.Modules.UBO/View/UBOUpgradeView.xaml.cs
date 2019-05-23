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
using UboConfigTool.Modules.UBO.ViewModel;

namespace UboConfigTool.Modules.UBO.View
{
    /// <summary>
    /// Interaction logic for UBOUpgradeView.xaml
    /// </summary>
    [Export( "UBOUpgradeView" )]
    public partial class UBOUpgradeView : UserControl
    {
        public UBOUpgradeView()
        {
            InitializeComponent();
            this.Loaded += UBOUpgradeView_Loaded;
        }

        void UBOUpgradeView_Loaded( object sender, RoutedEventArgs e )
        {
            this.DataContext = new UBOUpgradeViewModel();
        }
    }
}
