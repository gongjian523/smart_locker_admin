using CFLMedCab.BLL;
using CFLMedCab.DAL;
using CFLMedCab.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        InventoryBll inventoryBll = new InventoryBll();

        List<InventoryPlanLDB> list = new List<InventoryPlanLDB>();

        public InventoryPlanDetail()
        {
            InitializeComponent();
            //listView.DataContext = inventoryPlanDal.GetAllInventoryPlan().DefaultView;
            //使用ItemsSource的形式
            //listBox1.ItemsSource = GetDataTable().DefaultView;

            list = inventoryBll.GetInventoryPlan();

            listView.DataContext = list;
            listView.SelectedIndex = 0;
        }

        private object GetDataTable()
        {

            throw new NotImplementedException();
        }

        /// <summary>
        /// 保存事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onSave(object sender, RoutedEventArgs e)
        {
            inventoryBll.UpdateInventoryPlan(list);

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
            string inputstr = planInputTb.Text;
            string pattenStr = "^(0\\d{1}|1\\d{1}|2[0-3]):([0-5]\\d{1})$";
            var ran = new Random();


            if(! Regex.IsMatch(inputstr, pattenStr))
            {
                planInputTb.Text = "请按照格式输入！";
                return;
            }

            if(list.Where(item => item.inventorytime_str == inputstr).ToList().Count > 0)
            {
                planInputTb.Text = "这个时间已经被设置！";
                return;
            }


            list.Add(new InventoryPlanLDB
            {
                id = 0,
                code = "P" + DateTime.Now.ToString("HHmmss") + ran.Next(9999),
                inventorytime_str = inputstr,
                status = 0
            });

            listView.Items.Refresh();
        }
    }
}
