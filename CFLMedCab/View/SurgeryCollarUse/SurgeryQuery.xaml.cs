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

        /// <summary>
        /// 手术单号查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Query(object sender, RoutedEventArgs e)
        {
            SurgeryOrder surgeryOrder = new SurgeryOrder();
            SurgeryOrderDal surgeryOrderDal = new SurgeryOrderDal();
            SurgeryOrderdtlDal surgeryOrderdtlDal = new SurgeryOrderdtlDal();
            SurgeryOrder model = surgeryOrderDal.GetSurgeryOrderById(Convert.ToInt32(tbOddNumbers.Text));
            if (model!=null)
            {
                SurgeryNumQuery surgeryNumQuery = new SurgeryNumQuery(model);
                ContentFrame.Navigate(surgeryNumQuery);
            }
            else
            {
                surgeryOrder.id = Convert.ToInt32(tbOddNumbers.Text);
                surgeryOrder.surgery_dateiime = DateTime.Now;
                int num = surgeryOrderDal.InsertNewSurgeryOrder(surgeryOrder);
                if (num > 0)
                {
                    SurgeryOrderdtl surgeryOrderdtl = new SurgeryOrderdtl();
                    surgeryOrderdtl.surgery_order_id = surgeryOrder.id;
                    surgeryOrderdtl.name = "测试数据";
                    surgeryOrderdtl.number = 22;
                    surgeryOrderdtl.remarks = "暂无";
                    surgeryOrderdtlDal.InsertNewSurgeryOrderdtl(surgeryOrderdtl);
                    SurgeryNumQuery surgeryNumQuery = new SurgeryNumQuery(surgeryOrder);
                    ContentFrame.Navigate(surgeryNumQuery);
                }
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
