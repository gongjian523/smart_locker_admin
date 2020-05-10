using CFLMedCab.APO.GoodsChange;
using CFLMedCab.APO.Inventory;
using CFLMedCab.BLL;
using CFLMedCab.DTO.Goodss;
using CFLMedCab.Http.Bll;
using CFLMedCab.Http.Model;
using CFLMedCab.Infrastructure;
using CFLMedCab.Infrastructure.DeviceHelper;
using CFLMedCab.Model;
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
        public delegate void EnterStockDetailedHandler(object sender, StockDetailParas sdParas);
        public event EnterStockDetailedHandler EnterStockDetailedEvent;

        //提出或者隐藏盘点正在进行的页面
        public delegate void SetPopInventoryHandler(object sender, bool e);
        public event SetPopInventoryHandler SetPopInventoryEvent;

        private List<CatalogueCommodity> comboBoxList = new List<CatalogueCommodity>();
        private List<LocalCommodityCode> comboBoxList2 = new List<LocalCommodityCode>();

        //库存快照列表数据
        private List<CatalogueCommodity> listViewData = new List<CatalogueCommodity>();
        //有效期查询列表数据
        private List<CommodityCode> listView1Data = new List<CommodityCode>();
        //出入库查询列表数据
        private List<LocalCommodityCode> listView2Data = new List<LocalCommodityCode>();

        Timer iniTimer;

        public Stock()
        {
            InitializeComponent();

            listView1Data = new List<CommodityCode>();

            List<String> names = ConsumingBll.GetInstance().GetLocalCommodityName();
            comboBoxList2.Add(new LocalCommodityCode { name = "全部" });
            names.ForEach(item => {
                comboBoxList2.Add(new LocalCommodityCode { name = item });
            });

            iniTimer = new Timer(100);
            iniTimer.AutoReset = false;
            iniTimer.Enabled = true;
            iniTimer.Elapsed += new ElapsedEventHandler(onInitData);
        }

        private void onInitData(object sender, ElapsedEventArgs e)
        {
            GetCurrentGoodsInfo();

            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                queryFilter.Visibility = Visibility.Collapsed;

                CatalogueNameSP.Visibility = Visibility.Visible;
                goodsNameSP.Visibility = Visibility.Collapsed;

                goodsCodeSP.Visibility = Visibility.Hidden;
                operating_time.Visibility = Visibility.Hidden;

                stockSnapshotQuery.Visibility = Visibility.Visible;
                validityQuery.Visibility = Visibility.Hidden;

                stockQuery.Visibility = Visibility.Hidden;

                Content0.Visibility = Visibility.Visible;
                Content1.Visibility = Visibility.Hidden;
                Content2.Visibility = Visibility.Hidden;
                queryData(this, null);
            }));
        }

        /// <summary>
        /// 库存快照事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onStockSnapshot(object sender, RoutedEventArgs e)
        {
            ResetGoodsNameComboxInSnapShot();
            stockquery.IsChecked = true;
            iniTimer.Enabled = true;
        }

        /// <summary>
        /// 条件查询事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onConditionQuery(object sender, RoutedEventArgs e)
        {
            queryFilter.Visibility = Visibility.Visible;
            ResetGoodsNameComboxInSnapShot();
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void queryData(object sender, RoutedEventArgs e)
        {
            if (this.stockSnapshot.IsChecked == true)//库存快照
            {
                List<CatalogueCommodity> list = listViewData;

                if ((CatalogueCommodity)catalogue_name.SelectedValue != null)
                {
                    if (((CatalogueCommodity)catalogue_name.SelectedValue).CatalogueName != "全部")
                    {
                        list = listViewData.Where(item => item.CatalogueName == ((CatalogueCommodity)catalogue_name.SelectedValue).CatalogueName).ToList();
                    }
                }

                listView.DataContext = list;
                totalNum.Content = list.Sum(item => item.Num);
            }
            if (this.validity.IsChecked == true)//效期查询
            {
                List<CommodityCode> list = listView1Data;

                if ((CatalogueCommodity)catalogue_name.SelectedValue != null)
                {
                    if (((CatalogueCommodity)catalogue_name.SelectedValue).CatalogueName != "全部")
                    {
                        list = listView1Data.Where(item => item.CatalogueName == ((CatalogueCommodity)catalogue_name.SelectedValue).CatalogueName).ToList();
                    }
                }

                if (goods_code.Text != "")
                {
                    list = list.Where(item => item.name.Contains(goods_code.Text)).ToList();
                }

                if (single1.IsChecked == true)
                    list = list.Where(item=> item.ExpirationDate <DateTime.Now.AddMonths(1)).ToList();
                if (single2.IsChecked == true)
                    list = list.Where(item => item.ExpirationDate <DateTime.Now.AddMonths(2)).ToList();
                if (single3.IsChecked == true)
                    list = list.Where(item => item.ExpirationDate < DateTime.Now.AddMonths(3)).ToList();

                listView1.DataContext = list;
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

                if ((LocalCommodityCode)goods_name.SelectedValue != null)
                {
                    if (((LocalCommodityCode)goods_name.SelectedValue).name != "全部")
                    {
                        pageDataApo.name = ((LocalCommodityCode)goods_name.SelectedValue).name;
                    }
                }

                List<LocalCommodityCode> list = ConsumingBll.GetInstance().GetLocalCommodityCodeChange(pageDataApo, out int count);

                listView2.DataContext = list;
            }
        }

        private void ResetGoodsNameComboxInSnapShot()
        {
            comboBoxList.Clear();
            comboBoxList.Add(new CatalogueCommodity { CatalogueName = "全部" });
            listViewData.ForEach(item => comboBoxList.Add(item));

            catalogue_name.ItemsSource = comboBoxList.OrderByDescending(it => it.CatalogueName);
            catalogue_name.SelectedItem = comboBoxList.Where(it => it.CatalogueName == "全部").First();
        }

        private void ResetGoodsNameComboxInStockQuery()
        {
            goods_name.ItemsSource = comboBoxList2.OrderByDescending(it => it.name);
            goods_name.SelectedItem = comboBoxList2.Where(it => it.name == "全部").First();
        }
      
        /// <summary>
        /// 效期查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EffectivePeriod(object sender, RoutedEventArgs e)
        {
            queryFilter.Visibility = Visibility.Visible;

            CatalogueNameSP.Visibility = Visibility.Visible;
            goodsNameSP.Visibility = Visibility.Collapsed;

            goodsCodeSP.Visibility = Visibility.Visible;
            operating_time.Visibility = Visibility.Hidden;

            stockSnapshotQuery.Visibility = Visibility.Hidden;
            validityQuery.Visibility = Visibility.Visible;
            stockQuery.Visibility = Visibility.Hidden;

            Content0.Visibility = Visibility.Hidden;
            Content1.Visibility = Visibility.Visible;
            Content2.Visibility = Visibility.Hidden;

            ResetGoodsNameComboxInSnapShot();

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

            CatalogueNameSP.Visibility = Visibility.Collapsed;
            goodsNameSP.Visibility = Visibility.Visible;

            goodsCodeSP.Visibility = Visibility.Hidden;
            operating_time.Visibility = Visibility.Visible;

            stockSnapshotQuery.Visibility = Visibility.Hidden;
            validityQuery.Visibility = Visibility.Hidden;
            stockQuery.Visibility = Visibility.Visible;

            Content0.Visibility = Visibility.Hidden;
            Content1.Visibility = Visibility.Hidden;
            Content2.Visibility = Visibility.Visible;

            ResetGoodsNameComboxInStockQuery();

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
            string name = (string)((Button)sender).CommandParameter;

            StockDetailParas sdParas = new StockDetailParas {
                CatalogueName = name,
                SpecList = listViewData.Where(item=>item.CatalogueName == name).ToList().First().SpecList,
                commodityList = listView1Data
            };

            EnterStockDetailedEvent(this, sdParas);
        }

        private void GetCurrentGoodsInfo()
        {
            SetPopInventoryEvent(this, true);

#if TESTENV
            HashSet<CommodityEps> hs = RfidHelper.GetEpcDataJsonInventory(out bool isGetSuccess);
#else
            //HashSet<CommodityEps> hs = RfidHelper.GetEpcDataJson(out bool isGetSuccess, ApplicationState.GetAllRfidCom());
            HashSet<CommodityEps> hs = RfidHelper.GetEpcDataJsonInventory(out bool isGetSuccess);
#endif
            ApplicationState.SetGoodsInfo(hs);
			
            listViewData.Clear();
            listView1Data.Clear();

            listView1Data = LocalGoodsChangeBll.GetCommodity();

            if (listView1Data != null)
			{
                listViewData = LocalGoodsChangeBll.GetCatalogueCommodity(listView1Data);
            }
			else
			{
				listViewData = new List<CatalogueCommodity>();
			}

            SetPopInventoryEvent(this, false);
        }

    }
}
