using CFLMedCab.BLL;
using CFLMedCab.DAL;
using CFLMedCab.DTO.Picking;
using CFLMedCab.Http.Bll;
using CFLMedCab.Http.Helper;
using CFLMedCab.Http.Model;
using CFLMedCab.Http.Model.Base;
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

namespace CFLMedCab.View.Return
{
    /// <summary>
    /// ReturnGoodsConfirm.xaml 的交互逻辑
    /// </summary>
    public partial class ReturnGoodsDetail : UserControl
    {
        //进入拣货单详情开门状态页面
        public delegate void EnterReturnGoodsDetailOpenHandler(object sender, PickTask e);
        public event EnterReturnGoodsDetailOpenHandler EnterReturnGoodsDetailOpenEvent;

        //进入拣货单列表页面
        public delegate void EnterReturnGoodsHandler(object sender, RoutedEventArgs e);
        public event EnterReturnGoodsHandler EnterReturnGoodsEvent;

        private PickTask pickTask;

        public ReturnGoodsDetail(PickTask task)
        {
            InitializeComponent();

            pickTask = task;

            //操作人
            operatorName.Content = ApplicationState.GetValue<CurrentUser>((int)ApplicationKey.CurUser).name;
            operatorName.Content = ApplicationState.GetValue<CurrentUser>((int)ApplicationKey.CurUser).name;
            //工单号
            orderNum.Content = task.name;

            BaseData<PickCommodity> bdCommodityDetail = PickBll.GetInstance().GetPickTaskCommodityDetail(task);

            HttpHelper.GetInstance().ResultCheck(bdCommodityDetail, out bool isSuccess);
            if (!isSuccess)
            {
                MessageBox.Show("发生错误！", "温馨提示", MessageBoxButton.OK);
                return;
            }

            listView.DataContext = bdCommodityDetail.body.objects;
        }

        /// <summary>
        /// 返回工单列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onBackward(object sender, RoutedEventArgs e)
        {
            EnterReturnGoodsEvent(this, null);
        }

        /// <summary>
        /// 确认开柜
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnerDetailOpen(object sender, RoutedEventArgs e)
        {
            EnterReturnGoodsDetailOpenEvent(this, pickTask);
        }
    }
}
