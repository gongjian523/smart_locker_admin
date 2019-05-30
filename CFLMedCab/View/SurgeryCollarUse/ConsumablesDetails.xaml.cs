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

namespace CFLMedCab.View.SurgeryCollarUse
{
    /// <summary>
    /// ConsumablesDetails.xaml 的交互逻辑
    /// </summary>
    public partial class ConsumablesDetails : UserControl
    {
        public ConsumablesDetails(string surgeryNum)
        {
            InitializeComponent();
            SurgeryNum.Content = surgeryNum;
        }

        /// <summary>
        /// 返回领用页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Return(object sender, RoutedEventArgs e)
        {
            SurgeryNumQuery surgeryNumQuery = new SurgeryNumQuery(SurgeryNum.Content.ToString());
            ContentFrame.Navigate(surgeryNumQuery);

        }
    }
}
