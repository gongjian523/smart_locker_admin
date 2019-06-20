using CFLMedCab.APO.GoodsChange;
using CFLMedCab.APO.Inventory;
using CFLMedCab.BLL;
using CFLMedCab.DTO.Goodss;
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
        public delegate void EnterStockDetailedHandler(object sender, GoodDto goodDto);
        public event EnterStockDetailedHandler EnterStockDetailedEvent;
        GoodsBll goodsBll = new GoodsBll();
        GoodsChangeOrderBll goodsChangeOrderBll = new GoodsChangeOrderBll();
        private List<GoodDto> goodDtos;//最新库存信息
        public Stock()
        {
            InitializeComponent();
            stockquery.IsChecked = true;
            condition.IsChecked = true;
            queryData(this, null);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void queryData(object sender, RoutedEventArgs e)
        {
            int totalCount = 0;
            if (this.stockSnapshot.IsChecked == true)//库存快照
            {
                GetGoodApo getGoodApo = new GetGoodApo();
                Hashtable ht = ApplicationState.GetValue<Hashtable>((int)ApplicationKey.CurGoods);
                HashSet<string> goodsEpsHashSetDatas = new HashSet<string>();
                foreach (HashSet<string> goodsEpsData in ht.Values)
                {
                    goodsEpsHashSetDatas.UnionWith(goodsEpsData);
                }
                getGoodApo.goodsEpsDatas = goodsEpsHashSetDatas;
                getGoodApo.code = goods_code.SelectedValue == null ? "": ((GoodDto)goods_code.SelectedValue).goods_code;
                getGoodApo.name = goods_name.SelectedValue == null ? "" : ((GoodDto)goods_name.SelectedValue).name;
      
                List<GoodDto> goodDtos = goodsBll.GetStockGoodsDto(getGoodApo, out totalCount);
                listView.DataContext = goodDtos;
                goods_code.ItemsSource = goodDtos;
                goods_name.ItemsSource = goodDtos;
                totalNum.Content = goodsEpsHashSetDatas.Count;
            }
            if (this.validity.IsChecked == true)//效期查询
            {
                GetGoodsApo getGoodsApo = new GetGoodsApo();
                Hashtable ht = ApplicationState.GetValue<Hashtable>((int)ApplicationKey.CurGoods);
                HashSet<string> goodsEpsHashSetDatas = new HashSet<string>();
                foreach (HashSet<string> goodsEpsData in ht.Values)
                {
                    goodsEpsHashSetDatas.UnionWith(goodsEpsData);
                }
                if (single1.IsChecked == true)
                    getGoodsApo.expire_date = DateTime.Now.AddMonths(1);
                if (single2.IsChecked == true)
                    getGoodsApo.expire_date = DateTime.Now.AddMonths(2);
                if (single3.IsChecked == true)
                    getGoodsApo.expire_date = DateTime.Now.AddMonths(3);
                getGoodsApo.goodsEpsDatas = goodsEpsHashSetDatas;
                getGoodsApo.code = goods_code.SelectedValue == null ? "" : ((GoodDto)goods_code.SelectedValue).goods_code;
                getGoodsApo.name = goods_name.SelectedValue == null ? "" : ((GoodDto)goods_name.SelectedValue).name;
                List<GoodsDto> goodDtos = goodsBll.GetValidityGoodsDto(getGoodsApo, out totalCount);
                listView1.DataContext = goodDtos;
            }
            else if (this.stock.IsChecked == true)//库存查询
            {
                GoodsChangeApo goodsChangeApo = new GoodsChangeApo();
                if (this.outStock.IsChecked == true)
                    goodsChangeApo.operate_type = 0;
                else if (this.inStock.IsChecked == true)
                    goodsChangeApo.operate_type = 1;
                goodsChangeApo.name = goods_name.SelectedValue == null ? "" : ((GoodDto)goods_name.SelectedValue).name;
                List<GoodsChangeDto> goodsChangeDtos = goodsChangeOrderBll.GetGoodsChange(goodsChangeApo, out totalCount);
                listView2.DataContext = goodsChangeDtos;
            }
        }

        /// <summary>
        /// 库存快照事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StockSnapshot(object sender, RoutedEventArgs e)
        {
            goodsCode.Visibility = Visibility.Hidden;
            goodsName.Visibility = Visibility.Hidden;
            query.Visibility = Visibility.Hidden;
            
            stockSnapshotQuery.Visibility = Visibility.Visible;
            validityQuery.Visibility = Visibility.Hidden;
            stockQuery.Visibility = Visibility.Hidden;
            operating_time.Visibility = Visibility.Hidden;

            Content.Visibility = Visibility.Visible;
            Content1.Visibility = Visibility.Hidden;
            Content2.Visibility = Visibility.Hidden;
            queryData(this, null);
        }

        /// <summary>
        /// 条件查询事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConditionQuery(object sender, RoutedEventArgs e)
        {
            goodsCode.Visibility = Visibility.Visible;
            goodsName.Visibility = Visibility.Visible;
            query.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// 效期查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EffectivePeriod(object sender, RoutedEventArgs e)
        {
            stockSnapshotQuery.Visibility = Visibility.Hidden;
            validityQuery.Visibility = Visibility.Visible;
            stockQuery.Visibility = Visibility.Hidden;
            operating_time.Visibility = Visibility.Hidden;
            
            goodsCode.Visibility = Visibility.Visible;
            goodsName.Visibility = Visibility.Visible;

            Content.Visibility = Visibility.Hidden;
            Content1.Visibility = Visibility.Visible;
            Content2.Visibility = Visibility.Hidden;

            All.IsChecked = true;
            queryData(this, null);
        }

        /// <summary>
        /// 出入库记录查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StockQuery(object sender, RoutedEventArgs e)
        {

            stockSnapshotQuery.Visibility = Visibility.Hidden;
            validityQuery.Visibility = Visibility.Hidden;
            stockQuery.Visibility = Visibility.Visible;
            operating_time.Visibility = Visibility.Visible;
            
            goodsName.Visibility = Visibility.Visible;

            Content.Visibility = Visibility.Hidden;
            Content1.Visibility = Visibility.Hidden;
            Content2.Visibility = Visibility.Visible;

            outStock.IsChecked = true;
            queryData(this, null);
        }

        /// <summary>
        /// 库存明细
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onStockDetailed(object sender, RoutedEventArgs e)
        {
            GoodDto goodDto = (GoodDto)((Button)sender).Tag;
            EnterStockDetailedEvent(this, goodDto);
        }
    }
}
