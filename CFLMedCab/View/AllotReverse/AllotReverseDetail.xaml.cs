using CFLMedCab.Http.Bll;
using CFLMedCab.Http.Helper;
using CFLMedCab.Http.Model;
using CFLMedCab.Http.Model.Base;
using CFLMedCab.Infrastructure;
using System;
using System.Timers;
using System.Windows;
using System.Windows.Controls;

namespace CFLMedCab.View.AllotReverseView
{
    /// <summary>
    /// ReturnGoodsConfirm.xaml 的交互逻辑
    /// </summary>
    public partial class AllotReverseDetail : UserControl
    {
        //进入拣货单详情开门状态页面
        public delegate void EnterAllotReverseDetailOpenHandler(object sender, AllotReverse e);
        public event EnterAllotReverseDetailOpenHandler EnterAllotReverseDetailOpenEvent;

        //进入拣货单列表页面
        public delegate void EnterAllotReverseViewHandler(object sender, RoutedEventArgs e);
        public event EnterAllotReverseViewHandler EnterAllotReverseViewEvent;

        //显示加载数据的进度条
        public delegate void LoadingDataHandler(object sender, bool e);
        public event LoadingDataHandler LoadingDataEvent;

        private AllotReverse allotShelf;

        public AllotReverseDetail(AllotReverse task)
        {
            InitializeComponent();

            allotShelf = task;
            //操作人
            operatorName.Content = ApplicationState.GetUserInfo().name;
            //工单号
            orderNum.Content = task.name;

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
                LoadingDataEvent(this, true);
                BaseData<AllotReverseCommodity> bdAllotReverse = AllotReverseBll.GetInstance().GetAllotReverseCommodity(allotShelf);
                LoadingDataEvent(this, false);

                HttpHelper.GetInstance().ResultCheck(bdAllotReverse, out bool isSuccess);
                if (!isSuccess)
                {
                    MessageBox.Show("获取拣货单商品明细错误！", "温馨提示", MessageBoxButton.OK);
                    return;
                }

                listView.DataContext = bdAllotReverse.body.objects;
            }));
        }

        /// <summary>
        /// 返回工单列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onBackward(object sender, RoutedEventArgs e)
        {
            EnterAllotReverseViewEvent(this, null);
        }

        /// <summary>
        /// 确认开柜
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnerDetailOpen(object sender, RoutedEventArgs e)
        {
            EnterAllotReverseDetailOpenEvent(this, allotShelf);
        }
    }
}
