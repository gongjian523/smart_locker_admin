using CFLMedCab.BLL;
using CFLMedCab.DAL;
using CFLMedCab.DTO.Fetch;
using CFLMedCab.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// SurgeryNumQuery.xaml 的交互逻辑
    /// </summary>
    public partial class SurgeryOrderDetail : UserControl
    {
        private FetchOrder fetchOrder;
        //private SurgeryOrderBll surgeryOrderBll = new SurgeryOrderBll();
        //private FetchOrderBll fetchOrderBll = new FetchOrderBll(); 
        //private FetchOrderdtlBll fetchOrderdtlBll = new FetchOrderdtlBll();
        public delegate void EnterSurgeryConsumablesDetailHandler(object sender, FetchOrder fetchOrder);
        public event EnterSurgeryConsumablesDetailHandler EnterSurgeryConsumablesDetailEvent;

        public delegate void EnterSurgeryNumOpenHandler(object sender, FetchOrder fetchOrder);
        public event EnterSurgeryNumOpenHandler EnterSurgeryNumOpenEvent;
        public SurgeryOrderDetail(FetchOrder model)
        {
            InitializeComponent();
          
        }


        /// <summary>
        /// 手术耗材详情
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnterSurgeryDetail(object sender, RoutedEventArgs e)
        {
            EnterSurgeryConsumablesDetailEvent(this, fetchOrder);
        }

        /// <summary>
        /// 确认领用（开柜）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnterSurgeryNumOpen(object sender, RoutedEventArgs e)
        {
            EnterSurgeryNumOpenEvent(this, fetchOrder);
        }
    }
}
