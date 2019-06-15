using CFLMedCab.BLL;
using CFLMedCab.DAL;
using CFLMedCab.DTO.Fetch;
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
            SurgeryOrderDto surgeryOrderDto = new SurgeryOrderDto { id = 2, surgery_dateiime = DateTime.Now };
            surgeryNum.Content = surgeryOrderDto.id;
            time.Content = surgeryOrderDto.surgery_dateiime;
            List<SurgeryFetchDto> surgeryFetches = new List<SurgeryFetchDto>();
            for (int i = 5; i >= 0; i--)
            {
                SurgeryFetchDetailsDto surgeryFetch = new SurgeryFetchDetailsDto
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
                surgeryFetch.total_num = surgeryFetch.wait_num + surgeryFetch.fetch_num;
                surgeryFetches.Add(surgeryFetch);
            }
            listView.DataContext = surgeryFetches;
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
