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

namespace CFLMedCab.View.Inventory
{
    /// <summary>
    /// InventoryConfirm.xaml 的交互逻辑
    /// </summary>
    public partial class InventoryDetail : UserControl
    {

        public delegate void EnterInventoryHandler(object sender, RoutedEventArgs e);
        public event EnterInventoryHandler EnterInventoryEvent;


        public delegate void EnterAddProductHandler(object sender, RoutedEventArgs e);
        public event EnterAddProductHandler EnterAddProductEvent;


        public InventoryDetail()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 新增实际库存商品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onAddProduct(object sender, RoutedEventArgs e)
        {
            EnterAddProductEvent(this, null);
        }

        /// <summary>
        /// 盘点确认
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onConfirm(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onCancel(object sender, RoutedEventArgs e)
        {
            EnterInventoryEvent(this, null);
        }
    }
}
