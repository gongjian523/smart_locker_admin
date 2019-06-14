using CFLMedCab.DAL;
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
using System.Windows.Shapes;

namespace CFLMedCab.View.Inventory
{
    /// <summary>
    /// InventoryPlan.xaml 的交互逻辑
    /// </summary>
    public partial class InventoryPlanDetail : UserControl
    {
        public delegate void HidePopInventoryPlanHandler(object sender, RoutedEventArgs e);
        public event HidePopInventoryPlanHandler HidePopInventoryPlanEvent;

        InventoryPlanDal inventoryPlanDal = new InventoryPlanDal();
        public InventoryPlanDetail()
        {
            InitializeComponent();
            //listView.DataContext = inventoryPlanDal.GetAllInventoryPlan().DefaultView;
            //使用ItemsSource的形式
            //listBox1.ItemsSource = GetDataTable().DefaultView;
            listView.SelectedIndex = 0;
        }

        private object GetDataTable()
        {
            //inventoryPlanDal.GetAllInventoryPlan();
            throw new NotImplementedException();
        }

        /// <summary>
        /// 保存事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onSave(object sender, RoutedEventArgs e)
        {
            

            HidePopInventoryPlanEvent(this, null);
        }

        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onCancel(object sender, RoutedEventArgs e)
        {
            HidePopInventoryPlanEvent(this, null);
        }

        /// <summary>
        /// 添加计划 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onAddPlan(object sender, RoutedEventArgs e)
        {

        }
    }
}
