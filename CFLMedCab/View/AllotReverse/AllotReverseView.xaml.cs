using CFLMedCab.Http.Bll;
using CFLMedCab.Http.Helper;
using CFLMedCab.Http.Model;
using CFLMedCab.Http.Model.Base;
using CFLMedCab.Infrastructure.ToolHelper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CFLMedCab.View.AllotReverseView
{
    /// <summary>
    /// AllotReverseView.xaml 的交互逻辑
    /// </summary>
    public partial class AllotReverseView : UserControl
    {
        public delegate void EnterAllotReverseDetailHandler(object sender, AllotReverse e);
        public event EnterAllotReverseDetailHandler EnterAllotReverseDetailEvent;

        public delegate void EnterAllotReverseDetailOpenHandler(object sender, AllotReverse e);
        public event EnterAllotReverseDetailOpenHandler EnterAllotReverseDetailOpenEvent;

        //显示加载数据的进度条
        public delegate void LoadingDataHandler(object sender, bool e);
        public event LoadingDataHandler LoadingDataEvent;

        public AllotReverseView()
        {
            InitializeComponent();
            DataContext = this;

            Timer iniTimer = new Timer(100);
            iniTimer.AutoReset = false;
            iniTimer.Enabled = true;
            iniTimer.Elapsed += new ElapsedEventHandler(onInitData);
        }

        private ObservableCollection<AllotReverse> _allotReverseList = new ObservableCollection<AllotReverse>();
        public ObservableCollection<AllotReverse> AllotReverseList
        {
            get
            {
                return _allotReverseList;
            }
            set
            {
                _allotReverseList = value;
            }
        }

        /// <summary>
        /// 数据加载
        /// </summary>
        private void onInitData(object sender, ElapsedEventArgs e)
        {
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                AllotReverseList.Clear();

                LoadingDataEvent(this, true);
                BaseData<AllotReverse> bdAllotReverse = AllotReverseBll.GetInstance().GetAllotReverseTask();
                LoadingDataEvent(this, false);

                HttpHelper.GetInstance().ResultCheck(bdAllotReverse, out bool isSuccess);
                if (!isSuccess)
                {
                    return;
                }

                List<AllotReverse> tasks = bdAllotReverse.body.objects;
                tasks.ForEach(task =>
                {
                    DateTime dt = Convert.ToDateTime(task.created_at);
                    task.created_at = dt.ToString("yyyy年MM月dd日");
                    AllotReverseList.Add(task);
                });

                tbInputNumbers.Focus();
            }));
        }

        /// <summary>
        /// 查看详情
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterDetail(object sender, RoutedEventArgs e)
        {
            AllotReverse AllotReverse = (AllotReverse)((Button)sender).Tag;
            EnterAllotReverseDetailEvent(this, AllotReverse);
        }

        /// <summary>
        /// 确认开柜
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterDetailOpen(object sender, RoutedEventArgs e)
        {
            AllotReverse AllotReverse = (AllotReverse)((Button)sender).Tag;
            EnterAllotReverseDetailOpenEvent(this, AllotReverse);
        }

        /// <summary>
        /// 查看详情
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnterDetail_Click(object sender, RoutedEventArgs e)
        {
            //无法进入下一个页面，将输入框代码文字清空，并且重新设置焦点
            if (!HandleEnterDetail())
            {
                tbInputNumbers.Text = "";
                tbInputNumbers.Focus();
            }
        }

        /// <summary>
        /// 处理输入事件
        /// </summary>
        /// <returns></returns>
        private bool HandleEnterDetail()
        {
            string inputStr = tbInputNumbers.Text;
            if (string.IsNullOrWhiteSpace(inputStr))
            {
                MessageBox.Show("调拨上架工单号不可以为空！", "温馨提示", MessageBoxButton.OK);
                return false;
            }

            TaskOrder taskOrder;
            string name;
            try
            {
                taskOrder = JsonConvert.DeserializeObject<TaskOrder>(inputStr);
                name = taskOrder.name;
            }
            catch (Exception ex)
            {
                LogUtils.Error($"数据解析失败！{inputStr} ; 异常报错为：{ex.Message}");
                name = inputStr;
            }

            LoadingDataEvent(this, true);
            BaseData<AllotReverse> baseDataAllotReverse = AllotReverseBll.GetInstance().GetAllotReverseTask(name.ToUpper());
            LoadingDataEvent(this, false);

            HttpHelper.GetInstance().ResultCheck(baseDataAllotReverse, out bool isSuccess);
            if (!isSuccess)
            {
                MessageBox.Show("此反向调拨单中商品已经拣货完毕, 或没有登记在您名下，或者不存在！", "温馨提示", MessageBoxButton.OK);
                return false;
            }

            EnterAllotReverseDetailEvent(this, baseDataAllotReverse.body.objects[0]);

            return true;
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

    }
}
