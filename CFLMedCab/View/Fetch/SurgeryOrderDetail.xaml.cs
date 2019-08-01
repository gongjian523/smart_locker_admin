using CFLMedCab.APO.Surgery;
using CFLMedCab.BLL;
using CFLMedCab.DAL;
using CFLMedCab.DTO.Fetch;
using CFLMedCab.DTO.Goodss;
using CFLMedCab.DTO.Surgery;
using CFLMedCab.Http.Bll;
using CFLMedCab.Http.Model;
using CFLMedCab.Http.Model.Base;
using CFLMedCab.Infrastructure;
using CFLMedCab.Model;
using System;
using System.Collections;
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

namespace CFLMedCab.View.Fetch
{
    /// <summary>
    /// SurgeryNumQuery.xaml 的交互逻辑
    /// </summary>
    public partial class SurgeryOrderDetail : UserControl
    {
        public delegate void EnterSurgeryNumOpenHandler(object sender, FetchParam fetchParam);
        public event EnterSurgeryNumOpenHandler EnterSurgeryNumOpenEvent;

        private FetchParam param;

        public SurgeryOrderDetail(FetchParam fetchParam)
        {
            InitializeComponent();
            param = fetchParam;

            lbCodeContent.Content = fetchParam.bdConsumingOrder.body.objects[0].name;
            lbStatusContent.Content = fetchParam.bdConsumingOrder.body.objects[0].Status;

            if (fetchParam.bdConsumingOrder.body.objects[0].SourceBill.object_name == "OperationOrder")
            {
                lbCodeTitle.Content = "手术单号";
                listView.DataContext = fetchParam.bdOperationOrderGoodsDetail.body.objects;
            }
        }

        /// <summary>
        /// 确认领用（开柜）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnterSurgeryNumOpen(object sender, RoutedEventArgs e)
        {
            EnterSurgeryNumOpenEvent(this, param);
        }
    }
}
