using CFLMedCab.BLL;
using CFLMedCab.Http.Bll;
using CFLMedCab.Http.Helper;
using CFLMedCab.Http.Model;
using CFLMedCab.Http.Model.Base;
using CFLMedCab.Http.Model.Enum;
using CFLMedCab.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;

namespace CFLMedCab.View.AllotReverseView
{
    /// <summary>
    /// AllotReverseClose.xaml 的交互逻辑
    /// </summary>
    public partial class AllotReverseClose : UserControl
    {
        //进入反向调拨详情开门状态页面
        public delegate void EnterAllotReverseDetailOpenHandler(object sender, AllotReverse e);
        public event EnterAllotReverseDetailOpenHandler EnterAllotReverseDetailOpenEvent;

        //跳出关闭弹出框
        public delegate void EnterPopCloseHandler(object sender, bool e);
        public event EnterPopCloseHandler EnterPopCloseEvent;

        //显示加载数据的进度条
        public delegate void LoadingDataHandler(object sender, bool e);
        public event LoadingDataHandler LoadingDataEvent;

        private AllotReverse allotReverse;
        private HashSet<CommodityEps> after;

        private BaseData<CommodityCode> bdCommodityCode;
        private BaseData<AllotReverseCommodity> bdCommodityDetail;

        private List<string> locCodes = new List<string>();

        private bool bExit;

        bool isSuccess;

