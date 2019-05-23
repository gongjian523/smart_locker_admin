using MahApps.Metro.Controls;
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
using UboConfigTool.Modules.Logging.ViewModel;

namespace UboConfigTool.Modules.Logging.View
{
    /// <summary>
    /// Interaction logic for DeviceLogFilterView.xaml
    /// </summary>
    [Export]
    public partial class DeviceLogFilterView : Flyout
    {
        public DeviceLogFilterView()
        {
            InitializeComponent();
        }

        [Import]
        public LoggingViewModel ViewModel
        {
            set
            {
                this.DataContext = value;
            }
        }
    }
}
