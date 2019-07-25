﻿using CFLMedCab.BLL;
using CFLMedCab.DAL;
using CFLMedCab.DTO.Goodss;
using CFLMedCab.DTO.Picking;
using CFLMedCab.Http.Bll;
using CFLMedCab.Http.Enum;
using CFLMedCab.Http.Model;
using CFLMedCab.Http.Model.Base;
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
        public delegate void EnterReturnGoodsDetailOpenHandler(object sender, PickTask e);
        public event EnterReturnGoodsDetailOpenHandler EnterReturnGoodsDetailOpenEvent;

        //跳出关闭弹出框
        public delegate void EnterPopCloseHandler(object sender, bool e);
        public event EnterPopCloseHandler EnterPopCloseEvent;

        private Timer endTimer;

        private PickTask pickTask;
        private HashSet<CommodityEps> after;

        BaseData<CommodityCode> bdCommodityCode;
        BaseData<PickCommodity> bdCommodityDetail;

        bool bAbnormal = false;
        bool bExit;

        public ReturnGoodsClose(PickTask task, HashSet<CommodityEps> hs)
        {
            InitializeComponent();
            pickTask = task;
            //操作人
            //operatorName.Content = ApplicationState.GetValue<CurrentUser>((int)ApplicationKey.CurUser).name;
            ////工单号
            orderNum.Content = task.name;
            time.Content = DateTime.Now.ToString("yyyy年MM月dd日");

            HashSet<CommodityEps> before = ApplicationState.GetGoodsInfo();
            after = hs;

            bdCommodityCode = CommodityCodeBll.GetInstance().GetCompareCommodity(before, after);
            if (bdCommodityCode.code != 0)
            {
                return;
            }

            bdCommodityDetail = PickBll.GetInstance().GetPickTaskCommodityDetail(pickTask);

            PickBll.GetInstance().GetPickTaskChange(bdCommodityCode, pickTask, bdCommodityDetail);

            int outCnt = bdCommodityCode.body.objects.Where(item => item.operate_type == 1).ToList().Count;
            int abnormalInCnt = bdCommodityCode.body.objects.Where(item => item.operate_type == 1 && item.AbnormalDisplay == "异常").ToList().Count;
            int abnormalOutCnt = bdCommodityCode.body.objects.Where(item => item.operate_type == 0).ToList().Count;

            outNum.Content = outCnt;
            abnormalInNum.Content = abnormalInCnt;
            abnormalOutNum.Content = abnormalOutCnt;
            listView.DataContext = bdCommodityCode.body.objects;

            if (abnormalInCnt == 0 && abnormalOutCnt == 0)
            {
                normalBtmView.Visibility = Visibility.Visible;
                abnormalBtmView.Visibility = Visibility.Collapsed;
            }
            else
            {
                normalBtmView.Visibility = Visibility.Collapsed;
                abnormalBtmView.Visibility = Visibility.Visible;
            }

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
            if (bdCommodityDetail.body.objects.Where(item => item.PlanShelfNumber > item.CurShelfNumber).Count() > 0)
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
            EnterReturnGoodsDetailOpenEvent(this, pickTask);
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
            BasePutData<PickTask> putData = PickBll.GetInstance().PutPickTask(pickTask, AbnormalCauses.商品损坏);
            BasePostData<CommodityInventoryChange> basePostData = PickBll.GetInstance().CreatePickTaskCommodityInventoryChange(bdCommodityCode, pickTask);

            ApplicationState.SetGoodsInfo(after);
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
