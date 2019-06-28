using CFLMedCab.BLL;
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
using System.Windows.Shapes;

namespace CFLMedCab.View.Inventory
{
    /// <summary>
    /// AddStock.xaml 的交互逻辑
    /// </summary>
    /// 
    public partial class AddProduct : UserControl
    {
        public delegate void HidePopAddProductHandler(object sender, RoutedEventArgs e);
        public event HidePopAddProductHandler HidePopAddProductEvent;

        public delegate void EnterInventoryDetailHandler(object sender, InventoryDetailPara e);
        public event EnterInventoryDetailHandler EnterInventoryDetailEvent;


        GoodsBll goodsBll = new GoodsBll();
        InventoryBll inventoryBll = new InventoryBll();

        InventoryDetailPara inventoryPara = new InventoryDetailPara();
        List<AddGoodsCode> codeList = new List<AddGoodsCode>();

        public AddProduct(InventoryDetailPara dtlPara)
        {
            InitializeComponent();

            inventoryPara = dtlPara;
            listView.DataContext = codeList;

            codeInputTb.Focus();
        }

        private void onSave(object sender, RoutedEventArgs e)
        {
            HashSet<string> hs = new HashSet<string>();

            if(inventoryPara.newlyAddCodes != null)
                inventoryPara.newlyAddCodes.Clear();
            else
                inventoryPara.newlyAddCodes = new HashSet<string>();

            foreach (var code in codeList)
                inventoryPara.newlyAddCodes.Add(code.code);
            EnterInventoryDetailEvent(this, inventoryPara);

            HidePopAddProductEvent(this, null);
        }

        private void onCancel(object sender, RoutedEventArgs e)
        {
            HidePopAddProductEvent(this, null);
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


        private void onAddProduct(object sender, RoutedEventArgs e)
        {
            string inputStr = codeInputTb.Text;
            
            if(!goodsBll.IsGoodsInfoExsits(inputStr))
            {
                codeInputTb.Text = "无法查询到商品信息";
                return;
            }

            if(inventoryBll.IsGoodsInInventoryOrder(inventoryPara.id, inputStr))
            {
                codeInputTb.Text = "此商品已经在盘存单中";
                return;
            }

            if(inventoryPara.alreadyAddCodes != null)
            {
                if (inventoryPara.alreadyAddCodes.Contains(inputStr))
                {
                    codeInputTb.Text = "此商品已经添加";
                    return;
                }
            }

            if (codeList.Where(item => item.code == inputStr).ToList().Count > 0)
            {
                codeInputTb.Text = "此商品已经添加";
                return;
            }

            codeList.Add(new AddGoodsCode {
                code = inputStr,
                title = "单品码"
            });
            codeInputTb.Text = "";

            listView.Items.Refresh();
        }
    }
}
