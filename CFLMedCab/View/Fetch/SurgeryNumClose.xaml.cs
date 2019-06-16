using CFLMedCab.BLL;
using CFLMedCab.DTO.Fetch;
using CFLMedCab.DTO.Stock;
using CFLMedCab.Infrastructure;
using CFLMedCab.Infrastructure.DeviceHelper;
using CFLMedCab.Infrastructure.ToolHelper;
using CFLMedCab.Model;
using System;
using System.Collections;
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
    /// SurgeryNumClose.xaml 的交互逻辑
    /// </summary>
    public partial class SurgeryNumClose : UserControl
    {
        public delegate void EnterSurgeryNumOpenHandler(object sender, FetchOrder fetchOrder);
        public event EnterSurgeryNumOpenHandler EnterSurgeryNumOpenEvent;
        //跳出关闭弹出框
        public delegate void EnterPopCloseHandler(object sender, RoutedEventArgs e);
        public event EnterPopCloseHandler EnterPopCloseEvent;
        FetchOrder fetchOrder;
        //FetchOrderdtlBll fetchOrderdtlBll = new FetchOrderdtlBll();
        //SurgeryOrderBll surgeryOrderBll = new SurgeryOrderBll();
        public SurgeryNumClose(FetchOrder model)
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

            List<GoodsChageOrderdtlDto> goodsChageOrderdtls = new List<GoodsChageOrderdtlDto>();
            for (int i = 5; i >= 0; i--)
            {
                GoodsChageOrderdtlDto goodsChageOrderdtl = new GoodsChageOrderdtlDto
                {
                    id = i,
                    batch_number = "feg",
                    birth_date = DateTime.Now,
                    code = "ewfw",
                    exception_flag = 0,
                    expire_date = DateTime.Now,
                    exception_description = "测试数据",
                    fetch_type = 1,
                    goods_code = "fwe",
                    goods_id = 1,
                    good_change_orderid = 1,
                    name = "测试数据",
                    operate_type = 0,
                    position = "1号柜",
                    related_order_id = 1,
                    remarks = "测试数据",
                    status = 0,
                    valid_period = 4
                };
                goodsChageOrderdtls.Add(goodsChageOrderdtl);
            }
            listView1.DataContext = goodsChageOrderdtls;
        }

        /// <summary>
        /// 根据关门数据组合展示数据
        /// </summary>
        /// <param name="inHashtable">入库数据</param>
        /// <param name="outHashtable">出库 数据</param>
        /// <param name="type">领用类型</param>
        //public void data()
        //{
        //    bool isGetSuccess;
        //    Hashtable befroe = ApplicationState.GetValue<Hashtable>((int)ApplicationKey.CurGoods);
        //    Hashtable after = RfidHelper.GetEpcData(out isGetSuccess);
        //    HashSet<string> inHashtable;
        //    HashSet<string> outHashtable;
        //    CollectHelper.CompareCollect(befroe, after, out inHashtable, out outHashtable);
        //    goodsChageOrderdtls = fetchOrderdtlBll.newGoodsChageOrderdtls(inHashtable, 1, 0, "操作与业务类型冲突", ref exception_flag);
        //    foreach (GoodsChageOrderdtl item in fetchOrderdtlBll.newGoodsChageOrderdtls(outHashtable, 0, 0, "操作与业务类型冲突", ref exception_flag))
        //    {
        //        goodsChageOrderdtls.Add(item);
        //    }
        //    listView.DataContext = goodsChageOrderdtls.OrderBy(t => t.expire_date).OrderBy(t => t.exception_flag);
        //}

        /// <summary>
        /// 不结束本次领用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param> 
        public void onNoEndOperation(object sender, RoutedEventArgs e)
        {
            EnterSurgeryNumOpenEvent(this, null);
        }

        /// <summary>
        /// 结束本次领用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEndOperation(object sender, RoutedEventArgs e)
        {
            EnterPopCloseEvent(this, null);
        }
    }
}
