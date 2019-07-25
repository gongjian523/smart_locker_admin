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
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
    public partial class Inventory : UserControl, INotifyPropertyChanged
    {
        public delegate void EnterPopInventoryHandler(object sender, System.EventArgs e);
        public event EnterPopInventoryHandler EnterPopInventoryEvent;

        public delegate void HidePopInventoryHandler(object sender, System.EventArgs e);
        public event HidePopInventoryHandler HidePopInventoryEvent;

        public delegate void EnterPopInventoryPlanHandler(object sender, System.EventArgs e);
        public event EnterPopInventoryPlanHandler EnterPopInventoryPlanEvent;

        public delegate void EnterInventoryDetailHandler(object sender, InventoryDetailPara e);
        public event EnterInventoryDetailHandler EnterInventoryDetailEvent;

        public event PropertyChangedEventHandler PropertyChanged;

        InventoryBll inventoryBll = new InventoryBll();
        List<InventoryOrderDto> inventoryOrderDtos = new List<InventoryOrderDto>();

        private string _content;
        public string NextInvContent
        {
            get { return _content; }
            set
            {
                if (value == _content)
                    return;
                _content = value;
                NotifyPropertyChanged("NextInvContent");
            }
        }

        public Inventory()
        {
            InitializeComponent();

            // GetInventoryList();

            SetNextAutoInvTime();

            DataContext = this;
        }


        /// <summary>
        /// 盘点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterPopInventory(object sender, RoutedEventArgs e)
        {

            EnterPopInventoryEvent(this, null);

            Timer invTimer = new Timer(3000);
            invTimer.AutoReset = false;
            invTimer.Enabled = true;
            invTimer.Elapsed += new ElapsedEventHandler(onEndInventory);
        }

        public void onEndInventory(object sender, EventArgs e)
        {
            bool isGetSuccess;
            Hashtable ht = RfidHelper.GetEpcData(out isGetSuccess);

            ApplicationState.SetValue((int)ApplicationKey.CurGoods, ht);

            InventoryBll inventoryBll = new InventoryBll();
            GoodsBll goodsBll = new GoodsBll();

            List<GoodsDto> list = goodsBll.GetInvetoryGoodsDto(ht);
            int id = inventoryBll.NewInventory(list, InventoryType.Manual);

            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                GetInventoryList();
            }));

            HidePopInventoryEvent(this, null);

            InventoryOrderDto orderDto = inventoryOrderDtos.Where(item => item.id == id).ToList().First();
            InventoryDetailPara para = AutoMapperHelper.MapTo<InventoryDetailPara>(orderDto);

            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                EnterInventoryDetailEvent(this, para);
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
        }

        //“已确认”和“待确认”Tab 按钮更新
        private void onLoadInventory(object sender, RoutedEventArgs e)
        {
            GetInventoryList();
        }

        //自动盘点更新
        public void RefreshInventoryList()
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

            inventoryListView.DataContext = inventoryOrderDtos;
            inventoryListView.Items.Refresh();
        }

        public void SetNextAutoInvTime()
        {
            List<InventoryPlanLDB> inventoryPlans = inventoryBll.GetInventoryPlan().Where(it => it.status == 0).ToList();

            if(inventoryPlans.Count == 0)
            {
                //inventoryTime.Content = "暂无盘点计划"; 
                NextInvContent = "暂无盘点计划";
                return;
            }

            TimeSpan timeSpan = new TimeSpan();

            DateTime date1 = DateTime.Now;
            DateTime date = DateTime.Now;

            foreach (var item in inventoryPlans)
            {
                DateTime date2 = new DateTime(date1.Year, date1.Month, date1.Day, int.Parse(item.inventorytime_str.Substring(0, 2)), int.Parse(item.inventorytime_str.Substring(3, 2)), 0);
                if (date2 < date1)
                    date2 = date2.AddDays(1);
                TimeSpan timeSpan1 = date2 - date1;

                if (timeSpan > timeSpan1 || timeSpan == new TimeSpan())
                {
                    timeSpan = timeSpan1;
                    date = date2;
                }
            }
            if (timeSpan != new TimeSpan())
                //inventoryTime.Content = "下一次自动盘点时间:" + date.ToString("yyyy-MM-dd HH:mm");
                NextInvContent = "下一次自动盘点时间:" + date.ToString("yyyy-MM-dd HH:mm");
            else
                //inventoryTime.Content = "暂无盘点计划";
                NextInvContent = "暂无盘点计划";
        }


        // This method is called by the Set accessor of each property.  
        // The CallerMemberName attribute that is applied to the optional propertyName  
        // parameter causes the property name of the caller to be substituted as an argument.  
        private void NotifyPropertyChanged([CallerMemberName]String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    
}
