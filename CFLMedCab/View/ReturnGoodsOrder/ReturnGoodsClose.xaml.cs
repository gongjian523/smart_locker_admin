﻿using CFLMedCab.DAL;
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

namespace CFLMedCab.View.ReturnGoodsOrder
{
    /// <summary>
    /// ReturnGoodsClose.xaml 的交互逻辑
    /// </summary>
    public partial class ReturnGoodsClose : UserControl
    {
        PickingSubOrderdtlDal pickingSubOrderdtlDal = new PickingSubOrderdtlDal();
        public ReturnGoodsClose(PickingOrder model)
        {
            InitializeComponent();
            //操作人
            principal.Content = model.principal_id;
            //工单号
            workOrderNum.Content = model.id;
            lDate.Content= DateTime.Now.ToString("yyyy年MM月dd日");
            listView.DataContext = pickingSubOrderdtlDal.GetPickingSubOrderdtl(model.id);
        }
    }
}