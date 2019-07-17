﻿using CFLMedCab.BLL;
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

namespace CFLMedCab.View.Return
{
    /// <summary>
    /// ReturnGoodsDetailOpen.xaml 的交互逻辑
    /// </summary>
    public partial class ReturnGoodsDetailOpen : UserControl
    {
        private PickingOrderDto pickingOrderDto;
        PickingBll pickingBll = new PickingBll();
        public ReturnGoodsDetailOpen(PickingOrderDto model)
        {
            InitializeComponent();
            pickingOrderDto = model;
            //操作人
            operatorName.Content = ApplicationState.GetValue<CurrentUser>((int)ApplicationKey.CurUser).name;
            //工单号
            orderNum.Content = model.code;
            listView.DataContext = pickingBll.GetPickingOrderdtlDto(new PickingSubOrderdtlApo { picking_order_code = model.code }).Data;
        }
    }
}
