using AutoMapper;
using CFLMedCab.BLL;
using CFLMedCab.DTO.Goodss;
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
        //public delegate void EnterAddProductHandler(object sender, InventoryDetailPara e);
        //public event EnterAddProductHandler EnterAddProductEvent;

        public delegate void EnterPopInventoryHandler(object sender, System.EventArgs e);
        public event EnterPopInventoryHandler EnterPopInventoryEvent;

        public delegate void HidePopInventoryHandler(object sender, System.EventArgs e);
        public event HidePopInventoryHandler HidePopInventoryEvent;

        public delegate void BackInventoryHandler(object sender, RoutedEventArgs e);
        public event BackInventoryHandler BackInventoryEvent;

        public event PropertyChangedEventHandler PropertyChanged;

        InventoryBll inventoryBll = new InventoryBll();
        GoodsBll goodsBll = new GoodsBll();

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


        public InventoryDtlLocal()
        {
            InitializeComponent();


            Timer timer = new Timer(100);
            timer.AutoReset = false;
            timer.Enabled = true;
            timer.Elapsed += new ElapsedEventHandler(onLoadData);

            //order = para;
            ////DataContext = order;
            //DataContext = this;
            //Code = order.code;
            //CreateTime = order.create_time;
            //Type = order.type;
            //Status = order.status;

            //dtlList = inventoryBll.GetInventoryDetailsByInventoryId(para.id);

            ////添加收到增加的商品
            //if (order.alreadyAddCodes == null)
            //    order.alreadyAddCodes = new HashSet<string>();

            //if (order.newlyAddCodes == null)
            //    order.newlyAddCodes = new HashSet<string>();
            //else
            //    order.alreadyAddCodes.UnionWith(order.newlyAddCodes);

            //if (order.alreadyAddCodes.Count > 0)
            //{
            //    var addGoodList = goodsBll.GetInvetoryGoodsDto(order.alreadyAddCodes);

            //    var config = new MapperConfiguration(x => x.CreateMap<GoodsDto, InventoryOrderdtl>()
            //                                    .ForMember(d => d.goods_id, o => o.MapFrom(s => s.id)));
            //    IMapper mapper = new Mapper(config);
            //    var listDtl = mapper.Map<List<InventoryOrderdtl>>(addGoodList);

            //    foreach (InventoryOrderdtl item in listDtl)
            //    {
            //        item.id = 0;
            //        item.inventory_order_id = para.id;
            //        item.goods_type = (int)GoodsInventoryStatus.Manual;
            //        item.book_inventory = 0;
            //        item.actual_inventory = 1;
            //        item.num_differences = 1;

            //        dtlList.Add(item);
            //    }
            //}

            goodsDtllistCheckView.DataContext = dtlList;


        }


        /// <summary>
        /// 新增实际库存商品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onLoadData(object sender, EventArgs e)
        {
            EnterPopInventoryEvent(this, null);

#if TESTENV
            HashSet<CommodityEps> hs = RfidHelper.GetEpcDataJsonInventory(out bool isGetSuccess);
#else
            HashSet<CommodityEps> hs = RfidHelper.GetEpcDataJson(out bool isGetSuccess);
#endif
            
            foreach(var item in hs.ToList())
            {
                GoodsDto goodItem = new GoodsDto {
                    name = item.CommodityName,
                    code = item.CommodityCodeName,
                };
                list.Add(goodItem);
            }

            int id = inventoryBll.NewInventory(list, InventoryType.Manual);

            HidePopInventoryEvent(this, null);

            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                //goodsDtllistConfirmView.DataContext = list;
                //goodsDtllistConfirmView.Items.Refresh();

                //codeInputTb.Focus();

            }));
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
