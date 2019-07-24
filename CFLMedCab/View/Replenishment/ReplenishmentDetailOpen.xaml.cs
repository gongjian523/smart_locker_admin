using CFLMedCab.BLL;
using CFLMedCab.DTO.Replenish;
using CFLMedCab.Http.Model;
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

namespace CFLMedCab.View.ReplenishmentOrder
{
    /// <summary>
    /// ReplenishmentDetailOpen.xaml 的交互逻辑
    /// </summary>
    public partial class ReplenishmentDetailOpen : UserControl
    {
        ReplenishBll replenishBll = new ReplenishBll();

        public ReplenishmentDetailOpen(ShelfTask model)
        {
            InitializeComponent();
            operatorName.Content = ApplicationState.GetValue<CurrentUser>((int)ApplicationKey.CurUser).name;
            orderNum.Content = model.name;
            //listView.DataContext = replenishBll.GetReplenishOrderdtlDto(new ReplenishSubOrderdtlApo { replenish_order_code = model.code }).Data;
        }
    }
}
