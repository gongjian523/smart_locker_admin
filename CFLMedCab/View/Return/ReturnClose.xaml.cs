using CFLMedCab.BLL;
using CFLMedCab.DTO.Goodss;
using CFLMedCab.DTO.Stock;
using CFLMedCab.Http.Bll;
using CFLMedCab.Http.Enum;
using CFLMedCab.Http.Helper;
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

namespace CFLMedCab.View.Return
{
    /// <summary>
    /// NoSurgeryNumClose.xaml 的交互逻辑
    /// </summary>
    public partial class ReturnClose : UserControl
    {
        //回收取货开门事件
        public delegate void EnterReturnOpenHandler(object sender, CommodityRecovery e);
        public event EnterReturnOpenHandler EnterReturnOpenEvent;
            
        //库存调整开门事件
        public delegate void EnterStockSwitchOpenHandler(object sender, RoutedEventArgs e);
        public event EnterStockSwitchOpenHandler EnterStockSwitchOpenEvent;

        //跳出关闭弹出框
        public delegate void EnterPopCloseHandler(object sender, bool e);
        public event EnterPopCloseHandler EnterPopCloseEvent;

        private Timer endTimer;

        private bool isSuccess;

        private HashSet<CommodityEps> after;
        private BaseData<CommodityCode> bdCommodityCode;
        CommodityRecovery commodityRecovery;

        public ReturnClose(HashSet<CommodityEps> afterEps, CommodityRecovery order)
        {
            InitializeComponent();

            endTimer = new Timer(Contant.ClosePageEndTimer);
            endTimer.AutoReset = false;
            endTimer.Enabled = true;
            endTimer.Elapsed += new ElapsedEventHandler(onEndTimerExpired);

            time.Content = DateTime.Now.ToString("yyyy年MM月dd日");
            operatorName.Content = ApplicationState.GetUserInfo().name;
            lbTypeContent.Content = order == null ? "库存调整" : "回收下架";

            commodityRecovery = order;

            HashSet<CommodityEps> before = ApplicationState.GetGoodsInfo();
            after = afterEps;

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
                MessageBox.Show("获取商品信息错误！" + bdCommodityCode.message, "温馨提示", MessageBoxButton.OK);
                return;
            }

            listView.DataContext = bdCommodityCode.body.objects;
            outNum.Content = bdCommodityCode.body.objects.Where(item => item.operate_type == 0).Count();
            abnormalInNum.Content = bdCommodityCode.body.objects.Where(item => item.operate_type == 1).Count();

            //回收取货
            if (order != null)
            {
                bdCommodityCode.body.objects.Where(item => item.operate_type == 1).ToList().ForEach(it => {
                    it.AbnormalDisplay = AbnormalDisplay.异常.ToString();
                });

                if(bdCommodityCode.body.objects.Where(item => item.operate_type == 1).Count() > 0)
                {
                    normalBtmView.Visibility = Visibility.Collapsed;
                    abnormalBtmView.Visibility = Visibility.Visible;
                }
                else
                {
                    normalBtmView.Visibility = Visibility.Visible;
                    abnormalBtmView.Visibility = Visibility.Collapsed;
                }
            }
            //回收取货
            else
            {
                normalBtmView.Visibility = Visibility.Visible;
                abnormalBtmView.Visibility = Visibility.Collapsed;
            }
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

            if (commodityRecovery == null)
            {
                EnterStockSwitchOpenEvent(this, null);
            }
            else
            {
                EnterReturnOpenEvent(this, commodityRecovery);
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
                if (commodityRecovery == null)
                {
                    BasePostData<CommodityInventoryChange> bdCommodityInventoryChange = CommodityInventoryChangeBll.GetInstance().CreateCommodityInventoryChangeInStockChange(bdCommodityCode);

                    if (bdCommodityInventoryChange.code != 0)
                    {
                        MessageBox.Show("创建库存调整商品变更明细失败！" + bdCommodityInventoryChange.message, "温馨提示", MessageBoxButton.OK);
                    }

                    ConsumingBll.GetInstance().InsertLocalCommodityCodeInfo(bdCommodityCode, "IntentoryAdjust");
                }
                else
                {
                    BasePostData<CommodityInventoryChange> bdCommodityInventoryChange = CommodityInventoryChangeBll.GetInstance().CreateCommodityInventoryChange(bdCommodityCode, commodityRecovery);

                    if (bdCommodityInventoryChange.code != 0)
                    {
                        MessageBox.Show("创建库存调整商品变更明细失败！" + bdCommodityInventoryChange.message, "温馨提示", MessageBoxButton.OK);
                    }

                    ConsumingBll.GetInstance().InsertLocalCommodityCodeInfo(bdCommodityCode, "CommodityRecovery");
                }
            }

            ApplicationState.SetGoodsInfo(after);

            EnterPopCloseEvent(this, bExit);
        }
    }
}
