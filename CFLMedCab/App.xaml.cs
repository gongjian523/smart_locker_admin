using CFLMedCab.DAL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace CFLMedCab
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            SurgeryOrderDal surgeryOrderDal = new SurgeryOrderDal();
            surgeryOrderDal.CreateTable_SurgeryOrder();
            SurgeryOrderdtlDal surgeryOrderdtlDal = new SurgeryOrderdtlDal();
            surgeryOrderdtlDal.CreateTable_SurgeryOrderdtl();
            FetchOrderDal fetchOrderDal = new FetchOrderDal();
            fetchOrderDal.CreateTable_FetchOrder();
            GoodsDal goodsDal = new GoodsDal();
            goodsDal.CreateTable_Goods();
            InventoryPlanDal inventoryPlanDal = new InventoryPlanDal();
            inventoryPlanDal.CreateTable_InventoryPlan();
        }
    }
}
