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
using CFLMedCab.View.Return;
using CFLMedCab.DAL;
using SqlSugar;
using CFLMedCab.BLL;
using CFLMedCab.Infrastructure;
using System.Speech.Synthesis;
using CFLMedCab.View.Fetch;
using System.Collections;
using CFLMedCab.DTO.Replenish;
using CFLMedCab.DTO.Picking;
using CFLMedCab.Test;
using CFLMedCab.DTO.Goodss;
using CFLMedCab.APO.Surgery;
using CFLMedCab.DTO.Surgery;
using System.Runtime.InteropServices;
using CFLMedCab.Controls;
using static CFLMedCab.Controls.Taskbar;
using System.Windows.Forms;
using static CFLMedCab.Model.Enum.UserIdEnum;
using CFLMedCab.Infrastructure.BootUpHelper;
using System.Windows.Interop;

namespace CFLMedCab
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private DispatcherTimer ShowTimer;
        private DispatcherTimer InventoryTimer;
        private VeinHelper vein;

        private InventoryBll inventoryBll = new InventoryBll();
        private GoodsBll goodsBll = new GoodsBll();
        private UserBll userBll = new UserBll();
        private FetchOrderBll fetchOrderBll = new FetchOrderBll();
        private ReplenishBll replenishBll = new ReplenishBll();
        private PickingBll pickingBll = new PickingBll();

        private int cabClosedNum;

        private TestGoods test = new TestGoods();

        private User _curUser;
        public User CurUser { get {
                return _curUser;
            }
            set {
                _curUser = value;
            }
        }


#if TESTENV
        private System.Timers.Timer testTimer;
        private SurgeryOrderDto testSOPara = new SurgeryOrderDto();
        private ReplenishOrderDto testROPara = new ReplenishOrderDto();
        private PickingOrderDto testPOPara = new PickingOrderDto();
