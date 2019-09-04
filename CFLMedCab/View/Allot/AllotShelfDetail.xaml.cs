using CFLMedCab.BLL;
using CFLMedCab.DAL;
using CFLMedCab.DTO.Picking;
using CFLMedCab.Http.Bll;
using CFLMedCab.Http.Helper;
using CFLMedCab.Http.Model;
using CFLMedCab.Http.Model.Base;
using CFLMedCab.Infrastructure;
using CFLMedCab.Model;
using System;
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
using System.Windows.Threading;

namespace CFLMedCab.View.Allot
{
    /// <summary>
    /// ReturnGoodsConfirm.xaml 的交互逻辑
    /// </summary>
    public partial class AllotShelfDetail : UserControl
    {
        //进入拣货单详情开门状态页面
        public delegate void EnterAllotShelfDetailOpenHandler(object sender, AllotShelf e);
        public event EnterAllotShelfDetailOpenHandler EnterAllotShelfDetailOpenEvent;

        //进入拣货单列表页面
        public delegate void EnterAllotShelfViewHandler(object sender, RoutedEventArgs e);
        public event EnterAllotShelfViewHandler EnterAllotShelfViewEvent;

        //显示加载数据的进度条
        public delegate void LoadingDataHandler(object sender, bool e);
        public event LoadingDataHandler LoadingDataEvent;

        private AllotShelf allotShelf;

        public AllotShelfDetail(AllotShelf task)
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
                BaseData<AllotShelfCommodity> bdAllotShelf = AllotShelfBll.GetInstance().GetShelfTaskCommodityDetail(allotShelf);
                LoadingDataEvent(this, false);

                HttpHelper.GetInstance().ResultCheck(bdAllotShelf, out bool isSuccess);
                if (!isSuccess)
                {
                    MessageBox.Show("获取拣货单商品明细错误！", "温馨提示", MessageBoxButton.OK);
                    return;
                }

                listView.DataContext = bdAllotShelf.body.objects;
            }));
        }

        /// <summary>
        /// 返回工单列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onBackward(object sender, RoutedEventArgs e)
        {
            EnterAllotShelfViewEvent(this, null);
        }

        /// <summary>
        /// 确认开柜
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnerDetailOpen(object sender, RoutedEventArgs e)
        {
            EnterAllotShelfDetailOpenEvent(this, allotShelf);
        }
    }
}
