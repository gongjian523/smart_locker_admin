using CFLMedCab.BLL;
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

namespace CFLMedCab.View.Allot
{
    /// <summary>
    /// ReturnGoodsDetailOpen.xaml 的交互逻辑
    /// </summary>
    public partial class AllotShelfDetailOpen : UserControl
    {
        //显示加载数据的进度条
        public delegate void LoadingDataHandler(object sender, bool e);
        public event LoadingDataHandler LoadingDataEvent;

        public OpenDoorBtnBoard openDoorBtnBoard = new OpenDoorBtnBoard();

        private AllotShelf allotShelf;

        public AllotShelfDetailOpen(AllotShelf task)
        {
            InitializeComponent();
            ////操作人
            //operatorName.Content = ApplicationState.GetUserInfo().name;
            ////工单号
            //orderNum.Content = task.name;

            allotShelf = task;

            //只有一个柜门的时候，开门按钮不用显示，直接开门
            if (ApplicationState.GetAllLocIds().Count() == 1)
            {
                btnBorder.Visibility = Visibility.Collapsed;
            }
            else
            {
                btnBorder.Visibility = Visibility.Visible;
                btnGrid.Children.Add(openDoorBtnBoard);
            }


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

        public void onDoorClosed(string com)
        {
            openDoorBtnBoard.SetButtonEnable(true, com);
        }

        public AllotShelf GetShelfTask()
        {
            return allotShelf;
        }
    }
}
