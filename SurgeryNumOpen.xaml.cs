using CFLMedCab.BLL;
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
    /// SurgeryNumOpen.xaml 的交互逻辑
    /// </summary>
    public partial class SurgeryNumOpen : UserControl
    {
        private FetchOrder fetchOrder;
        private SurgeryOrderBll surgeryOrderBll = new SurgeryOrderBll();
        private FetchOrderBll fetchOrderBll = new FetchOrderBll();
        private FetchOrderdtlBll fetchOrderdtlBll = new FetchOrderdtlBll();
        public SurgeryNumOpen(int id)
        {
            InitializeComponent();
            fetchOrder = fetchOrderBll.GetById(id);
            SurgeryOrder surgeryOrder = surgeryOrderBll.GetById(fetchOrder.business_order_id);
            lNum.Content = surgeryOrder.id;
            time.Content = surgeryOrder.surgery_dateiime;
            listView.DataContext = fetchOrderdtlBll.GetDetailsUsage(id);
        }
    }
}
