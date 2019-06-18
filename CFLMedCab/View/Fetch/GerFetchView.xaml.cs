using CFLMedCab.BLL;
using CFLMedCab.DAL;
using CFLMedCab.DTO.Goodss;
using CFLMedCab.DTO.Stock;
using CFLMedCab.Infrastructure;
using CFLMedCab.Infrastructure.DeviceHelper;
using CFLMedCab.Infrastructure.ToolHelper;
using CFLMedCab.Model;
using CFLMedCab.Model.Enum;
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
        public event EnterFetchOpenHandler EnterGerFetch;
        //跳出关闭弹出框
        public delegate void EnterPopCloseHandler(object sender, RoutedEventArgs e);
        public event EnterPopCloseHandler EnterPopCloseEvent;

        private Hashtable after;
        private List<GoodsDto> goodsChageOrderdtls;
        private GoodsBll goodsBll = new GoodsBll();
        private FetchOrderBll fetchOrderBll = new FetchOrderBll();
        public GerFetchView(Hashtable hashtable)
        {
            InitializeComponent();
            time.Content = DateTime.Now;
            operatorName.Content = ApplicationState.GetValue<User>((int)ApplicationKey.CurUser).name;
            Hashtable before = ApplicationState.GetValue<Hashtable>((int)ApplicationKey.CurGoods);
            after = hashtable;
            List<GoodsDto> goodDtos = goodsBll.GetCompareSimpleGoodsDto(before, hashtable);

            int operateGoodsNum = 0, storageGoodsExNum = 0, outStorageGoodsExNum = 0;

            goodsChageOrderdtls = fetchOrderBll.GetGeneralFetchOrderdtlOperateDto(goodDtos, out operateGoodsNum, out  storageGoodsExNum,  out outStorageGoodsExNum);
            listView.DataContext = goodsChageOrderdtls;
        }

        /// <summary>
        /// 不结束本次领用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param> 
        public void onNoEndOperation(object sender, RoutedEventArgs e)
        {
            EnterGerFetch(this, null);
        }

        /// <summary>
        /// 结束本次领用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEndOperation(object sender, RoutedEventArgs e)
        {
            if (fetchOrderBll.InsertFetchAndGoodsChangeInfo(goodsChageOrderdtls, RequisitionType.一般领用,null))
                ApplicationState.SetValue((int)ApplicationKey.CurGoods, after);
            EnterPopCloseEvent(this, null);
        }
    }

}
