﻿using CFLMedCab.BLL;
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
    /// ReturnClose.xaml 的交互逻辑
    /// </summary>
    public partial class ReturnClose : UserControl
    {            
        //库存调整开门事件
        public delegate void EnterStockSwitchOpenHandler(object sender, RoutedEventArgs e);
        public event EnterStockSwitchOpenHandler EnterStockSwitchOpenEvent;

        //跳出关闭弹出框
        public delegate void EnterPopCloseHandler(object sender, bool e);
        public event EnterPopCloseHandler EnterPopCloseEvent;

        //显示加载数据的进度条
        public delegate void LoadingDataHandler(object sender, bool e);
        public event LoadingDataHandler LoadingDataEvent;

        private bool isSuccess;

        private HashSet<CommodityEps> after;
        private BaseData<CommodityCode> bdCommodityCode;

        private List<string> locCodes = new List<string>();

        public ReturnClose(HashSet<CommodityEps> afterEps, List<string> rfidComs)
        {
            InitializeComponent();

            time.Content = DateTime.Now.ToString("yyyy年MM月dd日");
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
                HttpHelper.GetInstance().ResultCheck(bdCommodityCode, out isSuccess);
                if (isSuccess)
                {
                    bdCommodityCode = CommodityCodeBll.GetInstance().GetQualityStatus(bdCommodityCode, out isSuccess);
                }
                LoadingDataEvent(this, false);

                HttpHelper.GetInstance().ResultCheck(bdCommodityCode, out isSuccess);
                if (!isSuccess)
                {
                    MessageBox.Show("获取商品信息错误！" + bdCommodityCode.message, "温馨提示", MessageBoxButton.OK);
                    return;
                }

                listView.DataContext = bdCommodityCode.body.objects;
                outNum.Content = bdCommodityCode.body.objects.Where(item => item.operate_type == 0).Count();
                abnormalInNum.Content = bdCommodityCode.body.objects.Where(item => item.operate_type == 1).Count();

                normalBtmView.Visibility = Visibility.Visible;
                abnormalBtmView.Visibility = Visibility.Collapsed;
            }));
        }

        /// <summary>
        /// 确认关柜
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEndOperation(object sender, RoutedEventArgs e)
        {
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
             EnterStockSwitchOpenEvent(this, null);
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
            if(isSuccess)
            {
                LoadingDataEvent(this, true);
                BasePostData<CommodityInventoryChange> bdCommodityInventoryChange = CommodityInventoryChangeBll.GetInstance().CreateCommodityInventoryChangeInStockChange(bdCommodityCode);
                LoadingDataEvent(this, false);

                if (bdCommodityInventoryChange.code != 0)
                {
                    MessageBox.Show("创建库存调整商品变更明细失败！" + bdCommodityInventoryChange.message, "温馨提示", MessageBoxButton.OK);
                }

                ConsumingBll.GetInstance().InsertLocalCommodityCodeInfo(bdCommodityCode, "IntentoryAdjust");

            }

            ApplicationState.SetGoodsInfoInSepcLoc(after, locCodes);

            EnterPopCloseEvent(this, bExit);
        }
    }
}
