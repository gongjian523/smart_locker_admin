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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using UboConfigTool.ViewModel;

namespace UboConfigTool.View
{
    /// <summary>
    /// Interaction logic for DashboardView.xaml
    /// </summary>
    [Export("DashboardView")]
    public partial class DashboardView : UserControl
    {
        [Import( AllowRecomposition = false )]
        public IRegionManager RegionManager;

        public DashboardView()
        {
            InitializeComponent();
        }

        [Import]
        public DashboardViewModel ViewModel
        {
            set
            {
                this.DataContext = value;
            }
        }
    }
}