#endif

        public MainWindow()
        {
            Taskbar.HideTask(true);

            //开启启动
            BootUpHelper.GetInstance().SetMeAutoStart();

			InitializeComponent();
       
            foreach (Screen scr in Screen.AllScreens)
            {
                if (scr.Primary)
                {
                    //设置窗体位置
                    WindowStartupLocation = WindowStartupLocation.Manual;
                    Left = scr.WorkingArea.Left;
                    Top = scr.WorkingArea.Top;
                    Width = scr.Bounds.Width;
                    Height = scr.Bounds.Height;
                    Topmost = true;
                    WindowState = WindowState.Maximized;
                    ResizeMode = ResizeMode.NoResize;
                    WindowStyle = WindowStyle.None;
                    WindowState = WindowState.Normal;
                    ShowInTaskbar = false;

                    break;
                }
            }
            //this.Loaded += MetroWindow_Loaded;
            //this.Deactivated += MainWindow_Deactivated;
            //this.StateChanged += MainWindow_StateChanged;

            MockData();

            DataContext = this;

            this.tbNameText.Text = ApplicationState.GetValue<User>((int)ApplicationKey.CurUser).name;

            ShowTime();
            ShowTimer = new DispatcherTimer();
            ShowTimer.Tick += new EventHandler(ShowCurTimer);//起个Timer一直获取当前时间
            ShowTimer.Interval = new TimeSpan(0, 0, 0, 1, 0);
            ShowTimer.Start();

            InventoryTimer = new DispatcherTimer();
            InventoryTimer.Tick += new EventHandler(onInventoryTimer);//起个Timer, 每分钟检查是否有扫描计划
            InventoryTimer.Interval = new TimeSpan(0, 0, 1, 0, 0);
            InventoryTimer.Start();

            vein = new VeinHelper("COM9", 9600);
            vein.DataReceived += new SerialDataReceivedEventHandler(onReceivedDataVein);
            Console.WriteLine("onStart");
            vein.ChekVein();

            ConsoleManager.Show();

            //LoginBkView.Visibility = Visibility.Visible;
        }

        private void MainWindow_StateChanged(object sender, EventArgs e)
        {
            this.WindowState = WindowState.Maximized;
        }

        private void MainWindow_Deactivated(object sender, EventArgs e)
        {
            this.Topmost = true;
        }

        private void onInventoryTimer(object sender, EventArgs e)
        {
            List <InventoryPlan> listPan = inventoryBll.GetInventoryPlan().ToList().Where(item => item.status == 0).ToList();

            foreach(var item in listPan)
            {

                DateTime date1 = DateTime.Now;
                DateTime date2 = new DateTime(date1.Year, date1.Month, date1.Day, int.Parse(item.inventorytime_str.Substring(0,2)), int.Parse(item.inventorytime_str.Substring(3,2)), 0);

                TimeSpan timeSpan = date2 - date1;

                if (timeSpan.TotalMinutes < 1 && timeSpan.TotalMinutes > 0 )
                {
                    bool isGetSuccess;
                    Hashtable ht = RfidHelper.GetEpcData(out isGetSuccess);

                    List<GoodsDto> list = goodsBll.GetInvetoryGoodsDto(ht);
                    inventoryBll.NewInventory(list,InventoryType.Auto);

                    return;
                }

                Console.WriteLine("onInventoryTimer:" + timeSpan.TotalMinutes);
            }
        }


        private void onLoginInfoHidenEvent(object sender, LoginStatus e)
        {
            //App.Current.Dispatcher.Invoke((Action)(() =>
            //{
            //    PopFrame.Visibility = Visibility.Hidden;
            //    MaskView.Visibility = Visibility.Hidden;

            //    //验证成功不跳出提示弹窗
            //    //if (e.LoginState == 1)
            //    //    LoginBkView.Visibility = Visibility.Hidden;
            //}));
            ClosePop();

            if (e.LoginState == 0)
            {
                Console.WriteLine("onLoginInfoHidenEvent");
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
                if (id == 0)
                {
                    sta.LoginState = 0;
                    sta.LoginString = "登录失败";
                    sta.LoginString2 = "请再次进行验证";
                }
                else
                {
                    User user = userBll.GetUserByVeinId(id);

                    //本地数据库中没有查询到此次指静脉信息
                    if (user == null)
                    {
                        sta.LoginState = 0;
                        sta.LoginString = "登录失败";
                        sta.LoginString2 = "请再次进行验证";

                        App.Current.Dispatcher.Invoke((Action)(() =>
                        {
                            LoginInfo loginInfo = new LoginInfo(sta);

                            PopFrame.Visibility = Visibility.Visible;
                            MaskView.Visibility = Visibility.Visible;

                            loginInfo.LoginInfoHidenEvent += new LoginInfo.LoginInfoHidenHandler(onLoginInfoHidenEvent);

                            PopFrame.Navigate(loginInfo);

                            

                        }));

                    }
                    //验证成功
                    else
                    {
                        sta.LoginState = 1;
                        sta.LoginString = "登录成功";
                        sta.LoginString2 = "欢迎" + user.name + "登录";

                        ApplicationState.SetValue((int)ApplicationKey.CurUser, user);

                        App.Current.Dispatcher.Invoke((Action)(() =>
                        {
                            LoginBkView.Visibility = Visibility.Hidden;
                            SetNavBtnVisiblity(user.role);
                            tbNameText.Text = ApplicationState.GetValue<User>((int)ApplicationKey.CurUser).name;
                        }));
                    }
                }
            }
            else
            {
                Console.WriteLine("onReceivedDataVein");
                vein.ChekVein();
            }

        }

        private void SetNavBtnVisiblity(int role)
        {
            bool isMedicalStuff = ((UserIdType)role == UserIdType.医生 || (UserIdType)role == UserIdType.护士 || (UserIdType)role == UserIdType.医院管理员) ? true : false;

            NavBtnEnterGerFetch.Visibility = isMedicalStuff ? Visibility.Visible : Visibility.Hidden;
            NavBtnEnterSurgery.Visibility = isMedicalStuff ? Visibility.Visible : Visibility.Hidden;
            NavBtnEnterReturnFetch.Visibility = isMedicalStuff ? Visibility.Visible : Visibility.Hidden;
            NavBtnEnterReplenishment.Visibility = (!isMedicalStuff) ? Visibility.Visible : Visibility.Hidden;
            NavBtnEnterReturnGoods.Visibility = (!isMedicalStuff) ? Visibility.Visible : Visibility.Hidden;
            NavBtnEnterInvtory.Visibility = (!isMedicalStuff) ? Visibility.Visible : Visibility.Hidden;
            NavBtnEnterStock.Visibility = (!isMedicalStuff) ? Visibility.Visible : Visibility.Hidden;
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
        }


        private void onExitApp(object sender, RoutedEventArgs e)
        {
            Taskbar.HideTask(false);
            System.Environment.Exit(0);
        }
        
        
        #region 领用
        #region 一般领用
        /// <summary>
        /// 一般领用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterGerFetch(object sender, RoutedEventArgs e)
        {
            HomePageView.Visibility = Visibility.Hidden;
            btnBackHP.Visibility = Visibility.Visible;
            NaviView.Visibility = Visibility.Hidden;

            GerFetchState gerFetchState = new GerFetchState(1);
            FullFrame.Navigate(gerFetchState);

            List<string> com = ComName.GetAllLockerCom();

            LockHelper.DelegateGetMsg delegateGetMsg = LockHelper.GetLockerData(com[0], out bool isGetSuccess);
            delegateGetMsg.DelegateGetMsgEvent += new LockHelper.DelegateGetMsg.DelegateGetMsgHandler(onEnterGerFectchLockerEvent);

            LockHelper.DelegateGetMsg delegateGetMsg2 = LockHelper.GetLockerData(com[1], out bool isGetSuccess2);
            delegateGetMsg2.DelegateGetMsgEvent += new LockHelper.DelegateGetMsg.DelegateGetMsgHandler(onEnterGerFectchLockerEvent);

            cabClosedNum = 0;

            SpeakerHelper.Sperker("柜门已开，请拿取您需要的耗材，拿取完毕请关闭柜门");
        }

        /// <summary>
        /// 一般领用关门状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterGerFectchLockerEvent(object sender, bool isClose)
        {
            System.Diagnostics.Debug.WriteLine("返回开锁状态{0}", isClose);

            if (!isClose)
                return;

            if (cabClosedNum == 0)
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    GerFetchState gerFetchState = new GerFetchState(3);
                    FullFrame.Navigate(gerFetchState);
                }));

                cabClosedNum++;
                return;
            }

            //弹出盘点中弹窗
            EnterInvotoryOngoing();

            Hashtable ht = RfidHelper.GetEpcData(out bool isGetSuccess);

            //关闭盘点中弹窗
            ClosePop();
            
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                GerFetchView gerFetchView = new GerFetchView(ht);
                gerFetchView.EnterPopCloseEvent += new GerFetchView.EnterPopCloseHandler(onEnterPopClose);
                gerFetchView.EnterGerFetch += new GerFetchView.EnterFetchOpenHandler(onEnterGerFetch);
                FullFrame.Navigate(gerFetchView);
            }));
        }
