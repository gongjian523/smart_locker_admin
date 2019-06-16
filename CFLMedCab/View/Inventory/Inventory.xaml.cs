using CFLMedCab.APO.Inventory;
using CFLMedCab.BLL;
using CFLMedCab.DTO.Goodss;
using CFLMedCab.DTO.Inventory;
using CFLMedCab.Infrastructure;
using CFLMedCab.Infrastructure.DeviceHelper;
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
            bool isGetSuccess;


            Hashtable ht = new Hashtable();
            HashSet<string> hs1 = new HashSet<string> { "E20000176012027919504D98", "E20000176012025319504D67", "E20000176012025619504D70", "E20000176012028119504DA5", "E20000176012023919504D48" };
            ht.Add("COM1", hs1);
            HashSet<string> hs4 = new HashSet<string> { "E20000176012028219504DAD", "E20000176012026619504D8D", "E20000176012026319404F98", "E20000176012028019504DA0", "E20000176012026519504D85" };
            ht.Add("COM4", hs4);

            //Hashtable ht = RfidHelper.GetEpcData(out isGetSuccess);

            ApplicationState.SetValue((int)ApplicationKey.CurGoods, ht);

            InventoryBll inventoryBll = new InventoryBll();
            GoodsBll goodsBll = new GoodsBll();
            List<GoodsDto> list = goodsBll.GetInvetoryGoodsDto(ht);
            int id = inventoryBll.NewInventory(InventoryType.Manual);
            inventoryBll.InsertInventoryDetails(list, id);

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

            int type = 0;

            if (btnItem.Name == "BtnDetail")
                type = 1;

            int id = (int)btnItem.CommandParameter;

            this.listView.Items.Refresh();

            EnterInventoryDetailEvent(this, new InventoryDetailPara {
                id = id,
                type = type
            });

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
