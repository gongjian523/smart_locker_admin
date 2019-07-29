using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
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
using CFLMedCab.View.ReplenishmentOrder;
using CFLMedCab.View.Return;
using CFLMedCab.BLL;
using CFLMedCab.Infrastructure;
using CFLMedCab.View.Fetch;
using System.Collections;
using CFLMedCab.DTO.Replenish;
using CFLMedCab.DTO.Picking;
using CFLMedCab.Test;
using CFLMedCab.DTO.Goodss;
using CFLMedCab.DTO.Surgery;
using CFLMedCab.Controls;
using System.Windows.Forms;
using static CFLMedCab.Model.Enum.UserIdEnum;
using CFLMedCab.Infrastructure.BootUpHelper;
using CFLMedCab.Http.Bll;
using CFLMedCab.Http.Model;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CFLMedCab.Http.Constant;
using CFLMedCab.Http.Model.Base;
using CFLMedCab.Http.Model.login;
using CFLMedCab.Http.Model.param;
using CFLMedCab.Http.Helper;

namespace CFLMedCab
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private DispatcherTimer ShowTimer;
        private DispatcherTimer InventoryTimer;
#if TESTENV
#else
#if VEINSERIAL
        private VeinHelper vein;
#else
        private VeinUtils vein;
#endif
#endif

        private InventoryBll inventoryBll = new InventoryBll();
        private GoodsBll goodsBll = new GoodsBll();
        private UserBll userBll = new UserBll();
        private FetchOrderBll fetchOrderBll = new FetchOrderBll();
        private ReplenishBll replenishBll = new ReplenishBll();
        private PickingBll pickingBll = new PickingBll();

#if DUALCAB
        private int cabClosedNum;
#endif

        private InventoryDtl inventoryDetailHandler;

        private TestGoods test = new TestGoods();

        private CurrentUser _curUser;
        public CurrentUser CurUser { get {
                return _curUser;
            }
            set {
                _curUser = value;
            }
        }

        private LoadingData loadingDataPage;

#if TESTENV
        private System.Timers.Timer testTimer;
        private FetchParam testFetchPara = new FetchParam();
        private ShelfTask testROPara = new ShelfTask();
        private PickTask testPOPara = new PickTask();
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

            this.tbNameText.Text = ApplicationState.GetValue<CurrentUser>((int)ApplicationKey.CurUser).name;

            ShowTime();
            ShowTimer = new DispatcherTimer();
            ShowTimer.Tick += new EventHandler(ShowCurTimer);//起个Timer一直获取当前时间
            ShowTimer.Interval = new TimeSpan(0, 0, 0, 1, 0);
            ShowTimer.Start();

            InventoryTimer = new DispatcherTimer();
            InventoryTimer.Tick += new EventHandler(onInventoryTimer);//起个Timer, 每分钟检查是否有扫描计划
            InventoryTimer.Interval = new TimeSpan(0, 0, 1, 0, 0);
            InventoryTimer.Start();

            loadingDataPage = new LoadingData();

#if TESTENV
#else
#if VEINSERIAL
            string veinCom = ApplicationState.GetMVeinCOM();
            //vein = new VeinHelper("COM9", 9600);
            vein = new VeinHelper(veinCom, 9600);
            vein.DataReceived += new SerialDataReceivedEventHandler(onReceivedDataVein);
            Console.WriteLine("onStart");
            vein.ChekVein();
#else
            vein = VeinUtils.GetInstance();
            vein.FingerDetectedEvent += new VeinUtils.FingerDetectedHandler(onFingerDetected);
            int vienSt = vein.LoadingDevice();

            if (vienSt != VeinUtils.FV_ERRCODE_SUCCESS && vienSt != VeinUtils.FV_ERRCODE_EXISTING)
            {
                onFingerDetected(this, -1);
            }
            else
            {
                if (vienSt == VeinUtils.FV_ERRCODE_SUCCESS)
                    vein.OpenDevice();

                Task.Factory.StartNew(vein.DetectFinger);
            }