#endregion

#region 手术领用

#region 无手术单领用
        /// <summary>
        /// 进入手术无单领用-开门状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterSurgeryNoNumOpen(object sender, RoutedEventArgs e)
        {
            NaviView.Visibility = Visibility.Hidden;

            GerFetchState gerFetchState = new GerFetchState(1);
            FullFrame.Navigate(gerFetchState);

            List<string> com = ComName.GetAllLockerCom();

            LockHelper.DelegateGetMsg delegateGetMsg = LockHelper.GetLockerData(com[0], out bool isGetSuccess);
            delegateGetMsg.DelegateGetMsgEvent += new LockHelper.DelegateGetMsg.DelegateGetMsgHandler(onEnterSurgeryNoNumLockerEvent);

            LockHelper.DelegateGetMsg delegateGetMsg2 = LockHelper.GetLockerData(com[1], out bool isGetSuccess2);
            delegateGetMsg2.DelegateGetMsgEvent += new LockHelper.DelegateGetMsg.DelegateGetMsgHandler(onEnterSurgeryNoNumLockerEvent);

            cabClosedNum = 0;

            SpeakerHelper.Sperker("柜门已开，请拿取您需要的耗材，拿取完毕请关闭柜门");
        }

        /// <summary>
        /// 手术无单领用关门状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterSurgeryNoNumLockerEvent(object sender, bool isClose)
        {
            System.Diagnostics.Debug.WriteLine("返回开锁状态{0}", isClose);

            if (!isClose)
                return;

            if (cabClosedNum == 0)
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    GerFetchState gerFetchState = new GerFetchState(3);
                    FullFrame.Navigate(gerFetchState);
                }));

                cabClosedNum++;
                return;
            }

            //弹出盘点中弹窗
            EnterInvotoryOngoing();

            Hashtable ht = RfidHelper.GetEpcData(out bool isGetSuccess);

            //关闭盘点中弹窗
            ClosePop();
            
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                SurgeryNoNumClose surgeryNoNumClose = new SurgeryNoNumClose(ht);
                surgeryNoNumClose.EnterPopCloseEvent += new SurgeryNoNumClose.EnterPopCloseHandler(onEnterPopClose);
                surgeryNoNumClose.EnterSurgeryNoNumOpenEvent += new SurgeryNoNumClose.EnterSurgeryNoNumOpenHandler(onEnterGerFetch);
                FullFrame.Navigate(surgeryNoNumClose);
            }));
        }
#endregion

#region 有手术单领用
        /// <summary>
        /// 手术领用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterSurgery(object sender, RoutedEventArgs e)
        {
            HomePageView.Visibility = Visibility.Hidden;
            btnBackHP.Visibility = Visibility.Visible;

            SurgeryQuery surgeryQuery = new SurgeryQuery();
            surgeryQuery.EnterSurgeryDetailEvent += new SurgeryQuery.EnterSurgeryDetailHandler(onEnterSurgeryDetail);//有手术单号进入手术领用单详情
            surgeryQuery.EnterSurgeryNoNumOpenEvent += new SurgeryQuery.EnterSurgeryNoNumOpenHandler(onEnterSurgeryNoNumOpen);//无手术单号直接开柜领用
            ContentFrame.Navigate(surgeryQuery);
        }
        /// <summary>
        /// 手术领用详情页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="fetchOrder"></param>
        private void onEnterSurgeryDetail(object sender, SurgeryOrderDto model)
        {
            SurgeryOrderDetail surgeryOrderDetail = new SurgeryOrderDetail(model);
            surgeryOrderDetail.EnterSurgeryNumOpenEvent += new SurgeryOrderDetail.EnterSurgeryNumOpenHandler(EnterSurgeryNumOpenEvent);
            surgeryOrderDetail.EnterSurgeryConsumablesDetailEvent += new SurgeryOrderDetail.EnterSurgeryConsumablesDetailHandler(EnterSurgeryConsumablesDetailEvent);
            ContentFrame.Navigate(surgeryOrderDetail);
        }

        /// <summary>
        /// 手术耗材详情
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="fetchOrder"></param>
        public void EnterSurgeryConsumablesDetailEvent(object sender, SurgeryOrderDto model)
        {
            ConsumablesDetails consumablesDetails = new ConsumablesDetails(model);
            consumablesDetails.EnterSurgeryDetailEvent += new ConsumablesDetails.EnterSurgeryDetailHandler(onEnterSurgeryDetail);
            ContentFrame.Navigate(consumablesDetails);
        }

        /// <summary>
        /// 进入手术有单领用-开门状态 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnterSurgeryNumOpenEvent(object sender, SurgeryOrderDto model)
        {
            NaviView.Visibility = Visibility.Hidden;

            SurgeryNumOpen surgeryNumOpen = new SurgeryNumOpen(model);
            FullFrame.Navigate(surgeryNumOpen);

            MaskView.Visibility = Visibility.Visible;
            PopFrame.Visibility = Visibility.Visible;
            OpenCabinet openCabinet = new OpenCabinet();
            openCabinet.HidePopOpenEvent += new OpenCabinet.HidePopOpenHandler(onHidePopOpen);
            PopFrame.Navigate(openCabinet);

#if TESTENV
            testTimer = new System.Timers.Timer(10000);
            testTimer.AutoReset = false;
            testTimer.Enabled = true;
            testTimer.Elapsed += new ElapsedEventHandler(onEnterSurgeryNumLockerTestEvent);
            testSOPara = model;
#else
            List<string> listCom = fetchOrderBll.GetSurgeryOrderdtlPosition(model.code);
            if (listCom.Count == 0)
                return;

            LockHelper.DelegateGetMsg delegateGetMsg = LockHelper.GetLockerData(listCom[0], out bool isGetSuccess);
            delegateGetMsg.DelegateGetMsgEvent += new LockHelper.DelegateGetMsg.DelegateGetMsgHandler(onEnterSurgeryNumLockerEvent);
            delegateGetMsg.userData = model;
            cabClosedNum = 1;

            if (listCom.Count == 2)
            {
                LockHelper.DelegateGetMsg delegateGetMsg2 = LockHelper.GetLockerData(listCom[1], out bool isGetSuccess2);
                delegateGetMsg2.DelegateGetMsgEvent += new LockHelper.DelegateGetMsg.DelegateGetMsgHandler(onEnterSurgeryNumLockerEvent);
                delegateGetMsg2.userData = model;
                cabClosedNum = 2;
            }
#endif
            SpeakerHelper.Sperker("柜门已开，请按照领用单拿取耗材，拿取完毕请关闭柜门");
        }


