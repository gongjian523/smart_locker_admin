using CFLMedCab.BLL;
using CFLMedCab.DTO.Goodss;
using CFLMedCab.Infrastructure;
using CFLMedCab.Infrastructure.DeviceHelper;
using CFLMedCab.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CFLMedCab.View.Inventory
{
    /// <summary>
    /// InvPopDialog.xaml 的交互逻辑
    /// </summary>
    public partial class InventoryOngoing : UserControl
    {
        private Timer timer;

        public delegate void HidePopInventoryHandler(object sender, RoutedEventArgs e);
        public event HidePopInventoryHandler HidePopInventoryEvent;

        public InventoryOngoing()
        {
            InitializeComponent();

            timer = new Timer(3000);
            timer.AutoReset = false;
            timer.Enabled = true;
            timer.Elapsed += new ElapsedEventHandler(onPopInventory);
        }

        /// <summary>
        /// 三秒之后显示提示消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void onPopInventory(object sender, EventArgs e)
        {
            bool isGetSuccess;
            Hashtable ht = RfidHelper.GetEpcData(out isGetSuccess);

            ApplicationState.SetValue((int)ApplicationKey.CurGoods, ht);

            InventoryBll inventoryBll = new InventoryBll();
            GoodsBll goodsBll = new GoodsBll();

            List<GoodsDto> list = goodsBll.GetInvetoryGoodsDto(ht);
            inventoryBll.NewInventory(list, InventoryType.Manual);

            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                HidePopInventoryEvent(this, null);
            }));
        }
    }
}
