﻿using CFLMedCab.BLL;
using CFLMedCab.Model;
using System.Windows.Controls;


namespace CFLMedCab.View.Fetch
{
    /// <summary>
    /// SurgeryNumOpen.xaml 的交互逻辑
    /// </summary>
    public partial class SurgeryNumOpen : UserControl
    {
        private FetchOrder fetchOrder;
        //private SurgeryOrderBll surgeryOrderBll = new SurgeryOrderBll();
        //private FetchOrderBll fetchOrderBll = new FetchOrderBll();
        //private FetchOrderdtlBll fetchOrderdtlBll = new FetchOrderdtlBll();
        public SurgeryNumOpen(FetchOrder model)
        {
            InitializeComponent();
            fetchOrder = model;
            //SurgeryOrder surgeryOrder = surgeryOrderBll.GetById(fetchOrder.business_order_id);
            lNum.Content = model.business_order_id;
            //time.Content = surgeryOrder.surgery_dateiime;
            //listView.DataContext = fetchOrderdtlBll.GetDetailsUsage(id);
        }
        
    }
}