#if TESTENV
        /// <summary>
        /// 进入上架单详情页-关门状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterSurgeryNumLockerTestEvent(object sender, EventArgs e)
        {
            //Hashtable ht = RfidHelper.GetEpcData(out bool isGetSuccess);
            Hashtable ht = new Hashtable();

            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                SurgeryNumClose surgeryNumClose = new SurgeryNumClose(testSOPara, ht);
                surgeryNumClose.EnterPopCloseEvent += new SurgeryNumClose.EnterPopCloseHandler(onEnterPopClose);
                surgeryNumClose.EnterSurgeryNumOpenEvent += new SurgeryNumClose.EnterSurgeryNumOpenHandler(EnterSurgeryNumOpenEvent);

                FullFrame.Navigate(surgeryNumClose);
            }));
        }
#else
        /// <summary>
        /// 手术有单领用关门状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterSurgeryNumLockerEvent(object sender, bool isClose)
        {

            if (!isClose)
                return;
            System.Diagnostics.Debug.WriteLine("返回开锁状态{0}", isClose);

            cabClosedNum--;

            if (cabClosedNum == 1)
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    GerFetchState gerFetchState = new GerFetchState(3);
                    FullFrame.Navigate(gerFetchState);
                }));
                return;
            }

            //弹出盘点中弹窗
            EnterInvotoryOngoing();

            Hashtable ht = RfidHelper.GetEpcData(out bool isGetSuccess);

            //关闭盘点中弹窗
            ClosePop();

            LockHelper.DelegateGetMsg delegateGetMsg = (LockHelper.DelegateGetMsg)sender;
            SurgeryOrderDto surgeryOrderDto = (SurgeryOrderDto)delegateGetMsg.userData;

            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                SurgeryNumClose surgeryNumClose = new SurgeryNumClose(surgeryOrderDto,ht);
                surgeryNumClose.EnterPopCloseEvent += new SurgeryNumClose.EnterPopCloseHandler(onEnterPopClose);
                surgeryNumClose.EnterSurgeryNumOpenEvent += new SurgeryNumClose.EnterSurgeryNumOpenHandler(EnterSurgeryNumOpenEvent);
                FullFrame.Navigate(surgeryNumClose);
            }));
        }
#endif
        #endregion
        #endregion

        #region 领用退回
        /// <summary>
        /// 领用退回
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterReturnFetch(object sender, RoutedEventArgs e)
        {
            HomePageView.Visibility = Visibility.Hidden;
            btnBackHP.Visibility = Visibility.Visible;
            NaviView.Visibility = Visibility.Hidden;

            GerFetchState gerFetchState = new GerFetchState(2);
            FullFrame.Navigate(gerFetchState);

            List<string> com = ComName.GetAllLockerCom();

            LockHelper.DelegateGetMsg delegateGetMsg = LockHelper.GetLockerData(com[0], out bool isGetSuccess);
            delegateGetMsg.DelegateGetMsgEvent += new LockHelper.DelegateGetMsg.DelegateGetMsgHandler(onEnterReturnFetchLockerEvent);

            LockHelper.DelegateGetMsg delegateGetMsg2 = LockHelper.GetLockerData(com[1], out bool isGetSuccess2);
            delegateGetMsg2.DelegateGetMsgEvent += new LockHelper.DelegateGetMsg.DelegateGetMsgHandler(onEnterReturnFetchLockerEvent);

            cabClosedNum = 0;

            SpeakerHelper.Sperker("柜门已开，请放入您需要回退的耗材，放回完毕请关闭柜门");
        }

        /// <summary>
        /// 领用退回关门状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterReturnFetchLockerEvent(object sender, bool isClose)
        {
            System.Diagnostics.Debug.WriteLine("返回开锁状态{0}", isClose);

            if (!isClose)
                return;

            if (cabClosedNum == 0)
            {

                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    GerFetchState gerFetchState = new GerFetchState(3);
                    FullFrame.Navigate(gerFetchState);
                }));

                cabClosedNum++;
                return;
            }

            //弹出盘点中弹窗
            EnterInvotoryOngoing();

            Hashtable ht = RfidHelper.GetEpcData(out bool isGetSuccess);

            //关闭盘点中弹窗
            ClosePop();

            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                ReturnFetchView returnFetchView = new ReturnFetchView(ht);
                returnFetchView.EnterPopCloseEvent += new ReturnFetchView.EnterPopCloseHandler(onEnterPopClose);
                returnFetchView.EnterReturnFetch += new ReturnFetchView.EnterReturnFetchHandler(onEnterReturnFetch);
                FullFrame.Navigate(returnFetchView);
            }));
        }
