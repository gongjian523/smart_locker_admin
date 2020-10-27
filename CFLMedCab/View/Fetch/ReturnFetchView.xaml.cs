using CFLMedCab.BLL;
using CFLMedCab.Http.Bll;
using CFLMedCab.Http.Enum;
using CFLMedCab.Http.ExceptionApi;
using CFLMedCab.Http.Helper;
using CFLMedCab.Http.Model;
using CFLMedCab.Http.Model.Base;
using CFLMedCab.Infrastructure;
using CFLMedCab.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;

namespace CFLMedCab.View.Fetch
{
    /// <summary>
    /// GerFetchView.xaml 的交互逻辑
    /// </summary>
    public partial class ReturnFetchView : UserControl
    {
        //跳出关闭弹出框
        public delegate void EnterPopCloseHandler(object sender, bool e);
        public event EnterPopCloseHandler EnterPopCloseEvent;

        public delegate void EnterReturnFetchHandler(object sender, RoutedEventArgs e);
        public event EnterReturnFetchHandler EnterReturnFetchEvent;

        public delegate void ShowDepartChooseBoardHandler(object sender, RoutedEventArgs e);
        public event ShowDepartChooseBoardHandler ShowDepartChooseBoardEvent;

        //显示加载数据的进度条
        public delegate void LoadingDataHandler(object sender, bool e);
        public event LoadingDataHandler LoadingDataEvent;

        private HashSet<CommodityEps> after;
        BaseData<CommodityCode> bdCommodityCode;

        private List<string> locCodes = new List<string>();

        private bool isSuccess;

