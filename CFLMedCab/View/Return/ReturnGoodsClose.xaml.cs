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
        //进入拣货单详情开门状态页面
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

        private string code;
        private int actInNum; 

        bool bExit;

        public ReturnGoodsClose(PickingOrderDto model, Hashtable hashtable)
        {
            InitializeComponent();
            pickingOrderDto = model;
            //操作人
            operatorName.Content = ApplicationState.GetValue<CurrentUser>((int)ApplicationKey.CurUser).name;
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

            code = model.code;
            actInNum = operateGoodsNum;

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
            //todo 判断条件还要修改
            if(abnormalInNum.Content == inNum.Content)
            {
                endTimer.Close();
                bExit = (((Button)sender).Name == "YesAndExitBtn" ? true : false);
                EndOperation(bExit);  
            }
            else
            {
                endTimer.Close();
                endTimer.Start();
                normalView.Visibility = Visibility.Collapsed;
                abnormalView.Visibility = Visibility.Visible;

                codeLb.Content = code;
                statusLb.Content = "异常";
                //TODO
                plaPickNumLb.Content = "";
                actPickNumLb.Content = actInNum;
            }
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


        /// <summary>
        /// 操作缺货按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onShowBtnShort(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            bthShortHide.Visibility = (btn.Name == "bthShortShow" ? Visibility.Visible : Visibility.Collapsed);
        }

        /// <summary>
        /// 操作损耗按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onShowBtnLoss(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            bthLossHide.Visibility = (btn.Name == "bthLossShow" ? Visibility.Visible : Visibility.Collapsed);
        }

        /// <summary>
        /// 操作其他按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onShowBtnOther(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            bthOtherHide.Visibility = (btn.Name == "bthOtherShow" ? Visibility.Visible : Visibility.Collapsed);
        }

        /// <summary>
        /// 提交按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onSubmit(object sender, RoutedEventArgs e)
        {
            endTimer.Close();
            EndOperation(bExit);
        }

    }
}
