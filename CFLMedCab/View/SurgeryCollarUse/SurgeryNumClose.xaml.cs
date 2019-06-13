using CFLMedCab.BLL;
using CFLMedCab.Infrastructure;
using CFLMedCab.Model;
using System;
using System.Collections;
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

namespace CFLMedCab.View.SurgeryCollarUse
{
    /// <summary>
    /// SurgeryNumClose.xaml 的交互逻辑
    /// </summary>
    public partial class SurgeryNumClose : UserControl
    {
        FetchOrderBll fetchOrderBll = new FetchOrderBll();
        FetchOrderdtlBll fetchOrderdtlBll = new FetchOrderdtlBll();
        SurgeryOrderBll surgeryOrderBll = new SurgeryOrderBll();
        public SurgeryNumClose(int id)
        {
            InitializeComponent();
            FetchOrder fetchOrder = fetchOrderBll.GetById(id);
            SurgeryOrder surgeryOrder = surgeryOrderBll.GetById(fetchOrder.business_order_id);
            lNum.Content = surgeryOrder.id;
            time.Content = surgeryOrder.surgery_dateiime;
            Operator.Content= ApplicationState.GetValue<User>((int)ApplicationKey.CurUser).name;
            listView.DataContext = fetchOrderdtlBll.GetDetailsUsage(id);
        }
        private int exceptional;
        private List<GoodsChageOrderdtl> goodsChageOrderdtls;

        /// <summary>
        /// 根据关门数据组合展示数据
        /// </summary>
        /// <param name="inHashtable">入库数据</param>
        /// <param name="outHashtable">出库 数据</param>
        /// <param name="type">领用类型</param>
        public void data(Hashtable inHashtable, Hashtable outHashtable, int type)
        {
            goodsChageOrderdtls = fetchOrderdtlBll.newGoodsChageOrderdtls(inHashtable, 1, type, ref exceptional);
            foreach (GoodsChageOrderdtl item in fetchOrderdtlBll.newGoodsChageOrderdtls(inHashtable, 3, type, ref exceptional))
            {
                goodsChageOrderdtls.Add(item);
            }
            listView.DataContext = goodsChageOrderdtls.OrderBy(t => t.expire_date).OrderBy(t => t.exceptional);
        }

        /// <summary>
        /// 结束本次领用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //CloseCabinet CloseCabinet = new CloseCabinet();
            //CloseCabinet.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            //CloseCabinet.Owner = Application.Current.MainWindow;
            //CloseCabinet.ShowDialog();
        }
    }
}
