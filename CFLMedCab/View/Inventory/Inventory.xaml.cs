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
        public Inventory()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 盘点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InvPopDialog(object sender, RoutedEventArgs e)
        {
            InvPopDialog invPopDialog = new InvPopDialog();
            invPopDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            invPopDialog.Owner = Application.Current.MainWindow;
            invPopDialog.ShowDialog();
        }

        /// <summary>
        /// 盘点计划
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InventoryPlanSetUp(object sender, RoutedEventArgs e)
        {
            InventoryPlan inventoryPlan = new InventoryPlan();
            inventoryPlan.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            inventoryPlan.Owner = Application.Current.MainWindow;
            inventoryPlan.ShowDialog();
        }
    }
}
