using CFLMedCab.BLL;
using CFLMedCab.DAL;
using CFLMedCab.DTO.Goodss;
using CFLMedCab.DTO.Stock;
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
    public partial class ReturnFetchView : UserControl
    {
        //跳出关闭弹出框
        public delegate void EnterPopCloseHandler(object sender, RoutedEventArgs e);
        public event EnterPopCloseHandler EnterPopCloseEvent;

        public delegate void EnterReturnFetchHandler(object sender, RoutedEventArgs e);
        public event EnterReturnFetchHandler EnterReturnFetch;

        private Hashtable after;
        private List<GoodsDto> goodsChageOrderdtls;
        private GoodsBll goodsBll = new GoodsBll();
        private FetchOrderBll fetchOrderBll = new FetchOrderBll();

        public ReturnFetchView(Hashtable hashtable)
        {
            InitializeComponent();
            time.Content = DateTime.Now;
            operatorName.Content = ApplicationState.GetValue<User>((int)ApplicationKey.CurUser).name;
            List<GoodsDto> goodsChageOrderdtls = new List<GoodsDto>();
            Hashtable before = ApplicationState.GetValue<Hashtable>((int)ApplicationKey.CurGoods);

            //for (int i = 5; i >= 0; i--)
            //{
            //    GoodsChageOrderdtlDto goodsChageOrderdtl = new GoodsChageOrderdtlDto
            //    {
            //        id = i,
            //        batch_number = "feg",
            //        birth_date = DateTime.Now,
            //        code = "ewfw",
            //        exception_flag = 0,
            //        expire_date = DateTime.Now,
            //        exception_description = "测试数据",
            //        fetch_type = 1,
            //        goods_code = "fwe",
            //        goods_id = 1,

            //        name = "测试数据",
            //        operate_type = 0,
            //        position = "1号柜",
            //        related_order_id = 1,
            //        remarks = "测试数据",

            //        valid_period = 4
            //    };
            //    goodsChageOrderdtls.Add(goodsChageOrderdtl);
            //}

            after = hashtable;
            List<GoodsDto> goodDtos = goodsBll.GetCompareSimpleGoodsDto(before, hashtable);
            int operateGoodsNum = 0, storageGoodsExNum = 0, outStorageGoodsExNum = 0;
            goodsChageOrderdtls = fetchOrderBll.GetGoBackFetchOrderdtlOperateDto(goodDtos, out operateGoodsNum, out storageGoodsExNum, out outStorageGoodsExNum);

            listView.DataContext = goodsChageOrderdtls;
        }


        /// <summary>
        /// 不结束本次退回
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param> 
        public void onNoEndOperation(object sender, RoutedEventArgs e)
        {
            EnterReturnFetch(this, null);
        }

        /// <summary>
        /// 结束本次退回
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEndOperation(object sender, RoutedEventArgs e)
        {

            

            EnterPopCloseEvent(this, null);


            
        }
    }

}
