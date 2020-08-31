using CFLMedCab.BLL;
using CFLMedCab.Http.Bll;
using CFLMedCab.Http.Enum;
using CFLMedCab.Http.Helper;
using CFLMedCab.Http.Model;
using CFLMedCab.Http.Model.Base;
using CFLMedCab.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;

namespace CFLMedCab.View.Recovery
{
    /// <summary>
    /// ReturnClose.xaml 的交互逻辑
    /// </summary>
    public partial class RecoveryClose : UserControl
    {
        //回收取货开门事件
        public delegate void EnterRecoveryOpenHandler(object sender, CommodityRecovery e);
        public event EnterRecoveryOpenHandler EnterRecoveryOpenEvent;
            
        //跳出关闭弹出框
        public delegate void EnterPopCloseHandler(object sender, bool e);
        public event EnterPopCloseHandler EnterPopCloseEvent;

        //显示加载数据的进度条
        public delegate void LoadingDataHandler(object sender, bool e);
        public event LoadingDataHandler LoadingDataEvent;

        private bool isSuccess;

        private HashSet<CommodityEps> after;
        private BaseData<CommodityCode> bdCommodityCode;
        private CommodityRecovery commodityRecovery;

        private BaseData<CommodityRecoveryDetail> bdCommodityDetail;

        private List<string> locCodes = new List<string>();

        bool bExit;

        public RecoveryClose(HashSet<CommodityEps> afterEps, List<string> rfidComs, CommodityRecovery order)
        {
            InitializeComponent();

            time.Content = DateTime.Now.ToString("yyyy年MM月dd日");
            operatorName.Content = ApplicationState.GetUserInfo().name;
            orderNum.Content = order.name;

            commodityRecovery = order;
            after = afterEps;

            rfidComs.ForEach(com =>
            {
                locCodes.Add(ApplicationState.GetLocCodeByRFidCom(com));
            });

            Timer iniTimer = new Timer(100);
            iniTimer.AutoReset = false;
            iniTimer.Enabled = true;
            iniTimer.Elapsed += new ElapsedEventHandler(onInitData);
        }

