using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace CFLMedCab.View
{
    /// <summary>
    /// OpenCabinet.xaml 的交互逻辑
    /// </summary>
    public partial class OpenCabinet : UserControl
    {
        private Timer timer;

        public delegate void HidePopOpenHandler(object sender, RoutedEventArgs e);
        public event HidePopOpenHandler HidePopOpenEvent;

        public OpenCabinet()
        {
            InitializeComponent();

            timer = new Timer(3000);
            timer.AutoReset = false;
            timer.Enabled = true;
            timer.Elapsed += new ElapsedEventHandler(onHidePopOpen);
        }

        /// <summary>
        /// 三秒之后显示提示消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void onHidePopOpen(object sender, EventArgs e)
        {
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                HidePopOpenEvent(this, null);
            }));
        }
    }
}
