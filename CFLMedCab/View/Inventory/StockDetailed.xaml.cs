using CFLMedCab.BLL;
using CFLMedCab.DTO.Goodss;
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

        GoodsBll goodsBll = new GoodsBll();
        public StockDetailed(GoodDto goodDto)
        {
            InitializeComponent();
            HashSet<string> code = new HashSet<string>();
            code.Add(goodDto.goods_code);
            listView.DataContext= goodsBll.GetInvetoryGoodsDto(code);
        }

        public void onStockEvent()
        {
            EnterStockEvent(this, null);
        }
    }
}