#endregion
#endregion

#region Replenishment
        /// <summary>
        /// 进入上架单列表页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterReplenishment(object sender, RoutedEventArgs e)
        {
            HomePageView.Visibility = Visibility.Hidden;
            btnBackHP.Visibility = Visibility.Visible;

            Replenishment replenishment = new Replenishment();
            replenishment.EnterReplenishmentDetailEvent += new Replenishment.EnterReplenishmentDetailHandler(onEnterReplenishmentDetail);
            replenishment.EnterReplenishmentDetailOpenEvent += new Replenishment.EnterReplenishmentDetailOpenHandler(onEnterReplenishmentDetailOpen);

            ContentFrame.Navigate(replenishment);

        }

        /// <summary>
        /// 进入上架单详情页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterReplenishmentDetail(object sender, ReplenishOrderDto e)
        {
            ReplenishmentDetail replenishmentDetail = new ReplenishmentDetail(e);
            replenishmentDetail.EnterReplenishmentDetailOpenEvent += new ReplenishmentDetail.EnterReplenishmentDetailOpenHandler(onEnterReplenishmentDetailOpen);
            replenishmentDetail.EnterReplenishmentEvent += new ReplenishmentDetail.EnterReplenishmentHandler(onEnterReplenishment);

            ContentFrame.Navigate(replenishmentDetail);
        }

        /// <summary>
        /// 进入上架单详情页-开门状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterReplenishmentDetailOpen(object sender, ReplenishOrderDto e)
        {
            NaviView.Visibility = Visibility.Hidden;

            ReplenishmentDetailOpen replenishmentDetailOpen = new ReplenishmentDetailOpen(e);
            FullFrame.Navigate(replenishmentDetailOpen);

            MaskView.Visibility = Visibility.Visible;
            PopFrame.Visibility = Visibility.Visible;
            OpenCabinet openCabinet = new OpenCabinet();
            openCabinet.HidePopOpenEvent += new OpenCabinet.HidePopOpenHandler(onHidePopOpen);
            PopFrame.Navigate(openCabinet);

            SpeakerHelper.Sperker("柜门已开，请您按照要求上架，上架完毕请关闭柜门");

#if TESTENV
            testTimer = new System.Timers.Timer(10000);
            testTimer.AutoReset = false;
            testTimer.Enabled = true;
            testTimer.Elapsed += new ElapsedEventHandler(onEnterReplenishmentCloseTestEvent);
            testROPara = e;
#else

            List<string> listCom = replenishBll.GetReplenishOrderPositons(new ReplenishSubOrderdtlApo { replenish_order_code = e.code});
            if (listCom.Count == 0)
                return;

            LockHelper.DelegateGetMsg delegateGetMsg = LockHelper.GetLockerData(listCom[0], out bool isGetSuccess);
            delegateGetMsg.DelegateGetMsgEvent += new LockHelper.DelegateGetMsg.DelegateGetMsgHandler(onEnterReplenishmentCloseEvent);
            delegateGetMsg.userData = e;
            cabClosedNum = 1;

            if (listCom.Count == 2)
            {
                LockHelper.DelegateGetMsg delegateGetMsg2 = LockHelper.GetLockerData(listCom[1], out bool isGetSuccess2);
                delegateGetMsg2.DelegateGetMsgEvent += new LockHelper.DelegateGetMsg.DelegateGetMsgHandler(onEnterReplenishmentCloseEvent);
                delegateGetMsg2.userData = e;
                cabClosedNum = 2;
            }
#endif
        }

#if TESTENV
        /// <summary>
        /// 进入上架单详情页-关门状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterReplenishmentCloseTestEvent(object sender, EventArgs e)
        {
            bool isGetSuccess;
            Hashtable ht = RfidHelper.GetEpcData(out isGetSuccess);
            ReplenishOrderDto replenishOrderDto = testROPara;
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                ReplenishmentClose replenishmentClose = new ReplenishmentClose(replenishOrderDto, ht);
                replenishmentClose.EnterReplenishmentDetailOpenEvent += new ReplenishmentClose.EnterReplenishmentDetailOpenHandler(onEnterReplenishmentDetailOpen);
                replenishmentClose.EnterPopCloseEvent += new ReplenishmentClose.EnterPopCloseHandler(onEnterPopClose);

                FullFrame.Navigate(replenishmentClose);
            }));
        }
