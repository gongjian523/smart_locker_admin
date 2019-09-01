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

namespace CFLMedCab.View.Allot
{
    /// <summary>
    /// AllotShelfClose.xaml 的交互逻辑
    /// </summary>
    public partial class AllotShelfClose : UserControl
    {
        //进入调拨上架详情开门状态页面
        public delegate void EnterAllotShelfDetailOpenHandler(object sender, AllotShelf e);
        public event EnterAllotShelfDetailOpenHandler EnterAllotShelfDetailOpenEvent;

        //跳出关闭弹出框
        public delegate void EnterPopCloseHandler(object sender, bool e);
        public event EnterPopCloseHandler EnterPopCloseEvent;

        //显示加载数据的进度条
        public delegate void LoadingDataHandler(object sender, bool e);
        public event LoadingDataHandler LoadingDataEvent;

        private AllotShelf allotShelf;
        private HashSet<CommodityEps> after;

        BaseData<CommodityCode> bdCommodityCode;
        BaseData<AllotShelfCommodity> bdCommodityDetail;

        private bool bExit;

        bool isSuccess;

        public AllotShelfClose(AllotShelf task, HashSet<CommodityEps> hs)
        {
            InitializeComponent();

            allotShelf = task;
            //操作人
            operatorName.Content = ApplicationState.GetUserInfo().name;
            ////工单号
            orderNum.Content = task.name;
            time.Content = DateTime.Now.ToString("yyyy年MM月dd日");
            after = hs;

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

                List<CommodityCode> commodityCodeList = CommodityCodeBll.GetInstance().GetCompareSimpleCommodity(before, after);
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

                bdCommodityDetail = AllotShelfBll.GetInstance().GetShelfTaskCommodityDetail(allotShelf);
                HttpHelper.GetInstance().ResultCheck(bdCommodityDetail, out isSuccess);
                if (!isSuccess)
                {
                    MessageBox.Show("获取拣货任务单商品明细信息错误！" + bdCommodityDetail.message, "温馨提示", MessageBoxButton.OK);
                    return;
                }

                LoadingDataEvent(this, true);
                AllotShelfBll.GetInstance().HandleAllotShelfChangeWithOrder(bdCommodityCode, allotShelf, bdCommodityDetail);
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
            //获取没有上架商品的信息

            List<string> codeIds = bdCommodityCode.body.objects.Select(item => item.id).ToList();

            List<AllotShelfCommodity> list = bdCommodityDetail.body.objects.Where(item => item.Status == "未上架" || !codeIds.Contains(item.CommodityCodeId)).ToList();

            //还有未上架的商品,让用户选择原因
            if(list.Count > 0)
            {
                normalView.Visibility = Visibility.Hidden;
                abnormalView.Visibility = Visibility.Visible;
            }
            else
            {
                BaseData<AllotShelfCommodity> bdAllotShelfCommodity = AllotShelfBll.GetInstance().GetShelfTaskAllCommodityDetail(allotShelf);

                string st;

                HttpHelper.GetInstance().ResultCheck(bdCommodityDetail, out bool isSuccess1);
                if (!isSuccess1)
                {
                    MessageBox.Show("获取拣货任务单商品明细信息错误！" + bdCommodityDetail.message, "温馨提示", MessageBoxButton.OK);
                    st = AllotShelfStatusEnum.异常.ToString();
                }
                else
                {
                    int cnt = bdAllotShelfCommodity.body.objects.Where(item => item.Status == "未上架" && item.EquipmentId != ApplicationState.GetEquipId()).Count();
                    if(cnt > 0)
                    {
                        st = AllotShelfStatusEnum.进行中.ToString(); 
                    }
                    else
                    {
                        st = AllotShelfStatusEnum.已完成.ToString(); ;
                    }
                }
               
                bExit = (((Button)sender).Name == "YesAndExitBtn" ? true : false);
                EndOperation(bExit, st);
            }
        }

       
        /// <summary>
        /// 继续操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onNoEndOperation(object sender, RoutedEventArgs e)
        {
            EnterAllotShelfDetailOpenEvent(this, allotShelf);
            return;
        }

        /// <summary>
        /// 长时间未操作界面
        /// </summary>        
        public void onExitTimerExpired()
        {
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                EndOperation(true, "异常",false);
            }));
        }

        /// <summary>
        /// 结束操作，包括主动提交和长时间未操作界面被动提交
        /// </summary>
        /// <param name="bExit">退出登陆还是回到首页</param>
        /// <param name="bAutoSubmit">是否是主动提交</param>
        private void EndOperation(bool bExit, string status, bool bAutoSubmit = true)
        {
            if(isSuccess)
            {
                LoadingDataEvent(this, true);
                BasePostData<CommodityInventoryChange> basePostData = AllotShelfBll.GetInstance().SubmitAllotShelfChangeWithOrder(bdCommodityCode, allotShelf, bdCommodityDetail);
                LoadingDataEvent(this, false);

                HttpHelper.GetInstance().ResultCheck(basePostData, out bool isSuccess1);

                if (!isSuccess1)
                {
                    if(bAutoSubmit)
                    {
                        MessageBox.Show("创建调拨上架任务单库存明细失败！" + basePostData.message, "温馨提示", MessageBoxButton.OK);
                    }
                }
                else
                {
                    AbnormalCauses abnormalCauses;

                    if ((bool)bthShortHide.IsChecked)
                        abnormalCauses = AbnormalCauses.商品缺失;
                    else if ((bool)bthLossHide.IsChecked)
                        abnormalCauses = AbnormalCauses.商品遗失;
                    else if ((bool)bthBadHide.IsChecked)
                        abnormalCauses = AbnormalCauses.商品损坏;
                    else if ((bool)bthOtherHide.IsChecked)
                        abnormalCauses = AbnormalCauses.其他;
                    else
                        abnormalCauses = AbnormalCauses.未选;


                    if (!bAutoSubmit)
                    {
                        allotShelf.Status = DocumentStatus.异常.ToString();
                    }
                    else
                    {
                        allotShelf.Status = status;
                        allotShelf.AbnormalCauses = abnormalCauses.ToString();
                    }

                    LoadingDataEvent(this, true);
                    BasePutData<AllotShelf> putData = AllotShelfBll.GetInstance().PutAllotShelf(allotShelf);
                    LoadingDataEvent(this, false);

                    HttpHelper.GetInstance().ResultCheck(putData, out bool isSuccess2);

                    if (!isSuccess2 && bAutoSubmit)
                    {
                        MessageBox.Show("更新挑拨上架任务单失败！" + putData.message, "温馨提示", MessageBoxButton.OK);
                    }
                }

                ConsumingBll.GetInstance().InsertLocalCommodityCodeInfo(bdCommodityCode, "AllotShelf");

            }

            ApplicationState.SetGoodsInfo(after);
            if(bAutoSubmit)
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
        /// 暂未完成按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onNotComplete(object sender, RoutedEventArgs e)
        {
            EndOperation(bExit,"进行中");
        }

        /// <summary>
        /// 异常提交
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onAbnormalSubmit(object sender, RoutedEventArgs e)
        {
            EndOperation(bExit, "异常");
        }
    }
}
