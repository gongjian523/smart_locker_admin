using CFLMedCab.APO.Surgery;
using CFLMedCab.BLL;
using CFLMedCab.DTO.Fetch;
using CFLMedCab.DTO.Goodss;
using CFLMedCab.DTO.Surgery;
using CFLMedCab.Http.Model;
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
        public SurgeryNumOpen(FetchParam fetchParam)
        {
            InitializeComponent();

            lbCodeContent.Content = fetchParam.bdConsumingOrder.body.objects[0].name;
            lbStatusContent.Content = fetchParam.bdConsumingOrder.body.objects[0].Status;

            if (fetchParam.bdConsumingOrder.body.objects[0].SourceBill.object_name == "OperationOrder")
            {
                lbCodeTitle.Content = "手术单号";
                listView.DataContext = fetchParam.bdOperationOrderGoodsDetail.body.objects;
            }
            else
            {
                lbCodeTitle.Content = "医嘱处方单号";
                listView.DataContext = fetchParam.bdPrescriptionOrderGoodsDetail.body.objects;
            }
        }
        
    }
}
