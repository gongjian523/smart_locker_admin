using CFLMedCab.BLL;
using CFLMedCab.DAL;
using CFLMedCab.DTO.Goodss;
using CFLMedCab.DTO.Stock;
using CFLMedCab.Http.Bll;
using CFLMedCab.Http.Enum;
using CFLMedCab.Http.Helper;
using CFLMedCab.Http.Model;
using CFLMedCab.Http.Model.Base;
using CFLMedCab.Http.Model.Common;
using CFLMedCab.Infrastructure;
using CFLMedCab.Infrastructure.DeviceHelper;
using CFLMedCab.Infrastructure.ToolHelper;
using CFLMedCab.Model;
using CFLMedCab.Model.Constant;
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
    public partial class ReturnFetchView : UserControl
    {
        //跳出关闭弹出框
        public delegate void EnterPopCloseHandler(object sender, bool e);
        public event EnterPopCloseHandler EnterPopCloseEvent;

        public delegate void EnterReturnFetchHandler(object sender, RoutedEventArgs e);
        public event EnterReturnFetchHandler EnterReturnFetchEvent;

        //显示加载数据的进度条
        //public delegate void LoadingDataHandler(object sender, bool e);
        //public event LoadingDataHandler LoadingDataEvent;

        private HashSet<CommodityEps> after;
        BaseData<CommodityCode> bdCommodityCode;

        private Timer endTimer;

		private bool isSuccess;

		public ReturnFetchView(HashSet<CommodityEps> hashtable)
        {
            InitializeComponent();
            time.Content = DateTime.Now.ToString("yyyy年MM月dd日");
            operatorName.Content = ApplicationState.GetUserInfo().name;

            endTimer = new Timer(Contant.ClosePageEndTimer);
            endTimer.AutoReset = false;
            endTimer.Enabled = true;
            endTimer.Elapsed += new ElapsedEventHandler(onEndTimerExpired);

            HashSet<CommodityEps> before = ApplicationState.GetGoodsInfo();
            after = hashtable;

			List<CommodityCode> commodityCodeList = CommodityCodeBll.GetInstance().GetCompareSimpleCommodity(before, after);

			if (commodityCodeList == null || commodityCodeList.Count <= 0)
			{
				MessageBox.Show("没有检测到商品变化！", "温馨提示", MessageBoxButton.OK);
                isSuccess = false;
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

            listView.DataContext = bdCommodityCode.body.objects;
            returnNum.Content = bdCommodityCode.body.objects.Where(item => item.operate_type == 1).Count();
            fetchNum.Content = bdCommodityCode.body.objects.Where(item => item.operate_type == 0).Count();

            bdCommodityCode.body.objects.Where(item => item.operate_type == 0).ToList().ForEach(it =>
            {
                it.AbnormalDisplay = AbnormalDisplay.异常.ToString();
            });
            
        }


        /// <summary>
        /// 不结束本次退回
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param> 
        public void onNoEndOperation(object sender, RoutedEventArgs e)
        {
            endTimer.Close();
            EnterReturnFetchEvent(this, null);
        }

        /// <summary>
        /// 结束本次退回
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

			if (isSuccess)
			{
				BasePostData<CommodityInventoryChange> bdCommodityInventoryChange
					   = CommodityInventoryChangeBll.GetInstance().CreateCommodityInventoryChange(bdCommodityCode);


				//校验是否含有数据
				HttpHelper.GetInstance().ResultCheck(bdCommodityInventoryChange, out bool isSuccess1);

				if (!isSuccess1)
				{
					MessageBox.Show("提交结果失败！" + bdCommodityInventoryChange.message, "温馨提示", MessageBoxButton.OK);
				}

				ConsumingBll.GetInstance().InsertLocalCommodityCodeInfo(bdCommodityCode, "ConsumingOrder");
			}

            ApplicationState.SetGoodsInfo(after);

            EnterPopCloseEvent(this, bExit);
        }
    }

}
