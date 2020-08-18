using CFLMedCab.BLL;
using CFLMedCab.DAL;
using CFLMedCab.DTO.Replenish;
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

namespace CFLMedCab.View.ReplenishmentOrder
{
    /// <summary>
    /// ReplenishmentDetail.xaml 的交互逻辑
    /// </summary>
    public partial class ReplenishmentDetail : UserControl
    {
        //进入补货单详情开门状态页面
        public delegate void EnterReplenishmentDetailOpenHandler(object sender, ShelfTask e);
        public event EnterReplenishmentDetailOpenHandler EnterReplenishmentDetailOpenEvent;

        //进入补货单列表页面
        public delegate void EnterReplenishmentHandler(object sender, RoutedEventArgs e);
        public event EnterReplenishmentHandler EnterReplenishmentEvent;

        //显示加载数据的进度条
        public delegate void LoadingDataHandler(object sender, bool e);
        public event LoadingDataHandler LoadingDataEvent;

        ShelfTask entity;

        public ReplenishmentDetail(ShelfTask model)
        {
            InitializeComponent();
            entity = model;
            operatorName.Content = ApplicationState.GetUserInfo().name;
            orderNum.Content = model.name;

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
                BaseData<ShelfTaskDetail> commodityDetail = ShelfBll.GetInstance().GetShelfTaskDetail(entity);
                LoadingDataEvent(this, false);

                HttpHelper.GetInstance().ResultCheck(commodityDetail, out bool isSuccess);
                if (!isSuccess)
                {
                    MessageBox.Show("获取上架单商品明细错误！", "温馨提示", MessageBoxButton.OK);
                    return;
                }

                listView.DataContext = commodityDetail.body.objects;
            }));
        }

        /// <summary>
        /// 返回工单列表页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Backwords_Click(object sender, RoutedEventArgs e)
        {
            EnterReplenishmentEvent(this, null);
        }

        private void EnterDetialOpen_Click(object sender, RoutedEventArgs e)
        {
            EnterReplenishmentDetailOpenEvent(this, entity);
        }
    }
}
