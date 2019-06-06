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

namespace CFLMedCab.View
{
    /// <summary>
    /// Replenishment.xaml 的交互逻辑
    /// </summary>
    public partial class Replenishment : UserControl
    {
        PickingOrderDal pickingOrderDal = new PickingOrderDal();
        PickingSubOrderdtlDal pickingSubOrderdtlDal = new PickingSubOrderdtlDal();
        public Replenishment()
        {
            InitializeComponent();
            listView.DataContext = pickingOrderDal.GetAllPickingOrder();
        }
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //mb.Visibility = Visibility.Hidden;
        }
    }
}