        /// <summary>
        /// 数据加载
        /// </summary>
        private void onInitData(object sender, ElapsedEventArgs e)
        {
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                HashSet<CommodityEps> before = ApplicationState.GetGoodsInfo();

                List<CommodityCode> commodityCodeList = CommodityCodeBll.GetInstance().GetCompareSimpleCommodity(before, after, locCodes);
                if (commodityCodeList == null || commodityCodeList.Count <= 0)
                {
                    MessageBox.Show("没有检测到商品变化！", "温馨提示", MessageBoxButton.OK);
                    isSuccess = false;
                    return;
                }

                LoadingDataEvent(this, true);
                bdCommodityCode = CommodityCodeBll.GetInstance().GetCommodityCodeStock(commodityCodeList);
                LoadingDataEvent(this, false);

                HttpHelper.GetInstance().ResultCheck(bdCommodityCode, out isSuccess);
                if (!isSuccess)
                {
                    MessageBox.Show("获取商品信息错误！" + bdCommodityCode.message, "温馨提示", MessageBoxButton.OK);
                    return;
                }

                bdCommodityDetail = CommodityRecoveryBll.GetInstance().GetCommodityRecoveryDetail(commodityRecovery);
                HttpHelper.GetInstance().ResultCheck(bdCommodityDetail, out isSuccess);
                if (!isSuccess)
                {
                    MessageBox.Show("获取拣货任务单商品明细信息错误！" + bdCommodityDetail.message, "温馨提示", MessageBoxButton.OK);
                    return;
                }

                LoadingDataEvent(this, true);
                CommodityRecoveryBll.GetInstance().GetCommodityRecoveryChange(bdCommodityCode, commodityRecovery, bdCommodityDetail);
                LoadingDataEvent(this, false);

                listView.DataContext = bdCommodityCode.body.objects;
                outNum.Content = bdCommodityCode.body.objects.Where(item => item.operate_type == 0).Count();
                inNum.Content = bdCommodityCode.body.objects.Where(item => item.operate_type == 1).Count();

                if (bdCommodityCode.body.objects.Where(item => item.operate_type == 1).Count() > 0)
                {
                    normalBtmView.Visibility = Visibility.Collapsed;
                    abnormalBtmView.Visibility = Visibility.Visible;
                }
                else
                {
                    normalBtmView.Visibility = Visibility.Visible;
                    abnormalBtmView.Visibility = Visibility.Collapsed;
                }

            }));
        }

        /// <summary>
        /// 确认关柜
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEndOperation(object sender, RoutedEventArgs e)
        {
            if (isSuccess)
            {
                bExit = (((Button)sender).Name == "YesAndExitBtn" ? true : false);

                //还有未上架的商品,让用户选择原因
                if (commodityRecovery.Status == CommodityRecoveryStatusEnum.进行中.ToString())
                {
                    normalView.Visibility = Visibility.Hidden;
                    abnormalView.Visibility = Visibility.Visible;

                    List<string> codes = bdCommodityCode.body.objects.Select(item => item.name).ToList();
                    list2View.DataContext = bdCommodityDetail.body.objects.Where(item => !codes.Contains(item.CommodityCodeName)).ToList();
                }
                else
                {
                    EndOperation(bExit);
                }
            }
            else
            {
                bExit = (((Button)sender).Name == "YesAndExitBtn" ? true : false);
                EndOperation(bExit);
            }

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
            EnterRecoveryOpenEvent(this, commodityRecovery);
        }

        /// <summary>
        /// 暂未完成按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onNotComplete(object sender, RoutedEventArgs e)
        {
            EndOperation(bExit);
        }

        /// <summary>
        /// 异常提交
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onAbnormalSubmit(object sender, RoutedEventArgs e)
        {
            commodityRecovery.Status = CommodityRecoveryStatusEnum.异常.ToString();
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

        /// <summary>
        /// 长时间未操作界面
        /// </summary>        
        public void onExitTimerExpired()
        {
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                commodityRecovery.Status = CommodityRecoveryStatusEnum.异常.ToString();
                EndOperation(true, false);
            }));
        }

        /// <summary>
        /// 结束操作，包括主动提交和长时间未操作界面被动提交
        /// </summary>
        /// <param name="bExit">退出登录还是回到首页</param>
        /// <param name="bAutoSubmit">是否是主动提交</param>
        private void EndOperation(bool bExit, bool bAutoSubmit = true)
        {
            if(isSuccess)
            {
                LoadingDataEvent(this, true);
                BasePostData<CommodityInventoryChange> bdCommodityInventoryChange = CommodityInventoryChangeBll.GetInstance().CreateCommodityInventoryChange(bdCommodityCode, commodityRecovery, bAutoSubmit);
                LoadingDataEvent(this, false);

                if (bdCommodityInventoryChange.code != 0)
                {
                    MessageBox.Show("创建回收取货商品变更明细失败！" + bdCommodityInventoryChange.message, "温馨提示", MessageBoxButton.OK);
                }
                else
                {
                    LoadingDataEvent(this, true);
                    BasePutData<CommodityRecovery> putData = CommodityRecoveryBll.GetInstance().PutCommodityRecovery(commodityRecovery);
                    LoadingDataEvent(this, false);

                    HttpHelper.GetInstance().ResultCheck(putData, out bool isSuccess1);

                    if (!isSuccess1)
                    {
                        MessageBox.Show("更新回收取货单失败！" + putData.message, "温馨提示", MessageBoxButton.OK);
                    }
                }

                ConsumingBll.GetInstance().InsertLocalCommodityCodeInfo(bdCommodityCode, "CommodityRecovery");
            }

            InOutRecordBll inOutBill = new InOutRecordBll();
            inOutBill.UpdateInOutRecord(isSuccess ? bdCommodityCode.body.objects : null, "CommodityRecovery");

            ApplicationState.SetGoodsInfoInSepcLoc(after, locCodes);
            ApplicationState.SetOpenDoorId(-1);

            if (bAutoSubmit)
            {
                EnterPopCloseEvent(this, bExit);
            }
        }
    }
}
