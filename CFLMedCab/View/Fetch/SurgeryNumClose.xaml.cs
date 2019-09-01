using CFLMedCab.APO.Surgery;
using CFLMedCab.BLL;
using CFLMedCab.DTO;
using CFLMedCab.DTO.Fetch;
using CFLMedCab.DTO.Goodss;
using CFLMedCab.DTO.Stock;
using CFLMedCab.DTO.Surgery;
using CFLMedCab.Http.Bll;
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
    /// SurgeryNumClose.xaml 的交互逻辑
    /// </summary>
    public partial class SurgeryNumClose : UserControl
    {
        public delegate void EnterSurgeryNumOpenHandler(object sender, FetchParam fetchParam);
        public event EnterSurgeryNumOpenHandler EnterSurgeryNumOpenEvent;
        //跳出关闭弹出框
        public delegate void EnterPopCloseHandler(object sender, bool e);
        public event EnterPopCloseHandler EnterPopCloseEvent;

        //显示加载数据的进度条
        public delegate void LoadingDataHandler(object sender, bool e);
        public event LoadingDataHandler LoadingDataEvent;

        bool bExit;

        private FetchParam fetchParam;
        private HashSet<CommodityEps> after;
        private BaseData<CommodityCode> bdCommodityCode;

		private bool isSuccess;

		public SurgeryNumClose(FetchParam param, HashSet<CommodityEps> afterEps)
        {
            InitializeComponent();

            fetchParam = param;
            operatorName.Content = ApplicationState.GetUserInfo().name;
            time.Content = DateTime.Now.ToString("yyyy年MM月dd日");
            surgeryNum.Content = param.bdConsumingOrder.body.objects[0].name;
            after = afterEps;

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
                List<CommodityCode> commodityCodeList = CommodityCodeBll.GetInstance().GetCompareSimpleCommodity(before, after);
                LoadingDataEvent(this, false);

                if (commodityCodeList == null || commodityCodeList.Count <= 0)
                {
                    MessageBox.Show("没有检测到商品变化！", "温馨提示", MessageBoxButton.OK);
                    isSuccess = false;
                    return;
                }

                LoadingDataEvent(this, true);
                bdCommodityCode = CommodityCodeBll.GetInstance().GetCommodityCode(commodityCodeList);
                HttpHelper.GetInstance().ResultCheck(bdCommodityCode, out isSuccess);
                if (isSuccess)
                {
                    bdCommodityCode = CommodityCodeBll.GetInstance().GetQualityStatus(bdCommodityCode, out isSuccess);
                }
                LoadingDataEvent(this, false);

                //校验是否含有数据
                HttpHelper.GetInstance().ResultCheck(bdCommodityCode, out isSuccess);

                if (!isSuccess)
                {
                    MessageBox.Show("获取商品比较信息错误！" + bdCommodityCode.message, "温馨提示", MessageBoxButton.OK);
                    return;
                }
                ConsumingBll.GetInstance().GetOperationOrderChangeWithOrder(bdCommodityCode, fetchParam.bdConsumingOrder.body.objects[0], fetchParam.bdOperationOrderGoodsDetail);

                listView1.DataContext = bdCommodityCode.body.objects;

                outNum.Content = bdCommodityCode.body.objects.Where(item => item.operate_type == 0).Count();//领用数
                abnormalInNum.Content = bdCommodityCode.body.objects.Where(item => item.operate_type == 1).Count();//异常入库
                abnormalOutNum.Content = bdCommodityCode.body.objects.Where(item => item.operate_type == 0 && item.AbnormalDisplay == "异常").Count();//异常出库

                //领用产品上包含过期商品不让用户主动提交
                if (bdCommodityCode.body.objects.Where(item => item.operate_type == 0 && item.QualityStatus == QualityStatusType.过期.ToString()).Count() == 0)
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
            EnterSurgeryNumOpenEvent(this, fetchParam);
        }

        /// <summary>
        /// 结束本次领用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEndOperation(object sender, RoutedEventArgs e)
        {
            bExit = (((Button)sender).Name == "YesAndExitBtn" ? true : false);
            EndOperation(bExit);
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
        /// <param name="bExit">退出登陆还是回到首页</param>
        /// <param name="bAutoSubmit">是否是主动提交</param>
        private void EndOperation(bool bExit, bool bAutoSubmit = true)
        {
            if(isSuccess)
            {
                LoadingDataEvent(this, true);
                BasePostData<CommodityInventoryChange> bdBasePostData =
						ConsumingBll.GetInstance().SubmitConsumingChangeWithOrder(bdCommodityCode, fetchParam.bdConsumingOrder.body.objects[0]);
                LoadingDataEvent(this, false);

                //校验是否含有数据
                HttpHelper.GetInstance().ResultCheck(bdBasePostData, out bool isSuccess1);

				if (!isSuccess1 && bAutoSubmit)
				{
					MessageBox.Show("提交结果失败！" + bdBasePostData.message, "温馨提示", MessageBoxButton.OK);
				}

				ConsumingBll.GetInstance().InsertLocalCommodityCodeInfo(bdCommodityCode, "SurgeryConsumingOrder");
			}

            ApplicationState.SetGoodsInfo(after);

            //主动提交，需要发送退出事件
            if (bAutoSubmit)
            {
                EnterPopCloseEvent(this, bExit);
            }
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
            EndOperation(bExit);
        }

    }
}
