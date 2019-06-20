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
            time.Content = DateTime.Now.ToString("yyyy年MM月dd日");
            operatorName.Content = ApplicationState.GetValue<User>((int)ApplicationKey.CurUser).name;
            List<GoodsDto> goodsChageOrderdtls = new List<GoodsDto>();
            Hashtable before = ApplicationState.GetValue<Hashtable>((int)ApplicationKey.CurGoods);
            after = hashtable;
            List<GoodsDto> goodDtos = goodsBll.GetCompareGoodsDto(before, hashtable);//获取关柜之后的库存变化信息
            goodsChageOrderdtls = fetchOrderBll.GetGoBackFetchOrderdtlOperateDto(goodDtos, out int operateGoodsNum, out int storageGoodsExNum, out int outStorageGoodsExNum);
            listView.DataContext = goodsChageOrderdtls;
            returnNum.Content = operateGoodsNum;
            abnormalInNum.Content = storageGoodsExNum;
            abnormalOutNum.Content = outStorageGoodsExNum;
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
            fetchOrderBll.UpdateGoBackFetchOrder(goodsChageOrderdtls);
            ApplicationState.SetValue((int)ApplicationKey.CurGoods, after);

            EnterPopCloseEvent(this, null);
        }
    }

}
