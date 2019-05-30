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
    public partial class InventoryConfirm : UserControl
    {
        public InventoryConfirm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 新增实际库存商品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AddSingleProduct addSingleProduct = new AddSingleProduct();
            addSingleProduct.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            addSingleProduct.Owner = Application.Current.MainWindow;
            addSingleProduct.ShowDialog();
        }
    }
}
