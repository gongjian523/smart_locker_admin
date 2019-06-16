using CFLMedCab.BLL;
using CFLMedCab.DAL;
using CFLMedCab.DTO.Replenish;
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

namespace CFLMedCab.View.ReplenishmentOrder
{
    /// <summary>
    /// ReplenishmentDetail.xaml 的交互逻辑
    /// </summary>
    public partial class ReplenishmentDetail : UserControl
    {
        //ReplenishSubOrderdtlDal replenishSubOrderdtlDal = new ReplenishSubOrderdtlDal();
        //ReplenishOrder entity = new ReplenishOrder();
        //public ReplenishmentDetail(ReplenishOrder model)

        //进入补货单详情开门状态页面
        public delegate void EnterReplenishmentDetailOpenHandler(object sender, ReplenishSubOrderDto e);
        public event EnterReplenishmentDetailOpenHandler EnterReplenishmentDetailOpenEvent;

        //进入补货单列表页面
        public delegate void EnterReplenishmentHandler(object sender, RoutedEventArgs e);
        public event EnterReplenishmentHandler EnterReplenishmentEvent;
        ReplenishBll replenishBll = new ReplenishBll();
        ReplenishSubOrderDto entity = new ReplenishSubOrderDto();
        public ReplenishmentDetail(ReplenishSubOrderDto model)
        {
            InitializeComponent();
            entity = model;
            listView.DataContext = replenishBll.GetReplenishSubOrderdtlDto(new ReplenishSubOrderdtlApo { replenish_sub_orderid = model.id }).Data;
        }

        /// <summary>
        /// 返回工单列表页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Backwords_Click(object sender, RoutedEventArgs e)
        {
            EnterReplenishmentEvent(this, null);
        }

        private void EnterDetialOpen_Click(object sender, RoutedEventArgs e)
        {
            //ReplenishSubShortOrder replenishOrder = (ReplenishSubShortOrder)((Button)sender).Tag;
            EnterReplenishmentDetailOpenEvent(this, entity);
        }
    }
}
