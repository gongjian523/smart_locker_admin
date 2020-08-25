using CFLMedCab.BLL;
using CFLMedCab.DAL;
using CFLMedCab.DTO.Goodss;
using CFLMedCab.DTO.Stock;
using CFLMedCab.Http.Bll;
using CFLMedCab.Http.Enum;
using CFLMedCab.Http.ExceptionApi;
using CFLMedCab.Http.Helper;
using CFLMedCab.Http.Model;
using CFLMedCab.Http.Model.Base;
using CFLMedCab.Infrastructure;
using CFLMedCab.Infrastructure.DeviceHelper;
using CFLMedCab.Infrastructure.ToolHelper;
using CFLMedCab.Model;
using CFLMedCab.Model.Constant;
using CFLMedCab.Model.Enum;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// GerFetchView.xaml 的交互逻辑
    /// </summary>
    public partial class GerFetchView : UserControl
    {
        public delegate void EnterFetchOpenHandler(object sender, RoutedEventArgs e);
        public event EnterFetchOpenHandler EnterGerFetch;
        //跳出关闭弹出框
        public delegate void EnterPopCloseHandler(object sender, bool e);
        public event EnterPopCloseHandler EnterPopCloseEvent;

        //显示加载数据的进度条
        public delegate void LoadingDataHandler(object sender, bool e);
        public event LoadingDataHandler LoadingDataEvent;

        private HashSet<CommodityEps> after;
        private BaseData<CommodityCode> bdCommodityCode;

        private List<string> locCodes = new List<string>();

        private bool isSuccess;

		public GerFetchView(HashSet<CommodityEps> afterEps, List<string> rfidComs)
        {
            InitializeComponent();
            time.Content = DateTime.Now.ToString("yyyy年MM月dd日"); ;
            operatorName.Content = ApplicationState.GetUserInfo().name;
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

				ExStepHandle.ExApiSendQueueFetchGoodsInitDataHandle(bdCommodityCode, commodityCodeList);

				HttpHelper.GetInstance().ResultCheck(bdCommodityCode, out isSuccess);
                if (isSuccess)
                {
                    bdCommodityCode = CommodityCodeBll.GetInstance().GetQualityStatus(bdCommodityCode, out isSuccess);
					ExStepHandle.ExApiSendQueueFetchGoodsInitDataHandle(bdCommodityCode, commodityCodeList);
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
                outNum.Content = bdCommodityCode.body.objects.Where(item => item.operate_type == 0).Count();
                int abnormalOut = bdCommodityCode.body.objects.Where(item => item.operate_type == 0 && item.QualityStatus == QualityStatusType.过期.ToString()).Count();
                int abnormalIn  = bdCommodityCode.body.objects.Where(item => item.operate_type == 1).Count();

                abnormalOutNum.Content = abnormalOut;
                abnormalInNum.Content = abnormalIn;

                bdCommodityCode.body.objects.ToList().ForEach(it =>
                {
                    if(it.operate_type == 1 || it.operate_type == 0 && it.QualityStatus == QualityStatusType.过期.ToString())
                    {
                        it.AbnormalDisplay = AbnormalDisplay.异常.ToString();
                    }
                });

                //领用产品上包含过期商品不让用户主动提交
                if (abnormalOut == 0)
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
        /// 不结束本次领用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param> 
        public void onNoEndOperation(object sender, RoutedEventArgs e)
        { 
            EnterGerFetch(this, null);
        }

        /// <summary>
        /// 结束本次领用
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
            if (isSuccess)
            {
                LoadingDataEvent(this, true);
                BasePostData<CommodityInventoryChange> bdBasePostData =
                        ConsumingBll.GetInstance().SubmitConsumingChangeWithoutOrder(bdCommodityCode, ConsumingOrderType.一般领用);
                LoadingDataEvent(this, false);

                //校验是否含有数据
                HttpHelper.GetInstance().ResultCheck(bdBasePostData, out bool isSuccess1);
                if (!isSuccess1 && bAutoSubmit)
                {
                    MessageBox.Show("提交结果失败！" + bdBasePostData.message, "温馨提示", MessageBoxButton.OK);

                }

                ConsumingBll.GetInstance().InsertLocalCommodityCodeInfo(bdCommodityCode, "ConsumingOrder");
            }

            InOutRecordBll inOutBill = new InOutRecordBll();
            inOutBill.UpdateInOutRecord(isSuccess ? bdCommodityCode.body.objects : null, "ConsumingOrder");

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
