using CFLMedCab.Model;
using System;
using System.Collections.Generic;
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

        public Inventory()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 盘点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterPopInventory(object sender, RoutedEventArgs e)
        {
            EnterPopInventoryEvent(this, null);
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
