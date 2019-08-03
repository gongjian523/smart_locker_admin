using CFLMedCab.BLL;
using CFLMedCab.DTO.Goodss;
using CFLMedCab.Http.Model;
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

        public StockDetailed(Commodity commodity)
        {
            InitializeComponent();
            listView.DataContext= commodity.codes;
        }

        /// <summary>
        /// 返回列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onStockEvent(object sender, RoutedEventArgs e)
        {
            EnterStockEvent(this, null);
        }
    }
}
