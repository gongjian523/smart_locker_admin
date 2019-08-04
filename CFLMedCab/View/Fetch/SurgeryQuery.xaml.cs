using CFLMedCab.APO.Surgery;
using CFLMedCab.BLL;
using CFLMedCab.Controls;
using CFLMedCab.DAL;
using CFLMedCab.DTO.Surgery;
using CFLMedCab.Http.Bll;
using CFLMedCab.Http.Model;
using CFLMedCab.Http.Model.Base;
using CFLMedCab.Infrastructure;
using CFLMedCab.Model;
using Newtonsoft.Json;
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
        public delegate void EnterSurgeryDetailHandler(object sender, FetchParam fetchParam);
        public event EnterSurgeryDetailHandler EnterSurgeryDetailEvent;
        
        public delegate void EnterSurgeryNoNumOpenHandler(object sender, ConsumingOrder e);
        public event EnterSurgeryNoNumOpenHandler EnterSurgeryNoNumOpenEvent;

        public delegate void EnterPrescriptionOpenHandler(object sender, ConsumingOrder e);
        public event EnterPrescriptionOpenHandler EnterPrescriptionOpenEvent;

        public delegate void ShowLoadDataHandler(object sender, RoutedEventArgs e);
        public event ShowLoadDataHandler ShowLoadDataEvent;

        public delegate void HideLoadDataHandler(object sender, RoutedEventArgs e);
        public event HideLoadDataHandler HideLoadDataEvent;

        KeyboardView kbHandler = new KeyboardView();

        ConsumingOrderType consumingOrderType;

        public SurgeryQuery(ConsumingOrderType type)
        {
            InitializeComponent();

            consumingOrderType = type;

            if (type == ConsumingOrderType.医嘱处方领用)
            {
                lbInputCode.Content = "请输入医嘱处方领号或扫描医嘱处方领单二维码";
                btnNoCode.Visibility = Visibility.Hidden;
            }
            else
            {
                lbInputCode.Content = "请输入手术单号或扫描手术单二维码";
                btnNoCode.Visibility = Visibility.Visible;
            }
            lbInputCode.Focus();
        }
    
        /// <summary>
        /// 查看详情
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnterDetail_Click(object sender, RoutedEventArgs e)
        {
            var inputStr = tbInputCode.Text;
            if (string.IsNullOrWhiteSpace(inputStr))
            {
                MessageBox.Show("单号不可以为空！", "温馨提示", MessageBoxButton.OK);
                return;
            }

            TaskOrder taskOrder;
            string name;
            try
            {
                taskOrder = JsonConvert.DeserializeObject<TaskOrder>(inputStr);
                name = taskOrder.name;
            }
            catch
            {
                name = inputStr;
            }

            ShowLoadDataEvent(this, null);

            FetchParam fetchParam = new FetchParam();
            fetchParam.bdConsumingOrder = ConsumingBll.GetInstance().GetConsumingOrder(name, consumingOrderType);

            if (fetchParam.bdConsumingOrder.code == 0)
            {
                if (fetchParam.bdConsumingOrder.body.objects[0].SourceBill.object_name == "OperationOrder")
                {
                    fetchParam.bdOperationOrderGoodsDetail = 
                        ConsumingBll.GetInstance().GetOperationOrderGoodsDetail(fetchParam.bdConsumingOrder);
                }
            }

            HideLoadDataEvent(this, null);

            if (fetchParam.bdConsumingOrder.code != 0)
            {
                MessageBox.Show("无法获取领用单详情！" + fetchParam.bdConsumingOrder.message, "温馨提示", MessageBoxButton.OK);
                return;
            }

            if (fetchParam.bdConsumingOrder.body.objects[0].SourceBill.object_name == "OperationOrder")
            {
                if (fetchParam.bdOperationOrderGoodsDetail.code != 0)
                {
                    MessageBox.Show("无法获取手术单物品详情！" + fetchParam.bdOperationOrderGoodsDetail.message, "温馨提示", MessageBoxButton.OK);
                    return;
                }

                HashSet<CommodityEps> hs = ApplicationState.GetGoodsInfo();
                BaseData<CommodityCode> bdCommodityCode =  CommodityCodeBll.GetInstance().GetCommodityCode(ApplicationState.GetGoodsInfo());

                ConsumingBll.GetInstance().CombinationStockNum(fetchParam.bdOperationOrderGoodsDetail, bdCommodityCode);

                EnterSurgeryDetailEvent(this, fetchParam);
            }
            else
            {
                EnterPrescriptionOpenEvent(this, fetchParam.bdConsumingOrder.body.objects[0]);
            }
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
                EnterDetail_Click(this, null);
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

        /// <summary>
        /// 获得焦点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onGotFocus(object sender, RoutedEventArgs e)
        {
            //kbHandler = new KeyboardView();
            kbHandler.SetCurrentControl((Control)sender);
            kbHandler.Topmost = true;
            kbHandler.Show();
        }

        /// <summary>
        /// 失去焦点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onLostFocus(object sender, RoutedEventArgs e)
        {
            kbHandler.Hide();
        }
    }
}
