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
using CFLMedCab.Model;
using CFLMedCab.View.Common;
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

        //显示加载数据的进度条
        public delegate void LoadingDataHandler(object sender, bool e);
        public event LoadingDataHandler LoadingDataEvent;

        public string Code { get; set; }
        public DateTime CreateTime { get; set; }
        public int Type { get; set; }
        public int Status { get; set; }

        private List<Locations> comboBoxList = new List<Locations>();

        private List<InventoryOrder> inventoryOrders = new List<InventoryOrder>();
        private List<CommodityCode> list = new List<CommodityCode>();

        public OpenDoorBtnBoard openDoorBtnBoard = new OpenDoorBtnBoard();

        public InventoryDtl(List<InventoryOrder> orders)
        {
            InitializeComponent();

            DataContext = this;
            CreateTime = DateTime.Now;
            Type = 1;
            Status = 0;
            Code = orders[0].name;
            inventoryOrders = orders;

            goodsDtllistConfirmView.DataContext = list;

            //加载开门按钮
            btnBorder.Visibility = Visibility.Visible;
            btnGrid.Children.Add(openDoorBtnBoard);

            comboBoxList = ApplicationState.GetLocations();
            locCb.ItemsSource = comboBoxList.OrderByDescending(it => it.Code);

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

            HashSet<CommodityEps> hs = RfidHelper.GetEpcDataJson(out bool isGetSuccess, ApplicationState.GetAllRfidCom());

            if (hs.Count > 0)
            {
                BaseData<CommodityCode> bdCommodityCode = CommodityCodeBll.GetInstance().GetCommodityCode(hs);
                HttpHelper.GetInstance().ResultCheck(bdCommodityCode, out bool isSuccess1);
                //CommodityCodeBll.GetInstance().GetExpirationAndManufactor(bdCommodityCode, out bool isSuccess2);
                CommodityCodeBll.GetInstance().GetExpiration(bdCommodityCode, out bool isSuccess2);
                if (isSuccess1)
                {
                    list = bdCommodityCode.body.objects.ToList();
                }
                else
                {
                    list = new List<CommodityCode>();
                }
            }
            else
            {
                list = new List<CommodityCode>();
            }

            List<string> ids = inventoryOrders.Select(item => item.GoodsLocationId).Distinct().ToList();
            BaseData<GoodsLocation> bdGoodsLocation = CommodityCodeBll.GetInstance().GetObjectByIds<GoodsLocation>(ids, out bool isSuccess);
            if(isSuccess)
            {
                inventoryOrders.ForEach(item => {
                    GoodsLocation goodsLocation = bdGoodsLocation.body.objects.Where(gl => gl.id == item.GoodsLocationId).First();
                    if (goodsLocation != null)
                    {
                        item.GoodsLocationName = goodsLocation.name;
                    }
                });
            }

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
                MessageBox.Show("商品编号不可以为空！", "温馨提示", MessageBoxButton.OK);
                return;
            }

            List<CommodityCode> adds = new List<CommodityCode>();

            LoadingDataEvent(this, true);
            BaseData<CommodityCode> bdCommodityCode = CommodityCodeBll.GetInstance().GetCommodityCodeByName(inputStr.ToUpper());
            //校验是否含有数据
            HttpHelper.GetInstance().ResultCheck(bdCommodityCode, out bool isSuccess);
            if(isSuccess)
            {
                //CommodityCodeBll.GetInstance().GetExpirationAndManufactor(bdCommodityCode, out bool isSuccess2);
                CommodityCodeBll.GetInstance().GetExpiration(bdCommodityCode, out bool isSuccess2);
            }
            LoadingDataEvent(this, false);

			if (!isSuccess)
			{
				MessageBox.Show("获取商品信息失败" + bdCommodityCode.message, "温馨提示", MessageBoxButton.OK);
				return;
			}

            bdCommodityCode.body.objects[0].EquipmentId = ApplicationState.GetEquipId();
            bdCommodityCode.body.objects[0].StoreHouseId = ApplicationState.GetHouseId();
            bdCommodityCode.body.objects[0].GoodsLocationId = ((Locations)locCb.SelectedItem).Id;
            bdCommodityCode.body.objects[0].GoodsLocationName = ((Locations)locCb.SelectedItem).Code;

            adds.Add(bdCommodityCode.body.objects[0]);

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
            BasePostData<InventoryOrderDetail> bdInventoryOrderDetail = InventoryTaskBll.GetInstance().CreateInventoryOrderDetail(list, inventoryOrders);
            LoadingDataEvent(this, false);

            //校验是否含有数据
            HttpHelper.GetInstance().ResultCheck(bdInventoryOrderDetail, out bool isSuccess1);

            if (!isSuccess1)
            {
                MessageBox.Show("创建盘点任务明细失败!" + bdInventoryOrderDetail.message, "温馨提示", MessageBoxButton.OK);
                return;
            }

            bool isSuccess = true;
            LoadingDataEvent(this, true);
            inventoryOrders.ForEach(item =>
            {
                BasePutData<InventoryOrder> bdInventoryOrder = InventoryTaskBll.GetInstance().UpdateInventoryOrderStatus(item);
                HttpHelper.GetInstance().ResultCheck(bdInventoryOrder, out bool isSuccessTemp);
                
                if(!isSuccessTemp)
                {
                    isSuccess = false;
                }
            });
            LoadingDataEvent(this, false);

            //校验是否含有数据
			if (!isSuccess)
			{
				MessageBox.Show("更新盘点任务单错误!", "温馨提示", MessageBoxButton.OK);
			}
            else
            {
                MessageBox.Show("更新盘点任务单成功!", "温馨提示", MessageBoxButton.OK);
                BackInventoryEvent(this, null);
            }
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

        public void SetButtonVisibility(bool bVisible)
        {
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                btnSubmit.Visibility = bVisible ? Visibility.Visible : Visibility.Hidden;
                btnCancel.Visibility = bVisible ? Visibility.Visible : Visibility.Hidden;
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
                string inputStr = codeInputTb.Text;

                CommodityOrder commodityOrder;
                try
                {
                    commodityOrder = JsonConvert.DeserializeObject<CommodityOrder>(inputStr);
                    codeInputTb.Text = commodityOrder.CommodityCodeName;
                }
                catch (Exception ex)
                {
                    LogUtils.Error($"数据解析失败！{inputStr} ; 异常报错为：{ex.Message}");
                    codeInputTb.Text = inputStr;
                }

            }
        }

        public void onDoorClosed(string com)
        {
            openDoorBtnBoard.SetButtonEnable(true, com);
        }
    }
}
