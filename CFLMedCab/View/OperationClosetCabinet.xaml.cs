using System;
using System.Collections.Generic;
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

namespace CFLMedCab.View
{
    /// <summary>
    /// OperationClosetCabinet.xaml 的交互逻辑
    /// </summary>
    public partial class OperationClosetCabinet : UserControl
    {
        public OperationClosetCabinet()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 结束本次领用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ClosetCabinet closetCabinet = new ClosetCabinet();
            closetCabinet.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            closetCabinet.Owner = Application.Current.MainWindow;
            closetCabinet.ShowDialog();
        }
    }
}
