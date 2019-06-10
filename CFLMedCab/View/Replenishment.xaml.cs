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
            //listView.DataContext = replenishOrderDal.GetAllReplenishOrder();
        }

        private ObservableCollection<ReplenishOrderView> _replenishOrderView = new ObservableCollection<ReplenishOrderView>();
        public ObservableCollection<ReplenishOrderView> ReplenishOrderViewList
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

        private void InitData()
        {
            ReplenishOrderViewList.Clear();
            List<ReplenishOrderView> replenishOrders= replenishOrderDal.GetAllReplenishOrder();
            replenishOrders.ForEach(replenishOrder => ReplenishOrderViewList.Add(replenishOrder));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //listView.SelectedItem = null;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            object data =(ReplenishOrderView)((Button)sender).Tag; ;
            ReturnGoodsConfirm returnGoodsConfirm = new ReturnGoodsConfirm();
            ContentFrame.Navigate(returnGoodsConfirm);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

        }

    }
}
