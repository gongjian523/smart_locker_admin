using AutoMapper;
using CFLMedCab.BLL;
using CFLMedCab.DTO.Goodss;
using CFLMedCab.Http.Bll;
using CFLMedCab.Http.Helper;
using CFLMedCab.Http.Model;
using CFLMedCab.Http.Model.Base;
using CFLMedCab.Infrastructure;
using CFLMedCab.Infrastructure.DeviceHelper;
using CFLMedCab.Infrastructure.ToolHelper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CFLMedCab.View.Inventory
{
    /// <summary>
    /// InventoryConfirm.xaml 的交互逻辑
    /// </summary>
    public partial class InventoryDtl : UserControl
    {
        public delegate void SetPopInventoryHandler(object sender, bool e);
        public event SetPopInventoryHandler SetPopInventoryEvent;

        public delegate void BackInventoryHandler(object sender, RoutedEventArgs e);
        public event BackInventoryHandler BackInventoryEvent;

        public delegate void OpenDoorHandler(object sender, string e);
        public event OpenDoorHandler OpenDoorEvent;

        //显示加载数据的进度条
        public delegate void LoadingDataHandler(object sender, bool e);
        public event LoadingDataHandler LoadingDataEvent;

        public string Code { get; set; }
        public DateTime CreateTime { get; set; }
        public int Type { get; set; }
        public int Status { get; set; }

        private InventoryTask inventoryTask;
        private InventoryOrder inventoryOrder;
        private List<CommodityCode> list = new List<CommodityCode>();

        public InventoryDtl(InventoryTask task)
        {
            InitializeComponent();

            DataContext = this;
            Code = task.name;
            CreateTime = DateTime.Now;
            Type = 1;
            Status = 0;

            inventoryTask = task;

            goodsDtllistConfirmView.DataContext = list;

            Timer timer = new Timer(100);
            timer.AutoReset = false;
            timer.Enabled = true;
            timer.Elapsed += new ElapsedEventHandler(onLoadData);
        }


        /// <summary>
        /// 新增实际库存商品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onLoadData(object sender, EventArgs e)
        {
            SetPopInventoryEvent(this, true);

            BaseData<InventoryOrder> bdInventoryOrder = InventoryTaskBll.GetInstance().GetInventoryOrdersByInventoryTaskName(inventoryTask.name);

			//校验是否含有数据
			HttpHelper.GetInstance().ResultCheck(bdInventoryOrder, out bool isSuccess1);

			if (!isSuccess1)
			{
                SetPopInventoryEvent(this, false);
				MessageBox.Show("无法获取盘点任务单！", "温馨提示", MessageBoxButton.OK);
				return;
			}

            inventoryOrder = bdInventoryOrder.body.objects[0];
#if TESTENV
            HashSet<CommodityEps> hs = RfidHelper.GetEpcDataJsonInventory(out bool isGetSuccess);
#else
            HashSet<CommodityEps> hs = RfidHelper.GetEpcDataJson(out bool isGetSuccess);
#endif
            list = CommodityCodeBll.GetInstance().GetCommodityCode(hs).body.objects.ToList();

            SetPopInventoryEvent(this, false);

            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                goodsDtllistConfirmView.DataContext = list;
                goodsDtllistConfirmView.Items.Refresh();

                codeInputTb.Focus();

            }));
        }

        /// <summary>
        /// 新增实际库存商品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onAddProduct(object sender, RoutedEventArgs e)
        {
            string inputStr = codeInputTb.Text;
            if (string.IsNullOrWhiteSpace(inputStr))
            {
                MessageBox.Show("盘点任务单号不可以为空！", "温馨提示", MessageBoxButton.OK);
                return;
            }

            List<CommodityCode> adds = new List<CommodityCode>();
#if TESTENV
            CommodityEps commodityEps = new CommodityEps
            {
                CommodityCodeId = "AQACQqweBhEBAAAAVF0JmCFcsxUkKAIA",
                CommodityCodeName = "QR00000035",
                CommodityName = "止血包",
                EquipmentId = "AQACQqweDg8BAAAAFUD8WDEPsxV_FwQA",
                EquipmentName = "E00000008",
                GoodsLocationId = "AQACQqweJ4wBAAAAjYv6XmUPsxWWowMA",
                GoodsLocationName = "L00000013"
            };

            HashSet<CommodityEps> addHs = new HashSet<CommodityEps>();
            addHs.Add(commodityEps);

            adds = CommodityCodeBll.GetInstance().GetCommodityCode(addHs).body.objects.ToList();
#else

            CommodityOrder commodityOrder;
            string name;
            try
            {
                commodityOrder = JsonConvert.DeserializeObject<CommodityOrder>(inputStr);
                name = commodityOrder.CommodityCodeName;
            }
            catch(Exception ex)
            {
				LogUtils.Error($"数据解析失败！{inputStr} ; 异常报错为：{ex.Message}");
				name = inputStr;
            }

            LoadingDataEvent(this, true);
            BaseData<CommodityCode> bdCommodityCode = CommodityCodeBll.GetInstance().GetCommodityCodeByName(name);
            LoadingDataEvent(this, false);

            //校验是否含有数据
            HttpHelper.GetInstance().ResultCheck(bdCommodityCode, out bool isSuccess);

			if (!isSuccess)
			{
				MessageBox.Show("获取商品信息失败" + bdCommodityCode.message, "温馨提示", MessageBoxButton.OK);
				return;
			}

            bdCommodityCode.body.objects[0].EquipmentId = inventoryOrder.EquipmentId;
            bdCommodityCode.body.objects[0].StoreHouseId = inventoryOrder.StoreHouseId;
            bdCommodityCode.body.objects[0].GoodsLocationId = inventoryOrder.GoodsLocationId;
            bdCommodityCode.body.objects[0].GoodsLocationName = inventoryOrder.GoodsLocationName;

            adds.Add(bdCommodityCode.body.objects[0]);
#endif

            if (list.Where(item => item.name == adds[0].name).Count() > 0)
            {
                MessageBox.Show("此商品已经在列表中！", "温馨提示", MessageBoxButton.OK);
                return;
            }

            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    list.AddRange(adds);
                    goodsDtllistConfirmView.Items.Refresh();
                }));
            }));
        }


        /// <summary>
        /// 盘点确认
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onSubmit(object sender, RoutedEventArgs e)
        {
            LoadingDataEvent(this, true);
            BasePutData<InventoryOrder> bdInventoryOrder = InventoryTaskBll.GetInstance().UpdateInventoryOrderStatus(inventoryOrder);
            LoadingDataEvent(this, false);

            //校验是否含有数据
            HttpHelper.GetInstance().ResultCheck(bdInventoryOrder, out bool isSuccess);

			if (!isSuccess)
			{
				MessageBox.Show("更新盘点任务单失败!" + bdInventoryOrder.message, "温馨提示", MessageBoxButton.OK);
				return;
			}

            LoadingDataEvent(this, true);
            BasePostData<InventoryDetail> bdInventoryDetail = InventoryTaskBll.GetInstance().CreateInventoryDetail(list, inventoryOrder.id);
            LoadingDataEvent(this, false);


            //校验是否含有数据
            HttpHelper.GetInstance().ResultCheck(bdInventoryDetail, out bool isSuccess1);

			if (isSuccess1)
			{
				MessageBox.Show("提交盘点任务单成功!" + bdInventoryDetail.message, "温馨提示", MessageBoxButton.OK);
				BackInventoryEvent(this, null);
			}
			else
				MessageBox.Show("创建盘点商品明细失败!" + bdInventoryDetail.message, "温馨提示", MessageBoxButton.OK);

            return;
        }

        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onCancel(object sender, RoutedEventArgs e)
        {
            BackInventoryEvent(this, null);
        }

        private void onDelete(object sender, RoutedEventArgs e)
        {
            Button btnItem = sender as Button;

            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                list.RemoveAll(item => item.name == (string)btnItem.CommandParameter);
                goodsDtllistConfirmView.Items.Refresh();
            }));
        }

        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onOpenDoor(object sender, RoutedEventArgs e)
        {
            SetButtonVisibility(false);
            OpenDoorEvent(this, inventoryOrder.GoodsLocationName);
        }

        public void SetButtonVisibility(bool bVisible)
        {
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                btnSubmit.Visibility = bVisible ? Visibility.Visible : Visibility.Hidden;
                btnCancel.Visibility = bVisible ? Visibility.Visible : Visibility.Hidden;
                btnOpenDoor.Visibility = bVisible ? Visibility.Visible : Visibility.Hidden;
            }));
        }

        /// <summary>
        /// 扫码查询事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                onAddProduct(this, null);
            }
        }



    }
}