#else
        /// <summary>
        /// 进入上架单详情页-关门状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterReplenishmentCloseEvent(object sender, bool isClose)
        {
            LockHelper.DelegateGetMsg delegateGetMsg = (LockHelper.DelegateGetMsg)sender;
            System.Diagnostics.Debug.WriteLine("返回开锁状态{0}", isClose);

            if (!isClose)
                return;

            cabClosedNum--;
            if (cabClosedNum == 1)
                return;

            //弹出盘点中弹窗
            EnterInvotoryOngoing();

            bool isGetSuccess;
            Hashtable ht = RfidHelper.GetEpcData(out isGetSuccess);

            //关闭盘点中弹窗
            ClosePop();

            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                ReplenishmentClose replenishmentClose = new ReplenishmentClose((ReplenishOrderDto)delegateGetMsg.userData, ht);
                replenishmentClose.EnterReplenishmentDetailOpenEvent += new ReplenishmentClose.EnterReplenishmentDetailOpenHandler(onEnterReplenishmentDetailOpen);
                replenishmentClose.EnterPopCloseEvent += new ReplenishmentClose.EnterPopCloseHandler(onEnterPopClose);

                FullFrame.Navigate(replenishmentClose);
            }));
        }
#endif

#endregion

#region  ReturnGoods
        /// <summary>
        /// 进入退货出库页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterReturnGoods(object sender, RoutedEventArgs e)
        {
            HomePageView.Visibility = Visibility.Hidden;
            btnBackHP.Visibility = Visibility.Visible;

            ReturnGoods returnGoods = new ReturnGoods();
            returnGoods.EnterReturnGoodsDetailEvent += new ReturnGoods.EnterReturnGoodsDetailHandler(onEnterReturnGoodsDetail);
            returnGoods.EnterReturnGoodsDetailOpenEvent += new ReturnGoods.EnterReturnGoodsDetailOpenHandler(onEnterReturnGoodsDetailOpen);

            ContentFrame.Navigate(returnGoods);
        }

        /// <summary>
        /// 进入退货出库详情页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterReturnGoodsDetail(object sender, PickingOrderDto e)
        {
            ReturnGoodsDetail returnGoodsDetail = new ReturnGoodsDetail(e);
            returnGoodsDetail.EnterReturnGoodsDetailOpenEvent += new ReturnGoodsDetail.EnterReturnGoodsDetailOpenHandler(onEnterReturnGoodsDetailOpen);
            returnGoodsDetail.EnterReturnGoodsEvent += new ReturnGoodsDetail.EnterReturnGoodsHandler(onEnterReturnGoods);

            ContentFrame.Navigate(returnGoodsDetail);
        }


        /// <summary>
        /// 进入退货出库详情页面-开门状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterReturnGoodsDetailOpen(object sender, PickingOrderDto e)
        {
            NaviView.Visibility = Visibility.Hidden;

            ReturnGoodsDetailOpen returnGoodsDetailOpen = new ReturnGoodsDetailOpen(e);
            FullFrame.Navigate(returnGoodsDetailOpen);

            MaskView.Visibility = Visibility.Visible;
            PopFrame.Visibility = Visibility.Visible;
            OpenCabinet openCabinet = new OpenCabinet();
            openCabinet.HidePopOpenEvent += new OpenCabinet.HidePopOpenHandler(onHidePopOpen);
            PopFrame.Navigate(openCabinet);

            SpeakerHelper.Sperker("柜门已开，请您按照要求拣货，拣货完毕请关闭柜门");

#if TESTENV
            testTimer = new System.Timers.Timer(10000);
            testTimer.AutoReset = false;
            testTimer.Enabled = true;
            testTimer.Elapsed += new ElapsedEventHandler(onEnterReturnGoodsCloseTestEvent);
            testPOPara = e;
#else
            List<string> listCom = pickingBll.GetPickingOrderPositons(new PickingSubOrderdtlApo { picking_order_code = e.code });
            if (listCom.Count == 0)
                return;

            LockHelper.DelegateGetMsg delegateGetMsg = LockHelper.GetLockerData(listCom[0], out bool isGetSuccess);
            delegateGetMsg.DelegateGetMsgEvent += new LockHelper.DelegateGetMsg.DelegateGetMsgHandler(onEnterReturnGoodsCloseEvent);
            delegateGetMsg.userData = e;
            cabClosedNum = 1;

            if (listCom.Count == 2)
            {
                LockHelper.DelegateGetMsg delegateGetMsg2 = LockHelper.GetLockerData(listCom[1], out bool isGetSuccess2);
                delegateGetMsg2.DelegateGetMsgEvent += new LockHelper.DelegateGetMsg.DelegateGetMsgHandler(onEnterReturnGoodsCloseEvent);
                delegateGetMsg2.userData = e;
                cabClosedNum = 2;
            }
#endif
        }

#if TESTENV
        /// <summary>
        /// 进入拣货单详情页-关门状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterReturnGoodsCloseTestEvent(object sender, EventArgs e)
        {
            bool isGetSuccess;
            //Hashtable ht = RfidHelper.GetEpcData(out isGetSuccess);
            Hashtable ht = new Hashtable();
            PickingOrderDto pickingSubOrderDto = testPOPara;

            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                ReturnGoodsClose returnGoodsClose = new ReturnGoodsClose(pickingSubOrderDto, ht);
                returnGoodsClose.EnterReturnGoodsDetailOpenEvent += new ReturnGoodsClose.EnterReturnGoodsDetailOpenHandler(onEnterReturnGoodsDetailOpen);
                returnGoodsClose.EnterPopCloseEvent += new ReturnGoodsClose.EnterPopCloseHandler(onEnterPopClose);

                FullFrame.Navigate(returnGoodsClose);
            }));
        }
