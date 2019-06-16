using CFLMedCab.DTO.Stock;
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

namespace CFLMedCab.View.Fetch
{
    /// <summary>
    /// NoSurgeryNumClose.xaml 的交互逻辑
    /// </summary>
    public partial class SurgeryNoNumClose : UserControl
    { 
        public delegate void EnterSurgeryNoNumOpenHandler(object sender, RoutedEventArgs e);
        public event EnterSurgeryNoNumOpenHandler EnterSurgeryNoNumOpenEvent;

        //跳出关闭弹出框
        public delegate void EnterPopCloseHandler(object sender, RoutedEventArgs e);
        public event EnterPopCloseHandler EnterPopCloseEvent;
        public SurgeryNoNumClose(string OddNumbers = null)
        {
            InitializeComponent();
            time.Content = DateTime.Now;
            List<GoodsChageOrderdtlDto> goodsChageOrderdtls = new List<GoodsChageOrderdtlDto>();
            for (int i = 5; i >= 0; i--)
            {
                GoodsChageOrderdtlDto goodsChageOrderdtl = new GoodsChageOrderdtlDto
                {
                    id = i,
                    batch_number = "feg",
                    birth_date = DateTime.Now,
                    code = "ewfw",
                    exception_flag = 0,
                    expire_date = DateTime.Now,
                    exception_description = "测试数据",
                    fetch_type = 1,
                    goods_code = "fwe",
                    goods_id = 1,
                    good_change_orderid = 1,
                    name = "测试数据",
                    operate_type = 0,
                    position = "1号柜",
                    related_order_id = 1,
                    remarks = "测试数据",
                    status = 0,
                    valid_period = 4
                };
                goodsChageOrderdtls.Add(goodsChageOrderdtl);
            }
            listView.DataContext = goodsChageOrderdtls;
        }

        /// <summary>
        /// 确认关柜
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEndOperation(object sender, RoutedEventArgs e)
        {
            EnterPopCloseEvent(this,null);
        }

        /// <summary>
        /// 不关柜
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onNoEndOperation(object sender, RoutedEventArgs e)
        {
            EnterSurgeryNoNumOpenEvent(this, null);
        }
    }
}
