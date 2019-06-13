﻿using CFLMedCab.Infrastructure;
using CFLMedCab.Infrastructure.DeviceHelper;
using CFLMedCab.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;


namespace CFLMedCab.View
{
    /// <summary>
    /// CloseCabinet.xaml 的交互逻辑
    /// </summary>
    public partial class CloseCabinet : UserControl
    {
        private Timer timer;

        public delegate void HidePopCloseHandler(object sender, RoutedEventArgs e);
        public event HidePopCloseHandler HidePopCloseEvent;

        public CloseCabinet()
        {
            InitializeComponent();
            bool isGetSuccess;
            ApplicationState.SetValue((int)ApplicationKey.CurGoods, RfidHelper.GetEpcData(out isGetSuccess));//读取机柜内当前的商品编码
            timer = new Timer(3000);
            timer.AutoReset = false;
            timer.Enabled = true;
            timer.Elapsed += new ElapsedEventHandler(onHidePopClose);
        }

        /// <summary>
        /// 三秒之后显示提示消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void onHidePopClose(object sender, EventArgs e)
        {
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                HidePopCloseEvent(this, null);
            }));
        }
    }
}