#endif
#endif
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
            return;
            List <InventoryPlanLDB> listPan = inventoryBll.GetInventoryPlan().ToList().Where(item => item.status == 0).ToList();

            foreach(var item in listPan)
            {
                DateTime date1 = DateTime.Now;
                DateTime date2 = new DateTime(date1.Year, date1.Month, date1.Day, int.Parse(item.inventorytime_str.Substring(0,2)), int.Parse(item.inventorytime_str.Substring(3,2)), 0);

                TimeSpan timeSpan = date2 - date1;

                if (timeSpan.TotalMinutes < 1 && timeSpan.TotalMinutes > -1 )
                {
                    bool isGetSuccess;
                    Hashtable ht = RfidHelper.GetEpcData(out isGetSuccess);

                    return;
                }
                Console.WriteLine("onInventoryTimer:" + timeSpan.TotalMinutes);
            }
        }


        private void onLoginInfoHidenEvent(object sender, LoginStatus e)
        {
            ClosePop();

            if (e.LoginState == 0)
            {
                Console.WriteLine("onLoginInfoHidenEvent");
#if TESTENV
#else
#if VEINSERIAL
                vein.ChekVein();
#else
                Task.Factory.StartNew(vein.DetectFinger);
#endif
#endif
            }
        }

#if TESTENV
#else
#if VEINSERIAL
        private void onReceivedDataVein(object sender, SerialDataReceivedEventArgs e)
        {
            int id = vein.GetVeinId();

            System.Diagnostics.Debug.WriteLine("VeinId {0}", id);

            if (id >= 0)
            {
                vein.Close();

                LoginStatus sta = new LoginStatus();
                CurrentUser user = userBll.GetUserByVeinId(id);

                if(id == 0 || user == null)
                {
                    App.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        LoginInfo loginInfo = new LoginInfo(new LoginStatus {
                            LoginState = 0,
                            LoginString = "登录失败",
                            LoginString2 = "请再次进行验证"
                    });

                    PopFrame.Visibility = Visibility.Visible;
                    MaskView.Visibility = Visibility.Visible;

                    loginInfo.LoginInfoHidenEvent += new LoginInfo.LoginInfoHidenHandler(onLoginInfoHidenEvent);

                    PopFrame.Navigate(loginInfo);
                    }));
                }
                else
                {
                    ApplicationState.SetValue((int)ApplicationKey.CurUser, user);

                    App.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        LoginBkView.Visibility = Visibility.Hidden;
                        SetNavBtnVisiblity(user.role);
                        tbNameText.Text = ApplicationState.GetValue<CurrentUser>((int)ApplicationKey.CurUser).name;
                    }));
                }
            }
            else
            {
                Console.WriteLine("onReceivedDataVein");
                vein.ChekVein();
            }

        }
