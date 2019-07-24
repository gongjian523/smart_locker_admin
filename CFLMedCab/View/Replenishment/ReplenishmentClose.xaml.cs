using CFLMedCab.BLL;
using CFLMedCab.DAL;
using CFLMedCab.DTO.Goodss;
using CFLMedCab.DTO.Replenish;
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

namespace CFLMedCab.View.ReplenishmentOrder
{
    /// <summary>
    /// ReplenishmentClose.xaml 的交互逻辑
    /// </summary>
    public partial class ReplenishmentClose : UserControl
    {
        //进入补货单详情开门状态页面
        public delegate void EnterReplenishmentDetailOpenHandler(object sender, ReplenishOrderDto e);
        public event EnterReplenishmentDetailOpenHandler EnterReplenishmentDetailOpenEvent;

        //跳出关闭弹出框
        public delegate void EnterPopCloseHandler(object sender, bool e);
        public event EnterPopCloseHandler EnterPopCloseEvent;



        private ReplenishOrderDto replenishOrderDto;
        private Hashtable after;
        private List<GoodsDto> goodsDetails;

        private Timer endTimer;

        private string code;
        private int actInNum;

        bool bExit;

        public ReplenishmentClose(ReplenishOrderDto model, Hashtable hashtable)
        {
            InitializeComponent();
            //操作人
            operatorName.Content = ApplicationState.GetUserInfo().name;
            //工单号
            orderNum.Content = model.code;
            time.Content = DateTime.Now.ToString("yyyy年MM月dd日");
            replenishOrderDto = model;

            Hashtable before = ApplicationState.GetValue<Hashtable>((int)ApplicationKey.CurGoods);
            after = hashtable;
            List<GoodsDto> goodDtos = goodsBll.GetCompareGoodsDto(before, hashtable);
            goodsDetails = replenishBll.GetReplenishSubOrderdtlOperateDto(model.code, goodDtos, out int operateGoodsNum, out int storageGoodsExNum, out int outStorageGoodsExNum);

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
            if (abnormalInNum.Content == inNum.Content)
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
            EnterReplenishmentDetailOpenEvent(this, replenishOrderDto);
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
            replenishBll.UpdateReplenishStatus(replenishOrderDto.code, goodsDetails);
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
