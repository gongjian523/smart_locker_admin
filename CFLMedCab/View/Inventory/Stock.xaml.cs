using CFLMedCab.APO.Inventory;
using CFLMedCab.BLL;
using CFLMedCab.Infrastructure;
using System;
using System.Collections;
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

namespace CFLMedCab.View
{
    /// <summary>
    /// Stock.xaml 的交互逻辑
    /// </summary>
    public partial class Stock : UserControl
    {       
        //跳出详情页面
        public delegate void EnterStockDetailedHandler(object sender, string goodsCode);
        public event EnterStockDetailedHandler EnterStockDetailedEvent;
        GoodsBll goodsBll = new GoodsBll();
        public Stock()
        {
            InitializeComponent();
            Hashtable ht = ApplicationState.GetValue<Hashtable>((int)ApplicationKey.CurGoods);
            HashSet<string> goodsEpsHashSetDatas = new HashSet<string>();
            foreach (HashSet<string> goodsEpsData in ht.Values)
            {
                goodsEpsHashSetDatas.UnionWith(goodsEpsData);
            }
            data(new GetGoodApo { goodsEpsDatas= goodsEpsHashSetDatas }, out int totalCount);

        }

        public void data(GetGoodApo getGoodApo,out int totalCount)
        {
            totalCount = 0;
            listView.DataContext = goodsBll.GetStockGoodsDto(getGoodApo, out totalCount);
        }

        /// <summary>
        /// 手术单号时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OperationNum(object sender, RoutedEventArgs e)
        {
            Tips.Visibility = Visibility.Visible;
            rbQuery.Visibility = Visibility.Visible;
            gQuery.Visibility = Visibility.Hidden;
            Content.Margin = new Thickness(10, 146, 0, 0);
            single1.Visibility = Visibility.Hidden;
            single2.Visibility = Visibility.Hidden;
            single3.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// 库存快照事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StockSnapshot(object sender, RoutedEventArgs e)
        {
            
        }

        /// <summary>
        /// 条件查询事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConditionQuery(object sender, RoutedEventArgs e)
        {
            gChoice1.Visibility = Visibility.Hidden;
            gChoice.Visibility = Visibility.Visible;
            gQuery.Visibility = Visibility.Visible;
            Content.Margin = new Thickness(10,196,0,0);
        }

        /// <summary>
        /// 效期查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EffectivePeriod(object sender, RoutedEventArgs e)
        {
            gChoice1.Visibility = Visibility.Hidden;
            gChoice.Visibility = Visibility.Visible;
            Tips.Visibility = Visibility.Hidden;
            rbQuery.Visibility = Visibility.Hidden;
            gQuery.Visibility = Visibility.Visible;
            Content.Margin = new Thickness(10, 196, 0, 0);
            single1.Visibility = Visibility.Visible;
            single2.Visibility = Visibility.Visible;
            single3.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// 库存查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StockQuery(object sender, RoutedEventArgs e)
        {
            gChoice.Visibility = Visibility.Hidden;
            gChoice1.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// 库存明细
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onStockDetailed(object sender, RoutedEventArgs e)
        {
            string code = (string)((Button)sender).Tag;
            EnterStockDetailedEvent(this,code);
        }
    }
}