#else
        private void onFingerDetected(object sender, int e)
        {
            LoginStatus sta = new LoginStatus();
#if LOCALSDK
            CurrentUser user = new CurrentUser();
#else
            User user = null;
#endif

            string info = "等待检测指静脉的时候发生错误";
            string info2 = "请再次进行验证";

            if (e == 0)
            {
                byte[] macthfeature = new byte[VeinUtils.FV_FEATURE_SIZE];

                if (vein.GrabFeature(macthfeature, out info) == VeinUtils.FV_ERRCODE_SUCCESS)
                {
#if LOCALSDK
                    List<CurrentUser> userList = userBll.GetAllUsers();

                    foreach (var item in userList)
                    {
                        if (item.reg_feature == null)
                            continue;

                        if (item.ai_feature == null)
                            item.ai_feature = item.reg_feature;

                        byte[] regfeature = new byte[VeinUtils.FEATURE_COLLECT_CNT *VeinUtils.FV_FEATURE_SIZE ];
                        regfeature = Convert.FromBase64String(item.reg_feature);

                        byte[] aifeature = new byte[VeinUtils.FV_DYNAMIC_FEATURE_CNT*VeinUtils.FV_FEATURE_SIZE];
                        aifeature = Convert.FromBase64String(item.ai_feature);

                        uint diff = 0;
                        uint ailen = VeinUtils.FV_DYNAMIC_FEATURE_CNT * VeinUtils.FV_FEATURE_SIZE;  //输入为动态特征缓冲区大小，输出为动态模板长度

                        if (vein.Match(macthfeature, regfeature, VeinUtils.FEATURE_COLLECT_CNT, aifeature, ref diff, (int)VeinUtils.Match_Flg.M_1_1, ref ailen) 
                            == VeinUtils.FV_ERRCODE_SUCCESS)
                        {
                            user = item;
                            if(ailen > 0)
                            {
                                item.ai_feature = Convert.ToBase64String(aifeature);
                                userBll.UpdateCurrentUsers(item);
                            }
                            break;
                        }
                    }

                    info = "没有找到和当前指静脉匹配的用户";
                    info2 = "请先绑定指静脉";

#else

                    //BasePostData<string> data = UserLoginBll.GetInstance().VeinmatchLogin(Convert.ToBase64String(macthfeature));

                    BaseSinglePostData<VeinMatch> data = UserLoginBll.GetInstance().VeinmatchLogin(new VeinmatchPostParam
                    {
                        regfeature = Convert.ToBase64String(macthfeature)
                    });

                    if (data.code == 0)
                    {
                        user = data.body.user;

                        ApplicationState.SetAccessToken(data.body.accessToken);
                        ApplicationState.SetAccessToken(data.body.refresh_token);

                        HttpHelper.GetInstance().SetHeaders(data.body.accessToken);
                    }
                    else
                    {
                        info = "没有找到和当前指静脉匹配的用户";
                        info2 = "请先绑定指静脉";
                    }
#endif
                }
            }

#if LOCALSDK 
            if (e == -1 || user.id == 0)
#else
            if (e == -1 || user ==null)
#endif
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    LoginInfo loginInfo = new LoginInfo(new LoginStatus
                    {
                        LoginState = 0,
                        LoginString = info,
                        LoginString2 = info2
                    });

                    PopFrame.Visibility = Visibility.Visible;
                    MaskView.Visibility = Visibility.Visible;

                    loginInfo.LoginInfoHidenEvent += new LoginInfo.LoginInfoHidenHandler(onLoginInfoHidenEvent);

                    PopFrame.Navigate(loginInfo);
                }));
            }
            else
            {
                

                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    LoginBkView.Visibility = Visibility.Hidden;
#if LOCALSDK
                    ApplicationState.SetValue((int)ApplicationKey.CurUser, user);
                    SetNavBtnVisiblity(user.role);
                    tbNameText.Text = ApplicationState.GetValue<CurrentUser>((int)ApplicationKey.CurUser).name;
#else
                    ApplicationState.SetUserInfo(user);
                    SetNavBtnVisiblity(user.Role);
                    tbNameText.Text = user.name;
#endif

                }));
            }
            
        }
#endif
#endif

