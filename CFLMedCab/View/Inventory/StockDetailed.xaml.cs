using CFLMedCab.BLL;
using CFLMedCab.DTO.Goodss;
using CFLMedCab.Http.Model;
using CFLMedCab.Model;
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

        StockDetailParas paras = new StockDetailParas();

        public StockDetailed(StockDetailParas sdParas)
        {
            InitializeComponent();

            CatalogueNameLb.Content = sdParas.CatalogueName;

            paras = sdParas;
            listView.DataContext= sdParas.SpecList;
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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterCommodityDetail(object sender, RoutedEventArgs e)
        {
            string spec = (string)((Button)sender).CommandParameter;

            SpecTitleLb.Visibility = Visibility.Visible;

            SpecLb.Visibility = Visibility.Visible;
            SpecLb.Content = spec;

            specView.Visibility = Visibility.Hidden;

            commodityView.Visibility = Visibility.Visible;
            listView2.DataContext = paras.commodityList.Where(item => item.CatalogueName == paras.CatalogueName && item.Specifications == spec).ToList();

            specBtnView.Visibility = Visibility.Hidden;
            commodityBtnView.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterSpecDetail(object sender, RoutedEventArgs e)
        {
            SpecTitleLb.Visibility = Visibility.Hidden;
            SpecLb.Visibility = Visibility.Hidden;

            specView.Visibility = Visibility.Visible;
            commodityView.Visibility = Visibility.Hidden;

            specBtnView.Visibility = Visibility.Visible;
            commodityBtnView.Visibility = Visibility.Hidden;
        }
    }
}
