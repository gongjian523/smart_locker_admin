using CFLMedCab.APO.Inventory;
using CFLMedCab.BLL;
using CFLMedCab.DTO.Goodss;
using CFLMedCab.DTO.Inventory;
using CFLMedCab.Http.Bll;
using CFLMedCab.Http.Model;
using CFLMedCab.Http.Model.Base;
using CFLMedCab.Infrastructure;
using CFLMedCab.Infrastructure.DeviceHelper;
using CFLMedCab.Infrastructure.ToolHelper;
using CFLMedCab.Model;
using Newtonsoft.Json;
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
    public partial class Inventory : UserControl
    {
        public delegate void EnterInventoryDetailHandler(object sender, InventoryTask e);
        public event EnterInventoryDetailHandler EnterInventoryDetailEvent;

        public Inventory()
        {
            InitializeComponent();
            DataContext = this;
        }


        /// <summary>
        /// 盘点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterInventoryDetail(object sender, RoutedEventArgs e)
        {
            string inputStr = tbInputNumbers.Text;
            if (string.IsNullOrWhiteSpace(inputStr))
            {
                MessageBox.Show("盘点任务单号不可以为空！", "温馨提示", MessageBoxButton.OK);
                return;
            }

            TaskOrder taskOrder;
            string taskName;
            try
            {
                taskOrder = JsonConvert.DeserializeObject<TaskOrder>(inputStr);
                taskName = taskOrder.name;
            }
            catch
            {
                taskName = inputStr;
            }

            BaseData<InventoryTask> bdInventoryTask =  InventoryTaskBll.GetInstance().GetInventoryTaskByInventoryTaskName(taskName);

            if(bdInventoryTask.code!=0)
            {
                MessageBox.Show("无法获取盘点任务单！", "温馨提示", MessageBoxButton.OK);
                return;
            }

            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                EnterInventoryDetailEvent(this, bdInventoryTask.body.objects[0]);
            }));
        }

        private void SearchBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                onEnterInventoryDetail(this, null);
            }
        }
    }
    
}
