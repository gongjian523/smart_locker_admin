using CFLMedCab.BLL;
using CFLMedCab.DAL;
using CFLMedCab.Infrastructure;
using CFLMedCab.Infrastructure.DeviceHelper;
using CFLMedCab.Infrastructure.ToolHelper;
using CFLMedCab.Model;
using System;
using System.Collections;
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
    /// GerFetchView.xaml 的交互逻辑
    /// </summary>
    public partial class GerFetchView : UserControl
    {
        public delegate void EnterFetchOpenHandler(object sender, RoutedEventArgs e);
        public event EnterFetchOpenHandler onEnterGerFetch;
        //跳出关闭弹出框
        public delegate void EnterPopCloseHandler(object sender, RoutedEventArgs e);
        public event EnterPopCloseHandler EnterPopCloseEvent;

        //private FetchOrderBll fetchOrderBll = new FetchOrderBll();
        //private FetchOrderdtlBll fetchOrderdtlBll = new FetchOrderdtlBll();
        //private GoodsBll goodsBll = new GoodsBll();
        //private UserBll userBll = new UserBll();
        //private GoodsChangeOrderBll goodsChangeOrderBll = new GoodsChangeOrderBll();
        //private GoodsChageOrderdtlBll goodsChageOrderdtlBll = new GoodsChageOrderdtlBll();
        public GerFetchView()
        {
            InitializeComponent();
            lDate.Content = DateTime.Now;
            Operator.Content = ApplicationState.GetValue<User>((int)ApplicationKey.CurUser).name;

        }

        private int userId;
        private List<GoodsChageOrderdtl> goodsChageOrderdtls;
        private int exceptional;//数据异常数量
                                /// <summary>
                                /// 关柜
                                /// </summary>
                                /// <param name="sender"></param>
                                /// <param name="e"></param>
        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    { //添加库存变化单
        //        GoodsChageOrder goodsChageOrder = new GoodsChageOrder();
        //        goodsChageOrder.create_time = DateTime.Now;
        //        goodsChageOrder.operator_id = userId;
        //        goodsChageOrder.type = 1;
        //        int goodsChageOrderId = goodsChangeOrderBll.Add(goodsChageOrder);
        //        //添加库存变化详情
        //        foreach (GoodsChageOrderdtl item in goodsChageOrderdtls)
        //        {
        //            item.good_change_orderid = goodsChageOrderId;
        //            if (item.exceptional == 0)
        //                item.status = 1;
        //            else
        //                item.status = 0;
        //        }
        //        goodsChageOrderdtlBll.AddGoodsChageOrderdtls(goodsChageOrderdtls);
        //        //添加领用单
        //        FetchOrder fetchOrder = new FetchOrder();
        //        fetchOrder.create_time = DateTime.Now;
        //        if (exceptional > 0)
        //            fetchOrder.status = 0;
        //        else
        //            fetchOrder.status = 1;
        //        fetchOrder.type = 1;
        //        fetchOrder.operator_id = userId;
        //        //添加领用单
        //        int fetchOrderId = fetchOrderBll.Add(fetchOrder);
        //        //添加领用详情
        //        fetchOrderdtlBll.AddFetchOrderdtls(goodsChageOrderdtls, fetchOrderId);
        //    }
        //}

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
        //    foreach (GoodsChageOrderdtl item in fetchOrderdtlBll.newGoodsChageOrderdtls(outHashtable, 0, 0,"操作与业务类型冲突", ref exceptional))
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
            onEnterGerFetch(this, null);
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
