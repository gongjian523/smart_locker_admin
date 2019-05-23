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
using UboConfigTool.Infrastructure.Interfaces;
using UboConfigTool.Modules.UBO.ViewModel;

namespace UboConfigTool.Modules.UBO.View
{
    /// <summary>
    /// Interaction logic for AddUBOGroupView.xaml
    /// </summary>
    [Export( "AddUBOView" )]
    public partial class AddUBOView : UserControl
    {
        public AddUBOView()
        {
            InitializeComponent();
            this.Loaded += AddUBOView_Loaded;
        }

        void AddUBOView_Loaded( object sender, RoutedEventArgs e )
        {
            this.DataContext = new AddUBOViewModel();
        }
    }
}
