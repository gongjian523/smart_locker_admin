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

namespace CFLMedCab.View.Return
{
    /// <summary>
    /// ReturnGoods.xaml 的交互逻辑
    /// </summary>
    public partial class ReturnGoods : UserControl
    {
        //PickingOrderDal pickingOrderDal = new PickingOrderDal();

        public delegate void EnterReturnGoodsDetailHandler(object sender, PickingSubShortOrder e);
        public event EnterReturnGoodsDetailHandler EnterReturnGoodsDetailEvent;

        public delegate void EnterReturnGoodsDetailOpenHandler(object sender, PickingSubShortOrder e);
        public event EnterReturnGoodsDetailOpenHandler EnterReturnGoodsDetailOpenEvent;

        public ReturnGoods()
        {
            InitializeComponent();
            DataContext = this;

            InitData();
        }

        //private ObservableCollection<PickingOrder> _pickingOrderView = new ObservableCollection<PickingOrder>();
        //public ObservableCollection<PickingOrder> PickingOrderList
        private ObservableCollection<PickingSubShortOrder> _pickingOrderView = new ObservableCollection<PickingSubShortOrder>();
        public ObservableCollection<PickingSubShortOrder> PickingOrderList
        {
            get
            {
                return _pickingOrderView;
            }
            set
            {
                _pickingOrderView = value;
            }
        }

        /// <summary>
        /// 数据加载
        /// </summary>
        private void InitData()
        {
            PickingOrderList.Clear();
            List<PickingSubShortOrder> pickingOrders =new List<PickingSubShortOrder> { new PickingSubShortOrder { id=1,create_time=DateTime.Now,unDoneNum=20} };
            pickingOrders.ForEach(pickingOrder => PickingOrderList.Add(pickingOrder));
        }

        /// <summary>
        /// 查看详情
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterDetail(object sender, RoutedEventArgs e)
        {
            PickingSubShortOrder pickingSubShortOrder = (PickingSubShortOrder)((Button)sender).Tag;
            EnterReturnGoodsDetailEvent(this, pickingSubShortOrder);
        }

        /// <summary>
        /// 确认开柜
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterDetailOpen(object sender, RoutedEventArgs e)
        {
            PickingSubShortOrder pickingSubShortOrder = (PickingSubShortOrder)((Button)sender).Tag;
            EnterReturnGoodsDetailOpenEvent(this, pickingSubShortOrder);
        }
    }
}
