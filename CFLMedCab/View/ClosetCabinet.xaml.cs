﻿using System;
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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace CFLMedCab.View
{
    /// <summary>
    /// ClosetCabinet.xaml 的交互逻辑
    /// </summary>
    public partial class ClosetCabinet : Window
    {
        private DispatcherTimer ShowTimer;
        public ClosetCabinet()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 页面加载完事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void window_contentRendered(object sender, EventArgs e)
        {
            ShowTimer = new DispatcherTimer();
            ShowTimer.Interval = TimeSpan.FromSeconds(3);//设置定时间隔为
            ShowTimer.Tick += new EventHandler(Time); ;//注册定时中断事件
            ShowTimer.Start();//定时器开启
        }

        /// <summary>
        /// 三秒之后显示提示消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Time(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
