using AutoMapper;
using CFLMedCab.BLL;
using CFLMedCab.DTO.Goodss;
using CFLMedCab.Model;
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
    /// InventoryConfirm.xaml 的交互逻辑
    /// </summary>
    public partial class InventoryDetail : UserControl
    {

        public delegate void EnterInventoryHandler(object sender, RoutedEventArgs e);
        public event EnterInventoryHandler EnterInventoryEvent;

        public delegate void EnterAddProductHandler(object sender, InventoryDetailPara e);
        public event EnterAddProductHandler EnterAddProductEvent;

        InventoryBll inventoryBll = new InventoryBll();
        GoodsBll goodsBll = new GoodsBll();

        InventoryDetailPara order = new InventoryDetailPara();
        List<InventoryOrderdtl> dtlList = new List<InventoryOrderdtl>();

        public InventoryDetail(InventoryDetailPara para)
        {
            InitializeComponent();

            order = para;
            DataContext = order;

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
                    item.book_inventory = 1;
                    item.actual_inventory = 1;
                    item.num_differences = 0;

                    dtlList.Add(item);
                }
            }

            goodsDtllistView.DataContext = dtlList;

            hideButtons();
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
            hideButtons();
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

        private void hideButtons()
        {
            btnAddProduct.Visibility = order.btnType == 0 ? Visibility.Visible : Visibility.Hidden;
            btnCancel.Visibility = order.btnType == 0 ? Visibility.Visible : Visibility.Hidden;
            btnConfirm.Visibility = order.btnType == 0 ? Visibility.Visible : Visibility.Hidden;
        }
    }
}
