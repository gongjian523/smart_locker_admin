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

namespace CFLMedCab.View.Return
{
    /// <summary>
    /// ReturnGoodsClose.xaml 的交互逻辑
    /// </summary>
    public partial class ReturnGoodsClose : UserControl
    {
        //进入补货单详情开门状态页面
        public delegate void EnterReturnGoodsDetailOpenHandler(object sender, PickingSubShortOrder e);
        public event EnterReturnGoodsDetailOpenHandler EnterReturnGoodsDetailOpenEvent;

        //跳出关闭弹出框
        public delegate void EnterPopCloseHandler(object sender, RoutedEventArgs e);
        public event EnterPopCloseHandler EnterPopCloseEvent;


        PickingSubOrderdtlDal pickingSubOrderdtlDal = new PickingSubOrderdtlDal();
        public ReturnGoodsClose(PickingOrder model)
        {
            InitializeComponent();
            //操作人
            principal.Content = model.principal_id;
            //工单号
            workOrderNum.Content = model.id;
            lDate.Content= DateTime.Now.ToString("yyyy年MM月dd日");
            //listView.DataContext = pickingSubOrderdtlDal.GetPickingSubOrderdtl(model.id);
        }

        /// <summary>
        /// 结束操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEndOperation(object sender, RoutedEventArgs e)
        {
            EnterPopCloseEvent(this, null);
        }

        /// <summary>
        /// 继续操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onNotEndOperation(object sender, RoutedEventArgs e)
        {
            EnterReturnGoodsDetailOpenEvent(this, new PickingSubShortOrder());
        }
    }
}