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

namespace CFLMedCab.View.Fetch
{
    /// <summary>
    /// Query.xaml 的交互逻辑
    /// </summary>
    public partial class SurgeryQuery : UserControl
    {

        public delegate void EnterSurgeryDetailHandler(object sender, FetchOrder e);
        public event EnterSurgeryDetailHandler EnterSurgeryDetailEvent;
        
        public delegate void EnterSurgeryNoNumOpenHandler(object sender, RoutedEventArgs e);
        public event EnterSurgeryNoNumOpenHandler EnterSurgeryNoNumOpenEvent;
        public SurgeryQuery()
        {
            InitializeComponent();
        }
        //private FetchOrderDal fetchOrderDal = new FetchOrderDal();
        /// <summary>
        /// 查看详情
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnterDetail_Click(object sender, RoutedEventArgs e)
        {
            var value = tbOddNumbers.Text;
            if (string.IsNullOrWhiteSpace(value))
            {
                MessageBox.Show("手术单号不可以为空！", "温馨提示", MessageBoxButton.OK);
                return;
            }
            EnterSurgeryDetailEvent(this, new FetchOrder());
            //根据领用单查找手术单
            //FetchOrder fetchOrder = fetchOrderDal.CurrentDb.GetById(Convert.ToInt32(value));
            //if (fetchOrder.business_order_id > 0)
            //{
            //    EnterSurgeryDetailEvent(this, fetchOrder);
            //}
            //else
            //{
            //    MessageBox.Show("手术单号不存在！", "温馨提示", MessageBoxButton.OK);
            //    return;
            //}
        }
        
        /// <summary>
        /// 扫码查询事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                EnterSurgeryDetailEvent(this, new FetchOrder());
            }
        }

        /// <summary>
        /// 暂无手术单号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnterNoNum_Click(object sender, RoutedEventArgs e)
        {
            EnterSurgeryNoNumOpenEvent(this,null);
        }
    }
}
