using CFLMedCab.APO.Inventory;
using CFLMedCab.BLL;
using CFLMedCab.DTO.Goodss;
using CFLMedCab.DTO.Inventory;
using CFLMedCab.Infrastructure;
using CFLMedCab.Infrastructure.DeviceHelper;
using CFLMedCab.Infrastructure.ToolHelper;
using CFLMedCab.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Inventory.xaml 的交互逻辑
    /// </summary>
    public partial class Inventory : UserControl
    {
        public delegate void EnterPopInventoryHandler(object sender, System.EventArgs e);
        public event EnterPopInventoryHandler EnterPopInventoryEvent;

        public delegate void EnterPopInventoryPlanHandler(object sender, System.EventArgs e);
        public event EnterPopInventoryPlanHandler EnterPopInventoryPlanEvent;

        public delegate void EnterInventoryDetailHandler(object sender, InventoryDetailPara e);
        public event EnterInventoryDetailHandler EnterInventoryDetailEvent;

        InventoryBll inventoryBll = new InventoryBll();
        List<InventoryOrderDto> inventoryOrderDtos = new List<InventoryOrderDto>();

        public Inventory()
        {
            InitializeComponent();

            GetInventoryList();
            
            //List<InventoryOrderDto> inventoryOrderDtos = new List<InventoryOrderDto>();
            //for(int i = 5; i > 0; i--)
            //{
            //    InventoryOrderDto inventoryOrderDto = new InventoryOrderDto
            //    {
            //        id = i,
            //        code = "dff12412",
            //        confirm_time=DateTime.Now,
            //        create_time=DateTime.Now,
            //        inspector_id=2,
            //        inspector_name="何海霞",
            //        operator_id=1,
            //        operator_name="何海霞",
            //        status=0,
            //        type=1
            //    };
            //    inventoryOrderDtos.Add(inventoryOrderDto);
            //}
        }

        /// <summary>
        /// 盘点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterPopInventory(object sender, RoutedEventArgs e)
        {

            EnterPopInventoryEvent(this, null);

            Timer invTimer = new Timer(1000);
            invTimer.AutoReset = false;
            invTimer.Enabled = true;
            invTimer.Elapsed += new ElapsedEventHandler(onStartInventory);
        }

        public void onStartInventory(object sender, EventArgs e)
        {
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                GetInventoryList();
            }));
        }

        /// <summary>
        /// 盘点计划
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterPopInventoryPlan(object sender, RoutedEventArgs e)
        {
            EnterPopInventoryPlanEvent(this, null);
        }

        private void onEnterInventoryDetail(object sender, RoutedEventArgs e)
        {
            Button btnItem = sender as Button;

            int id = (int)btnItem.CommandParameter;

            InventoryOrderDto orderDto = inventoryOrderDtos.Where(item => item.id == (int)btnItem.CommandParameter).ToList().First();
            InventoryDetailPara para = AutoMapperHelper.MapTo<InventoryDetailPara>(orderDto);
            para.btnType = btnItem.Name == "BtnDetail" ? 1 : 0;

            EnterInventoryDetailEvent(this, para);

            this.listView.Items.Refresh();

        }

        private void onLoadInventory(object sender, RoutedEventArgs e)
        {
            GetInventoryList();
        }


        private void GetInventoryList()
        {
            if ((bool)BtnAll.IsChecked)
            {
                inventoryOrderDtos = inventoryBll.GetInventoryOrder(new InventoryOrderApo
                {
                    PageSize = 0,
                }).Data;
            }
            else
            {
                inventoryOrderDtos = inventoryBll.GetInventoryOrder(new InventoryOrderApo
                {
                    PageSize = 0,
                    status = (bool)BtnUnconfirmed.IsChecked ? (int)InventoryStatus.Unconfirm : (int)InventoryStatus.Confirm
                }).Data;
            }

            listView.DataContext = inventoryOrderDtos;
        }

    }
}
