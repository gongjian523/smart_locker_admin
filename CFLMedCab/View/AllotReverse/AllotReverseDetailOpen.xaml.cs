using CFLMedCab.Http.Bll;
using CFLMedCab.Http.Helper;
using CFLMedCab.Http.Model;
using CFLMedCab.Http.Model.Base;
using CFLMedCab.Infrastructure;
using CFLMedCab.View.Common;
using System;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;

namespace CFLMedCab.View.AllotReverseView
{
    /// <summary>
    /// AllotReverseDetailOpen.xaml 的交互逻辑
    /// </summary>
    public partial class AllotReverseDetailOpen : UserControl
    {
        //显示加载数据的进度条
        public delegate void LoadingDataHandler(object sender, bool e);
        public event LoadingDataHandler LoadingDataEvent;

        public OpenDoorBtnBoard openDoorBtnBoard = new OpenDoorBtnBoard();

        private AllotReverse allotReverse;

        public AllotReverseDetailOpen(AllotReverse task)
        {
            InitializeComponent();

            allotReverse = task;

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
                BaseData<AllotReverseCommodity> bdAllotReverse = AllotReverseBll.GetInstance().GetAllotReverseCommodity(allotReverse);
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

        public void onDoorClosed(string com)
        {
            openDoorBtnBoard.SetButtonEnable(true, com);
        }

        public AllotReverse GetAllotReverse()
        {
            return allotReverse;
        }
    }
}
