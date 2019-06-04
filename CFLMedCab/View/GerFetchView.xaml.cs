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

namespace CFLMedCab.View
{
    /// <summary>
    /// GerFetchView.xaml 的交互逻辑
    /// </summary>
    public partial class GerFetchView : UserControl
    {
        private FetchOrderDal fetchOrderDal = new FetchOrderDal();
        public GerFetchView(string type)
        {
            InitializeComponent();
            lType.Content = type;
            lDate.Content = DateTime.Now;
            switch (type)
            {
                case "一般领用":
                    data(0, 1);
                    break;
                case "领用退回":
                    data(0, 3);
                    break;
            }

        }

        /// <summary>
        /// 关柜
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ClosetCabinet closetCabinet = new ClosetCabinet();
            closetCabinet.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            closetCabinet.Owner = Application.Current.MainWindow;
            closetCabinet.ShowDialog();
        }


        private ObservableCollection<FetchOrder> _fetchOrderList = new ObservableCollection<FetchOrder>();
        public ObservableCollection<FetchOrder> FetchOrderList
        {
            get
            {
                return _fetchOrderList;
            }
            set
            {
                _fetchOrderList = value;
            }
        }

        public void data(int user, int type)
        {
            FetchOrderList.Clear();
            FetchOrder fetchOrder = new FetchOrder();
            fetchOrder.type = 1;
            fetchOrder.status = 2;
            fetchOrder.operator_id = 0;
            fetchOrder.business_order_id = 2;
            fetchOrder.create_time = DateTime.Now;
            fetchOrderDal.InsertNewFetchOrder(fetchOrder);
            List<FetchOrder> fetchOrders = fetchOrderDal.GetAllFetchOrder(user, type);
            fetchOrders.ForEach(log => FetchOrderList.Add(log));
        }
    }

}
