using CFLMedCab.APO;
using CFLMedCab.BLL;
using CFLMedCab.DAL;
using CFLMedCab.DTO.Replenish;
using CFLMedCab.Http.Bll;
using CFLMedCab.Http.Helper;
using CFLMedCab.Http.Model;
using CFLMedCab.Http.Model.Base;
using CFLMedCab.Infrastructure.DeviceHelper;
using CFLMedCab.Infrastructure.ToolHelper;
using CFLMedCab.Model;
using Newtonsoft.Json;
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

namespace CFLMedCab.View.ReplenishmentOrder
{
    /// <summary>
    /// Replenishment.xaml 的交互逻辑
    /// </summary>
    public partial class Replenishment : UserControl
    {
        public delegate void EnterReplenishmentDetailHandler(object sender, ShelfTask e);
        public event EnterReplenishmentDetailHandler EnterReplenishmentDetailEvent;

        public delegate void EnterReplenishmentDetailOpenHandler(object sender, ShelfTask e);
        public event EnterReplenishmentDetailOpenHandler EnterReplenishmentDetailOpenEvent;

        //显示加载数据的进度条
        public delegate void LoadingDataHandler(object sender, bool e);
        public event LoadingDataHandler LoadingDataEvent;

        public Replenishment()
        {
            InitializeComponent();
            DataContext = this;

            Timer iniTimer = new Timer(100);
            iniTimer.AutoReset = false;
            iniTimer.Enabled = true;
            iniTimer.Elapsed += new ElapsedEventHandler(onInitData);
        }

        private ObservableCollection<ShelfTask> _replenishOrderView = new ObservableCollection<ShelfTask>();
        public ObservableCollection<ShelfTask> ReplenishOrderViewList
        {
            get
            {
                return _replenishOrderView;
            }
            set
            {
                _replenishOrderView = value;
            }
        }

        /// <summary>
        /// 数据加载
        /// </summary>
        private void onInitData(object sender, ElapsedEventArgs e)
        {
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                ReplenishOrderViewList.Clear();

                LoadingDataEvent(this, true);
                BaseData<ShelfTask> baseDataShelfTask = ShelfBll.GetInstance().GetShelfTask();
                LoadingDataEvent(this, false);

                HttpHelper.GetInstance().ResultCheck(baseDataShelfTask, out bool isSuccess);
                if (isSuccess)
                {
                    List<ShelfTask> tasks = baseDataShelfTask.body.objects;
                    tasks.ForEach(task =>
                    {
                        DateTime dt = Convert.ToDateTime(task.created_at);
                        task.created_at = dt.ToString("yyyy年MM月dd日");
                        ReplenishOrderViewList.Add(task);
                    });
                }

                tbInputNumbers.Focus();
            }));
        }
        
        /// <summary>
        /// 确认开柜
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterDetailOpen(object sender, RoutedEventArgs e)
        {
            ShelfTask shelfTask = (ShelfTask)((Button)sender).Tag;
            EnterReplenishmentDetailOpenEvent(this, shelfTask);
        }

        /// <summary>
        /// 查看详情
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterDetail(object sender, RoutedEventArgs e)
        {
            ShelfTask shelfTask = (ShelfTask)((Button)sender).Tag;
            EnterReplenishmentDetailEvent(this, shelfTask);
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
                MessageBox.Show("上架工单号不可以为空！", "温馨提示", MessageBoxButton.OK);
                return false;
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
            BaseData<ShelfTask> baseDataShelfTask = ShelfBll.GetInstance().GetShelfTask(name.ToUpper());
            LoadingDataEvent(this, false);

            HttpHelper.GetInstance().ResultCheck(baseDataShelfTask, out bool isSuccess);
            if (!isSuccess)
            {
                MessageBox.Show("此上架工单中商品已经领取完毕, 或没有登记在您名下，或者不存在！", "温馨提示", MessageBoxButton.OK);
                return false;
            }

            EnterReplenishmentDetailEvent(this, baseDataShelfTask.body.objects[0]);

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
