using CFLMedCab.DAL;
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
    public partial class GerFetchView : UserControl
    {
        private FetchOrderDal fetchOrderDal = new FetchOrderDal();
        private FetchOrderdtlDal fetchOrderdtlDal = new FetchOrderdtlDal();
        private GoodsDal goodsDal = new GoodsDal();
        private UserDal userDal = new UserDal();
        private GoodsChageOrderDal goodsChageOrderDal = new GoodsChageOrderDal();
        private GoodsChageOrderdtlDal goodsChageOrderdtlDal = new GoodsChageOrderdtlDal();
        public GerFetchView(string type,int Id)
        {
            InitializeComponent();
            lType.Content = type;
            lDate.Content = DateTime.Now;
            userId = Id;
            Operator.Content = userDal.CurrentDb.GetById(Id);
            switch (type)
            {
                case "一般领用":
                    pagetype = 1;
                    //data(0, 1);
                    break;
                case "领用退回":
                    pagetype = 3;
                    //data(0, 3);
                    break;
            }

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
            int goodsChageOrderId= goodsChageOrderDal.CurrentDb.InsertReturnIdentity(goodsChageOrder);
            //添加库存变化详情
            foreach(GoodsChageOrderdtl item in goodsChageOrderdtls)
            {
                item.good_change_orderid = goodsChageOrderId;
                if (item.exceptional == 0)
                    item.status = 1;
                else
                    item.status = 0;
            }
            goodsChageOrderdtlDal.CurrentDb.InsertRange(goodsChageOrderdtls);
            //添加领用单
            FetchOrder fetchOrder = new FetchOrder();
            fetchOrder.create_time = DateTime.Now;
            if (exceptional > 0)
                fetchOrder.status = 0;
            else
                fetchOrder.status = 1;
            fetchOrder.type = pagetype;
            fetchOrder.operator_id = userId;
            int fetchOrderId= fetchOrderDal.CurrentDb.InsertReturnIdentity(fetchOrder);
            //添加领用详情
            List<FetchOrderdtl> fetchOrderdtls = new List<FetchOrderdtl>();
            foreach (GoodsChageOrderdtl item in goodsChageOrderdtls)
            {
                FetchOrderdtl fetchOrderdtl = new FetchOrderdtl();
                fetchOrderdtl.batch_number = item.batch_number;
                fetchOrderdtl.birth_date = item.birth_date;
                fetchOrderdtl.code = item.code;
                fetchOrderdtl.expire_date = item.expire_date;
                fetchOrderdtl.fetch_type = 1;
                fetchOrderdtl.goods_code = item.goods_code;
                fetchOrderdtl.goods_id = item.id;
                fetchOrderdtl.name = item.name;
                fetchOrderdtl.position = item.position;
                fetchOrderdtl.remarks = item.remarks;
                fetchOrderdtl.valid_period = item.valid_period;
                fetchOrderdtl.status = 1;
                fetchOrderdtl.related_order_id = fetchOrderId;
                fetchOrderdtl.is_add = 0;
                fetchOrderdtls.Add(fetchOrderdtl);
            }
            fetchOrderdtlDal.CurrentDb.InsertRange(fetchOrderdtls);
            ClosetCabinet closetCabinet = new ClosetCabinet();
            closetCabinet.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            closetCabinet.Owner = Application.Current.MainWindow;
            closetCabinet.ShowDialog();
        }


        /// <summary>
        /// 根据关门数据组合展示数据
        /// </summary>
        /// <param name="inHashtable">入库数据</param>
        /// <param name="outHashtable">出库 数据</param>
        /// <param name="type">领用类型</param>
        public void data(Hashtable inHashtable, Hashtable outHashtable, int type)
        {
            goodsChageOrderdtls =  newGoodsChageOrderdtls(inHashtable, 1, type);
            foreach(GoodsChageOrderdtl item in  newGoodsChageOrderdtls(inHashtable, 3, type))
            {
                goodsChageOrderdtls.Add(item);
            }
            listView.DataContext = goodsChageOrderdtls.OrderBy(t=>t.expire_date).OrderBy(t=>t.exceptional);
        }

        /// <summary>
        /// 库存变化明显表
        /// </summary>
        /// <param name="hashtable">变化数据</param>
        /// <param name="dataType">机构数据类型 出库/入库</param>
        /// <param name="pageType">页面操作类型 出库/入库</param>
        /// <returns></returns>
        public List<GoodsChageOrderdtl> newGoodsChageOrderdtls(Hashtable hashtable, int dataType, int pageType)
        {
            List<GoodsChageOrderdtl> goodsChageOrderdtlsList = new List<GoodsChageOrderdtl>();
            foreach (int item in hashtable)
            {
                Goods goods = goodsDal.GetGoodsById(item);
                if (goods != null)
                {
                    GoodsChageOrderdtl goodsChageOrderdtl = new GoodsChageOrderdtl();
                    goodsChageOrderdtl.batch_number = goods.batch_number;
                    goodsChageOrderdtl.birth_date = goods.birth_date;
                    goodsChageOrderdtl.code = goods.code;
                    goodsChageOrderdtl.expire_date = goods.expiry_date;
                    goodsChageOrderdtl.fetch_type = 1;
                    goodsChageOrderdtl.goods_code = goods.goods_code;
                    goodsChageOrderdtl.goods_id = goods.id;
                    goodsChageOrderdtl.name = goods.name;
                    goodsChageOrderdtl.operate_type = 0;
                    goodsChageOrderdtl.position = goods.position;
                    goodsChageOrderdtl.remarks = goods.remarks;
                    goodsChageOrderdtl.valid_period = goods.valid_period;
                    if (dataType != pageType)//类型不同则为异常
                    {
                        exceptional++;
                        goodsChageOrderdtl.exceptional = 1;
                        goodsChageOrderdtl.explain = "操作与业务类型冲突";
                    }
                    else
                        goodsChageOrderdtl.exceptional = 0;
                    goodsChageOrderdtlsList.Add(goodsChageOrderdtl);
                }
            }
            return goodsChageOrderdtlsList;
        }
    }

}
