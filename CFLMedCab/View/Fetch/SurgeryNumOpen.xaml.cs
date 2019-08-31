using CFLMedCab.APO.Surgery;
using CFLMedCab.BLL;
using CFLMedCab.DTO.Fetch;
using CFLMedCab.DTO.Goodss;
using CFLMedCab.DTO.Surgery;
using CFLMedCab.Http.Model;
using CFLMedCab.Http.Model.Base;
using CFLMedCab.Infrastructure;
using CFLMedCab.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows;

namespace CFLMedCab.View.Fetch
{
    /// <summary>
    /// SurgeryNumOpen.xaml 的交互逻辑
    /// </summary>
    public partial class SurgeryNumOpen : UserControl
    {
        public OpenDoorBtnBoard openDoorBtnBoard = new OpenDoorBtnBoard();

        private FetchParam para;

        public SurgeryNumOpen(FetchParam fetchParam)
        {
            InitializeComponent();

            para = fetchParam;

            //lbCodeTitle.Content = "手术领用单号";

            ConsumingOrder consumingOrder = fetchParam.bdConsumingOrder.body.objects[0];
            List<ConsumingGoodsDetail> list = fetchParam.bdOperationOrderGoodsDetail.body.objects;

            //lbCodeContent.Content = consumingOrder.name;
            //lbStatusContent.Content = consumingOrder.Status;

            inStock.Content = list.Where(item => item.stockNum > 0).Count();
            noStock.Content = list.Where(item => item.stockNum == 0).Count();

            listView.DataContext = list;

            //只有一个柜门的时候，开门按钮不用显示，直接开门
            if (ApplicationState.GetAllLocIds().Count() == 1)
            {
                btnBorder.Visibility = Visibility.Collapsed;
            }
            else
            {
                btnBorder.Visibility = Visibility.Visible;
                btnGrid.Children.Add(openDoorBtnBoard);
            }
        }

        public void onDoorClosed(string com)
        {
            openDoorBtnBoard.SetButtonEnable(true, com);
        }

        public FetchParam GetFetchPara()
        {
            return para;
        }

    }
}
