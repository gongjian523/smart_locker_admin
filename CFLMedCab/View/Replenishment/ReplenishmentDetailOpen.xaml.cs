using CFLMedCab.BLL;
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

namespace CFLMedCab.View.ReplenishmentOrder
{
    /// <summary>
    /// ReplenishmentDetailOpen.xaml 的交互逻辑
    /// </summary>
    public partial class ReplenishmentDetailOpen : UserControl
    {
        //显示加载数据的进度条
        public delegate void LoadingDataHandler(object sender, bool e);
        public event LoadingDataHandler LoadingDataEvent;

        ShelfTask shelfTask;

        public ReplenishmentDetailOpen(ShelfTask model)
        {
            InitializeComponent();
            operatorName.Content = ApplicationState.GetUserInfo().name;
            orderNum.Content = model.name;
            shelfTask = model;

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
                BaseData<ShelfTaskCommodityDetail> commodityDetail = ShelfBll.GetInstance().GetShelfTaskCommodityDetail(shelfTask);
                LoadingDataEvent(this, false);

                HttpHelper.GetInstance().ResultCheck(commodityDetail, out bool isSuccess);
                if (!isSuccess)
                {
                    MessageBox.Show("发生错误！", "温馨提示", MessageBoxButton.OK);
                    return;
                }

                listView.DataContext = commodityDetail.body.objects;
            }));
        }
    }
}
