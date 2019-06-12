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
using GDotnet.Reader.Api.Protocol.Gx;
using GDotnet.Reader.Api.DAL;
using CFLMedCab.View.ReplenishmentOrder;
using CFLMedCab.View.ReturnGoodsOrder;

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

            ConsoleManager.Show();
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

                App.Current.Dispatcher.Invoke((Action)(() =>
                {

                    LoginInfo loginInfo = new LoginInfo(new LoginStatus
                    {
                        LoginState = (id > 0) ? 1 : 0 ,
                        LoginString = (id > 0) ? "登录成功":"登录失败",
                        LoginString2 = (id > 0) ? "欢迎您登录": "请再次进行验证"
                    });

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


        public static void testRFID()
        {
            GClient clientConn = new GClient();
            eConnectionAttemptEventStatusType status;
            //COM1  主柜rfid串口
            //COM4  副柜rfid串口  
            if (clientConn.OpenSerial("COM4:115200", 3000, out status))
            //if (clientConn.OpenTcp("192.168.1.168:8160", 3000, out status))
            {
                // 订阅标签上报事件
                clientConn.OnEncapedTagEpcLog += new delegateEncapedTagEpcLog(OnEncapedTagEpcLog);
                clientConn.OnEncapedTagEpcOver += new delegateEncapedTagEpcOver(OnEncapedTagEpcOver);

                // 停止指令，空闲态
                MsgBaseStop msgBaseStop = new MsgBaseStop();
                clientConn.SendSynMsg(msgBaseStop);
                if (0 == msgBaseStop.RtCode)
                {
                    Console.WriteLine("Stop successful.");
                }
                else { Console.WriteLine("Stop error."); }

                // 功率配置, 将4个天线功率都设置为30dBm.
                MsgBaseSetPower msgBaseSetPower = new MsgBaseSetPower();
                msgBaseSetPower.DicPower = new Dictionary<byte, byte>()
                {
                    {1, 30},
                    {2, 30},
                    {3, 30},
                    {4, 30}
                };
                clientConn.SendSynMsg(msgBaseSetPower);
                if (0 == msgBaseSetPower.RtCode)
                {
                    Console.WriteLine("Power configuration successful.");
                }
                else { Console.WriteLine("Power configuration error."); }
                Console.WriteLine("Enter any character to start reading the tag.");
                Console.ReadKey();

                // 4个天线读卡, 读取EPC数据区以及TID数据区
                MsgBaseInventoryEpc msgBaseInventoryEpc = new MsgBaseInventoryEpc();
                msgBaseInventoryEpc.AntennaEnable = (uint)(eAntennaNo._1 | eAntennaNo._2 | eAntennaNo._3 | eAntennaNo._4);
                msgBaseInventoryEpc.InventoryMode = (byte)eInventoryMode.Inventory;
                msgBaseInventoryEpc.ReadTid = new ParamEpcReadTid();                // tid参数
                msgBaseInventoryEpc.ReadTid.Mode = (byte)eParamTidMode.Auto;
                msgBaseInventoryEpc.ReadTid.Len = 6;
                clientConn.SendSynMsg(msgBaseInventoryEpc);
                if (0 == msgBaseInventoryEpc.RtCode)
                {
                    Console.WriteLine("Inventory epc successful.");
                }
                else { Console.WriteLine("Inventory epc error."); }
                Console.ReadKey();

                // 停止读卡，空闲态
                clientConn.SendSynMsg(msgBaseStop);
                if (0 == msgBaseStop.RtCode)
                {
                    Console.WriteLine("Stop successful.");
                }
                else { Console.WriteLine("Stop error."); }
            }
            else
            {
                Console.WriteLine("Connect failure.");
            }
            Console.ReadKey();
        }

        public static void OnEncapedTagEpcLog(EncapedLogBaseEpcInfo msg)
        {
            // 回调内部如有阻塞，会影响API正常使用
            // 标签回调数量较多，请将标签数据先缓存起来再作业务处理
            if (null != msg && 0 == msg.logBaseEpcInfo.Result)
            {
                Console.WriteLine(msg.logBaseEpcInfo.ToString());
            }
        }

        public static void OnEncapedTagEpcOver(EncapedLogBaseEpcOver msg)
        {
            if (null != msg)
            {
                Console.WriteLine("Epc log over.");
            }
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Normal;
            this.WindowStyle = System.Windows.WindowStyle.None;
            this.ResizeMode = System.Windows.ResizeMode.NoResize;
            this.Topmost = true;
            this.Left = 0.0;
            this.Top = 0.0;
            this.Width = System.Windows.SystemParameters.PrimaryScreenWidth;
            this.Height = System.Windows.SystemParameters.PrimaryScreenHeight;
        }
    }
}
