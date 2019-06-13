using CFLMedCab.DAL;
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
        ReplenishSubOrderdtlDal replenishSubOrderdtlDal = new ReplenishSubOrderdtlDal();
        //ReplenishOrder entity = new ReplenishOrder();
        //public ReplenishmentDetail(ReplenishOrder model)

        ReplenishSubShortOrder entity = new ReplenishSubShortOrder();
        public ReplenishmentDetail(ReplenishSubShortOrder model)
        {
            InitializeComponent();
            //操作人
            //principal.Content = model.principal_id;
            //principal.Content = ApplicationState.GetValue<User>((int)ApplicationKey.CurUser).name;
            //工单号
            workOrderNum.Content = model.id;
            //listView.DataContext = replenishSubOrderdtlDal.GetReplenishSubOrderdtl(model.id);
            entity = model;
        }
        
        /// <summary>
        /// 返回工单列表页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Replenishment replenishment = new Replenishment();
            ContentFrame.Navigate(replenishment);
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            //OpenCabinet openCabinet = new OpenCabinet();
            //openCabinet.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            //openCabinet.Owner = Application.Current.MainWindow;
            //openCabinet.ShowDialog();
        }
    }
}
