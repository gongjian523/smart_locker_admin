using CFLMedCab.BLL;
using CFLMedCab.DAL;
using CFLMedCab.DTO.Picking;
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

        public delegate void EnterReturnGoodsDetailHandler(object sender, PickingSubOrderDto e);
        public event EnterReturnGoodsDetailHandler EnterReturnGoodsDetailEvent;

        public delegate void EnterReturnGoodsDetailOpenHandler(object sender, PickingSubOrderDto e);
        public event EnterReturnGoodsDetailOpenHandler EnterReturnGoodsDetailOpenEvent;
        PickingBll pickingBll = new PickingBll();
        public ReturnGoods()
        {
            InitializeComponent();
            DataContext = this;

            InitData();
        }

        //private ObservableCollection<PickingOrder> _pickingOrderView = new ObservableCollection<PickingOrder>();
        //public ObservableCollection<PickingOrder> PickingOrderList
        private ObservableCollection<PickingSubOrderDto> _pickingOrderView = new ObservableCollection<PickingSubOrderDto>();
        public ObservableCollection<PickingSubOrderDto> PickingOrderList
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
            List<PickingSubOrderDto> pickingOrders = pickingBll.GetPickingSubOrderDto(new APO.BasePageDataApo()).Data;
            pickingOrders.ForEach(pickingOrder => PickingOrderList.Add(pickingOrder));
        }

        /// <summary>
        /// 查看详情
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterDetail(object sender, RoutedEventArgs e)
        {
            PickingSubOrderDto pickingSubShortOrder = (PickingSubOrderDto)((Button)sender).Tag;
            EnterReturnGoodsDetailEvent(this, pickingSubShortOrder);
        }

        /// <summary>
        /// 确认开柜
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterDetailOpen(object sender, RoutedEventArgs e)
        {
            PickingSubOrderDto pickingSubShortOrder = (PickingSubOrderDto)((Button)sender).Tag;
            EnterReturnGoodsDetailOpenEvent(this, pickingSubShortOrder);
        }
    }
}
