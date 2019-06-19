using CFLMedCab.BLL;
using CFLMedCab.DAL;
using CFLMedCab.DTO.Picking;
using CFLMedCab.Infrastructure;
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
using System.Windows.Threading;

namespace CFLMedCab.View.Return
{
    /// <summary>
    /// ReturnGoodsConfirm.xaml 的交互逻辑
    /// </summary>
    public partial class ReturnGoodsDetail : UserControl
    {
        //进入拣货单详情开门状态页面
        public delegate void EnterReturnGoodsDetailOpenHandler(object sender, PickingSubOrderDto e);
        public event EnterReturnGoodsDetailOpenHandler EnterReturnGoodsDetailOpenEvent;

        //进入拣货单列表页面
        public delegate void EnterReturnGoodsHandler(object sender, RoutedEventArgs e);
        public event EnterReturnGoodsHandler EnterReturnGoodsEvent;

        private PickingSubOrderDto pickingSubOrderDto;
        PickingBll pickingBll = new PickingBll();
        public ReturnGoodsDetail(PickingSubOrderDto model)
        {
            InitializeComponent();
            pickingSubOrderDto = model;
            //操作人
            operatorName.Content = ApplicationState.GetValue<User>((int)ApplicationKey.CurUser).name;
            //工单号
            orderNum.Content = model.code;
            listView.DataContext = pickingBll.GetPickingSubOrderdtlDto(new PickingSubOrderdtlApo { picking_sub_orderid = model.id }).Data;
        }

        /// <summary>
        /// 返回工单列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onBackward(object sender, RoutedEventArgs e)
        {
            EnterReturnGoodsEvent(this, null);
        }

        /// <summary>
        /// 确认开柜
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnerDetailOpen(object sender, RoutedEventArgs e)
        {
            EnterReturnGoodsDetailOpenEvent(this, pickingSubOrderDto);
        }
    }
}
