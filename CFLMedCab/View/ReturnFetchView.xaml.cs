using CFLMedCab.BLL;
using CFLMedCab.DAL;
using CFLMedCab.Infrastructure;
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

namespace CFLMedCab.View
{
    /// <summary>
    /// GerFetchView.xaml 的交互逻辑
    /// </summary>
    public partial class ReturnFetchView : UserControl
    {
        private FetchOrderBll fetchOrderBll = new FetchOrderBll();
        private FetchOrderdtlBll fetchOrderdtlBll = new FetchOrderdtlBll();
        private GoodsBll goodsBll = new GoodsBll();
        private UserBll userBll = new UserBll();
        private GoodsChangeOrderBll goodsChangeOrderBll = new GoodsChangeOrderBll();
        private GoodsChageOrderdtlBll goodsChageOrderdtlBll = new GoodsChageOrderdtlBll(); 
        public ReturnFetchView(string type,int Id)
        {
            InitializeComponent();
            lType.Content = type;
            lDate.Content = DateTime.Now;
            userId = Id;
            Operator.Content = ApplicationState.GetValue<User>((int)ApplicationKey.CurUser).name; 
        }

        private int userId;
        private int pagetype;
        private List<GoodsChageOrderdtl> goodsChageOrderdtls;
        private int exceptional;
        /// <summary>
        /// 关柜
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //添加库存变化单
            GoodsChageOrder goodsChageOrder = new GoodsChageOrder();
            goodsChageOrder.create_time = DateTime.Now;
            goodsChageOrder.operator_id = userId;
            goodsChageOrder.type = pagetype;
            int goodsChageOrderId= goodsChangeOrderBll.Add(goodsChageOrder);
            //添加库存变化详情
            foreach(GoodsChageOrderdtl item in goodsChageOrderdtls)
            {
                item.good_change_orderid = goodsChageOrderId;
                if (item.exceptional == 0)
                    item.status = 1;
                else
                    item.status = 0;
            }
            goodsChageOrderdtlBll.AddGoodsChageOrderdtls(goodsChageOrderdtls);
            //添加领用单
            FetchOrder fetchOrder = new FetchOrder();
            fetchOrder.create_time = DateTime.Now;
            if (exceptional > 0)
                fetchOrder.status = 0;
            else
                fetchOrder.status = 1;
            fetchOrder.type = pagetype;
            fetchOrder.operator_id = userId;
            //添加领用单
            int fetchOrderId= fetchOrderBll.Add(fetchOrder);
            //添加领用详情
            fetchOrderdtlBll.AddFetchOrderdtls(goodsChageOrderdtls, fetchOrderId);
        }


        /// <summary>
        /// 根据关门数据组合展示数据
        /// </summary>
        /// <param name="inHashtable">入库数据</param>
        /// <param name="outHashtable">出库 数据</param>
        /// <param name="type">领用类型</param>
        public void data(Hashtable inHashtable, Hashtable outHashtable, int type)
        {
            goodsChageOrderdtls = fetchOrderdtlBll.newGoodsChageOrderdtls(inHashtable, 1, type,ref exceptional);
            foreach(GoodsChageOrderdtl item in fetchOrderdtlBll.newGoodsChageOrderdtls(inHashtable, 3, type, ref exceptional))
            {
                goodsChageOrderdtls.Add(item);
            }
            listView.DataContext = goodsChageOrderdtls.OrderBy(t=>t.expire_date).OrderBy(t=>t.exceptional);
        }
    }

}
