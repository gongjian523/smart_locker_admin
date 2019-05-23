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
using UboConfigTool.Modules.Logging.ViewModel;

namespace UboConfigTool.Modules.Logging
{
    /// <summary>
    /// LoggingView.xaml 的交互逻辑
    /// </summary>
    [Export( "LoggingView" )]
    public partial class LoggingView : UserControl
    {
        public LoggingView()
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
