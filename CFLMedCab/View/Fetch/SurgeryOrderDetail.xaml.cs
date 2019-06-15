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
            SurgeryOrderDto surgeryOrderDto = new SurgeryOrderDto { id=2,surgery_dateiime=DateTime.Now};
            surgeryNum.Content = surgeryOrderDto.id;
            time.Content = surgeryOrderDto.surgery_dateiime;
            List<SurgeryFetchDto> surgeryFetches = new List<SurgeryFetchDto>();
            for(int i = 5; i >= 0; i--)
            {
                SurgeryFetchDto surgeryFetch = new SurgeryFetchDto
                {
                    fetch_order_id = i,
                    goods_name = "注射器",
                    goods_code = "gr1294",
                    fetch_type = 1,
                    wait_num = 2,
                    fetch_num = 1,
                    stock_num = 3,
                    remarks = "麻醉专用"
                };
                surgeryFetches.Add(surgeryFetch);
            }
            listView.DataContext = surgeryFetches;
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
