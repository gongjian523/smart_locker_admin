using CFLMedCab.BLL;
using CFLMedCab.DTO.Goodss;
using CFLMedCab.DTO.Stock;
using CFLMedCab.Infrastructure;
using CFLMedCab.Model;
using CFLMedCab.Model.Enum;
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
    /// NoSurgeryNumClose.xaml 的交互逻辑
    /// </summary>
    public partial class SurgeryNoNumClose : UserControl
    { 
        public delegate void EnterSurgeryNoNumOpenHandler(object sender, RoutedEventArgs e);
        public event EnterSurgeryNoNumOpenHandler EnterSurgeryNoNumOpenEvent;

        //跳出关闭弹出框
        public delegate void EnterPopCloseHandler(object sender, RoutedEventArgs e);
        public event EnterPopCloseHandler EnterPopCloseEvent;

        private Hashtable after;
        private List<GoodsDto> goodsChageOrderdtls;
        private GoodsBll goodsBll = new GoodsBll();
        private FetchOrderBll fetchOrderBll = new FetchOrderBll();
        public SurgeryNoNumClose(Hashtable hashtable)
        {
            InitializeComponent();
            time.Content = DateTime.Now;
            Hashtable before = ApplicationState.GetValue<Hashtable>((int)ApplicationKey.CurGoods);

            operatorName.Content = ApplicationState.GetValue<User>((int)ApplicationKey.CurUser).name;
            after = hashtable;
            List<GoodsDto> goodDtos = goodsBll.GetCompareSimpleGoodsDto(before, hashtable);
            int operateGoodsNum = 0, storageGoodsExNum = 0, outStorageGoodsExNum = 0;
            goodsChageOrderdtls = fetchOrderBll.GetSurgeryFetchOrderdtlOperateDto(goodDtos, out operateGoodsNum, out storageGoodsExNum, out outStorageGoodsExNum);
            listView.DataContext = goodsChageOrderdtls;
        }

        /// <summary>
        /// 确认关柜
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEndOperation(object sender, RoutedEventArgs e)
        {
            if (fetchOrderBll.InsertFetchAndGoodsChangeInfo(goodsChageOrderdtls, RequisitionType.无单手术领用, null))
                ApplicationState.SetValue((int)ApplicationKey.CurGoods, after);
            EnterPopCloseEvent(this,null);
        }

        /// <summary>
        /// 不关柜
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onNoEndOperation(object sender, RoutedEventArgs e)
        {
            EnterSurgeryNoNumOpenEvent(this, null);
        }
    }
}
