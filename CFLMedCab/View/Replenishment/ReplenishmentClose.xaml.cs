﻿using CFLMedCab.BLL;
using CFLMedCab.DAL;
using CFLMedCab.DTO.Goodss;
using CFLMedCab.DTO.Replenish;
using CFLMedCab.Http.Bll;
using CFLMedCab.Http.Enum;
using CFLMedCab.Http.Helper;
using CFLMedCab.Http.Model;
using CFLMedCab.Http.Model.Base;
using CFLMedCab.Infrastructure;
using CFLMedCab.Infrastructure.DeviceHelper;
using CFLMedCab.Infrastructure.ToolHelper;
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
        public delegate void EnterReplenishmentDetailOpenHandler(object sender, ShelfTask e);
        public event EnterReplenishmentDetailOpenHandler EnterReplenishmentDetailOpenEvent;

        //跳出关闭弹出框
        public delegate void EnterPopCloseHandler(object sender, bool e);
        public event EnterPopCloseHandler EnterPopCloseEvent;

        private ShelfTask shelfTask;
        private HashSet<CommodityEps> after;

        BaseData<CommodityCode> bdCommodityCode;
        BaseData<ShelfTaskCommodityDetail> bdCommodityDetail;

        private Timer endTimer;

        bool bExit;

        private bool isSuccess;

        public ReplenishmentClose(ShelfTask task, HashSet<CommodityEps> hs)
        {
            InitializeComponent();

            endTimer = new Timer(Contant.ClosePageEndTimer);
            endTimer.AutoReset = false;
            endTimer.Enabled = true;
            endTimer.Elapsed += new ElapsedEventHandler(onEndTimerExpired);

            //操作人
            operatorName.Content = ApplicationState.GetUserInfo().name;
            //工单号
            orderNum.Content = task.name;
            time.Content = DateTime.Now.ToString("yyyy年MM月dd日");
            shelfTask = task;

            HashSet<CommodityEps> before = ApplicationState.GetGoodsInfo();
            after = hs;

            List<CommodityCode> commodityCodeList = CommodityCodeBll.GetInstance().GetCompareSimpleCommodity(before, after);
            if (commodityCodeList == null || commodityCodeList.Count <= 0)
            {
                MessageBox.Show("没有检测到商品变化！", "温馨提示", MessageBoxButton.OK);
                isSuccess = false;
                return;
            }

            bdCommodityCode = CommodityCodeBll.GetInstance().GetCommodityCode(commodityCodeList);
            HttpHelper.GetInstance().ResultCheck(bdCommodityCode, out isSuccess);
            if (!isSuccess)
            {
                MessageBox.Show("获取商品比较信息错误！" + bdCommodityCode.message, "温馨提示", MessageBoxButton.OK);
                return;
            }

            bdCommodityDetail = ShelfBll.GetInstance().GetShelfTaskCommodityDetail(shelfTask);
            HttpHelper.GetInstance().ResultCheck(bdCommodityDetail, out isSuccess);
            if (!isSuccess)
            {
                MessageBox.Show("获取上架任务单商品明细信息错误！" + bdCommodityDetail.message, "温馨提示", MessageBoxButton.OK);
                return;
            }

            ShelfBll.GetInstance().GetShelfTaskChange(bdCommodityCode, shelfTask, bdCommodityDetail);

            int inCnt = bdCommodityCode.body.objects.Where(item => item.operate_type == 1).ToList().Count;
            int abnormalInCnt = bdCommodityCode.body.objects.Where(item => item.operate_type == 1 && item.AbnormalDisplay == "异常").ToList().Count;
            int abnormalOutCnt = bdCommodityCode.body.objects.Where(item => item.operate_type == 0).ToList().Count;

            inNum.Content = inCnt;
            abnormalInNum.Content = abnormalInCnt;
            abnormalOutNum.Content = abnormalOutCnt;
            int abnormalLargeNum = bdCommodityDetail.body.objects.Where(item => (item.CurShelfNumber > (item.NeedShelfNumber - item.AlreadyShelfNumber))).Count();

            listView.DataContext = bdCommodityCode.body.objects;

            if(abnormalInCnt == 0 && abnormalOutCnt == 0 && abnormalLargeNum == 0)
            {
                normalBtmView.Visibility = Visibility.Visible;
                abnormalBtmView.Visibility = Visibility.Collapsed;
            }
            else
            {
                normalBtmView.Visibility = Visibility.Collapsed;
                abnormalBtmView.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// 结束操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEndOperation(object sender, RoutedEventArgs e)
        {
            bool bGotoAbnormal = true;

            if (bdCommodityCode.code != 0 || bdCommodityDetail.code != 0)
            {
                bGotoAbnormal = false;
            }
            else
            {
                if (bdCommodityDetail.body.objects.Where(item => (item.NeedShelfNumber - item.AlreadyShelfNumber != item.CurShelfNumber)).Count() == 0)
                {
                    bGotoAbnormal = false;
                }
            }

            if (!bGotoAbnormal)
            {
                endTimer.Close();
                bExit = (((Button)sender).Name == "YesAndExitBtn" ? true : false);
                EndOperation(bExit);
            }
            else
            {
                normalView.Visibility = Visibility.Collapsed;
                abnormalView.Visibility = Visibility.Visible;

                listView2.DataContext = bdCommodityDetail.body.objects;
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
            EnterReplenishmentDetailOpenEvent(this, shelfTask);
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
            if(isSuccess)
            {
                AbnormalCauses abnormalCauses;

                if ((bool)bthShortHide.IsChecked)
                    abnormalCauses=AbnormalCauses.商品缺失;
                else if ((bool)bthLossHide.IsChecked)
                    abnormalCauses = AbnormalCauses.商品遗失;
                else if ((bool)bthBadHide.IsChecked)
                    abnormalCauses = AbnormalCauses.商品遗失;
                else
                    abnormalCauses = AbnormalCauses.其他;

                BasePutData<ShelfTask> putData = ShelfBll.GetInstance().PutShelfTask(shelfTask, abnormalCauses);

				HttpHelper.GetInstance().ResultCheck(putData, out bool isSuccess1);
				if (!isSuccess1)
				{
					MessageBox.Show("更新上架任务单失败！" + putData.message, "温馨提示", MessageBoxButton.OK);
				}
				else
				{
					BasePostData<CommodityInventoryChange> basePostData = ShelfBll.GetInstance().CreateShelfTaskCommodityInventoryChange(bdCommodityCode, shelfTask);

					HttpHelper.GetInstance().ResultCheck(basePostData, out bool isSuccess2);

					if (!isSuccess2)
					{
						MessageBox.Show("创建上架任务单库存明细失败！" + putData.message, "温馨提示", MessageBoxButton.OK);
					}
				}
            }

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
        /// 操作损耗按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onShowBtnBad(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            bthBadHide.Visibility = (btn.Name == "bthBadShow" ? Visibility.Visible : Visibility.Collapsed);
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


        /// <summary>
        /// 返回按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onBackwords(object sender, RoutedEventArgs e)
        {
            normalView.Visibility = Visibility.Visible;
            abnormalView.Visibility = Visibility.Collapsed;
        }
    }
}
