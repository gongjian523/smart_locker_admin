using AutoMapper;
using CFLMedCab.BLL;
using CFLMedCab.DTO.Goodss;
using CFLMedCab.DTO.Inventory;
using CFLMedCab.Http.Model;
using CFLMedCab.Infrastructure.DeviceHelper;
using CFLMedCab.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace CFLMedCab.View.Inventory
{
    /// <summary>
    /// InventoryConfirm.xaml 的交互逻辑
    /// </summary>
    public partial class InventoryDtlLocal : UserControl
    {
        public delegate void BackInventoryHandler(object sender, RoutedEventArgs e);
        public event BackInventoryHandler BackInventoryEvent;

        //public event PropertyChangedEventHandler PropertyChanged;

        InventoryBll inventoryBll = new InventoryBll();

        List<InventoryOrderdtl> dtlList = new List<InventoryOrderdtl>();
        List<CatalogueCommodity> catalogueList = new List<CatalogueCommodity>();
        List<GoodsDto> list = new List<GoodsDto>();

        string curCatalogueName;

        public string Code { get; set; }
        public DateTime CreateTime { get; set; }
        public int Type { get; set; }


        public InventoryDtlLocal(int inventoryId)
        {
            InitializeComponent();

            DataContext = this;

            InventoryOrderDto order = inventoryBll.GetInventoryOrdersByInventoryId(inventoryId)[0];
            dtlList = inventoryBll.GetInventoryDetailsByInventoryId(inventoryId);
            catalogueList = inventoryBll.GetCatalogueInfo(dtlList);

            Code = order.code;
            CreateTime = order.create_time;

            gridView1.Visibility = Visibility.Visible;
            gridView2.Visibility = Visibility.Hidden;
            gridView3.Visibility = Visibility.Hidden;

            setLableVisbility(true);

            listView1.DataContext = catalogueList;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterSpecDetail(object sender, RoutedEventArgs e)
        {
            curCatalogueName = (string)(sender as Button).CommandParameter;

            gridView1.Visibility = Visibility.Hidden;
            gridView2.Visibility = Visibility.Visible;

            List<SpecCommodity> specList = catalogueList.Where(item => item.CatalogueName == curCatalogueName).ToList().First().SpecList;

            listView2.DataContext = specList;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterCommodityDetail(object sender, RoutedEventArgs e)
        {
            string spec = (string)(sender as Button).CommandParameter;

            gridView2.Visibility = Visibility.Hidden;
            gridView3.Visibility = Visibility.Visible;

            setLableVisbility(false);
            commodityNameTb.Text = curCatalogueName;
            commoditySpecTb.Text = spec;

            List<InventoryOrderdtl> commodityList = dtlList.Where(item => item.CatalogueName == curCatalogueName && item.Specifications == spec).ToList();

            listView3.DataContext = commodityList;
        }


        /// <summary>
        /// 返回的盘点界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onBackword(object sender, RoutedEventArgs e)
        {
            BackInventoryEvent(this, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onBackwordSepcList(object sender, RoutedEventArgs e)
        {
            gridView2.Visibility = Visibility.Hidden;
            gridView1.Visibility = Visibility.Visible;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onBackwordSepcDetial(object sender, RoutedEventArgs e)
        {
            setLableVisbility(true);

            gridView3.Visibility = Visibility.Hidden;
            gridView2.Visibility = Visibility.Visible;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="isSpecDetail"></param>
        private void setLableVisbility(bool isSpecDetail)
        {
            invTimerLb.Visibility = isSpecDetail ? Visibility.Visible : Visibility.Hidden;
            invTimerTb.Visibility = isSpecDetail ? Visibility.Visible : Visibility.Hidden;
            invTypel.Visibility = isSpecDetail ? Visibility.Visible : Visibility.Hidden;
            invTypel2.Visibility = isSpecDetail ? Visibility.Visible : Visibility.Hidden;

            commodityNameLb.Visibility = !isSpecDetail ? Visibility.Visible : Visibility.Hidden;
            commodityNameTb.Visibility = !isSpecDetail ? Visibility.Visible : Visibility.Hidden;
            commoditySpecLb.Visibility = !isSpecDetail ? Visibility.Visible : Visibility.Hidden;
            commoditySpecTb.Visibility = !isSpecDetail ? Visibility.Visible : Visibility.Hidden;           
        }

    }
}
