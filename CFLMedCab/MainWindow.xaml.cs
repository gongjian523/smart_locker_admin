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
using CFLMedCab.View;
using CFLMedCab.View.Inventory;
using CFLMedCab.View.SurgeryCollarUse;
using MahApps.Metro.Controls;
using CFLMedCab.Infrastructure.VeinHelper;
using System.IO.Ports;
using System.Timers;
using System.Media;
using CFLMedCab.View.Login;
using CFLMedCab.Model;

namespace CFLMedCab
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private DispatcherTimer ShowTimer;
        private VeinHelper vein;

        private Timer loginTimer;

        private SoundPlayer media;

        private Inventory inventory = new Inventory();
        //private LoginStatus loginStatus = new LoginStatus();


        private int _loginStatus;
        public int LoginStatus
        {
            set{
                _loginStatus = value;
            }
            get{
                return _loginStatus;
            }
        }

        private String  _loginString;
        public String LoginString
        {
            set{
                _loginString = value;
            }
            get{
                return _loginString;
            }
        }

        private String _loginString2;
        public String LoginString2
        {
            set
            {
                _loginString2 = value;
            }
            get
            {
                return _loginString2;
            }
        }


        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;

            ShowTime();
            ShowTimer = new DispatcherTimer();
            ShowTimer.Tick += new EventHandler(ShowCurTimer);//起个Timer一直获取当前时间
            ShowTimer.Interval = new TimeSpan(0, 0, 0, 1, 0);
            ShowTimer.Start();

            vein = new VeinHelper("COM9", 9600);
            vein.DataReceived += new SerialDataReceivedEventHandler(onReceivedDataVein);

            //vein.ChekVein();

            loginTimer = new Timer(3000);
            loginTimer.AutoReset = false;
            loginTimer.Enabled = true;
            loginTimer.Elapsed += new ElapsedEventHandler(onLoginTimerUp);

            _loginStatus = 1;
            _loginString = "登录成功";
            _loginString2 = "欢迎您登录";

            //media = new SoundPlayer("C:\\Open-GerFetch.wav"); 
            //media.Play();
        }



        private void onLoginTimerUp(object sender, ElapsedEventArgs e)
        {
            //LoginInfo.Visibility = Visibility.Hidden;
            //if (_loginStatus == 1)
            //{
            //    LoginBk.Visibility = Visibility.Hidden;
            //    FrameView.Visibility = Visibility.Visible;
            //}
            //else
            //{
            //    vein.ChekVein();
            //}



            App.Current.Dispatcher.Invoke((Action)(() =>
            {

                LoginInfo loginInfo = new LoginInfo(new LoginStatus
                {
                    LoginState = 0,
                    LoginString = "登录成功",
                    LoginString2 = "欢迎您登录"
                });

                PopFrame.Visibility = Visibility.Visible;
                MaskView.Visibility = Visibility.Visible;

                loginInfo.LoginInfoHidenEvent += new LoginInfo.LoginInfoHidenHandler(onLoginInfoHidenEvent);

                PopFrame.Navigate(loginInfo);

            }));
        }


        private void onLoginInfoHidenEvent(object sender, System.EventArgs e)
        {
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                PopFrame.Visibility = Visibility.Hidden;
                MaskView.Visibility = Visibility.Hidden;

                //if (loginStatus.LoginState == 1)
                LoginBkView.Visibility = Visibility.Hidden;
            }));
        }


        private void onReceivedDataVein(object sender, SerialDataReceivedEventArgs e)
        {
            int id = vein.GetVeinId();

            if (id > 0)
            {
                vein.Close();

                _loginStatus = 1;
                _loginString = "登录成功";
                _loginString2 = "欢迎您登录";



                //LoginInfo.Visibility = Visibility.Visible;
            }
            else if(id == 0)
            {
                _loginStatus = 0;
                _loginString = "登录失败";
                _loginString2 = "请再次进行验证";


                //LoginInfo.Visibility = Visibility.Visible;
            }
            else 
                vein.ChekVein();
               
        }

        private void ShowCurTimer(object sender, EventArgs e)
        {
            ShowTime();
        }

        //ShowTime方法
        private void ShowTime()
        {
            //获得年月日
            this.tbDateText.Text = DateTime.Now.ToString("yyyy年MM月dd日");
            //获得时分秒
            this.tbTimeText.Text = DateTime.Now.ToString("HH:mm");
        }

        /// <summary>
        /// 一般领用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnterGerFetch_Click(object sender, RoutedEventArgs e)
        {
            string log = (string)((RadioButton)sender).Content;
            GerFetchView gerFetchView = new GerFetchView(log);
            ContentFrame.Navigate(gerFetchView);

        }

        /// <summary>
        /// 手术领用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OperationCollarUse(object sender, RoutedEventArgs e)
        {
            SurgeryQuery surgeryQuery = new SurgeryQuery(); 
            ContentFrame.Navigate(surgeryQuery);
        }

        /// <summary>
        /// 补货入库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Replenishment(object sender, RoutedEventArgs e)
        {
            Replenishment replenishment = new Replenishment();
            ContentFrame.Navigate(replenishment);
        }

        /// <summary>
        /// 退货出库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReturnGoods(object sender, RoutedEventArgs e)
        {
            ReturnGoods returnGoods = new ReturnGoods();
            ContentFrame.Navigate(returnGoods);
        }

        /// <summary>
        /// 库存盘点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnterInv_Click(object sender, RoutedEventArgs e)
        {
            HomePageView.Visibility = Visibility.Hidden;
            BtnEntetInv.IsChecked = true;

            ContentFrame.Navigate(inventory);

            inventory.MaskShowEvent += new Inventory.MaskShowHandler(onInventoryMaskShowEvent);

        }

        private void onInventoryMaskShowEvent(object sender, System.EventArgs e)
        {
            //MaskView.Visibility = Visibility.Visible;
        }



        /// <summary>
        /// 库存查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Stock(object sender, RoutedEventArgs e)
        {
            Stock stock = new Stock();
            ContentFrame.Navigate(stock);
        }
    }
}