#else
                /// <summary>
        /// 进入拣货单详情页-关门状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterReturnGoodsCloseEvent(object sender, bool isClose)
        {
            LockHelper.DelegateGetMsg delegateGetMsg = (LockHelper.DelegateGetMsg)sender;
            System.Diagnostics.Debug.WriteLine("返回开锁状态{0}", isClose);

            if (!isClose)
                return;

            cabClosedNum--;
            if (cabClosedNum == 1)
                return;

            //弹出盘点中弹窗
            EnterInvotoryOngoing();

            bool isGetSuccess;
            Hashtable ht = RfidHelper.GetEpcData(out isGetSuccess);

            //关闭盘点中弹窗
            ClosePop();

            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                ReturnGoodsClose returnGoodsClose = new ReturnGoodsClose((PickingOrderDto)delegateGetMsg.userData, ht);
                returnGoodsClose.EnterReturnGoodsDetailOpenEvent += new ReturnGoodsClose.EnterReturnGoodsDetailOpenHandler(onEnterReturnGoodsDetailOpen);
                returnGoodsClose.EnterPopCloseEvent += new ReturnGoodsClose.EnterPopCloseHandler(onEnterPopClose);

                FullFrame.Navigate(returnGoodsClose);
            }));
        }
#endif
#endregion

#region Inventory
        /// <summary>
        /// 库存盘点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterInvtory(object sender, RoutedEventArgs e)
        {
            HomePageView.Visibility = Visibility.Hidden;
            btnBackHP.Visibility = Visibility.Visible;

            Inventory inventory = new Inventory();
            inventory.EnterPopInventoryEvent += new Inventory.EnterPopInventoryHandler(onEnterPopInventory);
            inventory.HidePopInventoryEvent += new Inventory.HidePopInventoryHandler(onHidePopInventory);
            inventory.EnterPopInventoryPlanEvent += new Inventory.EnterPopInventoryPlanHandler(onEnterPopInventoryPlan);
            inventory.EnterInventoryDetailEvent += new Inventory.EnterInventoryDetailHandler(onEnterInventoryDetail);

            ContentFrame.Navigate(inventory);

        }

        /// <summary>
        /// 弹出库存盘点正在进行中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterPopInventory(object sender, System.EventArgs e)
        {
            InventoryOngoing inventoryOngoing = new InventoryOngoing();

            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                PopFrame.Visibility = Visibility.Visible;
                MaskView.Visibility = Visibility.Visible;

                PopFrame.Navigate(inventoryOngoing);
            }));
        }

        /// <summary>
        /// 关闭库存盘点正在进行中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onHidePopInventory(object sender, System.EventArgs e)
        {
            //App.Current.Dispatcher.Invoke((Action)(() =>
            //{
            //    PopFrame.Visibility = Visibility.Hidden;
            //    MaskView.Visibility = Visibility.Hidden;        
            //}));         
            ClosePop();
        }

        /// <summary>
        /// 关闭库存盘点正在进行中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterInventoryDetail(object sender, InventoryDetailPara e)
        {
            InventoryDetail inventoryDetail = new InventoryDetail(e);
            inventoryDetail.EnterAddProductEvent += new InventoryDetail.EnterAddProductHandler(onEnterPopAddProduct);
            inventoryDetail.EnterInventoryEvent += new InventoryDetail.EnterInventoryHandler(onEnterInvtory);

            ContentFrame.Navigate(inventoryDetail);
        }


        /// <summary>
        /// 弹出盘点计划
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterPopInventoryPlan(object sender, System.EventArgs e)
        {
            InventoryPlanDetail inventoryPlanDetail = new InventoryPlanDetail();
            inventoryPlanDetail.HidePopInventoryPlanEvent += new InventoryPlanDetail.HidePopInventoryPlanHandler(onHidePopInventoryPlan);

            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                PopFrame.Visibility = Visibility.Visible;
                MaskView.Visibility = Visibility.Visible;

                PopFrame.Navigate(inventoryPlanDetail);
            }));
        }

        /// <summary>
        /// 关闭盘点计划
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onHidePopInventoryPlan(object sender, System.EventArgs e)
        {
            //App.Current.Dispatcher.Invoke((Action)(() =>
            //{
            //    PopFrame.Visibility = Visibility.Hidden;
            //    MaskView.Visibility = Visibility.Hidden;
            //}));
            ClosePop();
        }


        /// <summary>
        /// 进入添加单品码弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterPopAddProduct(object sender, InventoryDetailPara e)
        {
            AddProduct addProduct = new AddProduct(e);
            addProduct.HidePopAddProductEvent += new AddProduct.HidePopAddProductHandler (onHidePopAddProduct);
            addProduct.EnterInventoryDetailEvent += new AddProduct.EnterInventoryDetailHandler(onEnterInventoryDetail);

            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                PopFrame.Visibility = Visibility.Visible;
                MaskView.Visibility = Visibility.Visible;

                PopFrame.Navigate(addProduct);
            }));
        }


        /// <summary>
        /// 关闭添加单品码弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onHidePopAddProduct(object sender, RoutedEventArgs e)
        {
            //App.Current.Dispatcher.Invoke((Action)(() =>
            //{
            //    PopFrame.Visibility = Visibility.Hidden;
            //    MaskView.Visibility = Visibility.Hidden;
            //}));
            ClosePop();
        }

        /// <summary>
        /// 库存查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterStock(object sender, RoutedEventArgs e)
        {
            HomePageView.Visibility = Visibility.Hidden;
            btnBackHP.Visibility = Visibility.Visible;

            Stock stock = new Stock();
            stock.EnterStockDetailedEvent += new Stock.EnterStockDetailedHandler(onEnterStockDetailedEvent);
            ContentFrame.Navigate(stock);
        }

        private void onEnterStockDetailedEvent(object sender, GoodDto goodDto)
        {
            StockDetailed stockDetailed = new StockDetailed(goodDto);
            stockDetailed.EnterStockEvent += new StockDetailed.EnterStockHandler(colseStockDetailedEvent);

            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                PopFrame.Visibility = Visibility.Visible;
                MaskView.Visibility = Visibility.Visible;

                PopFrame.Navigate(stockDetailed);
            }));
        }

        private void colseStockDetailedEvent(object sender, RoutedEventArgs e)
        {
            //App.Current.Dispatcher.Invoke((Action)(() =>
            //{
            //    PopFrame.Visibility = Visibility.Hidden;
            //    MaskView.Visibility = Visibility.Hidden;
            //}));
            ClosePop();
        }
