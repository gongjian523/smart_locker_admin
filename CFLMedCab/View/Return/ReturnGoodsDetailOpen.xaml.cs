﻿using CFLMedCab.BLL;
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

namespace CFLMedCab.View.Return
{
    /// <summary>
    /// ReturnGoodsDetailOpen.xaml 的交互逻辑
    /// </summary>
    public partial class ReturnGoodsDetailOpen : UserControl
    {
        //显示加载数据的进度条
        public delegate void LoadingDataHandler(object sender, bool e);
        public event LoadingDataHandler LoadingDataEvent;

        private PickTask pickTask;

        public ReturnGoodsDetailOpen(PickTask task)
        {
            InitializeComponent();
            //操作人
            operatorName.Content = ApplicationState.GetUserInfo().name;
            //工单号
            orderNum.Content = task.name;

            pickTask = task;

            Timer iniTimer = new Timer(100);
            iniTimer.AutoReset = false;
            iniTimer.Enabled = true;
            iniTimer.Elapsed += new ElapsedEventHandler(onInitData);
        }

        /// <summary>
        /// 数据加载
        /// </summary>
        private void onInitData(object sender, ElapsedEventArgs e)
        {
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                LoadingDataEvent(this, true);
                BaseData<PickCommodity> bdCommodityDetail = PickBll.GetInstance().GetPickTaskCommodityDetail(pickTask);
                LoadingDataEvent(this, false);

                HttpHelper.GetInstance().ResultCheck(bdCommodityDetail, out bool isSuccess);
                if (!isSuccess)
                {
                    MessageBox.Show("获取拣货单商品明细错误！", "温馨提示", MessageBoxButton.OK);
                    return;
                }

                listView.DataContext = bdCommodityDetail.body.objects;
            }));
        }
    }
}
