using CFLMedCab.BLL;
using CFLMedCab.DAL;
using CFLMedCab.DTO.Goodss;
using CFLMedCab.DTO.Stock;
using CFLMedCab.Http.Bll;
using CFLMedCab.Http.Enum;
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

        private Timer endTimer;

        private HashSet<CommodityEps> after;
        private BaseData<CommodityCode> bdCommodityCode;

        public GerFetchView(HashSet<CommodityEps> afterEps)
        {
            InitializeComponent();
            time.Content = DateTime.Now.ToString("yyyy年MM月dd日"); ;
            operatorName.Content = ApplicationState.GetUserInfo().name;

            HashSet<CommodityEps> before = ApplicationState.GetGoodsInfo();
            after = afterEps;

            bdCommodityCode = CommodityCodeBll.GetInstance().GetCompareCommodity(before, afterEps);

            listView.DataContext = bdCommodityCode.body.objects;
            normalNum.Content = bdCommodityCode.body.objects.Where(item => item.operate_type == 0).Count(); 
            abnormalNum.Content = bdCommodityCode.body.objects.Where(item => item.operate_type == 1).Count();

            bdCommodityCode.body.objects.Where(item => item.operate_type == 1).ToList().ForEach(it => {
                    it.AbnormalDisplay = AbnormalDisplay.异常.ToString();
            });


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
            EnterGerFetch(this, null);
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
            App.Current.Dispatcher.Invoke((Action)(()=> {
                EndOperation(true);
            }));
        }

        private void EndOperation(bool bExit)
        {
            if(bdCommodityCode.body.objects.Count > 0)
            {
                BasePostData<CommodityInventoryChange> bdBasePostData =
                    ConsumingBll.GetInstance().SubmitConsumingChangeWithoutOrder(bdCommodityCode, ConsumingOrderType.一般领用);

                if (bdBasePostData.code != 0)
                {
                    MessageBox.Show("提交结果失败！" + bdBasePostData.message, "温馨提示", MessageBoxButton.OK);
                }

                ConsumingBll.GetInstance().InsertLocalCommodityCodeInfo(bdCommodityCode, "ConsumingOrder");
            }

            ApplicationState.SetValue((int)ApplicationKey.CurGoods, after);
            EnterPopCloseEvent(this, bExit);
        }
    }

}
