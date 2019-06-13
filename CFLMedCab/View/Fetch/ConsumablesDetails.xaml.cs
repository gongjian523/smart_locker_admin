using CFLMedCab.Model;
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
        private SurgeryOrder surgeryOrder;
        public ConsumablesDetails(SurgeryOrder model)
        {
            InitializeComponent();
            surgeryOrder = model;
            SurgeryNum.Content = surgeryOrder.id;
        }

        /// <summary>
        /// 返回领用页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Return(object sender, RoutedEventArgs e)
        {
            SurgeryNumQuery surgeryNumQuery = new SurgeryNumQuery(surgeryOrder);
            ContentFrame.Navigate(surgeryNumQuery);

        }
    }
}
