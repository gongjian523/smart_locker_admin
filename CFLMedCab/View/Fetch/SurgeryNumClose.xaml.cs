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

        private Timer endTimer;
        bool bExit;

        private FetchParam fetchParam;
        private HashSet<CommodityEps> after;
        private BaseData<CommodityCode> bdCommodityCode;

		private bool isSuccess;

		public SurgeryNumClose(FetchParam param, HashSet<CommodityEps> afterEps)
        {
            InitializeComponent();

            fetchParam = param;

            endTimer = new Timer(Contant.ClosePageEndTimer);
            endTimer.AutoReset = false;
            endTimer.Enabled = true;
            endTimer.Elapsed += new ElapsedEventHandler(onEndTimerExpired);

            operatorName.Content = ApplicationState.GetUserInfo().name;
            time.Content = DateTime.Now.ToString("yyyy年MM月dd日");
            type.Content = param.bdConsumingOrder.body.objects[0].Type;
            surgeryNum.Content = param.bdConsumingOrder.body.objects[0].name;

            HashSet<CommodityEps> before = ApplicationState.GetGoodsInfo();
            after = afterEps;

			List<CommodityCode> commodityCodeList = CommodityCodeBll.GetInstance().GetCompareSimpleCommodity(before, after);

			if (commodityCodeList == null || commodityCodeList.Count <= 0)
			{
				MessageBox.Show("没有检测到商品变化！", "温馨提示", MessageBoxButton.OK);
				return;
			}

			bdCommodityCode = CommodityCodeBll.GetInstance().GetCommodityCode(commodityCodeList);

			//校验是否含有数据
			HttpHelper.GetInstance().ResultCheck(bdCommodityCode, out isSuccess);

			if (!isSuccess)
			{
				MessageBox.Show("获取商品比较信息错误！" + bdCommodityCode.message, "温馨提示", MessageBoxButton.OK);
				return;
			}
			ConsumingBll.GetInstance().GetOperationOrderChangeWithOrder(bdCommodityCode, fetchParam.bdConsumingOrder.body.objects[0], fetchParam.bdOperationOrderGoodsDetail);

            listView1.DataContext = bdCommodityCode.body.objects;

            //inNum.Content = currentOperateNum;//领用数
            //abnormalInNum.Content = storageOperateExNum;//异常入库
            //abnormalOutNum.Content = notStorageOperateExNum;//异常出库
            //waitNum.Content = notFetchGoodsNum;//待领用数
        }

        /// <summary>
        /// 不结束本次领用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param> 
        public void onNoEndOperation(object sender, RoutedEventArgs e)
        {
            endTimer.Close();
            EnterSurgeryNumOpenEvent(this, fetchParam);
        }

        /// <summary>
        /// 结束本次领用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEndOperation(object sender, RoutedEventArgs e)
        {
            if(true)
            { 
                endTimer.Close();

                bExit = (((Button)sender).Name == "YesAndExitBtn" ? true : false);
                EndOperation(bExit);
            }
            else
            {
                //endTimer.Close();
                //endTimer.Start();
                //normalView.Visibility = Visibility.Collapsed;
                //abnormalView.Visibility = Visibility.Visible;

                //listView2.DataContext = bdCommodityDetail.body.objects;
            }
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
            if(isSuccess)
            {
				BasePostData<CommodityInventoryChange> bdBasePostData =
						ConsumingBll.GetInstance().SubmitConsumingChangeWithOrder(bdCommodityCode, fetchParam.bdConsumingOrder.body.objects[0]);

				//校验是否含有数据
				HttpHelper.GetInstance().ResultCheck(bdBasePostData, out bool isSuccess);

				if (!isSuccess)
				{
					MessageBox.Show("提交结果失败！" + bdBasePostData.message, "温馨提示", MessageBoxButton.OK);
				}

				ConsumingBll.GetInstance().InsertLocalCommodityCodeInfo(bdCommodityCode, "ConsumingOrder");
			}

            ApplicationState.SetGoodsInfo(after);
            EnterPopCloseEvent(this, bExit);
        }

        private void onConfirmed(object sender, RoutedEventArgs e)
        {

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
