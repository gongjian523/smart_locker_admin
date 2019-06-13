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

namespace CFLMedCab.View.Fetch
{
    /// <summary>
    /// NoSurgeryNumClose.xaml 的交互逻辑
    /// </summary>
    public partial class NoSurgeryNumClose : UserControl
    {
        public NoSurgeryNumClose(string OddNumbers = null)
        {
            InitializeComponent();
            lName.Content = OddNumbers;
        }

        /// <summary>
        /// 确认关柜
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //CloseCabinet CloseCabinet = new CloseCabinet();
            //CloseCabinet.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            //CloseCabinet.Owner = Application.Current.MainWindow;
            //CloseCabinet.ShowDialog();
        }

        /// <summary>
        /// 不关柜
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //OpenCabinet openCabinet = new OpenCabinet();
            //openCabinet.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            //openCabinet.Owner = Application.Current.MainWindow;
            //openCabinet.ShowDialog();
        }
    }
}
