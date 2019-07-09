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
        public delegate void EnterReturnGoodsDetailHandler(object sender, PickingOrderDto e);
        public event EnterReturnGoodsDetailHandler EnterReturnGoodsDetailEvent;

        public delegate void EnterReturnGoodsDetailOpenHandler(object sender, PickingOrderDto e);
        public event EnterReturnGoodsDetailOpenHandler EnterReturnGoodsDetailOpenEvent;

        PickingBll pickingBll = new PickingBll();


        public ReturnGoods()
        {
            InitializeComponent();
            DataContext = this;

            tbInputNumbers.Focus();

            InitData();
        }

        private ObservableCollection<PickingOrderDto> _pickingOrderView = new ObservableCollection<PickingOrderDto>();
        public ObservableCollection<PickingOrderDto> PickingOrderList
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
            List<PickingOrderDto> pickingOrders =  pickingBll.GetPickingOrderDto(new APO.BasePageDataApo()).Data; 
            pickingOrders.ForEach(pickingOrder => PickingOrderList.Add(pickingOrder));
        }

        /// <summary>
        /// 查看详情
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterDetail(object sender, RoutedEventArgs e)
        {
            PickingOrderDto pickingShortOrder = (PickingOrderDto)((Button)sender).Tag;
            EnterReturnGoodsDetailEvent(this, pickingShortOrder);
        }

        /// <summary>
        /// 确认开柜
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterDetailOpen(object sender, RoutedEventArgs e)
        {
            PickingOrderDto pickingShortOrder = (PickingOrderDto)((Button)sender).Tag;
            EnterReturnGoodsDetailOpenEvent(this, pickingShortOrder);
        }

        /// <summary>
        /// 查看详情
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnterDetail_Click(object sender, RoutedEventArgs e)
        {
            var value = tbInputNumbers.Text;
            if (string.IsNullOrWhiteSpace(value))
            {
                MessageBox.Show("拣货工单号不可以为空！", "温馨提示", MessageBoxButton.OK);
                return;
            }

            if (PickingOrderList.Where(item => item.code == value).ToList().Count == 0)
            {
                MessageBox.Show("此拣货工单中商品已经领取完毕, 或没有登记在您名下，或者不存在！", "温馨提示", MessageBoxButton.OK);
                return;
            }

            EnterReturnGoodsDetailEvent(this, PickingOrderList.Where(item => item.code == value).First());
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