#if LOCALSDK
        private void SetNavBtnVisiblity(int role)
        {
            bool isMedicalStuff = ((UserIdType)role == UserIdType.医生 || (UserIdType)role == UserIdType.护士 || (UserIdType)role == UserIdType.医院管理员) ? true : false;
#else
        private void SetNavBtnVisiblity(string  role)
        {
            bool isMedicalStuff = (role == "医院医护人员") ? true : false;
#endif

            NavBtnEnterGerFetch.Visibility = isMedicalStuff ? Visibility.Visible : Visibility.Hidden;
            NavBtnEnterSurgery.Visibility = isMedicalStuff ? Visibility.Visible : Visibility.Hidden;
            NavBtnEnterReturnFetch.Visibility = isMedicalStuff ? Visibility.Visible : Visibility.Hidden;
            NavBtnEnterReplenishment.Visibility = (!isMedicalStuff) ? Visibility.Visible : Visibility.Hidden;
            NavBtnEnterReturnGoods.Visibility = (!isMedicalStuff) ? Visibility.Visible : Visibility.Hidden;
            NavBtnEnterInvtory.Visibility = (!isMedicalStuff) ? Visibility.Visible : Visibility.Hidden;
            NavBtnEnterStock.Visibility = (!isMedicalStuff) ? Visibility.Visible : Visibility.Hidden;
            btnExitApp.Visibility = Visibility.Visible;
#if TESTENV
#else
#if LOCALSDK
            btnExitApp.Visibility = ((UserIdType)role == UserIdType.SPD经理) ? Visibility.Visible : Visibility.Hidden;
#endif
#endif
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

            List<string> com = ApplicationState.GetAllLockerCom();

            LockHelper.DelegateGetMsg delegateGetMsg = LockHelper.GetLockerData(com[0], out bool isGetSuccess);
            delegateGetMsg.DelegateGetMsgEvent += new LockHelper.DelegateGetMsg.DelegateGetMsgHandler(onEnterGerFectchLockerEvent);

#if DUALCAB
            LockHelper.DelegateGetMsg delegateGetMsg2 = LockHelper.GetLockerData(com[1], out bool isGetSuccess2);
            delegateGetMsg2.DelegateGetMsgEvent += new LockHelper.DelegateGetMsg.DelegateGetMsgHandler(onEnterGerFectchLockerEvent);

            cabClosedNum = 0;
#endif
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
#if DUALCAB
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
#endif

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

            List<string> com = ApplicationState.GetAllLockerCom();

            LockHelper.DelegateGetMsg delegateGetMsg = LockHelper.GetLockerData(com[0], out bool isGetSuccess);
            delegateGetMsg.DelegateGetMsgEvent += new LockHelper.DelegateGetMsg.DelegateGetMsgHandler(onEnterSurgeryNoNumLockerEvent);

#if DUALCAB
            LockHelper.DelegateGetMsg delegateGetMsg2 = LockHelper.GetLockerData(com[1], out bool isGetSuccess2);
            delegateGetMsg2.DelegateGetMsgEvent += new LockHelper.DelegateGetMsg.DelegateGetMsgHandler(onEnterSurgeryNoNumLockerEvent);

            cabClosedNum = 0;
#endif

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

#if DUALCAB
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
#endif

            //弹出盘点中弹窗
            EnterInvotoryOngoing();

            HashSet<CommodityEps> hs = RfidHelper.GetEpcDataJson(out bool isGetSuccess);

            //关闭盘点中弹窗
            ClosePop();
            
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                SurgeryNoNumClose surgeryNoNumClose = new SurgeryNoNumClose(hs, ConsumingOrderType.手术领用);
                surgeryNoNumClose.EnterPopCloseEvent += new SurgeryNoNumClose.EnterPopCloseHandler(onEnterPopClose);
                surgeryNoNumClose.EnterSurgeryNoNumOpenEvent += new SurgeryNoNumClose.EnterSurgeryNoNumOpenHandler(onEnterGerFetch);
                FullFrame.Navigate(surgeryNoNumClose);
            }));
        }
        #endregion

        #region 有手术单领用和医嘱处方领用
        /// <summary>
        /// 手术领用医嘱处方领用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterSurgery(object sender, RoutedEventArgs e)
        {
            HomePageView.Visibility = Visibility.Hidden;
            btnBackHP.Visibility = Visibility.Visible;

            ConsumingOrderType type;
            if (((System.Windows.Controls.Button)sender).Name == "NavBtnEnterSurgery")
                type = ConsumingOrderType.手术领用;
            else
                type = ConsumingOrderType.医嘱处方领用;

            SurgeryQuery surgeryQuery = new SurgeryQuery(type);
            surgeryQuery.EnterSurgeryDetailEvent += new SurgeryQuery.EnterSurgeryDetailHandler(onEnterSurgeryDetail);//有手术单号进入手术领用单详情
            surgeryQuery.EnterSurgeryNoNumOpenEvent += new SurgeryQuery.EnterSurgeryNoNumOpenHandler(onEnterSurgeryNoNumOpen);//无手术单号直接开柜领用

            surgeryQuery.ShowLoadDataEvent += new SurgeryQuery.ShowLoadDataHandler(onShowLoadingData);
            surgeryQuery.HideLoadDataEvent += new SurgeryQuery.HideLoadDataHandler(onHideLoadingData);



            ContentFrame.Navigate(surgeryQuery);
        }
        /// <summary>
        /// 手术领用详情页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="fetchOrder"></param>
        private void onEnterSurgeryDetail(object sender, FetchParam fetchParam)
        {
            SurgeryOrderDetail surgeryOrderDetail = new SurgeryOrderDetail(fetchParam);
            surgeryOrderDetail.EnterSurgeryNumOpenEvent += new SurgeryOrderDetail.EnterSurgeryNumOpenHandler(EnterSurgeryNumOpenEvent);
            ContentFrame.Navigate(surgeryOrderDetail);
        }

        /// <summary>
        /// 手术耗材详情
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="fetchOrder"></param>
        public void EnterSurgeryConsumablesDetailEvent(object sender, BaseData<ConsumingOrder> bdOrder)
        {
            //ConsumablesDetails consumablesDetails = new ConsumablesDetails(model);
            //consumablesDetails.EnterSurgeryDetailEvent += new ConsumablesDetails.EnterSurgeryDetailHandler(onEnterSurgeryDetail);
            //ContentFrame.Navigate(consumablesDetails);
        }

        /// <summary>
        /// 进入手术有单领用-开门状态 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnterSurgeryNumOpenEvent(object sender, FetchParam fetchParam)
        {
            NaviView.Visibility = Visibility.Hidden;

            SurgeryNumOpen surgeryNumOpen = new SurgeryNumOpen(fetchParam);
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
            testFetchPara = fetchParam;
#else

#if DUALCAB
            List<string> listCom = ComName.GetAllLockerCom();
            if (listCom.Count == 0)
                return;
#else
           List<string> listCom = ComName.GetAllLockerCom();
#endif

            LockHelper.DelegateGetMsg delegateGetMsg = LockHelper.GetLockerData(listCom[0], out bool isGetSuccess);
            delegateGetMsg.DelegateGetMsgEvent += new LockHelper.DelegateGetMsg.DelegateGetMsgHandler(onEnterSurgeryNumLockerEvent);
            delegateGetMsg.userData = fetchParam;

#if DUALCAB
            cabClosedNum = 1;

            if (listCom.Count == 2)
            {
                LockHelper.DelegateGetMsg delegateGetMsg2 = LockHelper.GetLockerData(listCom[1], out bool isGetSuccess2);
                delegateGetMsg2.DelegateGetMsgEvent += new LockHelper.DelegateGetMsg.DelegateGetMsgHandler(onEnterSurgeryNumLockerEvent);
                delegateGetMsg2.userData = fetchParam;
                cabClosedNum = 2;
            }
#endif
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

            HashSet<CommodityEps> hs = RfidHelper.GetEpcDataJson(out bool isGetSuccess);

            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                SurgeryNumClose surgeryNumClose = new SurgeryNumClose(testFetchPara, hs);
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

#if DUALCAB
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
#endif

            //弹出盘点中弹窗
            EnterInvotoryOngoing();

            HashSet<CommodityEps> ht = RfidHelper.GetEpcDataJson(out bool isGetSuccess);

            //关闭盘点中弹窗
            ClosePop();

            LockHelper.DelegateGetMsg delegateGetMsg = (LockHelper.DelegateGetMsg)sender;
            FetchParam fetchParam = (FetchParam)delegateGetMsg.userData;

            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                SurgeryNumClose surgeryNumClose = new SurgeryNumClose(fetchParam, ht);
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

            List<string> com = ApplicationState.GetAllLockerCom();

            LockHelper.DelegateGetMsg delegateGetMsg = LockHelper.GetLockerData(com[0], out bool isGetSuccess);
            delegateGetMsg.DelegateGetMsgEvent += new LockHelper.DelegateGetMsg.DelegateGetMsgHandler(onEnterReturnFetchLockerEvent);

#if DUALCAB
            LockHelper.DelegateGetMsg delegateGetMsg2 = LockHelper.GetLockerData(com[1], out bool isGetSuccess2);
            delegateGetMsg2.DelegateGetMsgEvent += new LockHelper.DelegateGetMsg.DelegateGetMsgHandler(onEnterReturnFetchLockerEvent);

            cabClosedNum = 0;
#endif
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

#if DUALCAB
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
#endif
            //弹出盘点中弹窗
            EnterInvotoryOngoing();

            HashSet<CommodityEps> hs = RfidHelper.GetEpcDataJson(out bool isGetSuccess);

            //关闭盘点中弹窗
            ClosePop();

            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                ReturnFetchView returnFetchView = new ReturnFetchView(hs);
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

            replenishment.ShowLoadDataEvent += new Replenishment.ShowLoadDataHandler(onShowLoadingData);
            replenishment.HideLoadDataEvent += new Replenishment.HideLoadDataHandler(onHideLoadingData);

            ContentFrame.Navigate(replenishment);
        }

        /// <summary>
        /// 进入上架单详情页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterReplenishmentDetail(object sender, ShelfTask e)
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
        private void onEnterReplenishmentDetailOpen(object sender, ShelfTask e)
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

