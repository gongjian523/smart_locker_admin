using CFLMedCab.APO.Inventory;
using CFLMedCab.BLL;
using CFLMedCab.DTO.Goodss;
using CFLMedCab.DTO.Inventory;
using CFLMedCab.Http.Bll;
using CFLMedCab.Http.Helper;
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

        public delegate void EnterInventoryDetailLcoalHandler(object sender, int e);
        public event EnterInventoryDetailLcoalHandler EnterInventoryDetailLocalEvent;

        public delegate void EnterPopInventoryHandler(object sender, System.EventArgs e);
        public event EnterPopInventoryHandler EnterPopInventoryEvent;

        public delegate void HidePopInventoryHandler(object sender, System.EventArgs e);
        public event HidePopInventoryHandler HidePopInventoryEvent;

        //显示加载数据的进度条
        public delegate void LoadingDataHandler(object sender, bool e);
        public event LoadingDataHandler LoadingDataEvent;

        InventoryBll inventoryBll = new InventoryBll();
        List<InventoryOrderDto> inventoryOrderDtos = new List<InventoryOrderDto>();

        public Inventory()
        {
            InitializeComponent();

            Timer iniTimer = new Timer(100);
            iniTimer.AutoReset = false;
            iniTimer.Enabled = true;
            iniTimer.Elapsed += new ElapsedEventHandler(onInitData);
        }

        /// <summary>
        /// 加载数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onInitData(object sender, ElapsedEventArgs e)
        {
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                LoadingDataEvent(this, true);
                BaseData<InventoryPlan> bdInventoryPlan = InventoryTaskBll.GetInstance().GetInventoryPlanByEquipmnetNameOrId(ApplicationState.GetEquipName());
                LoadingDataEvent(this, false);

                if (bdInventoryPlan.code != 0)
                {
                    inventoryTime.Content = "无法获取盘点计划";
                }
                else
                {
                    InventoryPlan plan = bdInventoryPlan.body.objects[0];

                    if (plan.CheckPeriod == "每日")
                        inventoryTime.Content = plan.CheckPeriod + " " + plan.InventoryTime;
                    else if (plan.CheckPeriod == "每周")
                        inventoryTime.Content = plan.CheckPeriod + plan.InventoryWeekday + " " + plan.InventoryTime;
                    else if (plan.CheckPeriod == "每月")
                        inventoryTime.Content = plan.CheckPeriod + plan.InventoryDay + " " + plan.InventoryTime;
                    else
                        inventoryTime.Content = "";
                }
                DataContext = this;

                GetInventoryList();

                tbInputNumbers.Focus();
            }));
        }


        /// <summary>
        /// 进入盘点详情事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterInventoryDetail(object sender, RoutedEventArgs e)
        {
            //无法进入下一个页面，将输入框代码文字清空，并且重新设置焦点
            if (!HanleInventoryDetail())
            {
                tbInputNumbers.Text = "";
                tbInputNumbers.Focus();
            }
        }

        /// <summary>
        /// 处理进入盘点详情事件
        /// </summary>
        /// <returns></returns>
        private bool HanleInventoryDetail()
        {
            string inputStr = tbInputNumbers.Text;
            if (string.IsNullOrWhiteSpace(inputStr))
            {
                MessageBox.Show("盘点任务单号不可以为空！", "温馨提示", MessageBoxButton.OK);
                return false;
            }

            TaskOrder taskOrder;
            string taskName;
            try
            {
                taskOrder = JsonConvert.DeserializeObject<TaskOrder>(inputStr);
                taskName = taskOrder.name;
            }
            catch (Exception ex)
            {
                LogUtils.Error($"数据解析失败！{inputStr} ; 异常报错为：{ex.Message}");
                taskName = inputStr;
            }

            LoadingDataEvent(this, true);
            BaseData<InventoryTask> bdInventoryTask = InventoryTaskBll.GetInstance().GetInventoryTaskByInventoryTaskName(taskName);
            LoadingDataEvent(this, false);

            //校验是否含有数据
            HttpHelper.GetInstance().ResultCheck(bdInventoryTask, out bool isSuccess);

            if (!isSuccess)
            {
                MessageBox.Show("无法获取盘点任务单！", "温馨提示", MessageBoxButton.OK);
                return false;
            }

            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                EnterInventoryDetailEvent(this, bdInventoryTask.body.objects[0]);
            }));

            return true;
        }

        /// <summary>
        /// 进入盘点详情事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterInventoryDetailLocal(object sender, RoutedEventArgs e)
        {

            Button btnItem = sender as Button;
            int id;

            if (btnItem.Name == "LocalInventoryBtn")
            {
                EnterPopInventoryEvent(this, null);

#if TESTENV
                HashSet<CommodityEps> hs = RfidHelper.GetEpcDataJsonInventory(out bool isGetSuccess);
#else
                HashSet<CommodityEps> hs = RfidHelper.GetEpcDataJson(out bool isGetSuccess);
#endif
                List<GoodsDto> list = new List<GoodsDto>();
                foreach (var item in hs.ToList())
                {
                    GoodsDto goodItem = new GoodsDto
                    {
                        name = item.CommodityName,
                        code = item.CommodityCodeName,
                        position = item.GoodsLocationName
                    };
                    list.Add(goodItem);
                }

                id = inventoryBll.NewInventory(list, InventoryType.Manual);

                GetInventoryList();

                HidePopInventoryEvent(this, null);
            }
            else
            {
                id = (int)btnItem.CommandParameter;
            }

            EnterInventoryDetailLocalEvent(this, id);
        }

        private void SearchBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                onEnterInventoryDetail(this, null);
            }
        }


        private void GetInventoryList()
        {
            inventoryOrderDtos = inventoryBll.GetInventoryOrder(new InventoryOrderApo
            {
                PageSize = 0,
            }).Data;

            inventoryListView.DataContext = inventoryOrderDtos;
            inventoryListView.Items.Refresh();
        }
    }
    
}
