using AutoMapper;
using CFLMedCab.BLL;
using CFLMedCab.DTO.Goodss;
using CFLMedCab.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
    /// InventoryConfirm.xaml 的交互逻辑
    /// </summary>
    public partial class InventoryDtl : UserControl,INotifyPropertyChanged
    {

        public delegate void EnterInventoryHandler(object sender, RoutedEventArgs e);
        public event EnterInventoryHandler EnterInventoryEvent;

        public delegate void EnterAddProductHandler(object sender, InventoryDetailPara e);
        public event EnterAddProductHandler EnterAddProductEvent;

        public event PropertyChangedEventHandler PropertyChanged;

        InventoryBll inventoryBll = new InventoryBll();
        GoodsBll goodsBll = new GoodsBll();

        InventoryDetailPara order = new InventoryDetailPara();
        List<InventoryOrderdtl> dtlList = new List<InventoryOrderdtl>();


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


        public InventoryDtl(InventoryDetailPara para)
        {
            InitializeComponent();

            order = para;
            //DataContext = order;
            DataContext = this;
            Code = order.code;
            CreateTime = order.create_time;
            Type = order.type;
            Status = order.status;

            dtlList = inventoryBll.GetInventoryDetailsByInventoryId(para.id);

            //添加收到增加的商品
            if (order.alreadyAddCodes == null)
                order.alreadyAddCodes = new HashSet<string>();
            
            if(order.newlyAddCodes == null)
                order.newlyAddCodes = new HashSet<string>();
            else
                order.alreadyAddCodes.UnionWith(order.newlyAddCodes);

            if (order.alreadyAddCodes.Count > 0)
            {
                var addGoodList = goodsBll.GetInvetoryGoodsDto(order.alreadyAddCodes);

                var config = new MapperConfiguration(x => x.CreateMap<GoodsDto, InventoryOrderdtl>()
                                                .ForMember(d => d.goods_id, o => o.MapFrom(s => s.id)));
                IMapper mapper = new Mapper(config);
                var listDtl = mapper.Map<List<InventoryOrderdtl>>(addGoodList);

                foreach (InventoryOrderdtl item in listDtl)
                {
                    item.id = 0;
                    item.inventory_order_id = para.id;
                    item.goods_type = (int)GoodsInventoryStatus.Manual;
                    item.book_inventory = 0;
                    item.actual_inventory = 1;
                    item.num_differences = 1;

                    dtlList.Add(item);
                }
            }

            goodsDtllistConfirmView.DataContext = dtlList;
            goodsDtllistCheckView.DataContext = dtlList;

            hideButtons(order.btnType == 0 ? true : false);
            goodsDtllistConfirmView.Visibility = order.btnType == 0 ? Visibility.Visible : Visibility.Collapsed;
            goodsDtllistCheckView.Visibility = order.btnType != 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 新增实际库存商品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onAddProduct(object sender, RoutedEventArgs e)
        {
            EnterAddProductEvent(this, order);
        }

        /// <summary>
        /// 盘点确认
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onConfirm(object sender, RoutedEventArgs e)
        {
            inventoryBll.ConfirmInventory((InventoryOrder)order);
            inventoryBll.UpdateInventoryDetails(dtlList);
            hideButtons(false);

            goodsDtllistConfirmView.Visibility = Visibility.Collapsed;
            goodsDtllistCheckView.Visibility = Visibility.Visible;

            Status = 1;
            goodsDtllistCheckView.Items.Refresh();
        }

        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onCancel(object sender, RoutedEventArgs e)
        {
            EnterInventoryEvent(this, null);
        }

        private void hideButtons(bool visible)
        {
            btnAddProduct.Visibility = visible ? Visibility.Visible : Visibility.Hidden;
            btnCancel.Visibility = visible ? Visibility.Visible : Visibility.Hidden;
            btnConfirm.Visibility = visible ? Visibility.Visible : Visibility.Hidden;
        }

        // This method is called by the Set accessor of each property.  
        // The CallerMemberName attribute that is applied to the optional propertyName  
        // parameter causes the property name of the caller to be substituted as an argument.  
        private void NotifyPropertyChanged([CallerMemberName]String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
