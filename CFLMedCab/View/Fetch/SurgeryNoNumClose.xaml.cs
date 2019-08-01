﻿using CFLMedCab.BLL;
using CFLMedCab.DTO.Goodss;
using CFLMedCab.DTO.Stock;
using CFLMedCab.Http.Bll;
using CFLMedCab.Http.Enum;
using CFLMedCab.Http.Model;
using CFLMedCab.Http.Model.Base;
using CFLMedCab.Http.Model.Common;
using CFLMedCab.Infrastructure;
using CFLMedCab.Model;
using CFLMedCab.Model.Constant;
using CFLMedCab.Model.Enum;
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
        public delegate void EnterPopCloseHandler(object sender, bool e);
        public event EnterPopCloseHandler EnterPopCloseEvent;

        private Timer endTimer;

        private HashSet<CommodityEps> after;
        private BaseData<CommodityCode> bdCommodityCode;
        private ConsumingOrder consumingOrder;
        private ConsumingOrderType consumingOrderType;

        public SurgeryNoNumClose(HashSet<CommodityEps> afterEps, ConsumingOrderType type, ConsumingOrder order)
        {
            InitializeComponent();
            time.Content = DateTime.Now.ToString("yyyy年MM月dd日");
            operatorName.Content = ApplicationState.GetUserInfo().name;

            HashSet<CommodityEps> before = ApplicationState.GetGoodsInfo();
            after = afterEps;

            consumingOrder = order;
            consumingOrderType = type;

            bdCommodityCode = CommodityCodeBll.GetInstance().GetCompareCommodity(before, afterEps);

            if (bdCommodityCode.code != 0)
            {
                MessageBox.Show("获取商品信息错误！", "温馨提示", MessageBoxButton.OK);
                return;
            }

            if (bdCommodityCode.body.objects.Count == 0)
            {
                MessageBox.Show("没有检测到商品变化！", "温馨提示", MessageBoxButton.OK);
                return;
            }

            listView.DataContext = bdCommodityCode.body.objects;
            lbTypeContent.Content = type;
            outNum.Content = bdCommodityCode.body.objects.Where(item => item.operate_type == 0).Count();
            abnormalInNum.Content = bdCommodityCode.body.objects.Where(item => item.operate_type == 1).Count();

            bdCommodityCode.body.objects.Where(item => item.operate_type == 1).ToList().ForEach(it => {
                it.AbnormalDisplay = AbnormalDisplay.异常.ToString();
            });

            endTimer = new Timer(Contant.ClosePageEndTimer);
            endTimer.AutoReset = false;
            endTimer.Enabled = true;
            endTimer.Elapsed += new ElapsedEventHandler(onEndTimerExpired);
        }

        /// <summary>
        /// 确认关柜
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
        /// 不关柜
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onNoEndOperation(object sender, RoutedEventArgs e)
        {
            endTimer.Close();
            EnterSurgeryNoNumOpenEvent(this, null);
        }

        /// <summary>
        /// 结束定时器超时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEndTimerExpired(object sender, ElapsedEventArgs e)
        {
            App.Current.Dispatcher.Invoke((Action)(() => {
                EndOperation(true);
            }));
        }

        private void EndOperation(bool bExit)
        {
            if (bdCommodityCode.code == 0)
            {
                if(bdCommodityCode.body.objects.Count > 0)
                {

                    if(consumingOrderType == ConsumingOrderType.手术领用)
                    {
                        BasePostData<CommodityInventoryChange> bdBasePostData =
                            ConsumingBll.GetInstance().SubmitConsumingChangeWithoutOrder(bdCommodityCode, ConsumingOrderType.手术领用);

                        if (bdBasePostData.code != 0)
                        {
                            MessageBox.Show("提交结果失败！" + bdBasePostData.message, "温馨提示", MessageBoxButton.OK);
                        }
                    }
                    else
                    {
                        BasePostData<CommodityInventoryChange> bdBasePostData =
                            ConsumingBll.GetInstance().SubmitConsumingChangeWithoutOrder(bdCommodityCode, ConsumingOrderType.医嘱处方领用,new SourceBill {
                                object_id = consumingOrder.id,
                                object_name = "PrescriptionOrder"
                            });

                        if (bdBasePostData.code != 0)
                        {
                            MessageBox.Show("提交结果失败！" + bdBasePostData.message, "温馨提示", MessageBoxButton.OK);
                        }

                    }
                    ConsumingBll.GetInstance().InsertLocalCommodityCodeInfo(bdCommodityCode, "ConsumingOrder");
                }
            }

            ApplicationState.SetGoodsInfo(after);

            EnterPopCloseEvent(this, bExit);
        }
    }
}
