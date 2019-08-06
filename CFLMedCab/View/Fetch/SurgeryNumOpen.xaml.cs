using CFLMedCab.APO.Surgery;
using CFLMedCab.BLL;
using CFLMedCab.DTO.Fetch;
using CFLMedCab.DTO.Goodss;
using CFLMedCab.DTO.Surgery;
using CFLMedCab.Http.Model;
using CFLMedCab.Http.Model.Base;
using CFLMedCab.Infrastructure;
using CFLMedCab.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;


namespace CFLMedCab.View.Fetch
{
    /// <summary>
    /// SurgeryNumOpen.xaml 的交互逻辑
    /// </summary>
    public partial class SurgeryNumOpen : UserControl
    {
        public SurgeryNumOpen(FetchParam fetchParam)
        {
            InitializeComponent();

            lbCodeTitle.Content = "手术单号";

            ConsumingOrder consumingOrder = fetchParam.bdConsumingOrder.body.objects[0];
            List<ConsumingGoodsDetail> list = fetchParam.bdOperationOrderGoodsDetail.body.objects;

            lbCodeContent.Content = consumingOrder.name;
            lbStatusContent.Content = consumingOrder.Status;

            inStock.Content = list.Where(item => item.stockNum > 0).Count();
            noStock.Content = list.Where(item => item.stockNum == 0).Count();

            listView.DataContext = list;
        }
        
    }
}
