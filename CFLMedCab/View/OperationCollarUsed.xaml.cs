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
    /// OperationCollarUsed.xaml 的交互逻辑
    /// </summary>
    public partial class OperationCollarUsed : UserControl
    {
        public OperationCollarUsed(string num=null)
        {
            InitializeComponent();
            if (num != null)
            {
                lNum.Content = num;
            }
        }
        
        /// <summary>
        /// 手术耗材详情
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OperationDetails operationDetails = new OperationDetails();
            ContentFrame.Navigate(operationDetails);
        }
        
        /// <summary>
        /// 本次领用情况
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            OperationClosetCabinet operationClosetCabinet = new OperationClosetCabinet();
            ContentFrame.Navigate(operationClosetCabinet);
        }
    }
}