        public AllotReverseClose(AllotReverse task, HashSet<CommodityEps> hs, List<string> rfidComs)
        {
            InitializeComponent();

            allotReverse = task;
            //操作人
            operatorName.Content = ApplicationState.GetUserInfo().name;
            ////工单号
            orderNum.Content = task.name;
            time.Content = DateTime.Now.ToString("yyyy年MM月dd日");
            after = hs;

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

                bdCommodityDetail = AllotReverseBll.GetInstance().GetAllotReverseCommodity(allotReverse);
                HttpHelper.GetInstance().ResultCheck(bdCommodityDetail, out isSuccess);
                if (!isSuccess)
                {
                    MessageBox.Show("获取拣货任务单商品明细信息错误！" + bdCommodityDetail.message, "温馨提示", MessageBoxButton.OK);
                    return;
                }

                LoadingDataEvent(this, true);
                bool  bAllowSubmit = AllotReverseBll.GetInstance().GetAllotReverseChange(bdCommodityCode, allotReverse, bdCommodityDetail);
                LoadingDataEvent(this, false);

                int outCnt = bdCommodityCode.body.objects.Where(item => item.operate_type == 0).ToList().Count;
                int abnormalOutCnt = bdCommodityCode.body.objects.Where(item => item.operate_type == 0 && item.AbnormalDisplay == "异常").ToList().Count;
                int abnormalInCnt = bdCommodityCode.body.objects.Where(item => item.operate_type == 1).ToList().Count;

                outNum.Content = outCnt;
                abnormalInNum.Content = abnormalInCnt;
                abnormalOutNum.Content = abnormalOutCnt;
                listView.DataContext = bdCommodityCode.body.objects;

                //没有异常品种的商品或者拿出商品才能进入正常的提交页面
                if (bAllowSubmit)
                {
                    normalBtmView.Visibility = Visibility.Visible;
                    abnormalBtmView.Visibility = Visibility.Collapsed;
                }
                else
                {
                    normalBtmView.Visibility = Visibility.Collapsed;
                    abnormalBtmView.Visibility = Visibility.Visible;
                }
            }));
        }

        /// <summary>
        /// 结束操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEndOperation(object sender, RoutedEventArgs e)
        {
            if (isSuccess)
            {
                //还有未上架的商品,让用户选择原因
                if (allotReverse.Status == AllotReverseStatus.进行中.ToString())
                {
                    normalView.Visibility = Visibility.Hidden;
                    abnormalView.Visibility = Visibility.Visible;

                    List<string> cids = bdCommodityCode.body.objects.Select(item => item.CommodityId).ToList();

                    list2View.DataContext = bdCommodityDetail.body.objects.Where(item => !cids.Contains(item.Commodity)).ToList();
                }
                else
                {
                    bExit = (((Button)sender).Name == "YesAndExitBtn" ? true : false);
                    EndOperation(bExit);
                }
            }
            else
            {
                bExit = (((Button)sender).Name == "YesAndExitBtn" ? true : false);
                EndOperation(bExit);
            } 
        }
        /// <summary>
        /// 继续操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onNoEndOperation(object sender, RoutedEventArgs e)
        {
            EnterAllotReverseDetailOpenEvent(this, allotReverse);
            return;
        }

        /// <summary>
        /// 长时间未操作界面
        /// </summary>        
        public void onExitTimerExpired()
        {
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                allotReverse.Status = AllotReverseStatus.异常.ToString();
                EndOperation(true, false);              
            }));
        }

        /// <summary>
        /// 结束操作，包括主动提交和长时间未操作界面被动提交
        /// </summary>
        /// <param name="bExit">退出登陆还是回到首页</param>
        /// <param name="bAutoSubmit">是否是主动提交</param>
        private void EndOperation(bool bExit, bool bAutoSubmit = true)
        {
            if (isSuccess)
            {
                LoadingDataEvent(this, true);
                BasePutData<AllotReverse> putData = AllotReverseBll.GetInstance().PutAllotReverse(allotReverse);
                LoadingDataEvent(this, false);

                HttpHelper.GetInstance().ResultCheck(putData, out bool isSuccess1);
                if (!isSuccess1)
                {
                    if(bAutoSubmit)
                    {
                        MessageBox.Show("更新反向调拨单失败！" + putData.message, "温馨提示", MessageBoxButton.OK);
                    }
                }
                else
                {
                    bool isSuccess3 = true;
                    string errInfo = "";

                    LoadingDataEvent(this, true);
                    BasePostData<Http.Model.AllotReverseDetail> bpdAllotReverseDetail = AllotReverseBll.GetInstance().CreateAllotReverseDetail(bdCommodityCode, allotReverse);
                    HttpHelper.GetInstance().ResultCheck(bpdAllotReverseDetail, out bool isSuccess2);
                    //存在入库商品时需要提交库存变更单
                    if(bdCommodityCode.body.objects.Where(item => item.operate_type == (int)OperateType.入库).Count() > 0)
                    {
                        BasePostData<CommodityInventoryChange> bpdCommodityInventoryChange = AllotReverseBll.GetInstance().CreateCommodityInventoryChange(bdCommodityCode, allotReverse);
                        HttpHelper.GetInstance().ResultCheck(bpdCommodityInventoryChange, out isSuccess3);
                        errInfo = bpdCommodityInventoryChange.message;
                    }
                    LoadingDataEvent(this, false);

                    if (bAutoSubmit)
                    {
                        if(!isSuccess2 && isSuccess3)
                        {
                            MessageBox.Show("创建反向调拨单明细失败！" + bpdAllotReverseDetail.message, "温馨提示", MessageBoxButton.OK);
                        }
                        else if(isSuccess2 && !isSuccess3)
                        {
                            MessageBox.Show("创建库存变更明细失败！" + errInfo, "温馨提示", MessageBoxButton.OK);
                        }
                        else if (!isSuccess2 && !isSuccess3)
                        {
                            MessageBox.Show("创建反向调拨单明细和库存变更明细失败！" + bpdAllotReverseDetail.message + "," + errInfo, "温馨提示", MessageBoxButton.OK);
                        }
                    }
                }
                ConsumingBll.GetInstance().InsertLocalCommodityCodeInfo(bdCommodityCode, "AllotReverse");
            }

            InOutRecordBll inOutBill = new InOutRecordBll();
            inOutBill.UpdateInOutRecord(isSuccess ? bdCommodityCode.body.objects : null, "AllotReverse");

            ApplicationState.SetGoodsInfoInSepcLoc(after,locCodes);
            ApplicationState.SetOpenDoorId(-1);

            if (bAutoSubmit)
            {
                EnterPopCloseEvent(this, bExit);
            }
        }

        /// <summary>
        /// 暂未完成按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onNotComplete(object sender, RoutedEventArgs e)
        {
            allotReverse.Status = AllotReverseStatus.进行中.ToString();
            EndOperation(bExit);
        }

        /// <summary>
        /// 异常提交
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onAbnormalSubmit(object sender, RoutedEventArgs e)
        {
            allotReverse.Status = AllotReverseStatus.异常.ToString();
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
