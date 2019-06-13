﻿using CFLMedCab.DAL;
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
        private SurgeryOrder surgeryOrder;
        private SurgeryOrderdtlDal surgeryOrderdtlDal = new SurgeryOrderdtlDal();
        public SurgeryNumQuery()
        {
            InitializeComponent();
        }

        public SurgeryNumQuery(SurgeryOrder model)
        {
            InitializeComponent();
           // listView.DataContext = surgeryOrderdtlDal.GetList();
            //使用ItemsSource的形式
            //listBox1.ItemsSource = GetDataTable().DefaultView;
            listView.SelectedIndex = 0;
            if (model != null)
            {
                surgeryOrder = model;
                lNum.Content = surgeryOrder.id;
            }
        }


        /// <summary>
        /// 手术耗材详情
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ConsumablesDetails consumablesDetails=new ConsumablesDetails(surgeryOrder);
            ContentFrame.Navigate(consumablesDetails);
        }

        /// <summary>
        /// 确认领用（开柜）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //OpenCabinet openCabinet = new OpenCabinet();
            //openCabinet.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            //openCabinet.Owner = Application.Current.MainWindow;
            //openCabinet.ShowDialog();
        }
    }
}
