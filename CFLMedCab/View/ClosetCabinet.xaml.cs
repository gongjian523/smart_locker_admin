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
            ShowTimer.Interval = TimeSpan.FromSeconds(3);//设置定时间隔为1s
            ShowTimer.Tick += new EventHandler(time); ;//注册定时中断事件
            ShowTimer.Start();//定时器开启
        }

        /// <summary>
        /// 三秒之后显示提示消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void time(object sender, EventArgs e)
        {
            lTips.Content = "本次操作完成";
            tips.Visibility = Visibility.Visible;
        }

    }
}
