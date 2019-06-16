using CFLMedCab.APO;
using CFLMedCab.BLL;
using CFLMedCab.DAL;
using CFLMedCab.DTO.Replenish;
using CFLMedCab.Infrastructure.DeviceHelper;
using CFLMedCab.Model;
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
        public delegate void EnterReplenishmentDetailHandler(object sender, ReplenishSubOrderDto e);
        public event EnterReplenishmentDetailHandler EnterReplenishmentDetailEvent;

        public delegate void EnterReplenishmentDetailOpenHandler(object sender, ReplenishSubOrderDto e);
        public event EnterReplenishmentDetailOpenHandler EnterReplenishmentDetailOpenEvent;

        ReplenishBll replenishBll = new ReplenishBll();
        public Replenishment()
        {
            InitializeComponent();
            DataContext = this;

            InitData();
        }

        //private ObservableCollection<ReplenishOrder> _replenishOrderView = new ObservableCollection<ReplenishOrder>();
        //public ObservableCollection<ReplenishOrder> ReplenishOrderViewList     
        private ObservableCollection<ReplenishSubOrderDto> _replenishOrderView = new ObservableCollection<ReplenishSubOrderDto>();
        public ObservableCollection<ReplenishSubOrderDto> ReplenishOrderViewList
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
            List<ReplenishSubOrderDto> replenishOrders = replenishBll.GetReplenishSubOrderDto(new BasePageDataApo()).Data;
            replenishOrders.ForEach(replenishOrder => ReplenishOrderViewList.Add(replenishOrder));
        }
        
        /// <summary>
        /// 确认开柜
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnterDetailOpen_Click(object sender, RoutedEventArgs e)
        {
            ReplenishSubOrderDto replenishSubShortOrder = (ReplenishSubOrderDto)((Button)sender).Tag;
            EnterReplenishmentDetailOpenEvent(this, replenishSubShortOrder);
        }

        /// <summary>
        /// 查看详情
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnterDetail_Click(object sender, RoutedEventArgs e)
        {
            ReplenishSubOrderDto replenishSubShortOrder = (ReplenishSubOrderDto)((Button)sender).Tag;
            EnterReplenishmentDetailEvent(this, replenishSubShortOrder);
        }
    }
}
