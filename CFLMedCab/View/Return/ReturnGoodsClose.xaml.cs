using CFLMedCab.BLL;
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

namespace CFLMedCab.View.Return
{
    /// <summary>
    /// ReturnGoodsClose.xaml 的交互逻辑
    /// </summary>
    public partial class ReturnGoodsClose : UserControl
    {
        //进入拣货单详情开门状态页面
        public delegate void EnterReturnGoodsDetailOpenHandler(object sender, PickTask e);
        public event EnterReturnGoodsDetailOpenHandler EnterReturnGoodsDetailOpenEvent;

        //跳出关闭弹出框
        public delegate void EnterPopCloseHandler(object sender, bool e);
        public event EnterPopCloseHandler EnterPopCloseEvent;

        //显示加载数据的进度条
        public delegate void LoadingDataHandler(object sender, bool e);
        public event LoadingDataHandler LoadingDataEvent;

        private PickTask pickTask;
        private HashSet<CommodityEps> after;

        BaseData<CommodityCode> bdCommodityCode;
        BaseData<PickCommodity> bdCommodityDetail;

        private List<string> locCodes = new List<string>();

        private bool bExit;

        bool isSuccess;

        public ReturnGoodsClose(PickTask task, HashSet<CommodityEps> hs, List<string> rfidComs)
        {
            InitializeComponent();

            pickTask = task;
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
                bdCommodityCode = CommodityCodeBll.GetInstance().GetCommodityCode(commodityCodeList);
                LoadingDataEvent(this, false);

                HttpHelper.GetInstance().ResultCheck(bdCommodityCode, out isSuccess);
                if (!isSuccess)
                {
                    MessageBox.Show("获取商品信息错误！" + bdCommodityCode.message, "温馨提示", MessageBoxButton.OK);
                    return;
                }

                bdCommodityDetail = PickBll.GetInstance().GetPickTaskCommodityDetail(pickTask);
                HttpHelper.GetInstance().ResultCheck(bdCommodityDetail, out isSuccess);
                if (!isSuccess)
                {
                    MessageBox.Show("获取拣货任务单商品明细信息错误！" + bdCommodityDetail.message, "温馨提示", MessageBoxButton.OK);
                    return;
                }

                LoadingDataEvent(this, true);
                PickBll.GetInstance().GetPickTaskChange(bdCommodityCode, pickTask, bdCommodityDetail);
                LoadingDataEvent(this, false);

                int outCnt = bdCommodityCode.body.objects.Where(item => item.operate_type == 0).ToList().Count;
                int abnormalOutCnt = bdCommodityCode.body.objects.Where(item => item.operate_type == 0 && item.AbnormalDisplay == "异常").ToList().Count;
                int abnormalInCnt = bdCommodityCode.body.objects.Where(item => item.operate_type == 1).ToList().Count;

                outNum.Content = outCnt;
                abnormalInNum.Content = abnormalInCnt;
                abnormalOutNum.Content = abnormalOutCnt;
                listView.DataContext = bdCommodityCode.body.objects;

                int abnormalLargeNum = bdCommodityDetail.body.objects.Where(item => item.CurPickNumber > (item.Number - item.PickNumber)).Count();

                if (abnormalInCnt == 0 && abnormalOutCnt == 0 && abnormalLargeNum == 0)
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
                //任务单里的商品拣货架全部完成
                if (bdCommodityDetail.body.objects.Where(item => (item.Number - item.PickNumber != item.CurPickNumber)).Count() == 0)
                {
                    //shelftask的状态在数据初始化的时候赋值
                    bExit = (((Button)sender).Name == "YesAndExitBtn" ? true : false);
                    EndOperation(bExit);
                }
                else
                {
                    normalView.Visibility = Visibility.Collapsed;
                    abnormalView.Visibility = Visibility.Visible;

                    listView2.DataContext = bdCommodityDetail.body.objects;
                }
            } 
        }

        /// <summary>
        /// 继续操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onNoEndOperation(object sender, RoutedEventArgs e)
        {         
            EnterReturnGoodsDetailOpenEvent(this, pickTask);
            return;
        }

        /// <summary>
        /// 长时间未操作界面
        /// </summary>        
        public void onExitTimerExpired()
        {
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                pickTask.BillStatus = DocumentStatus.异常.ToString();
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
                BasePostData<CommodityInventoryChange> basePostData = PickBll.GetInstance().CreatePickTaskCommodityInventoryChange(bdCommodityCode, pickTask, bAutoSubmit);
                LoadingDataEvent(this, false);

                HttpHelper.GetInstance().ResultCheck(basePostData, out bool isSuccess1);

                if (!isSuccess1)
                {
                    if(bAutoSubmit)
                    {
                        MessageBox.Show("创建取货任务单库存明细失败！" + basePostData.message, "温馨提示", MessageBoxButton.OK);
                    }
                }
                else
                {
                    pickTask.BillStatus = abnormalOptions.GetAbnormal();

                    LoadingDataEvent(this, true);
                    BasePutData<PickTask> putData = PickBll.GetInstance().PutPickTask(pickTask);
                    LoadingDataEvent(this, false);

                    HttpHelper.GetInstance().ResultCheck(putData, out bool isSuccess2);

                    if (!isSuccess2 && bAutoSubmit)
                    {
                        MessageBox.Show("更新取货任务单失败！" + putData.message, "温馨提示", MessageBoxButton.OK);
                    }
                }

                ConsumingBll.GetInstance().InsertLocalCommodityCodeInfo(bdCommodityCode, "PickTask");
            }

            ApplicationState.SetGoodsInfoInSepcLoc(after, locCodes);
            if(bAutoSubmit)
            {
                EnterPopCloseEvent(this, bExit);
            }
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

        private void AbnOptBoard_Loaded(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// 暂未完成按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onNotComplete(object sender, RoutedEventArgs e)
        {
            pickTask.BillStatus = DocumentStatus.进行中.ToString();
            EndOperation(bExit);
        }

        /// <summary>
        /// 异常提交
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onAbnormalSubmit(object sender, RoutedEventArgs e)
        {
            pickTask.BillStatus = DocumentStatus.异常.ToString();
            EndOperation(bExit);
        }
    }
}
