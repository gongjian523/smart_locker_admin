using CFLMedCab.DAL;
using CFLMedCab.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace CFLMedCab.View.ReplenishmentOrder
{
    /// <summary>
    /// Replenishment.xaml 的交互逻辑
    /// </summary>
    public partial class Replenishment : UserControl
    {
        ReplenishOrderDal replenishOrderDal = new ReplenishOrderDal();
        public Replenishment()
        {
            InitializeComponent();
            DataContext = this;

            InitData();
        }

        private ObservableCollection<ReplenishOrder> _replenishOrderView = new ObservableCollection<ReplenishOrder>();
        public ObservableCollection<ReplenishOrder> ReplenishOrderViewList
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
        private void InitData()
        {
            ReplenishOrderViewList.Clear();
            List<ReplenishOrder> replenishOrders= replenishOrderDal.GetAllReplenishOrder();
            replenishOrders.ForEach(replenishOrder => ReplenishOrderViewList.Add(replenishOrder));
        }
        
        /// <summary>
        /// 确认开柜
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            OpenCabinet openCabinet = new OpenCabinet();
            openCabinet.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            openCabinet.Owner = Application.Current.MainWindow;
            openCabinet.ShowDialog();
        }

        /// <summary>
        /// 查看详情
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Retract_Click(object sender, RoutedEventArgs e)
        {
            ReplenishOrder replenishOrder= (ReplenishOrder)((Button)sender).Tag;
            ReplenishmentConfirm replenishmentConfirm = new ReplenishmentConfirm(replenishOrder);
            ContentFrame.Navigate(replenishmentConfirm);
        }
    }
}
