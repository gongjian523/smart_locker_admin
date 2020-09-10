using CFLMedCab.Http.Bll;
using CFLMedCab.Http.Enum;
using CFLMedCab.Http.Helper;
using CFLMedCab.Http.Model;
using CFLMedCab.Http.Model.Base;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CFLMedCab.View.Recovery
{
    /// <summary>
    /// Recovery.xaml 的交互逻辑
    /// </summary>
    public partial class Recovery : UserControl
    {
        public delegate void EnterRecoveryDetailHandler(object sender, CommodityRecovery e);
        public event EnterRecoveryDetailHandler EnterRecoveryDetailEvent;

        public delegate void EnterRecoveryDetailOpenHandler(object sender, CommodityRecovery e);
        public event EnterRecoveryDetailOpenHandler EnterRecoveryDetailOpenEvent;

        //显示加载数据的进度条
        public delegate void LoadingDataHandler(object sender, bool e);
        public event LoadingDataHandler LoadingDataEvent;

        public Recovery()
        {
            InitializeComponent();
            DataContext = this;

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
                CommodityRecoveryList.Clear();

                LoadingDataEvent(this, true);
                BaseData<CommodityRecovery> bdCommodityRecovery = CommodityRecoveryBll.GetInstance().GetCommodityRecovery();
                LoadingDataEvent(this, false);

                HttpHelper.GetInstance().ResultCheck(bdCommodityRecovery, out bool isSuccess);
                if (!isSuccess)
                {
                    return;
                }

                List<CommodityRecovery> tasks = bdCommodityRecovery.body.objects;
                tasks.ForEach(task =>
                {
                    DateTime dt = Convert.ToDateTime(task.created_at);
                    task.created_at = dt.ToString("yyyy年MM月dd日");
                    CommodityRecoveryList.Add(task);
                });

                tbInputCode.Focus();
            }));
        }

        private ObservableCollection<CommodityRecovery> _commodityRecoveryView = new ObservableCollection<CommodityRecovery>();
        public ObservableCollection<CommodityRecovery> CommodityRecoveryList
        {
            get
            {
                return _commodityRecoveryView;
            }
            set
            {
                _commodityRecoveryView = value;
            }
        }

        /// <summary>
        /// 查看详情
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnterDetail_Click(object sender, RoutedEventArgs e)
        {
            var inputStr = tbInputCode.Text;
            if (string.IsNullOrWhiteSpace(inputStr))
            {
                MessageBox.Show("单号不可以为空！", "温馨提示", MessageBoxButton.OK);
                return;
            }

            TaskOrder taskOrder;
            string name;
            try
            {
                taskOrder = JsonConvert.DeserializeObject<TaskOrder>(inputStr);
                name = taskOrder.name;
            }
            catch
            {
                name = inputStr;
            }

            LoadingDataEvent(this, true);
            BaseData<CommodityRecovery> bdCommodityRecovery = CommodityRecoveryBll.GetInstance().GetCommodityRecovery(name.ToUpper());
            LoadingDataEvent(this, false);

			if (bdCommodityRecovery.code != (int)ResultCode.OK)
			{
                string waring;
                if(bdCommodityRecovery.code == (int)ResultCode.Parameter_Exception)
                {
                    waring = "回收取货单名字为空！";
                }
                else if(bdCommodityRecovery.code == (int)ResultCode.Task_Exception)
                {
                    waring = "此回收取货单已经完成，或者被撤销！";
                }
                else
                {
                    waring = "此回收取货单没有登记在您名下，或者不存在！";
                }

                MessageBox.Show(waring, "温馨提示", MessageBoxButton.OK);
				return;
			}

            EnterRecoveryDetailEvent(this, bdCommodityRecovery.body.objects[0]);
        }
        
        /// <summary>
        /// 扫码查询事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                EnterDetail_Click(this, null);
            }
        }


        /// <summary>
        /// 查看详情
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterDetail(object sender, RoutedEventArgs e)
        {
            CommodityRecovery commodityRecovery = (CommodityRecovery)((Button)sender).Tag;
            EnterRecoveryDetailEvent(this, commodityRecovery);
        }

        /// <summary>
        /// 确认开柜
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterDetailOpen(object sender, RoutedEventArgs e)
        {
            CommodityRecovery commodityRecovery = (CommodityRecovery)((Button)sender).Tag;
            EnterRecoveryDetailOpenEvent(this, commodityRecovery);
        }
    }
}
