using CFLMedCab.BLL;
using CFLMedCab.DAL;
using CFLMedCab.DTO.Goodss;
using CFLMedCab.DTO.Stock;
using CFLMedCab.Http.Bll;
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
        public event EnterReturnFetchHandler EnterReturnFetch;

        private HashSet<CommodityEps> after;
        BaseData<CommodityCode> bdCommodityCode;

        private Timer endTimer;

        public ReturnFetchView(HashSet<CommodityEps> hashtable)
        {
            InitializeComponent();
            time.Content = DateTime.Now.ToString("yyyy年MM月dd日");
            operatorName.Content = ApplicationState.GetUserInfo().name;

            HashSet<CommodityEps> before = ApplicationState.GetGoodsInfo();
            after = hashtable;

            bdCommodityCode = CommodityCodeBll.GetInstance().GetCompareCommodity(before, after);
            if (bdCommodityCode.code != 0)
            {
                MessageBox.Show("没有检测到任何商品变化！", "温馨提示", MessageBoxButton.OK);
                return;
            }

            listView.DataContext = bdCommodityCode.body.objects;
            returnNum.Content = bdCommodityCode.body.objects.Where(item => item.operate_type==1).Count();
            fetchNum.Content = bdCommodityCode.body.objects.Where(item => item.operate_type == 0).Count();

            endTimer = new Timer(Contant.ClosePageEndTimer);
            endTimer.AutoReset = false;
            endTimer.Enabled = true;
            endTimer.Elapsed += new ElapsedEventHandler(onEndTimerExpired);
        }


        /// <summary>
        /// 不结束本次退回
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param> 
        public void onNoEndOperation(object sender, RoutedEventArgs e)
        {
            endTimer.Close();
            EnterReturnFetch(this, null);
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
            BasePostData<CommodityInventoryChange> bdCommodityInventoryChange 
                = CommodityInventoryChangeBll.GetInstance().CreateCommodityInventoryChange(bdCommodityCode, new SourceBill {
                    object_name = "ConsumingReturnOrder",
                    object_id = ""
                });

            ApplicationState.SetGoodsInfo(after);

            EnterPopCloseEvent(this, bExit);
        }
    }

}
