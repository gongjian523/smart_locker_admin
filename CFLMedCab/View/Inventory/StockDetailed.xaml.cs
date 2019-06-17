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
    /// StockDetailed.xaml 的交互逻辑
    /// </summary>
    public partial class StockDetailed : UserControl
    {       
        //返回列表页面
        public delegate void EnterStockHandler(object sender, RoutedEventArgs e);
        public event EnterStockHandler EnterStockEvent;
        public StockDetailed(string code)
        {
            InitializeComponent();
        }
    }
}
