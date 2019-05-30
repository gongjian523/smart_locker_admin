using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using CFLMedCab.View;
using CFLMedCab.View.Inventory;
using CFLMedCab.View.SurgeryCollarUse;
using MahApps.Metro.Controls;

namespace CFLMedCab
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private DispatcherTimer ShowTimer;

        public MainWindow()
        {
            InitializeComponent();
            ShowTime();
            ShowTimer = new System.Windows.Threading.DispatcherTimer();
            ShowTimer.Tick += new EventHandler(ShowCurTimer);//起个Timer一直获取当前时间
            ShowTimer.Interval = new TimeSpan(0, 0, 0, 1, 0);
            ShowTimer.Start();
        }
        public void ShowCurTimer(object sender, EventArgs e)
        {
            ShowTime();
        }

        //ShowTime方法
        private void ShowTime()
        {
            //获得年月日
            this.tbDateText.Text = DateTime.Now.ToString("yyyy年MM月dd日");
            //获得时分秒
            this.tbTimeText.Text = DateTime.Now.ToString("HH:mm");
        }

        /// <summary>
        /// 一般领用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnterGerFetch(object sender, RoutedEventArgs e)
        {
            string log = (string)((RadioButton)sender).Content;
            GerFetchView gerFetchView = new GerFetchView(log);
            ContentFrame.Navigate(gerFetchView);
        }

        /// <summary>
        /// 手术领用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OperationCollarUse(object sender, RoutedEventArgs e)
        {
            SurgeryQuery surgeryQuery = new SurgeryQuery(); 
            ContentFrame.Navigate(surgeryQuery);
        }

        /// <summary>
        /// 补货入库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Replenishment(object sender, RoutedEventArgs e)
        {
            Replenishment replenishment = new Replenishment();
            ContentFrame.Navigate(replenishment);
        }

        /// <summary>
        /// 退货出库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReturnGoods(object sender, RoutedEventArgs e)
        {
            ReturnGoods returnGoods = new ReturnGoods();
            ContentFrame.Navigate(returnGoods);
        }

        /// <summary>
        /// 库存盘点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Inventory(object sender, RoutedEventArgs e)
        {
            Inventory inventory = new Inventory();
            ContentFrame.Navigate(inventory);
        }

        /// <summary>
        /// 库存查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Stock(object sender, RoutedEventArgs e)
        {
            Stock stock = new Stock();
            ContentFrame.Navigate(stock);
        }
    }
}
