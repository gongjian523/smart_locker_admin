using CFLMedCab.DAL;
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
    /// Query.xaml 的交互逻辑
    /// </summary>
    public partial class SurgeryQuery : UserControl
    {
        public SurgeryQuery()
        {
            InitializeComponent();
        }
        private FetchOrderDal fetchOrderDal = new FetchOrderDal();
        /// <summary>
        /// 手术单号查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Query(object sender, RoutedEventArgs e)
        {

            var value = tbOddNumbers.Text;
            if (string.IsNullOrWhiteSpace(value))
            {
                MessageBox.Show("手术单号不可以为空！", "温馨提示", MessageBoxButton.OK);
                return;
            }
            //根据领用单查找手术单
            int surgeryId = fetchOrderDal.CurrentDb.GetById(Convert.ToInt32(value)).business_order_id;
            if (surgeryId >0)
            {
                SurgeryNumQuery numQuery = new SurgeryNumQuery(surgeryId);
                ContentFrame.Navigate(numQuery);
            }
            else
            {
                MessageBox.Show("手术单号不存在！", "温馨提示", MessageBoxButton.OK);
                return;
            }
        }

        /// <summary>
        /// 暂无手术单号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NoNum(object sender, RoutedEventArgs e)
        {
            NoSurgeryNumClose noSurgeryNumClose = new NoSurgeryNumClose();
            ContentFrame.Navigate(noSurgeryNumClose);
        }
    }
}
