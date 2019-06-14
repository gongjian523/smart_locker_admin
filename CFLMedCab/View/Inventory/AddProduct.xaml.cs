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
using System.Windows.Shapes;

namespace CFLMedCab.View.Inventory
{
    /// <summary>
    /// AddStock.xaml 的交互逻辑
    /// </summary>
    public partial class AddProduct : UserControl
    {
        public delegate void HidePopAddProductHandler(object sender, System.EventArgs e);
        public event HidePopAddProductHandler HidePopAddProductEvent;

        public AddProduct()
        {
            InitializeComponent();
        }

        private void onSave(object sender, RoutedEventArgs e)
        {
            HidePopAddProductEvent(this, null);
        }

        private void onCancel(object sender, RoutedEventArgs e)
        {
            HidePopAddProductEvent(this, null);
        }

        private void onAddProduct(object sender, RoutedEventArgs e)
        {

        }
    }
}