#endregion


        /// <summary>
        /// 弹出关门提示框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterPopClose(object sender, bool e)
        {
            if (!e)
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    NaviView.Visibility = Visibility.Visible;
                    HomePageView.Visibility = Visibility.Visible;
                    btnBackHP.Visibility = Visibility.Hidden;
                }));
            }
            else
            {
                CloseCabinet closeCabinet = new CloseCabinet();
                closeCabinet.HidePopCloseEvent += new CloseCabinet.HidePopCloseHandler(onHidePopClose);

                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    PopFrame.Visibility = Visibility.Visible;
                    MaskView.Visibility = Visibility.Visible;

                    PopFrame.Navigate(closeCabinet);
                }));
            }
        }

        /// <summary>
        /// 关门提示框关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onHidePopClose(object sender, RoutedEventArgs e)
        {
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                PopFrame.Visibility = Visibility.Hidden;
                MaskView.Visibility = Visibility.Hidden;
                NaviView.Visibility = Visibility.Visible;
                HomePageView.Visibility = Visibility.Visible;
                btnBackHP.Visibility = Visibility.Hidden;

#if TESTENV
#else
                LoginBkView.Visibility = Visibility.Visible;
#endif

                vein.ChekVein();
            }));
        }


        /// <summary>
        /// 开门提示框关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onHidePopOpen(object sender, RoutedEventArgs e)
        {
            //App.Current.Dispatcher.Invoke((Action)(() =>
            //{
            //    PopFrame.Visibility = Visibility.Hidden;
            //    MaskView.Visibility = Visibility.Hidden;
            //}));
            ClosePop();
        }

        /// <summary>
        /// 弹出盘存中提示框操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnterInvotoryOngoing()
        {
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                InventoryOngoing inventoryOngoing = new InventoryOngoing();

                PopFrame.Visibility = Visibility.Visible;
                MaskView.Visibility = Visibility.Visible;

                PopFrame.Navigate(inventoryOngoing);
            }));
        }

        /// <summary>
        /// 关闭弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClosePop()
        {
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                PopFrame.Visibility = Visibility.Hidden;
                MaskView.Visibility = Visibility.Hidden;
            }));
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Taskbar.HideTask(true);
        }

        /// <summary>
        /// 退出按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onExit(object sender, RoutedEventArgs e)
        {
            NaviView.Visibility = Visibility.Visible;
            HomePageView.Visibility = Visibility.Visible;
            btnBackHP.Visibility = Visibility.Hidden;
#if TESTENV
#else
            LoginBkView.Visibility = Visibility.Visible;
#endif
            vein.ChekVein();
        }

        /// <summary>
        /// 返回首页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onBackToHP(object sender, RoutedEventArgs e)
        {
            NaviView.Visibility = Visibility.Visible;
            HomePageView.Visibility = Visibility.Visible;
            btnBackHP.Visibility = Visibility.Hidden;
            Taskbar.HideTask(true);
        }

#region test

        private void MockData()
        {
            test.InitGoodsInfo();
            test.InitUsersInfo();

#if TESTENV
            LoginBkView.Visibility = Visibility.Hidden;
        
            User user = userBll.GetTestUser();        
            ApplicationState.SetValue((int)ApplicationKey.CurUser, user);

            TestGoods testGoods = new TestGoods();
            testGoods.GetCurrentRFid();
#else
            System.Timers.Timer iniGoodstimer = new System.Timers.Timer(1000);
            iniGoodstimer.AutoReset = false;
            iniGoodstimer.Enabled = true;
            iniGoodstimer.Elapsed += new ElapsedEventHandler(onInitGoods);
#endif
            test.InitReplenishOrder();
            test.InitPickingOrder();
            test.InitSurgerOrder();
        }


        private void onInitGoods(object sender, EventArgs e)
        {
            bool isGetSuccess;
            Hashtable ht = RfidHelper.GetEpcData(out isGetSuccess);
            ApplicationState.SetValue((int)ApplicationKey.CurGoods, ht);
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
#endregion

        private void MetroWindow_Closed(object sender, EventArgs e)
        {
           Taskbar.HideTask(true);
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
