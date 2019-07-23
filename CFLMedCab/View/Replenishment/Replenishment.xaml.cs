using CFLMedCab.APO;
using CFLMedCab.BLL;
using CFLMedCab.DAL;
using CFLMedCab.DTO.Replenish;
using CFLMedCab.Http.Bll;
using CFLMedCab.Http.Helper;
using CFLMedCab.Http.Model;
using CFLMedCab.Http.Model.Base;
using CFLMedCab.Infrastructure.DeviceHelper;
using CFLMedCab.Model;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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
    /// Replenishment.xaml 的交互逻辑
    /// </summary>
    public partial class Replenishment : UserControl
    {
        public delegate void EnterReplenishmentDetailHandler(object sender, ReplenishOrderDto e);
        public event EnterReplenishmentDetailHandler EnterReplenishmentDetailEvent;

        public delegate void EnterReplenishmentDetailOpenHandler(object sender, ReplenishOrderDto e);
        public event EnterReplenishmentDetailOpenHandler EnterReplenishmentDetailOpenEvent;

        ReplenishBll replenishBll = new ReplenishBll();
        public Replenishment()
        {
            InitializeComponent();
            DataContext = this;

            InitData();
        }

        private ObservableCollection<ReplenishOrderDto> _replenishOrderView = new ObservableCollection<ReplenishOrderDto>();
        public ObservableCollection<ReplenishOrderDto> ReplenishOrderViewList
        {
            get
            {
                return _replenishOrderView;
            }
            set
            {
                _replenishOrderView = value;
            }
        }

        /// <summary>
        /// 数据加载
        /// </summary>
        private void InitData()
        {
            ReplenishOrderViewList.Clear();

            List<ReplenishOrderDto> replenishOrders = replenishBll.GetReplenishOrderDto(new BasePageDataApo()).Data;
            replenishOrders.ForEach(replenishOrder => ReplenishOrderViewList.Add(replenishOrder));
        }
        
        /// <summary>
        /// 确认开柜
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterDetailOpen(object sender, RoutedEventArgs e)
        {
            ReplenishOrderDto replenishShortOrder = (ReplenishOrderDto)((Button)sender).Tag;
            EnterReplenishmentDetailOpenEvent(this, replenishShortOrder);
        }

        /// <summary>
        /// 查看详情
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterDetail(object sender, RoutedEventArgs e)
        {
            ReplenishOrderDto replenishShortOrder = (ReplenishOrderDto)((Button)sender).Tag;
            EnterReplenishmentDetailEvent(this, replenishShortOrder);
        }

        /// <summary>
        /// 查看详情
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnterDetail_Click(object sender, RoutedEventArgs e)
        {
            var value = tbInputNumbers.Text;
            if (string.IsNullOrWhiteSpace(value))
            {
                MessageBox.Show("拣货工单号不可以为空！", "温馨提示", MessageBoxButton.OK);
                return;
            }

#if LOCALSDK
            if (ReplenishOrderViewList.Where(item => item.code == value).ToList().Count == 0)
            {
                MessageBox.Show("此拣货工单中商品已经领取完毕, 或没有登记在您名下，或者不存在！", "温馨提示", MessageBoxButton.OK);
                return;
            }

            EnterReplenishmentDetailEvent(this, ReplenishOrderViewList.Where(item => item.code == value).First());
#else

            //TaskOrder taskOrder = JsonConvert.DeserializeObject<TaskOrder>(value);

            string name = "OS20190721000052";

            //if (taskOrder.name == null)
            //    name = value;
            //else
            //    name = taskOrder.name;

            BaseData<ShelfTask> baseDataShelfTask = ShelfBll.GetInstance().GetShelfTask(name);

            HttpHelper.GetInstance().ResultCheck(baseDataShelfTask, out bool isSuccess);
            if(!isSuccess)
            {
                MessageBox.Show("此拣货工单中商品已经领取完毕, 或没有登记在您名下，或者不存在！", "温馨提示", MessageBoxButton.OK);
                return;
            }

            BaseData<ShelfTaskCommodityDetail> baseDataShelfTaskCommodityDetail = ShelfBll.GetInstance().GetShelfTaskCommodityDetail(baseDataShelfTask);

            HttpHelper.GetInstance().ResultCheck(baseDataShelfTaskCommodityDetail, out bool isSuccess2);
            if (!isSuccess2)
            {
                MessageBox.Show("此拣货工单中商品已经领取完毕, 或没有登记在您名下，或者不存在！", "温馨提示", MessageBoxButton.OK);
                return;
            }

            EnterReplenishmentDetailEvent(this, ReplenishOrderViewList.Where(item => item.code == value).First());


#endif
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
    }
}
