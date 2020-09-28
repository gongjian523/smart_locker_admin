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

namespace CFLMedCab.View.ShelfFast
{
    /// <summary>
    /// ShelfFastDetail.xaml 的交互逻辑
    /// </summary>
    public partial class ShelfFastDetail : UserControl
    {
        //进入拣货单详情开门状态页面
        public delegate void EnterShelfFastDetailOpenHandler(object sender, ShelfTaskFast e);
        public event EnterShelfFastDetailOpenHandler EnterShelfFastDetailOpenEvent;

        //进入拣货单列表页面
        public delegate void EnterShelfFastViewHandler(object sender, RoutedEventArgs e);
        public event EnterShelfFastViewHandler EnterShelfFastViewEvent;

        //显示加载数据的进度条
        public delegate void LoadingDataHandler(object sender, bool e);
        public event LoadingDataHandler LoadingDataEvent;

        private ShelfTaskFast shelfTaskFast;

        public ShelfFastDetail(ShelfTaskFast task)
        {
            InitializeComponent();

            shelfTaskFast = task;
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
                BaseData<ShelfTaskFastDetail> bdShelfTaskFastDetail = ShelfFastBll.GetInstance().GetShelfTaskFastDetail(shelfTaskFast);
                LoadingDataEvent(this, false);

                HttpHelper.GetInstance().ResultCheck(bdShelfTaskFastDetail, out bool isSuccess);
                if (!isSuccess)
                {
                    MessageBox.Show("获取便捷上架任务单商品明细错误！", "温馨提示", MessageBoxButton.OK);
                    return;
                }

                listView.DataContext = bdShelfTaskFastDetail.body.objects;
            }));
        }

        /// <summary>
        /// 返回工单列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onBackward(object sender, RoutedEventArgs e)
        {
            EnterShelfFastViewEvent(this, null);
        }

        /// <summary>
        /// 确认开柜
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnerDetailOpen(object sender, RoutedEventArgs e)
        {
            EnterShelfFastDetailOpenEvent(this, shelfTaskFast);
        }
    }
}
