using CFLMedCab.BLL;
using CFLMedCab.DAL;
using CFLMedCab.DTO.Goodss;
using CFLMedCab.DTO.Picking;
using CFLMedCab.Infrastructure;
using CFLMedCab.Infrastructure.DeviceHelper;
using CFLMedCab.Model;
using CFLMedCab.Model.Constant;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CFLMedCab.View.Return
{
    /// <summary>
    /// ReturnGoodsClose.xaml 的交互逻辑
    /// </summary>
    public partial class ReturnGoodsClose : UserControl
    {
        //进入补货单详情开门状态页面
        public delegate void EnterReturnGoodsDetailOpenHandler(object sender, PickingOrderDto e);
        public event EnterReturnGoodsDetailOpenHandler EnterReturnGoodsDetailOpenEvent;

        //跳出关闭弹出框
        public delegate void EnterPopCloseHandler(object sender, bool e);
        public event EnterPopCloseHandler EnterPopCloseEvent;

        private Timer endTimer;

        PickingBll pickingBll = new PickingBll();
        GoodsBll goodsBll = new GoodsBll();

        private PickingOrderDto pickingOrderDto;
        private Hashtable after;
        private List<GoodsDto> goodsDetails;

        public ReturnGoodsClose(PickingOrderDto model, Hashtable hashtable)
        {
            InitializeComponent();
            pickingOrderDto = model;
            //操作人
            operatorName.Content = ApplicationState.GetValue<User>((int)ApplicationKey.CurUser).name;
            ////工单号
            orderNum.Content = model.code;
            time.Content = DateTime.Now.ToString("yyyy年MM月dd日");

            Hashtable before = ApplicationState.GetValue<Hashtable>((int)ApplicationKey.CurGoods);
            after = hashtable;
            List<GoodsDto> goodDtos = goodsBll.GetCompareGoodsDto(before, hashtable);
            goodsDetails = pickingBll.GetPickingSubOrderdtlOperateDto(model.code, goodDtos, out int operateGoodsNum, out int storageGoodsExNum, out int outStorageGoodsExNum);

            listView.DataContext = goodsDetails;
            inNum.Content = operateGoodsNum;
            abnormalInNum.Content = storageGoodsExNum;
            abnormalOutNum.Content = outStorageGoodsExNum;
            listView.DataContext = goodsDetails;

            endTimer = new Timer(Contant.ClosePageEndTimer);
            endTimer.AutoReset = false;
            endTimer.Enabled = true;
            endTimer.Elapsed += new ElapsedEventHandler(onEndTimerExpired);
        }

        /// <summary>
        /// 结束操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEndOperation(object sender, RoutedEventArgs e)
        {
            endTimer.Close();
            Button btn = (Button)sender;
            EndOperation(btn.Name == "YesAndExitBtn" ? true : false);
        }

        /// <summary>
        /// 继续操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onNoEndOperation(object sender, RoutedEventArgs e)
        {
            endTimer.Close();
            EnterReturnGoodsDetailOpenEvent(this, pickingOrderDto);
            return;
        }

        /// <summary>
        /// 结束定时器超时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEndTimerExpired(object sender, ElapsedEventArgs e)
        {
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                EndOperation(true);
            }));
        }

        private void EndOperation(bool bEixt)
        {
            pickingBll.UpdatePickingStatus(pickingOrderDto.code, goodsDetails);
            ApplicationState.SetValue((int)ApplicationKey.CurGoods, after);
            EnterPopCloseEvent(this, bEixt);
        }
    }
}
