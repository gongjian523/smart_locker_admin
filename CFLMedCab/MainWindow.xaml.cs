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
using CFLMedCab.Infrastructure.DeviceHelper;
using System.IO.Ports;
using System.Timers;
using System.Media;
using CFLMedCab.View.Login;
using CFLMedCab.Model;
using GDotnet.Reader.Api.Protocol.Gx;
using GDotnet.Reader.Api.DAL;
using CFLMedCab.View.ReplenishmentOrder;
using CFLMedCab.View.ReturnGoodsOrder;
using CFLMedCab.DAL;
using SqlSugar;
using CFLMedCab.BLL;
using CFLMedCab.Infrastructure;
using System.Speech.Synthesis;

namespace CFLMedCab
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private DispatcherTimer ShowTimer;
        private VeinHelper vein;
        private SerialPort com;

        private Timer loginTimer;

        private SoundPlayer media;

        private Inventory inventory = new Inventory();
        //private LoginStatus loginStatus = new LoginStatus();
        
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
            Console.WriteLine("onStart");
            vein.ChekVein();

            //loginTimer = new Timer(20000);
            //loginTimer.AutoReset = false;
            //loginTimer.Enabled = true;
            //loginTimer.Elapsed += new ElapsedEventHandler(onLoginTimerUp);

            //App.Current.Dispatcher.Invoke((Action)(() =>
            //{
            //    PopFrame.Visibility = Visibility.Hidden;
            //    MaskView.Visibility = Visibility.Hidden;

            //        LoginBkView.Visibility = Visibility.Hidden;
            //}));

            //media = new SoundPlayer(@"../../Resources/Medias/Open-GerFetch.wav");
            //media.Play();

            //var user = new UserDal();
            //user.Insert(new User
            //{
            //    name = "aaa",
            //    role = 1,
            //    vein_id = "111sfadfasd"
            //});
            //user.GetList();

            ConsoleManager.Show();
        }



        private void onLoginTimerUp(object sender, ElapsedEventArgs e)
        {
            //LoginInfo.Visibility = Visibility.Hidden;
            //if (_loginStatus == 1)
            //{
            //    LoginBk.Visibility = Visibility.Hidden;
            //    NaviView.Visibility = Visibility.Visible;
            //}
            //else
            //{
            //    vein.ChekVein();
            //}
            Console.WriteLine("onLoginTimerUp");
            vein.ChekVein();
        }


        private void onLoginInfoHidenEvent(object sender, LoginStatus e)
        {
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                PopFrame.Visibility = Visibility.Hidden;
                MaskView.Visibility = Visibility.Hidden;

                if (e.LoginState == 1)
                    LoginBkView.Visibility = Visibility.Hidden;
            }));

            if (e.LoginState == 0)
            {
                Console.WriteLine("onLoginInfoHidenEvent");
                vein.Close();
                vein.ChekVein();
            }
        }


        private void onReceivedDataVein(object sender, SerialDataReceivedEventArgs e)
        {
            int id = vein.GetVeinId();

            if (id >= 0)
            {
                vein.Close();

                LoginStatus sta = new LoginStatus();

                //验证失败
                if (id == 0) {
                    sta.LoginState = 0;
                    sta.LoginString = "登录失败";
                    sta.LoginString2 = "请再次进行验证";
                }
                else {
                    //UserBll userBll = new UserBll();
                    //User user = userBll.GetUserByVeinId(id);
                    UserDal userDal = new UserDal();
                    User user = userDal.GetUserByVeinId(id);

                    //本地数据库中没有查询到此次指静脉信息
                    if (user == null) {
                        sta.LoginState = 0;
                        sta.LoginString = "登录失败";
                        sta.LoginString2 = "请再次进行验证";
                    }
                    //验证成功
                    else
                    {
                        sta.LoginState = 1;
                        sta.LoginString = "登录成功";
                        sta.LoginString2 = "欢迎" + user.name + "登录";

                        ApplicationState.SetValue((int)ApplicationKey.CurUser, user);
                    }
                }

                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    LoginInfo loginInfo = new LoginInfo(sta);

                    PopFrame.Visibility = Visibility.Visible;
                    MaskView.Visibility = Visibility.Visible;

                    loginInfo.LoginInfoHidenEvent += new LoginInfo.LoginInfoHidenHandler(onLoginInfoHidenEvent);

                    PopFrame.Navigate(loginInfo);

                }));
            }
            else
            {
                Console.WriteLine("onReceivedDataVein");
                vein.Close();
                vein.ChekVein();

            }

        }

        private void ShowCurTimer(object sender, EventArgs e)
        {
            ShowTime();
        }

        //ShowTime方法
        private void ShowTime()
        {
            //获得年月日
            this.tbDateText.Text = DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss");
            //获得时分秒
           // this.tbTimeText.Text = DateTime.Now.ToString("HH:mm:ss");
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
            SurgeryNumQuery surgeryQuery = new SurgeryNumQuery(); 
            ContentFrame.Navigate(surgeryQuery);
        }

        /// <summary>
        /// 补货入库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnterReplenishment_Click(object sender, RoutedEventArgs e)
        {
            Replenishment replenishment = new Replenishment();
            replenishment.EnterReplenishmentDetailEvent += new Replenishment.EnterReplenishmentDetailHandler(onEnterReplenishmentDetail);
            replenishment.EnterReplenishmentDetailOpenEvent += new Replenishment.EnterReplenishmentDetailOpenHandler(onEnterReplenishmentDetailOpen);

            ContentFrame.Navigate(replenishment);

        }

        private void onEnterReplenishmentDetail(object sender, ReplenishSubShortOrder e)
        {
            ReplenishmentDetail replenishmentDetail = new ReplenishmentDetail(e);
            ContentFrame.Navigate(replenishmentDetail);
        }

        private void onEnterReplenishmentDetailOpen(object sender, ReplenishSubShortOrder e)
        {
            NaviView.Visibility = Visibility.Hidden;

            ReplenishmentDetailOpen replenishmentDetailOpen = new ReplenishmentDetailOpen(e);
            FullFrame.Navigate(replenishmentDetailOpen);
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






        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Normal;
            this.WindowStyle = WindowStyle.None;
            this.ResizeMode = ResizeMode.CanResize;
#if !DEBUG
            this.Topmost = true; 
#endif
            this.Left = 0.0;
            this.Top = 0.0;
            this.Width = SystemParameters.PrimaryScreenWidth;
            this.Height = SystemParameters.PrimaryScreenHeight;
        }






		private void TestLocker(object sender, ElapsedEventArgs e)
		{
			Console.ReadKey();
			LockHelper.DelegateGetMsg delegateGetMsg = LockHelper.GetLockerData("COM2", out bool isGetSuccess);
			delegateGetMsg.DelegateGetMsgEvent += new LockHelper.DelegateGetMsg.DelegateGetMsgHandler(TestLockerEvent);
		}

		private void TestLockerEvent(object sender, bool isClose)
		{
			Console.WriteLine("返回开锁状态{0}", isClose);
			System.Diagnostics.Debug.WriteLine("返回开锁状态{0}", isClose); 
		}

	}
}
