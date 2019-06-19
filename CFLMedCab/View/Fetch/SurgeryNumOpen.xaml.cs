using CFLMedCab.APO.Surgery;
using CFLMedCab.BLL;
using CFLMedCab.DTO.Fetch;
using CFLMedCab.DTO.Goodss;
using CFLMedCab.DTO.Surgery;
using CFLMedCab.Infrastructure;
using CFLMedCab.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Controls;


namespace CFLMedCab.View.Fetch
{
    /// <summary>
    /// SurgeryNumOpen.xaml 的交互逻辑
    /// </summary>
    public partial class SurgeryNumOpen : UserControl
    {
        private SurgeryOrderDto surgeryOrderDto;
        private GoodsBll goodsBll = new GoodsBll();
        private FetchOrderBll fetchOrderBll = new FetchOrderBll();
        public SurgeryNumOpen(SurgeryOrderDto model)
        {
            InitializeComponent();
            surgeryOrderDto = model;
            surgeryNum.Content = model.code;
            time.Content = model.surgery_time.ToString("yyyy年MM月dd日");
            Hashtable before = ApplicationState.GetValue<Hashtable>((int)ApplicationKey.CurGoods);
            List<GoodsDto> goodsDtos = goodsBll.GetInvetoryGoodsDto(before);
            listView.DataContext = fetchOrderBll.GetSurgeryOrderdtlDto(new SurgeryOrderApo { SurgeryOrderCode = surgeryOrderDto.code, GoodsDtos = goodsDtos }, out int stockGoodsNum, out int notStockGoodsNum).Data;
        }
        
    }
}
