﻿using CFLMedCab.BLL;
using CFLMedCab.DAL;
using CFLMedCab.DTO.Goodss;
using CFLMedCab.DTO.Picking;
using CFLMedCab.Http.Bll;
using CFLMedCab.Http.Enum;
using CFLMedCab.Http.Helper;
using CFLMedCab.Http.Model;
using CFLMedCab.Http.Model.Base;
using CFLMedCab.Infrastructure;
using CFLMedCab.Infrastructure.DeviceHelper;
using CFLMedCab.Model;
using CFLMedCab.Model.Constant;
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

namespace CFLMedCab.View.ShelfFast
{
    /// <summary>
    /// AllotShelfClose.xaml 的交互逻辑
    /// </summary>
    public partial class ShelfFastClose : UserControl
    {
        //进入调拨上架详情开门状态页面
        public delegate void EnterShelfFastDetailOpenHandler(object sender, ShelfTaskFast e);
        public event EnterShelfFastDetailOpenHandler EnterShelfFastDetailOpenEvent;

        //跳出关闭弹出框
        public delegate void EnterPopCloseHandler(object sender, bool e);
        public event EnterPopCloseHandler EnterPopCloseEvent;

        //显示加载数据的进度条
        public delegate void LoadingDataHandler(object sender, bool e);
        public event LoadingDataHandler LoadingDataEvent;

        private ShelfTaskFast shelfTaskFast;
        private HashSet<CommodityEps> after;

        private BaseData<CommodityCode> bdCommodityCode;
        private BaseData<ShelfTaskFastDetail> bdCommodityDetail;

        private List<string> locCodes = new List<string>();

        private bool bExit;

        bool isSuccess;

        public ShelfFastClose(ShelfTaskFast task, HashSet<CommodityEps> hs, List<string> rfidComs)
        {
            InitializeComponent();

            shelfTaskFast = task;
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

                bdCommodityDetail = ShelfFastBll.GetInstance().GetShelfTaskFastDetail(shelfTaskFast);
                HttpHelper.GetInstance().ResultCheck(bdCommodityDetail, out isSuccess);
                if (!isSuccess)
                {
                    MessageBox.Show("获取拣货任务单商品明细信息错误！" + bdCommodityDetail.message, "温馨提示", MessageBoxButton.OK);
                    return;
                }

                LoadingDataEvent(this, true);
                ShelfFastBll.GetInstance().GetShelfTaskChange(bdCommodityCode, shelfTaskFast, bdCommodityDetail);
                LoadingDataEvent(this, false);

                int inCnt = bdCommodityCode.body.objects.Where(item => item.operate_type == 1).ToList().Count;
                int abnormalOutCnt = bdCommodityCode.body.objects.Where(item => item.operate_type == 0 ).ToList().Count;
                int abnormalInCnt = bdCommodityCode.body.objects.Where(item => item.operate_type == 1 && item.AbnormalDisplay == "异常").ToList().Count;

                inNum.Content = inCnt;
                abnormalInNum.Content = abnormalInCnt;
                abnormalOutNum.Content = abnormalOutCnt;
                listView.DataContext = bdCommodityCode.body.objects;

                //没有异常商品才能进入提交页面
                if (abnormalInCnt == 0 && abnormalOutCnt == 0)
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
                if (shelfTaskFast.Status == AllotShelfStatusEnum.进行中.ToString())
                {
                    normalView.Visibility = Visibility.Hidden;
                    abnormalView.Visibility = Visibility.Visible;

                    List<string> codes = bdCommodityCode.body.objects.Select(item => item.name).ToList();

                    //list2View.DataContext = bdCommodityDetail.body.objects.Where(item => !codes.Contains(item.CommodityCodeName)).ToList();
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
            EnterShelfFastDetailOpenEvent(this, shelfTaskFast);
            return;
        }

        /// <summary>
        /// 长时间未操作界面
        /// </summary>        
        public void onExitTimerExpired()
        {
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                shelfTaskFast.Status = AllotShelfStatusEnum.异常.ToString();
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
                BasePostData<CommodityInventoryChange> basePostData = ShelfFastBll.GetInstance().CreateShelfTaskCommodityInventoryChange(bdCommodityCode, shelfTaskFast, bAutoSubmit);
                LoadingDataEvent(this, false);

                HttpHelper.GetInstance().ResultCheck(basePostData, out bool isSuccess1);

                if (!isSuccess1)
                {
                    if (bAutoSubmit)
                    {
                        MessageBox.Show("创建调拨上架任务单库存明细失败！" + basePostData.message, "温馨提示", MessageBoxButton.OK);
                    }
                }
                else
                {
                    shelfTaskFast.AbnormalCauses = abnormalOptions.GetAbnormal().ToString();

                    LoadingDataEvent(this, true);
                    BasePutData<ShelfTaskFast> putData = ShelfFastBll.GetInstance().PutShelfTaskFast(shelfTaskFast);
                    LoadingDataEvent(this, false);

                    HttpHelper.GetInstance().ResultCheck(putData, out bool isSuccess2);

                    if (!isSuccess2 && bAutoSubmit)
                    {
                        MessageBox.Show("更新挑拨上架任务单失败！" + putData.message, "温馨提示", MessageBoxButton.OK);
                    }
                }

                ConsumingBll.GetInstance().InsertLocalCommodityCodeInfo(bdCommodityCode, "AllotShelf",  out List<LocalCommodityCode> localCommodities);

                InOutRecordBll inOutBill = new InOutRecordBll();
                inOutBill.NewInOutRecord(localCommodities, "AllotShelf");
            }

            ApplicationState.SetGoodsInfoInSepcLoc(after,locCodes);

            if(bAutoSubmit)
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
            shelfTaskFast.Status = AllotShelfStatusEnum.进行中.ToString();
            EndOperation(bExit);
        }

        /// <summary>
        /// 异常提交
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onAbnormalSubmit(object sender, RoutedEventArgs e)
        {
            shelfTaskFast.Status = AllotShelfStatusEnum.异常.ToString();
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