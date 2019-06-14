using CFLMedCab.BLL;
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
        //FetchOrderBll fetchOrderBll = new FetchOrderBll();
        //FetchOrderdtlBll fetchOrderdtlBll = new FetchOrderdtlBll();
        //SurgeryOrderBll surgeryOrderBll = new SurgeryOrderBll();
        public SurgeryNumClose(FetchOrder fetchOrder)
        {
            InitializeComponent();
            //FetchOrder fetchOrder = fetchOrderBll.GetById(id);
            //SurgeryOrder surgeryOrder = surgeryOrderBll.GetById(fetchOrder.business_order_id);
            //lNum.Content = surgeryOrder.id;
            //time.Content = surgeryOrder.surgery_dateiime;
            //Operator.Content= ApplicationState.GetValue<User>((int)ApplicationKey.CurUser).name;
            //listView.DataContext = fetchOrderdtlBll.GetDetailsUsage(id);
        }
        private int exceptional;
        private List<GoodsChageOrderdtl> goodsChageOrderdtls;

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
        //    goodsChageOrderdtls = fetchOrderdtlBll.newGoodsChageOrderdtls(inHashtable, 1, 0, "操作与业务类型冲突", ref exceptional);
        //    foreach (GoodsChageOrderdtl item in fetchOrderdtlBll.newGoodsChageOrderdtls(outHashtable, 0, 0, "操作与业务类型冲突", ref exceptional))
        //    {
        //        goodsChageOrderdtls.Add(item);
        //    }
        //    listView.DataContext = goodsChageOrderdtls.OrderBy(t => t.expire_date).OrderBy(t => t.exceptional);
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
