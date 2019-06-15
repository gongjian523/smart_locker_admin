using CFLMedCab.BLL;
using CFLMedCab.DTO.Fetch;
using CFLMedCab.Model;
using System;
using System.Collections.Generic;
using System.Windows.Controls;


namespace CFLMedCab.View.Fetch
{
    /// <summary>
    /// SurgeryNumOpen.xaml 的交互逻辑
    /// </summary>
    public partial class SurgeryNumOpen : UserControl
    {
        private FetchOrder fetchOrder;
        //private SurgeryOrderBll surgeryOrderBll = new SurgeryOrderBll();
        //private FetchOrderBll fetchOrderBll = new FetchOrderBll();
        //private FetchOrderdtlBll fetchOrderdtlBll = new FetchOrderdtlBll();
        public SurgeryNumOpen(FetchOrder model)
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
        
    }
}
