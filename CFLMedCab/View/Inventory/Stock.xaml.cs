using CFLMedCab.APO.GoodsChange;
using CFLMedCab.APO.Inventory;
using CFLMedCab.BLL;
using CFLMedCab.DTO.Goodss;
using CFLMedCab.Http.Bll;
using CFLMedCab.Http.Model;
using CFLMedCab.Infrastructure;
using CFLMedCab.Infrastructure.DeviceHelper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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
        public delegate void EnterStockDetailedHandler(object sender, Commodity commodity);
        public event EnterStockDetailedHandler EnterStockDetailedEvent;

        public delegate void EnterPopInventoryHandler(object sender, RoutedEventArgs e);
        public event EnterPopInventoryHandler EnterPopInventoryEvent;

        public delegate void HidePopInventoryHandler(object sender, RoutedEventArgs e);
        public event HidePopInventoryHandler HidePopInventoryEvent;

        private List<GoodDto> comboBoxList = new List<GoodDto>();

        public Stock()
        {
            InitializeComponent();
            InitGoodsCodeNameCombox();

            Timer invTimer = new Timer(100);
            invTimer.AutoReset = false;
            invTimer.Enabled = true;
            invTimer.Elapsed += new ElapsedEventHandler(onInitData);

        }

		[Obsolete]
        private void onEndInventory(object sender, ElapsedEventArgs e)
        {
            GetGoodApo getGoodApo = new GetGoodApo();

            Hashtable ht = RfidHelper.GetEpcData(out bool isGetSuccess);
            ApplicationState.SetValue((int)ApplicationKey.CurGoods, ht);

            HashSet<string> goodsEpsHashSetDatas = new HashSet<string>();
            foreach (HashSet<string> goodsEpsData in ht.Values)
            {
                goodsEpsHashSetDatas.UnionWith(goodsEpsData);
            }
            getGoodApo.goodsEpsDatas = goodsEpsHashSetDatas;
            getGoodApo.code = goods_code.SelectedValue == null || ((GoodDto)goods_code.SelectedValue).goods_code == "全部" ? "" : ((GoodDto)goods_code.SelectedValue).goods_code;
            getGoodApo.name = goods_name.SelectedValue == null || ((GoodDto)goods_name.SelectedValue).name == "全部" ? "" : ((GoodDto)goods_name.SelectedValue).name;

            listView.Items.Refresh();
        }

        private void onInitData(object sender, ElapsedEventArgs e)
        {
            onStockSnapshot(this, null);
        }



        /// <summary>
        /// 库存快照事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onStockSnapshot(object sender, RoutedEventArgs e)
        {
            queryFilter.Visibility = Visibility.Collapsed;

            ResetGoodsCodeNameCombox();

            goodsCodeSP.Visibility = Visibility.Visible;
            operating_time.Visibility = Visibility.Hidden;

            stockSnapshotQuery.Visibility = Visibility.Visible;
            validityQuery.Visibility = Visibility.Hidden;
            stockQuery.Visibility = Visibility.Hidden;

            Content0.Visibility = Visibility.Visible;
            Content1.Visibility = Visibility.Hidden;
            Content2.Visibility = Visibility.Hidden;
            queryData(this, null);
        }

        /// <summary>
        /// 条件查询事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onConditionQuery(object sender, RoutedEventArgs e)
        {
            queryFilter.Visibility = Visibility.Visible;
            ResetGoodsCodeNameCombox();
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void queryData(object sender, RoutedEventArgs e)
        {
            //int totalCount = 0;
            if (this.stockSnapshot.IsChecked == true)//库存快照
            {
                EnterPopInventoryEvent(this, null);

                HashSet<CommodityEps> hs = RfidHelper.GetEpcDataJson(out bool isGetSuccess);
                if(!isGetSuccess)
                {
                    HidePopInventoryEvent(this, null);

                    MessageBox.Show("盘点本地商品失败！", "温馨提示", MessageBoxButton.OK);
                    return;
                }

                ApplicationState.SetGoodsInfo(hs);

                List<Commodity> list = LocalGoodsChangeBll.GetCommodity();                

                HidePopInventoryEvent(this, null);

                if (list == null)
                {
                    MessageBox.Show("获取商品信息失败！", "温馨提示", MessageBoxButton.OK);
                }

                listView.DataContext = list;
                totalNum.Content = list.Sum(item => item.Count);
            }
            if (this.validity.IsChecked == true)//效期查询
            {
                //GetGoodsApo getGoodsApo = new GetGoodsApo();
                //Hashtable ht = ApplicationState.GetValue<Hashtable>((int)ApplicationKey.CurGoods);
                //HashSet<string> goodsEpsHashSetDatas = new HashSet<string>();
                //foreach (HashSet<string> goodsEpsData in ht.Values)
                //{
                //    goodsEpsHashSetDatas.UnionWith(goodsEpsData);
                //}
                //if (single1.IsChecked == true)
                //    getGoodsApo.expire_date = DateTime.Now.AddMonths(1);
                //if (single2.IsChecked == true)
                //    getGoodsApo.expire_date = DateTime.Now.AddMonths(2);
                //if (single3.IsChecked == true)
                //    getGoodsApo.expire_date = DateTime.Now.AddMonths(3);

                //getGoodsApo.goodsEpsDatas = goodsEpsHashSetDatas;
                //getGoodsApo.code = goods_code.SelectedValue == null || ((GoodDto)goods_code.SelectedValue).goods_code == "全部" ? "" : ((GoodDto)goods_code.SelectedValue).goods_code;
                //getGoodsApo.name = goods_name.SelectedValue == null || ((GoodDto)goods_name.SelectedValue).name == "全部" ? "" : ((GoodDto)goods_name.SelectedValue).name;

                //List<GoodsDto> goodDtos = goodsBll.GetValidityGoodsDto(getGoodsApo, out totalCount);
                //listView1.DataContext = goodDtos;
            }
            else if (this.stock.IsChecked == true)//库存查询
            {
                InventoryChangesApo pageDataApo = new InventoryChangesApo();

                if (this.outStock.IsChecked == true)
                    pageDataApo.operate_type = 0;
                else if (this.inStock.IsChecked == true)
                    pageDataApo.operate_type = 1;
                if (!string.IsNullOrEmpty(startTime.Text) && !string.IsNullOrWhiteSpace(startTime.Text))
                    pageDataApo.startTime = Convert.ToDateTime(startTime.Text);

                if (!string.IsNullOrEmpty(endTime.Text) && !string.IsNullOrWhiteSpace(endTime.Text))
                {
                    DateTime time = Convert.ToDateTime(endTime.Text.Replace("0:00:00", "23:59:59"));
                    pageDataApo.endTime = new DateTime(time.Year, time.Month, time.Day, 23, 59, 59);
                }
                //goods_name.SelectedValue == null || ((GoodDto)goods_name.SelectedValue).name == "全部" ? "" : ((GoodDto)goods_name.SelectedValue).name;

                List<LocalCommodityCode> list = ConsumingBll.GetInstance().GetLocalCommodityCodeChange(pageDataApo, out int count);

                listView2.DataContext = list;
            }
        }

        public void InitGoodsCodeNameCombox()
        {

            if (comboBoxList.Where(it => it.name == "全部").ToList().Count <=0)
                comboBoxList.Add(new GoodDto { name = "全部", goods_code = "全部" });

            goods_name.ItemsSource = comboBoxList.OrderByDescending(it => it.goods_code);
            goods_code.ItemsSource = comboBoxList.OrderByDescending(it => it.goods_code);
            goods_name.SelectedItem = comboBoxList.Where(it => it.goods_code == "全部").First();
            goods_code.SelectedItem = comboBoxList.Where(it => it.goods_code == "全部").First();

        }

        public void ResetGoodsCodeNameCombox()
        {
            goods_name.SelectedItem = comboBoxList.Where(it => it.goods_code == "全部").First();
            goods_code.SelectedItem = comboBoxList.Where(it => it.goods_code == "全部").First();
        }

        /// <summary>
        /// 效期查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EffectivePeriod(object sender, RoutedEventArgs e)
        {
            queryFilter.Visibility = Visibility.Visible;

            goodsCodeSP.Visibility = Visibility.Visible;
            operating_time.Visibility = Visibility.Hidden;

            ResetGoodsCodeNameCombox();

            stockSnapshotQuery.Visibility = Visibility.Hidden;
            validityQuery.Visibility = Visibility.Visible;
            stockQuery.Visibility = Visibility.Hidden;

            Content0.Visibility = Visibility.Hidden;
            Content1.Visibility = Visibility.Visible;
            Content2.Visibility = Visibility.Hidden;
            //queryCriteria();
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

            queryFilter.Visibility = Visibility.Visible;

            goodsCodeSP.Visibility = Visibility.Hidden;
            operating_time.Visibility = Visibility.Visible;

            ResetGoodsCodeNameCombox();

            stockSnapshotQuery.Visibility = Visibility.Hidden;
            validityQuery.Visibility = Visibility.Hidden;
            stockQuery.Visibility = Visibility.Visible;

            Content0.Visibility = Visibility.Hidden;
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
            Commodity commodity = (Commodity)((Button)sender).Tag;
            EnterStockDetailedEvent(this, commodity);
        }

        /// <summary>
        /// 商品名称选择框变化事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onGoodsNameChanged(object sender, RoutedEventArgs e)
        {

        }


        /// <summary>
        /// 商品名称选择框变化事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onGoodsCodeChanged(object sender, RoutedEventArgs e)
        {

        }

}
}
