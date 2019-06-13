﻿using CFLMedCab.BLL;
using CFLMedCab.DAL;
using CFLMedCab.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace CFLMedCab.View.SurgeryCollarUse
{
    /// <summary>
    /// SurgeryNumQuery.xaml 的交互逻辑
    /// </summary>
    public partial class SurgeryNumQuery : UserControl
    {
        private FetchOrder fetchOrder;
        private SurgeryOrderBll surgeryOrderBll = new SurgeryOrderBll();
        private FetchOrderBll fetchOrderBll = new FetchOrderBll(); 
        private FetchOrderdtlBll fetchOrderdtlBll = new FetchOrderdtlBll();
        public SurgeryNumQuery(int id)
        {
            InitializeComponent();
            fetchOrder = fetchOrderBll.GetById(id);
            SurgeryOrder surgeryOrder = surgeryOrderBll.GetById(fetchOrder.business_order_id);
            lNum.Content = surgeryOrder.id;
            time.Content = surgeryOrder.surgery_dateiime;
            listView.DataContext = fetchOrderdtlBll.GetDetailsUsage(id);
        }


        /// <summary>
        /// 手术耗材详情
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ConsumablesDetails consumablesDetails = new ConsumablesDetails(fetchOrder);
            ContentFrame.Navigate(consumablesDetails);
        }

        /// <summary>
        /// 确认领用（开柜）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
        }
    }
}