		public ReturnFetchView(HashSet<CommodityEps> hashtable, List<string> rfidComs)
        {
            InitializeComponent();
            time.Content = DateTime.Now.ToString("yyyy年MM月dd日");
            operatorName.Content = ApplicationState.GetUserInfo().name;
            after = hashtable;

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
        /// 加载数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onInitData(object sender, ElapsedEventArgs e)
        {
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                HashSet<CommodityEps> before = ApplicationState.GetGoodsInfo();

                LoadingDataEvent(this, true);
                List<CommodityCode> commodityCodeList = CommodityCodeBll.GetInstance().GetCompareSimpleCommodity(before, after, locCodes);
                LoadingDataEvent(this, false);

                if (commodityCodeList == null || commodityCodeList.Count <= 0)
                {
                    MessageBox.Show("没有检测到商品变化！", "温馨提示", MessageBoxButton.OK);
                    isSuccess = false;
                    return;
                }

                LoadingDataEvent(this, true);
                bdCommodityCode = CommodityCodeBll.GetInstance().GetCommodityCodeStock(commodityCodeList);
				ExStepHandle.ExApiSendQueueReturnGoodsInitDataHandle(bdCommodityCode, commodityCodeList);
				HttpHelper.GetInstance().ResultCheck(bdCommodityCode, out isSuccess);
                if (isSuccess)
                {
                    bdCommodityCode = CommodityCodeBll.GetInstance().GetQualityStatus(bdCommodityCode, out isSuccess);
					ExStepHandle.ExApiSendQueueReturnGoodsInitDataHandle(bdCommodityCode, commodityCodeList);
				}
                if (isSuccess)
                {
                    bdCommodityCode = CommodityCodeBll.GetInstance().GetDempartment(bdCommodityCode, out isSuccess);
                    ExStepHandle.ExApiSendQueueReturnGoodsInitDataHandle(bdCommodityCode, commodityCodeList);
                }
                LoadingDataEvent(this, false);

                //校验是否含有数据
                HttpHelper.GetInstance().ResultCheck(bdCommodityCode, out isSuccess);
                if (!isSuccess)
                {
                    MessageBox.Show("获取商品比较信息错误！" + bdCommodityCode.message, "温馨提示", MessageBoxButton.OK);
                    return;
                }

                listView.DataContext = bdCommodityCode.body.objects;
                returnNum.Content = bdCommodityCode.body.objects.Where(item => item.operate_type == 1).Count();
                int outNum = bdCommodityCode.body.objects.Where(item => item.operate_type == 0).Count();
                fetchNum.Content = outNum;
                int expiredCnt = bdCommodityCode.body.objects.Where(item => item.operate_type == 0 && item.QualityStatus == QualityStatusType.过期.ToString()).Count();

                expiredNum.Content = expiredCnt;

                bdCommodityCode.body.objects.Where(item => item.operate_type == 0).ToList().ForEach(it =>
                {
                    it.AbnormalDisplay = AbnormalDisplay.异常.ToString();
                });

                var department = ApplicationState.GetDepartInfo();
                //存在领用商品时，无法获取科室信息时，也不让用户提交
                if (outNum > 0 && department.body.objects.Count == 0)
                {
                    normalBtmView.Visibility = Visibility.Collapsed;
                    abnormalBtmView.Visibility = Visibility.Visible;
                    abnormalInfoLbl.Content = "无法获取您的科室信息！";

                    //用于强制提交
                    ApplicationState.SetFetchDepartment(new FetchDepartment() {
                        Id = "",
                        Name = "",
                    });
                }
                //领用产品上包含过期商品不让用户主动提交
                else if (expiredCnt > 0)
                {

                    //用于强制提交
                    ApplicationState.SetFetchDepartment(new FetchDepartment()
                    {
                        Id = "",
                        Name = "",
                    });
                    normalBtmView.Visibility = Visibility.Collapsed;
                    abnormalBtmView.Visibility = Visibility.Visible;
                    abnormalInfoLbl.Content = "请将过期商品取出！";
                }
                else
                {
                    normalBtmView.Visibility = Visibility.Visible;
                    abnormalBtmView.Visibility = Visibility.Collapsed;
                    //存在领用商品时
                    if (outNum > 0)
                    {
                        if (department.body.objects.Count == 1)
                        {
                            ApplicationState.SetFetchDepartment(new FetchDepartment()
                            {
                                Id = department.body.objects[0].id,
                                Name = department.body.objects[0].name,
                            });
                        }
                        else
                        {
                            ShowDepartChooseBoardEvent(this, null);
                        }
                    }
                }
            }));
        }

        /// <summary>
        /// 不结束本次退回
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param> 
        public void onNoEndOperation(object sender, RoutedEventArgs e)
        {
            EnterReturnFetchEvent(this, null);
        }

        /// <summary>
        /// 结束本次退回
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEndOperation(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            EndOperation(btn.Name == "YesAndExitBtn" ? true : false);
        }

        /// <summary>
        /// 长时间未操作界面
        /// </summary>        
        public void onExitTimerExpired()
        {
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                EndOperation(true,false);
            }));
        }

        /// <summary>
        /// 结束操作，包括主动提交和长时间未操作界面被动提交
        /// </summary>
        /// <param name="bExit">退出登录还是回到首页</param>
        /// <param name="bAutoSubmit">是否是主动提交</param>
        private void EndOperation(bool bExit, bool bAutoSubmit = true)
        {
			if (isSuccess)
			{
                LoadingDataEvent(this, true);
                BasePostData<CommodityInventoryChange> bdCommodityInventoryChange
					   = CommodityInventoryChangeBll.GetInstance().CreateCommodityInventoryChange(bdCommodityCode);
                LoadingDataEvent(this, false);

                //校验是否含有数据
                HttpHelper.GetInstance().ResultCheck(bdCommodityInventoryChange, out bool isSuccess1);

				if (!isSuccess1 && bAutoSubmit)
				{
					MessageBox.Show("提交结果失败！" + bdCommodityInventoryChange.message, "温馨提示", MessageBoxButton.OK);
				}

				ConsumingBll.GetInstance().InsertLocalCommodityCodeInfo(bdCommodityCode, "ConsumingReturnOrder");
            }

            InOutRecordBll inOutBill = new InOutRecordBll();
            inOutBill.UpdateInOutRecord(isSuccess ? bdCommodityCode.body.objects : null, "ConsumingReturnOrder");

            ApplicationState.SetGoodsInfoInSepcLoc(after, locCodes);
            ApplicationState.SetOpenDoorId(-1);

            //主动提交，需要发送退出事件
            if (bAutoSubmit)
            {
                EnterPopCloseEvent(this, bExit);
            }
        }
    }

}
