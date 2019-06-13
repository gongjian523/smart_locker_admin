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
        public delegate void EnterReplenishmentDetailHandler(object sender, ReplenishSubShortOrder e);
        public event EnterReplenishmentDetailHandler EnterReplenishmentDetailEvent;

        public delegate void EnterReplenishmentDetailOpenHandler(object sender, ReplenishSubShortOrder e);
        public event EnterReplenishmentDetailOpenHandler EnterReplenishmentDetailOpenEvent;

        ReplenishOrderDal replenishOrderDal = new ReplenishOrderDal();
        public Replenishment()
        {
            InitializeComponent();
            DataContext = this;

            InitData();
        }

        //private ObservableCollection<ReplenishOrder> _replenishOrderView = new ObservableCollection<ReplenishOrder>();
        //public ObservableCollection<ReplenishOrder> ReplenishOrderViewList     
        private ObservableCollection<ReplenishSubShortOrder> _replenishOrderView = new ObservableCollection<ReplenishSubShortOrder>();
        public ObservableCollection<ReplenishSubShortOrder> ReplenishOrderViewList
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

            //List<ReplenishOrder> replenishOrders= replenishOrderDal.GetList();
            List<ReplenishSubShortOrder> replenishOrders = new List<ReplenishSubShortOrder>();
            replenishOrders.Add(new ReplenishSubShortOrder
            {
                id = 1,
                unDoneNum = 1
            });
            replenishOrders.ForEach(replenishOrder => ReplenishOrderViewList.Add(replenishOrder));
        }
        
        /// <summary>
        /// 确认开柜
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnterDetailOpen_Click(object sender, RoutedEventArgs e)
        {
            ReplenishSubShortOrder replenishOrder = (ReplenishSubShortOrder)((Button)sender).Tag;
            EnterReplenishmentDetailOpenEvent(this, replenishOrder);
        }

        /// <summary>
        /// 查看详情
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnterDetail_Click(object sender, RoutedEventArgs e)
        {
            ReplenishSubShortOrder replenishOrder= (ReplenishSubShortOrder)((Button)sender).Tag;
            EnterReplenishmentDetailEvent(this, replenishOrder);
        }
    }
}
