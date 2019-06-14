using CFLMedCab.BLL;
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

namespace CFLMedCab.View.Fetch
{
    /// <summary>
    /// ConsumablesDetails.xaml 的交互逻辑
    /// </summary>
    public partial class ConsumablesDetails : UserControl
    {
        private FetchOrder fetchOrder;
        public delegate void EnterSurgeryDetailHandler(object sender, FetchOrder e);
        public event EnterSurgeryDetailHandler EnterSurgeryDetailEvent;
        //private SurgeryOrderDal surgeryOrderDal = new SurgeryOrderDal();
        //private FetchOrderdtlBll fetchOrderdtlBll = new FetchOrderdtlBll();
        public ConsumablesDetails(FetchOrder model)
        {
            InitializeComponent();
            fetchOrder = model;
            SurgeryNum.Content = model.business_order_id;
            //listView.DataContext = fetchOrderdtlBll.GetDetailsUsage(model.id);
        }

        /// <summary>
        /// 返回领用页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Return(object sender, RoutedEventArgs e)
        {
            EnterSurgeryDetailEvent(this, fetchOrder);
            //SurgeryNumQuery surgeryNumQuery = new SurgeryNumQuery(fetchOrder.id);
            //ContentFrame.Navigate(surgeryNumQuery);

        }
    }
}