#if DUALCAB
            //TODO
            List<string> listCom = ComName.GetAllLockerCom();
#else
            List<string> listCom = ComName.GetAllLockerCom();
#endif
            LockHelper.DelegateGetMsg delegateGetMsg = LockHelper.GetLockerData(listCom[0], out bool isGetSuccess);
            delegateGetMsg.DelegateGetMsgEvent += new LockHelper.DelegateGetMsg.DelegateGetMsgHandler(onEnterReplenishmentCloseEvent);
            delegateGetMsg.userData = e;

#if DUALCAB
            cabClosedNum = 1;
            if (listCom.Count == 2)
            {
                LockHelper.DelegateGetMsg delegateGetMsg2 = LockHelper.GetLockerData(listCom[1], out bool isGetSuccess2);
                delegateGetMsg2.DelegateGetMsgEvent += new LockHelper.DelegateGetMsg.DelegateGetMsgHandler(onEnterReplenishmentCloseEvent);
                delegateGetMsg2.userData = e;
                cabClosedNum = 2;
            }
#endif
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

            ApplicationState.SetGoodsInfo(new HashSet<CommodityEps>()
                {
                    new CommodityEps
                    {
                        CommodityCodeId = "AQACQqweBhEBAAAAwXCOmiFcsxUmKAIA",
                        CommodityCodeName = "QR00000038",
                        CommodityName = "止血包",
                        EquipmentId = "AQACQqweDg8BAAAAFUD8WDEPsxV_FwQA",
                        EquipmentName = "E00000008",
                        GoodsLocationId = "AQACQqweJ4wBAAAAjYv6XmUPsxWWowMA",
                        GoodsLocationName = "L00000013"
                    }
                });

            HashSet<CommodityEps> hs = RfidHelper.GetEpcDataJsonReplenishment(out isGetSuccess);
            ShelfTask shelfTask = testROPara;
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                ReplenishmentClose replenishmentClose = new ReplenishmentClose(shelfTask, hs);
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

