using AutoMapper;
using CFLMedCab.BLL;
using CFLMedCab.DTO.Goodss;
using CFLMedCab.DTO.Inventory;
using CFLMedCab.Http.Model;
using CFLMedCab.Infrastructure.DeviceHelper;
using CFLMedCab.Model;
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
    public partial class InventoryDtlLocal : UserControl, INotifyPropertyChanged
    {
        public delegate void BackInventoryHandler(object sender, RoutedEventArgs e);
        public event BackInventoryHandler BackInventoryEvent;

        public event PropertyChangedEventHandler PropertyChanged;

        InventoryBll inventoryBll = new InventoryBll();

        List<InventoryOrderdtl> dtlList = new List<InventoryOrderdtl>();
        List<GoodsDto> list = new List<GoodsDto>();

        public string Code { get; set; }
        public DateTime CreateTime { get; set; }
        public int Type { get; set; }

        private int _status;
        public int Status
        {
            get { return _status; }
            set
            {
                if (value == _status)
                    return;
                _status = value;
                NotifyPropertyChanged("Status");
            }
        }


        public InventoryDtlLocal(int inventoryId)
        {
            InitializeComponent();

            DataContext = this;

            InventoryOrderDto order = inventoryBll.GetInventoryOrdersByInventoryId(inventoryId)[0];
            List<InventoryOrderdtl> dtlList = inventoryBll.GetInventoryDetailsByInventoryId(inventoryId);

            Code = order.code;
            CreateTime = order.create_time;
            //Type = order.type;
            //Status = order.status;

            goodsDtllistCheckView.DataContext = dtlList;

        }

        /// <summary>
        /// 返回的盘点界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onBackword(object sender, RoutedEventArgs e)
        {
            BackInventoryEvent(this, null);
        }


        ///// <summary>
        ///// 新增实际库存商品
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void onAddProduct(object sender, RoutedEventArgs e)
        //{
        //    EnterAddProductEvent(this, order);
        //}

        ///// <summary>
        ///// 盘点确认
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void onConfirm(object sender, RoutedEventArgs e)
        //{
        //    inventoryBll.ConfirmInventory((InventoryOrder)order);
        //    inventoryBll.UpdateInventoryDetails(dtlList);
        //    hideButtons(false);

        //    goodsDtllistConfirmView.Visibility = Visibility.Collapsed;
        //    goodsDtllistCheckView.Visibility = Visibility.Visible;

        //    Status = 1;
        //    goodsDtllistCheckView.Items.Refresh();
        //}

        ///// <summary>
        ///// 取消
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void onCancel(object sender, RoutedEventArgs e)
        //{
        //    EnterInventoryEvent(this, null);
        //}

        //private void hideButtons(bool visible)
        //{
        //    btnAddProduct.Visibility = visible ? Visibility.Visible : Visibility.Hidden;
        //    btnCancel.Visibility = visible ? Visibility.Visible : Visibility.Hidden;
        //    btnConfirm.Visibility = visible ? Visibility.Visible : Visibility.Hidden;
        //}

        // This method is called by the Set accessor of each property.  
        // The CallerMemberName attribute that is applied to the optional propertyName  
        // parameter causes the property name of the caller to be substituted as an argument.  
        private void NotifyPropertyChanged([CallerMemberName]String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
