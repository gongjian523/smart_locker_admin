using CFLMedCab.APO.Surgery;
using CFLMedCab.BLL;
using CFLMedCab.DAL;
using CFLMedCab.DTO.Fetch;
using CFLMedCab.DTO.Goodss;
using CFLMedCab.DTO.Surgery;
using CFLMedCab.Infrastructure;
using CFLMedCab.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace CFLMedCab.View.Fetch
{
    /// <summary>
    /// SurgeryNumQuery.xaml 的交互逻辑
    /// </summary>
    public partial class SurgeryOrderDetail : UserControl
    {
        public delegate void EnterSurgeryConsumablesDetailHandler(object sender, SurgeryOrderDto surgeryOrderDto);
        public event EnterSurgeryConsumablesDetailHandler EnterSurgeryConsumablesDetailEvent;

        public delegate void EnterSurgeryNumOpenHandler(object sender, SurgeryOrderDto surgeryOrderDto);
        public event EnterSurgeryNumOpenHandler EnterSurgeryNumOpenEvent;

        private Hashtable after;
        private SurgeryOrderDto surgeryOrderDto;
        private List<GoodsDto> goodsChageOrderdtls;
        private GoodsBll goodsBll = new GoodsBll();
        private FetchOrderBll fetchOrderBll = new FetchOrderBll();
        public SurgeryOrderDetail(SurgeryOrderDto model)
        {
            InitializeComponent();
            surgeryOrderDto = model;
            surgeryNum.Content = model.code;
            time.Content = model.surgery_time;
            Hashtable before = ApplicationState.GetValue<Hashtable>((int)ApplicationKey.CurGoods);
            List<GoodsDto> goodsDtos = goodsBll.GetInvetoryGoodsDto(before);
            listView.DataContext= fetchOrderBll.GetSurgeryOrderdtlDto(new SurgeryOrderApo { SurgeryOrderCode= surgeryOrderDto.code, GoodsDtos=goodsDtos }, out int stockGoodsNum, out int notStockGoodsNum).Data;
            inStock.Content = stockGoodsNum;
            noStock.Content = notStockGoodsNum;
        }


        /// <summary>
        /// 手术耗材详情
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnterSurgeryDetail(object sender, RoutedEventArgs e)
        {
            EnterSurgeryConsumablesDetailEvent(this, surgeryOrderDto);
        }

        /// <summary>
        /// 确认领用（开柜）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnterSurgeryNumOpen(object sender, RoutedEventArgs e)
        {
            EnterSurgeryNumOpenEvent(this, surgeryOrderDto);
        }
    }
}