#if DUALCAB
            cabClosedNum--;
            if (cabClosedNum == 1)
                return;
#endif

            //弹出盘点中弹窗
            EnterInvotoryOngoing();

            HashSet<CommodityEps> hs = RfidHelper.GetEpcDataJson(out bool isGetSuccess);

            //关闭盘点中弹窗
            ClosePop();

            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                ReplenishmentClose replenishmentClose = new ReplenishmentClose((ShelfTask)delegateGetMsg.userData, hs);
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
        private void onEnterReturnGoodsDetail(object sender, PickTask e)
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
        private void onEnterReturnGoodsDetailOpen(object sender, PickTask e)
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
#if DUALCAB
            List<string> listCom = ComName.GetAllLockerCom();
            if (listCom.Count == 0)
                return;
#else
            List<string> listCom = ComName.GetAllLockerCom();
#endif

            LockHelper.DelegateGetMsg delegateGetMsg = LockHelper.GetLockerData(listCom[0], out bool isGetSuccess);
            delegateGetMsg.DelegateGetMsgEvent += new LockHelper.DelegateGetMsg.DelegateGetMsgHandler(onEnterReturnGoodsCloseEvent);
            delegateGetMsg.userData = e;

#if DUALCAB
            cabClosedNum = 1;
            if (listCom.Count == 2)
            {
                LockHelper.DelegateGetMsg delegateGetMsg2 = LockHelper.GetLockerData(listCom[1], out bool isGetSuccess2);
                delegateGetMsg2.DelegateGetMsgEvent += new LockHelper.DelegateGetMsg.DelegateGetMsgHandler(onEnterReturnGoodsCloseEvent);
                delegateGetMsg2.userData = e;
                cabClosedNum = 2;
            }
