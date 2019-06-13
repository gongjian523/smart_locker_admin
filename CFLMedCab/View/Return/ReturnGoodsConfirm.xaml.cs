using CFLMedCab.DAL;
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

namespace CFLMedCab.View.ReturnGoodsOrder
{
    /// <summary>
    /// ReturnGoodsConfirm.xaml 的交互逻辑
    /// </summary>
    public partial class ReturnGoodsConfirm : UserControl
    {
        PickingOrder entity = new PickingOrder();
        PickingSubOrderdtlDal pickingSubOrderdtlDal = new PickingSubOrderdtlDal();
        public ReturnGoodsConfirm(PickingOrder model)
        {
            InitializeComponent();
            //操作人
            principal.Content = model.principal_id;
            //工单号
            workOrderNum.Content = model.id;
            //listView.DataContext = pickingSubOrderdtlDal.GetPickingSubOrderdtl(model.id);
            entity = model;
        }

        /// <summary>
        /// 返回工单列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ReturnGoods returnGoods = new ReturnGoods();
            ContentFrame.Navigate(returnGoods);
        }

        /// <summary>
        /// 确认开柜
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            //OpenCabinet openCabinet = new OpenCabinet();
            //openCabinet.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            //openCabinet.Owner = Application.Current.MainWindow;
            //openCabinet.ShowDialog();
        }
    }
}
