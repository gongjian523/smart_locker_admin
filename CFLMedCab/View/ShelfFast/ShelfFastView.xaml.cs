using CFLMedCab.BLL;
using CFLMedCab.DAL;
using CFLMedCab.DTO.Picking;
using CFLMedCab.Http.Bll;
using CFLMedCab.Http.Helper;
using CFLMedCab.Http.Model;
using CFLMedCab.Http.Model.Base;
using CFLMedCab.Infrastructure.ToolHelper;
using CFLMedCab.Model;
using Newtonsoft.Json;
using System;
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

namespace CFLMedCab.View.ShelfFast
{
    /// <summary>
    /// ReturnGoods.xaml 的交互逻辑
    /// </summary>
    public partial class ShelfFastView : UserControl
    {
        public delegate void EnterShelfFastDetailHandler(object sender, ShelfTaskFast e);
        public event EnterShelfFastDetailHandler EnterShelfFastDetailEvent;

        public delegate void EnterAllotShelfDetailOpenHandler(object sender, ShelfTaskFast e);
        public event EnterAllotShelfDetailOpenHandler EnterAllotShelfDetailOpenEvent;

        //显示加载数据的进度条
        public delegate void LoadingDataHandler(object sender, bool e);
        public event LoadingDataHandler LoadingDataEvent;

        public ShelfFastView()
        {
            InitializeComponent();
            DataContext = this;

            Timer iniTimer = new Timer(100);
            iniTimer.AutoReset = false;
            iniTimer.Enabled = true;
            iniTimer.Elapsed += new ElapsedEventHandler(onInitData);
        }

        private ObservableCollection<AllotShelf> _allotShelfView = new ObservableCollection<AllotShelf>();
        public ObservableCollection<AllotShelf> AllotShelfList
        {
            get
            {
                return _allotShelfView;
            }
            set
            {
                _allotShelfView = value;
            }
        }

        /// <summary>
        /// 数据加载
        /// </summary>
        private void onInitData(object sender, ElapsedEventArgs e)
        {
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                AllotShelfList.Clear();

                LoadingDataEvent(this, true);
                BaseData<AllotShelf> baseDataAllotShelf = AllotShelfBll.GetInstance().GetAllotShelfTask();
                LoadingDataEvent(this, false);

                HttpHelper.GetInstance().ResultCheck(baseDataAllotShelf, out bool isSuccess);
                if (!isSuccess)
                {
                    //MessageBox.Show("此拣货工单中失败！", "温馨提示", MessageBoxButton.OK);
                    return;
                }

                List<AllotShelf> tasks = baseDataAllotShelf.body.objects;
                tasks.ForEach(task =>
                {
                    DateTime dt = Convert.ToDateTime(task.created_at);
                    task.created_at = dt.ToString("yyyy年MM月dd日");
                    AllotShelfList.Add(task);
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
            ShelfTaskFast shelfTaskFast = (ShelfTaskFast)((Button)sender).Tag;
            EnterShelfFastDetailEvent(this, shelfTaskFast);
        }

        /// <summary>
        /// 确认开柜
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterDetailOpen(object sender, RoutedEventArgs e)
        {
            ShelfTaskFast shelfTaskFast = (ShelfTaskFast)((Button)sender).Tag;
            EnterAllotShelfDetailOpenEvent(this, shelfTaskFast);
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
            BaseData<ShelfTaskFast> baseDataShelfTaskFast = ShelfFastBll.GetInstance().GetShelfTaskFast(name.ToUpper());
            LoadingDataEvent(this, false);

            HttpHelper.GetInstance().ResultCheck(baseDataShelfTaskFast, out bool isSuccess);
            if (!isSuccess)
            {
                MessageBox.Show("此调拨上架工单中商品已经领取完毕, 或没有登记在您名下，或者不存在！", "温馨提示", MessageBoxButton.OK);
                return false;
            }

            EnterShelfFastDetailEvent(this, baseDataShelfTaskFast.body.objects[0]);

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