#endif
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
            HashSet<CommodityEps> hs = RfidHelper.GetEpcDataJsonReplenishment(out bool isGetSuccess);
            PickTask pickTask = testPOPara;

            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                ReturnGoodsClose returnGoodsClose = new ReturnGoodsClose(pickTask, hs);
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

#if DUALCAB
            cabClosedNum--;
            if (cabClosedNum == 1)
                return;
#endif

            //弹出盘点中弹窗
            EnterInvotoryOngoing();

            HashSet<CommodityEps> hs = RfidHelper.GetEpcDataJson(out bool isGetSuccess);

            //关闭盘点中弹窗
            ClosePop();

            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                ReturnGoodsClose returnGoodsClose = new ReturnGoodsClose((PickTask)delegateGetMsg.userData, hs);
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
            inventory.EnterInventoryDetailEvent += new Inventory.EnterInventoryDetailHandler(onEnterInventoryDetail);
            inventory.EnterInventoryDetailLocalEvent += new Inventory.EnterInventoryDetailLcoalHandler(onEnterInventoryDetailLocal);
            inventory.EnterPopInventoryEvent += new Inventory.EnterPopInventoryHandler(onEnterPopInventory);
            inventory.HidePopInventoryEvent += new Inventory.HidePopInventoryHandler(onHidePopInventory);

            ContentFrame.Navigate(inventory);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterInventoryDetailLocal(object sender, int e)
        {
            HomePageView.Visibility = Visibility.Hidden;
            NaviView.Visibility = Visibility.Hidden;

            InventoryDtlLocal inventoryDetailLocal = new InventoryDtlLocal(e);
            inventoryDetailLocal.BackInventoryEvent += new InventoryDtlLocal.BackInventoryHandler(onBackInventory);

            FullFrame.Navigate(inventoryDetailLocal);
        }


        /// <summary>
        /// 进入盘点详情页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterInventoryDetail(object sender, InventoryTask e)
        {
            HomePageView.Visibility = Visibility.Hidden;
            NaviView.Visibility = Visibility.Hidden;

            InventoryDtl inventoryDetail = new InventoryDtl(e);

            inventoryDetail.EnterPopInventoryEvent += new InventoryDtl.EnterPopInventoryHandler(onEnterPopInventory);
            inventoryDetail.HidePopInventoryEvent += new InventoryDtl.HidePopInventoryHandler(onHidePopInventory);
            inventoryDetail.BackInventoryEvent += new InventoryDtl.BackInventoryHandler(onBackInventory);
            inventoryDetail.OpenDoorEvent += new InventoryDtl.OpenDoorHandler(onInventoryDoorOpen);

            inventoryDetailHandler = inventoryDetail;

            FullFrame.Navigate(inventoryDetail);
        }

        /// <summary>
        /// 回到盘点页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onBackInventory(object sender, RoutedEventArgs e)
        {
            btnBackHP.Visibility = Visibility.Visible;
            NaviView.Visibility = Visibility.Visible;
            inventoryDetailHandler = null;
        }


        /// <summary>
        /// 进入添加单品码弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterPopAddProduct(object sender, RoutedEventArgs e)
        {
            AddProduct addProduct = new AddProduct();
            addProduct.HidePopAddProductEvent += new AddProduct.HidePopAddProductHandler (onHidePopAddProduct);

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
            ClosePop();
        }

        /// <summary>
        /// 弹出库存盘点正在进行中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterPopInventory(object sender, System.EventArgs e)
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
        /// 关闭库存盘点正在进行中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onHidePopInventory(object sender, System.EventArgs e)
        {
            ClosePop();
        }

        /// <summary>
        /// 盘点过程中开门
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onInventoryDoorOpen(object sender, string e)
        {

#if TESTENV
            testTimer = new System.Timers.Timer(3000);
            testTimer.AutoReset = false;
            testTimer.Enabled = true;
            testTimer.Elapsed += new ElapsedEventHandler(onInventoryDoorCloseTest);
#else
            string lockerCom = ComName.GetLockerComByCabName((string)e);

            LockHelper.DelegateGetMsg delegateGetMsg = LockHelper.GetLockerData(lockerCom, out bool isGetSuccess);
            delegateGetMsg.DelegateGetMsgEvent += new LockHelper.DelegateGetMsg.DelegateGetMsgHandler(onInventoryDoorClose);
#endif
        }


