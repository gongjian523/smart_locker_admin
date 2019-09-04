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

namespace CFLMedCab.View.Common
{
    /// <summary>
    /// LoginInfo.xaml 的交互逻辑
    /// </summary>
    public partial class LoginInfo : UserControl
    {
        public delegate void LoginInfoHidenHandler(object sender, LoginStatus e);
        public event LoginInfoHidenHandler LoginInfoHidenEvent;

        private Timer loginTimer;

        private LoginStatus loginSta;


        public LoginInfo(LoginStatus login)
        {
            InitializeComponent();
            DataContext = login;
            loginSta = login;

            loginTimer = new Timer(3000);
            loginTimer.AutoReset = false;
            loginTimer.Enabled = true;
            loginTimer.Elapsed += new ElapsedEventHandler(onTimerUp);
        }

        private void onTimerUp(object sender, ElapsedEventArgs e)
        {
            //App.Current.Dispatcher.Invoke((Action)(() =>
            //{
                LoginInfoHidenEvent(this, loginSta);
            //}));
        }
    }
}
