using CFLMedCab.Http.Model;
using CFLMedCab.Infrastructure;
using CFLMedCab.Model;
using CFLMedCab.Model.Enum;
using CFLMedCab.View.Common;
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
    /// GerFetchState.xaml 的交互逻辑
    /// </summary>
    public partial class GerFetchState : UserControl
    {
        public OpenDoorBtnBoard openDoorBtnBoard = new OpenDoorBtnBoard();

        private OpenDoorViewType viewType;

        //只是用来传递医嘱处方领用中的领用单
        private ConsumingOrder consumingOrder;

        //只是用来传递医嘱处方领用中的领用单
        private CommodityRecovery  commodityRecovery;

        public GerFetchState(OpenDoorViewType openDoorViewType, ConsumingOrder order = null, CommodityRecovery recovery= null)
        {
            InitializeComponent();

            viewType = openDoorViewType;

            consumingOrder = order;
            commodityRecovery = recovery;

            //只有一个柜门的时候，开门按钮不用显示，直接开门
            if (ApplicationState.GetAllLocIds().Count() == 1)
            {
                btnBorder.Visibility = Visibility.Collapsed;
            }
            else
            {
                btnBorder.Visibility = Visibility.Visible;
                btnGrid.Children.Add(openDoorBtnBoard);
                attention.Content = "请点击下列按钮，开启对应的货柜";
            }
        }

        public void  onDoorClosed(string com)
        {
            attention.Content = "还有柜门未关，操作完毕请关门";
            openDoorBtnBoard.SetButtonEnable(true, com);
        }

        public void onDoorOpen()
        {
            info.Visibility = Visibility.Visible;
            if (viewType == OpenDoorViewType.Fetch)
                attention.Content = "请拿取您需要的耗材，拿取完毕请关闭柜门";
            else if (viewType == OpenDoorViewType.FetchReturn)
                attention.Content = "请放入您需要回退的的耗材，放回完毕请关闭柜门";
            else
                attention.Content = "请您根据需要调整耗材，操作完毕请关闭柜门";
        }

        public ConsumingOrder GetConsumingOrder()
        {
            return consumingOrder;
        }

        public CommodityRecovery GetCommodityRecovery()
        {
            return commodityRecovery;
        }
    }
}