#if TESTENV
        /// <summary>
        /// 盘点过程中关门事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onInventoryDoorCloseTest(object sender, EventArgs e)
        {
            inventoryDetailHandler.SetButtonVisibility(true);
        }
#else
        /// <summary>
        /// 盘点过程中关门事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onInventoryDoorClose(object sender, bool isClose)
        {
            LockHelper.DelegateGetMsg delegateGetMsg = (LockHelper.DelegateGetMsg)sender;
            System.Diagnostics.Debug.WriteLine("返回开锁状态{0}", isClose);

            if (!isClose)
                return;

            if (inventoryDetailHandler == null)
                return;

            inventoryDetailHandler.SetButtonVisibility(true);
        }
#endif

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
            ClosePop();
        }
#endregion

        private void onEnterBindingVein(object sender, RoutedEventArgs e)
        {
            PopFrame.Visibility = Visibility.Visible;

            BindingVein bindingVein = new BindingVein();
            bindingVein.HidePopCloseEvent += new BindingVein.HidePopCloseHandler(onHidePopClose);
            PopFrame.Navigate(bindingVein);
#if TESTENV
#else
#if VEINSERIAL
            vein.Close();
#else
            vein.SetDetectFingerState(true);
#endif
#endif
        }


        private void onEnterSysSetting(object sender, RoutedEventArgs e)
        {
            HomePageView.Visibility = Visibility.Hidden;
            btnBackHP.Visibility = Visibility.Visible;

            SystemSetting systemSetting = new SystemSetting();

            ContentFrame.Navigate(systemSetting);
        }

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
#if VEINSERIAL
                vein.ChekVein();
#else
                Task.Factory.StartNew(vein.DetectFinger);
#endif
#endif
            }));
        }

        /// <summary>
        /// 开门提示框关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onHidePopOpen(object sender, RoutedEventArgs e)
        {
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

        /// <summary>
        /// 弹出获取数据提示框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onShowLoadingData(object sender, RoutedEventArgs e)
        {
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                PopFrame.Visibility = Visibility.Visible;
                MaskView.Visibility = Visibility.Visible;

                PopFrame.Navigate(loadingDataPage);
            }));
        }

        /// <summary>
        /// 弹出获取数据提示框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onHideLoadingData(object sender, RoutedEventArgs e)
        {
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                ClosePop();
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
#if VEINSERIAL
            vein.ChekVein();
#else
            Task.Factory.StartNew(vein.DetectFinger);
#endif
#endif
            inventoryDetailHandler = null;

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

            inventoryDetailHandler = null;
        }

#region test

        private void MockData()
        {
            test.InitGoodsInfo();
            test.InitUsersInfo();

#if TESTENV
            LoginBkView.Visibility = Visibility.Hidden;
        
            CurrentUser user = userBll.GetTestUser();        
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
