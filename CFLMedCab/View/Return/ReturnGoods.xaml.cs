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

namespace CFLMedCab.View.ReturnGoodsOrder
{
    /// <summary>
    /// ReturnGoods.xaml 的交互逻辑
    /// </summary>
    public partial class ReturnGoods : UserControl
    {
        PickingOrderDal pickingOrderDal = new PickingOrderDal();
        public ReturnGoods()
        {
            InitializeComponent();
            DataContext = this;

            InitData();
        }

        private ObservableCollection<PickingOrder> _pickingOrderView = new ObservableCollection<PickingOrder>();
        public ObservableCollection<PickingOrder> PickingOrderList
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
            //List<PickingOrder> pickingOrders = pickingOrderDal.GetAllPickingOrder();
            //pickingOrders.ForEach(pickingOrder => PickingOrderList.Add(pickingOrder));
        }

        /// <summary>
        /// 确认开柜
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
          //OpenCabinet openCabinet = new OpenCabinet();
          //  openCabinet.WindowStartupLocation = WindowStartupLocation.CenterOwner;
          //  openCabinet.Owner = Application.Current.MainWindow;
          //  openCabinet.ShowDialog();
        }

        /// <summary>
        /// 查看详情
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Retract_Click(object sender, RoutedEventArgs e)
        {
            PickingOrder pickingOrder = (PickingOrder)((Button)sender).Tag;
            ReturnGoodsConfirm returnGoodsConfirm = new ReturnGoodsConfirm(pickingOrder);
            ContentFrame.Navigate(returnGoodsConfirm);
        }
    }
}
