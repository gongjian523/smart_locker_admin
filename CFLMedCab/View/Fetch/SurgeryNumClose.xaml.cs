using CFLMedCab.APO.Surgery;
using CFLMedCab.BLL;
using CFLMedCab.DTO;
using CFLMedCab.DTO.Fetch;
using CFLMedCab.DTO.Goodss;
using CFLMedCab.DTO.Stock;
using CFLMedCab.DTO.Surgery;
using CFLMedCab.Http.Bll;
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

        private FetchParam fetchParam;
        private HashSet<CommodityEps> after;
        private BaseData<CommodityCode> bdCommodityCode;

        public SurgeryNumClose(FetchParam param, HashSet<CommodityEps> afterEps)
        {
            InitializeComponent();

            fetchParam = param;

            operatorName.Content = ApplicationState.GetUserInfo().name;
            time.Content = DateTime.Now.ToString("yyyy年MM月dd日");
            type.Content = param.bdConsumingOrder.body.objects[0].Type;
            surgeryNum.Content = param.bdConsumingOrder.body.objects[0].name;

            HashSet<CommodityEps> before = ApplicationState.GetGoodsInfo();
            after = afterEps;

            bdCommodityCode = CommodityCodeBll.GetInstance().GetCompareCommodity(before, afterEps);

            if (bdCommodityCode.code != 0)
            {
                MessageBox.Show("获取商品信息错误！", "温馨提示", MessageBoxButton.OK);
                return;
            }

            if (bdCommodityCode.body.objects.Count == 0)
            {
                MessageBox.Show("没有检测到商品变化！", "温馨提示", MessageBoxButton.OK);
                return;
            }

            listView1.DataContext = bdCommodityCode.body.objects;

            //inNum.Content = currentOperateNum;//领用数
            //abnormalInNum.Content = storageOperateExNum;//异常入库
            //abnormalOutNum.Content = notStorageOperateExNum;//异常出库
            //waitNum.Content = notFetchGoodsNum;//待领用数

            endTimer = new Timer(Contant.ClosePageEndTimer);
            endTimer.AutoReset = false;
            endTimer.Enabled = true;
            endTimer.Elapsed += new ElapsedEventHandler(onEndTimerExpired);
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
            if(bdCommodityCode.code == 0)
            {
                if(bdCommodityCode.body.objects.Count > 0)
                {
                    BasePostData<CommodityInventoryChange> bdBasePostData =
                        ConsumingBll.GetInstance().SubmitConsumingChangeWithOrder(bdCommodityCode, fetchParam.bdConsumingOrder.body.objects[0]);

                    if (bdBasePostData.code != 0)
                    {
                        MessageBox.Show("提交结果失败！" + bdBasePostData.message, "温馨提示", MessageBoxButton.OK);
                    }

                    ConsumingBll.GetInstance().InsertLocalCommodityCodeInfo(bdCommodityCode, "ConsumingOrder");
                }
            }

            ApplicationState.SetGoodsInfo(after);
            EnterPopCloseEvent(this, bExit);
        }

        private void onConfirmed(object sender, RoutedEventArgs e)
        {

        }

    }
}
