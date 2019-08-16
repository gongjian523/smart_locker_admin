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
using CFLMedCab.Controls;
using CFLMedCab.Http.Bll;
using CFLMedCab.Http.Model;
using System.Threading;
using System.Threading.Tasks;
using CFLMedCab.Http.Model.Base;
using CFLMedCab.Http.Helper;
using CFLMedCab.Infrastructure.ToolHelper;
using System.Runtime.InteropServices;
using CFLMedCab.Infrastructure.QuartzHelper.scheduler;
using CFLMedCab.Infrastructure.QuartzHelper.job;
using CFLMedCab.Infrastructure.QuartzHelper.trigger;
using CFLMedCab.Infrastructure.QuartzHelper.quartzEnum;
using CFLMedCab.Model.Enum;
using System.Windows.Controls;
using CFLMedCab.Http.Model.login;
using CFLMedCab.Http.Model.param;
using WindowsInput.Native;
using WindowsInput;
using System.Xml;
using CFLMedCab.Model.Constant;

namespace CFLMedCab
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {

		#region 空闲时间处理相关定义
		[DllImport("user32.dll")]
		private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);
		private DispatcherTimer mIdleTimer;
		private LASTINPUTINFO mLastInputInfo;
        #endregion

        private InputSimulator inputSimulator = new InputSimulator();

        private System.Timers.Timer processRingTimer;

        //private DispatcherTimer InventoryTimer;

#if TESTENV
        private System.Timers.Timer testTimer;
        private FetchParam testFetchPara = new FetchParam();
        private ShelfTask testROPara = new ShelfTask();
        private PickTask testPOPara = new PickTask();
#else
#if VEINSERIAL
        private VeinHelper vein;
#else
        private VeinUtils vein;
#endif
#endif

        //[Obsolete]
        //private InventoryBll inventoryBll = new InventoryBll();

#if DUALCAB
        private int cabClosedNum;
#endif
        /// <summary>
        /// 子页面的句柄
        /// </summary>
        private UserControl subViewHandler;
        /// <summary>
        /// 子页面的类型
        /// </summary>
        private SubViewType subViewType;
        private TestGoods test = new TestGoods();

        //实例化一个互斥锁,用来保证同一时间内只有一个线程调用指静脉sdk
        public static Mutex mutex = new Mutex();

        bool bUsing = false;


        public MainWindow()
        {
			#region 空闲时间处理相关定义
			mLastInputInfo = new LASTINPUTINFO();
			mLastInputInfo.cbSize = Marshal.SizeOf(mLastInputInfo);

			mIdleTimer = new DispatcherTimer();
			mIdleTimer.Tick += new EventHandler(IdleTime);//起个Timer一直获取当前时间 
			mIdleTimer.Interval = new TimeSpan(0, 0, 0, 1, 0);
			mIdleTimer.Start();
			#endregion

			//线程池设置
			ThreadPool.SetMaxThreads(100, 100);
			ThreadPool.SetMinThreads(5, 5);

			InitializeComponent();
            LogUtils.Debug("MainWindow initial...");

            foreach (System.Windows.Forms.Screen scr in System.Windows.Forms.Screen.AllScreens)
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

            //InventoryTimer = new DispatcherTimer();
            //InventoryTimer.Tick += new EventHandler(onInventoryTimer);//起个Timer, 每分钟检查是否有扫描计划
            //InventoryTimer.Interval = new TimeSpan(0, 0, 1, 0, 0);
            //InventoryTimer.Start();

            processRingTimer = new System.Timers.Timer(1000*60*1);
            processRingTimer.AutoReset = false;
            processRingTimer.Enabled = false;
            processRingTimer.Elapsed += new ElapsedEventHandler(onProcessRingTimerExpired);

            Task.Factory.StartNew(initCurrentGoodsInfo);
            Task.Factory.StartNew(startAutoInventory);

            LogUtils.Debug("Task initial...");
#if TESTENV
#else
#if VEINSERIAL
            string veinCom = ApplicationState.GetMVeinCOM();
            //vein = new VeinHelper("COM9", 9600);
            vein = new VeinHelper(veinCom, 9600);
            vein.DataReceived += new SerialDataReceivedEventHandler(onReceivedDataVein);
            LogUtils.Debug("onStart");
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
				{
                    vienSt = vein.OpenDevice();

                    if (vienSt != VeinUtils.FV_ERRCODE_SUCCESS && vienSt != VeinUtils.FV_ERRCODE_EXISTING)
					{                          
						onFingerDetected(this, -1);
					}
					else
					{
                        //Console.ReadKey();
                        if (RegisterVein())
                        {
                            //ThreadPool.QueueUserWorkItem(new WaitCallback(vein.DetectFinger));
                            LogUtils.Debug("detectFinger in initial...");
                            ThreadPool.QueueUserWorkItem(new WaitCallback(detectFinger));
                        }
                        else
                        {
							onFingerDetected(this, -2);
						}
                    }
				}
            }
#endif
#endif
            LogUtils.Debug("Vein initial...");
        }

        private void MainWindow_StateChanged(object sender, EventArgs e)
        {
            this.WindowState = WindowState.Maximized;
        }

        private void MainWindow_Deactivated(object sender, EventArgs e)
        {
            this.Topmost = true;
        }

		[Obsolete]
        private void onInventoryTimer(object sender, EventArgs e)
        {
            //List <InventoryPlanLDB> listPan = inventoryBll.GetInventoryPlan().ToList().Where(item => item.status == 0).ToList();

            //foreach(var item in listPan)
            //{
            //    DateTime date1 = DateTime.Now;
            //    DateTime date2 = new DateTime(date1.Year, date1.Month, date1.Day, int.Parse(item.inventorytime_str.Substring(0,2)), int.Parse(item.inventorytime_str.Substring(3,2)), 0);

            //    TimeSpan timeSpan = date2 - date1;

            //    if (timeSpan.TotalMinutes < 1 && timeSpan.TotalMinutes > -1 )
            //    {
            //        bool isGetSuccess;
            //        RfidHelper.GetEpcDataJson(out isGetSuccess);

            //        LogUtils.Debug("onInventoryTimer:" + timeSpan.TotalMinutes);
            //        }
                
            //}
            return;
        }

        //登录提示框消失后
        private void onLoginInfoHidenEvent(object sender, LoginStatus e)
        {
            ClosePop();

            SetSubViewInfo(null, SubViewType.Login);

            if (e.LoginState == 0)
            {
                LogUtils.Debug("onLoginInfoHidenEvent");
#if TESTENV
#else
#if VEINSERIAL
                vein.ChekVein();
#else
                //Task.Factory.StartNew(vein.DetectFinger);
                //ThreadPool.QueueUserWorkItem(new WaitCallback(vein.DetectFinger));
                LogUtils.Debug("detectFinger in onLoginInfoHidenEvent...");
                ThreadPool.QueueUserWorkItem(new WaitCallback(detectFinger));
#endif
#endif
            }
        }

        #region 指静脉
        void detectFinger(object obj)
        {
            if(bUsing)
            {
                LogUtils.Debug("detectFinger bUsing true return");
                return;
            }
            else
            {
                LogUtils.Debug("detectFinger bUsing false turn true");
                bUsing = true;
            }

            mutex.WaitOne();
#if TESTENV
#else
            vein.DetectFinger(obj);
#endif
            mutex.ReleaseMutex();
            LogUtils.Debug("detectFinger bUsing true turn false");
            bUsing = false;
        }


#if VEINSERIAL
        private void onReceivedDataVein(object sender, SerialDataReceivedEventArgs e)
        {
            int id = vein.GetVeinId();

            LogUtils.Debug("VeinId {0}", id);

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
                LogUtils.Debug("onReceivedDataVein");
                vein.ChekVein();
            }

        }
#else
        /// <summary>
        /// 指静脉检测到手指
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onFingerDetected(object sender, int e)
        {
#if TESTENV
            return;
#else
            mutex.WaitOne();
            bUsing = true;
            LogUtils.Debug("detectFinger bUsing turn true 1");

            LoginStatus sta = new LoginStatus();

            User user = null;

            onLoadingData(this,true);
            string info = "等待检测指静脉的时候发生错误";
            string info2 = "请再次进行验证";
            if(e == -2)
            {
                info = "设置指静脉设备本地签名时发生错误";
                info2 = "请重启设备，如果仍然失败，请联系管理员！";
            }
            else if(e == -3)
            {
                info = "设置指静脉设备在使用过程中发生故障";
                info2 = "请重启设备，如果仍然失败，请联系管理员！";
            }

            if (e == 0)
            {
				DateTime LoginStartTime = DateTime.Now;
				
				byte[] macthfeature = new byte[VeinUtils.FV_FEATURE_SIZE];

				bool isGrabFeature = (vein.GrabFeature(macthfeature, out info) == VeinUtils.FV_ERRCODE_SUCCESS);

				DateTime grabFeatureEndTime = DateTime.Now;

				LogUtils.Debug($"调用检测指纹Sdk耗时{grabFeatureEndTime.Subtract(LoginStartTime).TotalMilliseconds}");

				if (isGrabFeature)
                {
                    BaseSinglePostData<VeinMatch> data = UserLoginBll.GetInstance().VeinmatchLogin(new VeinmatchPostParam
                    {
                        regfeature = Convert.ToBase64String(macthfeature)
                    });

                    if (data.code == 0)
                    {
                        user = data.body.user;

                        ApplicationState.SetAccessToken(data.body.accessToken);
                        ApplicationState.SetRefreshToken(data.body.refresh_token);

                        HttpHelper.GetInstance().SetHeaders(data.body.accessToken);
                    }
                    else
                    {
                        info = "没有找到和当前指静脉匹配的用户";
                        info2 = "请先绑定指静脉或者再次尝试";
                        LogUtils.Error("没有找到和当前指静脉匹配的用户：" + data.message);
                    }

					DateTime grabFeatureHttpEndTime = DateTime.Now;
					LogUtils.Debug($"调用指纹请求http耗时{grabFeatureHttpEndTime.Subtract(grabFeatureEndTime).TotalMilliseconds}");
				}
            }
            onLoadingData(this, false);

            if (e < 0 || user ==null)
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    LoginInfo loginInfo = new LoginInfo(new LoginStatus
                    {
                        LoginState = 0,
                        LoginString = info,
                        LoginString2 = info2
                    });

                    loginInfo.LoginInfoHidenEvent += new LoginInfo.LoginInfoHidenHandler(onLoginInfoHidenEvent);

                    onShowPopFrame(loginInfo);
                    //PopFrame.Visibility = Visibility.Visible;
                    //MaskView.Visibility = Visibility.Visible;
                    //PopFrame.Navigate(loginInfo);
                }));
            }
            else
            {
                SimulateKeybordInput0();

                // 进入首页
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    onEnterHomePage(user);

                    //LoginBkView.Visibility = Visibility.Hidden;
                    //ApplicationState.SetUserInfo(user);
                    //SetNavBtnVisiblity(user.Role);
                    //tbNameText.Text = user.name;
                }));
            }
            bUsing = false;
            LogUtils.Debug("detectFinger bUsing turn false 1");
            mutex.ReleaseMutex();
#endif
        }
#endif

        /// <summary>
        /// 指静脉签名注册
        /// </summary>
        /// <returns></returns>
        private bool RegisterVein()
        {
#if TESTENV
            return true;
#else
            byte[] devSign = new byte[36];
            int veinSt = vein.GetDevSign(devSign, out ushort devSignLen);
            if (veinSt != VeinUtils.FV_ERRCODE_SUCCESS)
            {
                LogUtils.Error("获取本地指静脉设备签名失败！" + veinSt);
                return false;
            }

            //转换成16进制字符串
            string devStr = HexHelper.ByteToHexStr(devSign);

            BaseSinglePostData<VeinRegister> bdVeinRegister = UserLoginBll.GetInstance().VeinmatchRegister(new VeinregisterPostParam
            {
                devsign = devStr
            });
            HttpHelper.GetInstance().ResultCheck(bdVeinRegister, out bool isSuccess);
            if (!isSuccess)
            {
                LogUtils.Error("获取服务器指静脉设备签名失败！");
                return false;
            }

            byte[] serSign = HexHelper.StrToToHexByte(bdVeinRegister.body.srvsign);

            veinSt = vein.SetDevSign(serSign, (ushort)serSign.Count());

            if (veinSt != VeinUtils.FV_ERRCODE_FUNCTION_INVALID)
            {
                LogUtils.Error("设置本地指静脉设备签名失败！" + veinSt);
                return false;
            }
            return true;
#endif
        }
#endregion


        private void SetNavBtnVisiblity(string  role)
        {
            bool isMedicalStuff = (role == "医院医护人员") ? true : false;

            NavBtnEnterGerFetch.Visibility = isMedicalStuff ? Visibility.Visible : Visibility.Hidden;
            NavBtnEnterSurgery.Visibility = isMedicalStuff ? Visibility.Visible : Visibility.Hidden;
            NavBtnEnterPrescription.Visibility = isMedicalStuff ? Visibility.Visible : Visibility.Hidden;
            NavBtnEnterReturnFetch.Visibility = isMedicalStuff ? Visibility.Visible : Visibility.Hidden;

            NavBtnEnterReplenishment.Visibility = (!isMedicalStuff) ? Visibility.Visible : Visibility.Hidden;
            NavBtnEnterReturnGoods.Visibility = (!isMedicalStuff) ? Visibility.Visible : Visibility.Hidden;
            NavBtnEnterReturnAll.Visibility = (!isMedicalStuff) ? Visibility.Visible : Visibility.Hidden;
            NavBtnEnterStockSwitch.Visibility = (!isMedicalStuff) ? Visibility.Visible : Visibility.Hidden;
            NavBtnEnterInvtory.Visibility = Visibility.Visible;
            NavBtnEnterStock.Visibility = Visibility.Visible;
            NavBtnEnterSysSetting.Visibility = (!isMedicalStuff) ? Visibility.Visible : Visibility.Hidden;
        }

        /// <summary>
        /// 设置子页面信息
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="type"></param>
        private void SetSubViewInfo(UserControl handler, SubViewType type)
        {
            subViewHandler = handler;
            subViewType = type;
        }

        /// <summary>
        /// 空闲监听定时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IdleTime(object sender, EventArgs e)
        {
            //显示当前时间
            tbDateText.Text = DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss");

            if (!GetLastInputInfo(ref mLastInputInfo))
            {
                System.Windows.MessageBox.Show("GetLastInputInfo Failed!");
                return;
            }

            if ((Environment.TickCount - (long)mLastInputInfo.dwTime) / 1000 < (60 * Contant.IdleTimeExpireLength))
            {
                return;
            }

            //处于登录页，不用处理：当货柜门打开的时候，不能强制退出登陆
            if (subViewType == SubViewType.Login || subViewType == SubViewType.DoorOpen)
                return;

            switch(subViewType)
            {
                case SubViewType.GerFetchClose:
                    ((GerFetchView)subViewHandler).onExitTimerExpired();
                    break;
                case SubViewType.SurFetchWoOrderClose:
                    ((SurgeryNoNumClose)subViewHandler).onExitTimerExpired();
                    break;
                case SubViewType.SurFetchWOrderClose:
                    ((SurgeryNumClose)subViewHandler).onExitTimerExpired();
                    break;
                case SubViewType.ReturnFetchClose:
                    ((ReturnFetchView)subViewHandler).onExitTimerExpired();
                    break;
                case SubViewType.ReplenishmentClose:
                    ((ReplenishmentClose)subViewHandler).onExitTimerExpired();
                    break;
                case SubViewType.ReturnClose:
                    ((ReturnClose)subViewHandler).onExitTimerExpired();
                    break;
                case SubViewType.ReturnGoodsClose:
                    ((ReturnGoodsClose)subViewHandler).onExitTimerExpired();
                    break;
            }
            onReturnToLogin();
        }

        /// <summary>
        /// 模拟从键盘向系统输入0
        /// </summary>
        protected void SimulateKeybordInput0()
        {           
            inputSimulator.Keyboard.KeyDown(VirtualKeyCode.NUMPAD0);
            Thread.Sleep(100);
            inputSimulator.Keyboard.KeyDown(VirtualKeyCode.NUMPAD0);
        }


        //退出登录，回到登录页
        private void onReturnToLogin()
        {
            LogUtils.Debug("onReturnToLogin");
            PopFrame.Visibility = Visibility.Hidden;
            MaskView.Visibility = Visibility.Hidden;

            NaviView.Visibility = Visibility.Visible;
            HomePageView.Visibility = Visibility.Visible;
            btnBackHP.Visibility = Visibility.Hidden;

            LoginBkView.Visibility = Visibility.Visible;

            //回到登陆页
            SetSubViewInfo(null, SubViewType.Login);

            //用adminToken取代旧用户的accessToken，
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load($"{ApplicationState.GetProjectRootPath()}/MyProject.xml");
            XmlNode root = xmlDoc.SelectSingleNode("config");//指向根节点
            XmlNode adminToken = root.SelectSingleNode("admin_token");//指向设备节点
            HttpHelper.GetInstance().SetHeaders(adminToken.InnerText);

#if TESTENV
#else
#if VEINSERIAL
            vein.ChekVein();
#else
            //ThreadPool.QueueUserWorkItem(new WaitCallback(vein.DetectFinger));
            //Task.Factory.StartNew(vein.DetectFinger);
            ThreadPool.QueueUserWorkItem(new WaitCallback(detectFinger));
#endif
#endif
        }

        //登陆，进入首页
        private void onEnterHomePage(User user)
        {
            LoginBkView.Visibility = Visibility.Hidden;
            PopFrame.Visibility = Visibility.Hidden;

            ApplicationState.SetUserInfo(user);
            SetNavBtnVisiblity(user.Role);
            tbNameText.Text = user.name;

            //进入首页，将句柄设置成null，避免错误调用
            SetSubViewInfo(null, SubViewType.Home);
        }

        private void onShowPopFrame(object content)
        {
            PopFrame.Visibility = Visibility.Visible;
            MaskView.Visibility = Visibility.Visible;

            PopFrame.Navigate(content);
        }

        private void onExitApp(object sender, RoutedEventArgs e)
        {
            Taskbar.HideTask(false);
            System.Environment.Exit(0);
        }

        private void initCurrentGoodsInfo()
        {
#if TESTENV
            HashSet<CommodityEps> hs = new HashSet<CommodityEps>();
#else
            HashSet<CommodityEps> hs = RfidHelper.GetEpcDataJson(out bool isGetSuccess);
#endif
            ApplicationState.SetGoodsInfo(hs);
        }

		/// <summary>
		/// 开启自动任务盘点信息
		/// </summary>
        private void startAutoInventory()
        {
            CustomizeScheduler.GetInstance().SchedulerStart<GetInventoryPlanJoB>(CustomizeTrigger.GetInventoryPlanTrigger(), GroupName.GetInventoryPlan);
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
            //进入一般领用开门页面，将句柄设置成null，类型设置成DoorOpen
            SetSubViewInfo(null, SubViewType.DoorOpen);

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
            LogUtils.Debug($"返回开锁状态{isClose}");

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

            //模拟从键盘输入0，空闲时间重新开始计时
            SimulateKeybordInput0();
            
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                GerFetchView gerFetchView = new GerFetchView(hs);
                gerFetchView.EnterPopCloseEvent += new GerFetchView.EnterPopCloseHandler(onEnterPopClose);
                gerFetchView.EnterGerFetch += new GerFetchView.EnterFetchOpenHandler(onEnterGerFetch);
                gerFetchView.LoadingDataEvent += new GerFetchView.LoadingDataHandler(onLoadingData);
                FullFrame.Navigate(gerFetchView);

                //进入一般领用关门页面
                SetSubViewInfo(gerFetchView, SubViewType.GerFetchClose);
            }));
        }
#endregion

#region 手术领用

#region 无手术单领用和医嘱处方领用
        /// <summary>
        /// 进入手术无单领用和医嘱处方领用-开门状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterSurgeryNoNumOpen(object sender, ConsumingOrder e)
        {
            NaviView.Visibility = Visibility.Hidden;

            GerFetchState gerFetchState = new GerFetchState(1);
            FullFrame.Navigate(gerFetchState);
            //进入一般手术无单领用开门页面，将句柄设置成null，类型设置成DoorOpen
            SetSubViewInfo(null, SubViewType.DoorOpen);

            List<string> com = ApplicationState.GetAllLockerCom();

            LockHelper.DelegateGetMsg delegateGetMsg = LockHelper.GetLockerData(com[0], out bool isGetSuccess);
            delegateGetMsg.DelegateGetMsgEvent += new LockHelper.DelegateGetMsg.DelegateGetMsgHandler(onEnterSurgeryNoNumLockerEvent);
            delegateGetMsg.userData = e;

#if DUALCAB
            LockHelper.DelegateGetMsg delegateGetMsg2 = LockHelper.GetLockerData(com[1], out bool isGetSuccess2);
            delegateGetMsg2.DelegateGetMsgEvent += new LockHelper.DelegateGetMsg.DelegateGetMsgHandler(onEnterSurgeryNoNumLockerEvent);
            delegateGetMsg.userData = e;

            cabClosedNum = 0;
#endif

            SpeakerHelper.Sperker("柜门已开，请拿取您需要的耗材，拿取完毕请关闭柜门");
        }

        /// <summary>
        /// 手术无单领用和和医嘱处方领用-关门状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterSurgeryNoNumLockerEvent(object sender, bool isClose)
        {
            LogUtils.Debug($"返回开锁状态{isClose}");

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

            LockHelper.DelegateGetMsg delegateGetMsg = (LockHelper.DelegateGetMsg)sender;
            ConsumingOrder consumingOrder = (ConsumingOrder)delegateGetMsg.userData;

            //关闭盘点中弹窗
            ClosePop();

            //模拟从键盘输入0，空闲时间重新开始计时
            SimulateKeybordInput0();

            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                SurgeryNoNumClose surgeryNoNumClose;
                if (consumingOrder == null)
                {
                    surgeryNoNumClose  = new SurgeryNoNumClose(hs, ConsumingOrderType.手术领用, new ConsumingOrder());
                }
                else
                {
                    surgeryNoNumClose = new SurgeryNoNumClose(hs, ConsumingOrderType.医嘱处方领用, consumingOrder);
                }
                
                surgeryNoNumClose.EnterPopCloseEvent += new SurgeryNoNumClose.EnterPopCloseHandler(onEnterPopClose);
                surgeryNoNumClose.EnterSurgeryNoNumOpenEvent += new SurgeryNoNumClose.EnterSurgeryNoNumOpenHandler(onEnterGerFetch);
                surgeryNoNumClose.LoadingDataEvent += new SurgeryNoNumClose.LoadingDataHandler(onLoadingData);
                FullFrame.Navigate(surgeryNoNumClose);
                //进入一般手术无单领用和医嘱处方的关门页面
                SetSubViewInfo(surgeryNoNumClose, SubViewType.SurFetchWoOrderClose);
            }));
        }
#endregion

#region 有手术单领用
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
            surgeryQuery.EnterPrescriptionOpenEvent += new SurgeryQuery.EnterPrescriptionOpenHandler(onEnterSurgeryNoNumOpen);//医嘱处方领用直接开柜领用
            surgeryQuery.LoadingDataEvent += new SurgeryQuery.LoadingDataHandler(onLoadingData);

            ContentFrame.Navigate(surgeryQuery);
            //进入手术有单领用查询页面，将句柄设置成null，类型设置成other，避免错误调用
            SetSubViewInfo(null, SubViewType.Others);
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
            //进入一般手术领用单产品详情页面，将句柄设置成null，类型设置成other，避免错误调用
            SetSubViewInfo(null, SubViewType.Others);
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
            //进入手术有单领用开门页面，将句柄设置成null，类型设置成DoorOpen
            SetSubViewInfo(null, SubViewType.DoorOpen);

            OpenCabinet openCabinet = new OpenCabinet();
            openCabinet.HidePopOpenEvent += new OpenCabinet.HidePopOpenHandler(onHidePopOpen);
            onShowPopFrame(openCabinet);
            //MaskView.Visibility = Visibility.Visible;
            //PopFrame.Visibility = Visibility.Visible;
            //PopFrame.Navigate(openCabinet);

#if TESTENV
            testTimer = new System.Timers.Timer(10000);
            testTimer.AutoReset = false;
            testTimer.Enabled = true;
            testTimer.Elapsed += new ElapsedEventHandler(onEnterSurgeryNumLockerTestEvent);
            testFetchPara = fetchParam;
#else

#if DUALCAB
            List<string> listCom = ApplicationState.GetAllLockerCom();
            if (listCom.Count == 0)
                return;
#else
            List<string> listCom = ApplicationState.GetAllLockerCom();
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
                surgeryNumClose.LoadingDataEvent += new SurgeryNumClose.LoadingDataHandler(onLoadingData);
                FullFrame.Navigate(surgeryNumClose);
                //进入手术有单领用关门页面，
                SetSubViewInfo(surgeryNumClose, SubViewType.SurFetchWOrderClose);
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
            LogUtils.Debug($"返回开锁状态{isClose}");

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

            //模拟从键盘输入0，空闲时间重新开始计时
            SimulateKeybordInput0();

            LockHelper.DelegateGetMsg delegateGetMsg = (LockHelper.DelegateGetMsg)sender;
            FetchParam fetchParam = (FetchParam)delegateGetMsg.userData;

            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                SurgeryNumClose surgeryNumClose = new SurgeryNumClose(fetchParam, ht);
                surgeryNumClose.EnterPopCloseEvent += new SurgeryNumClose.EnterPopCloseHandler(onEnterPopClose);
                surgeryNumClose.EnterSurgeryNumOpenEvent += new SurgeryNumClose.EnterSurgeryNumOpenHandler(EnterSurgeryNumOpenEvent);
                surgeryNumClose.LoadingDataEvent += new SurgeryNumClose.LoadingDataHandler(onLoadingData);
                FullFrame.Navigate(surgeryNumClose);
                //进入手术有单领用关门页面，
                SetSubViewInfo(surgeryNumClose, SubViewType.SurFetchWOrderClose);
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
            //进入领用退回开门页面
            SetSubViewInfo(null, SubViewType.DoorOpen);

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
            LogUtils.Debug($"返回开锁状态{isClose}");

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

            //模拟从键盘输入0，空闲时间重新开始计时
            SimulateKeybordInput0();

            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                ReturnFetchView returnFetchView = new ReturnFetchView(hs);
                returnFetchView.EnterPopCloseEvent += new ReturnFetchView.EnterPopCloseHandler(onEnterPopClose);
                returnFetchView.EnterReturnFetchEvent += new ReturnFetchView.EnterReturnFetchHandler(onEnterReturnFetch);
                returnFetchView.LoadingDataEvent += new ReturnFetchView.LoadingDataHandler(onLoadingData);
                FullFrame.Navigate(returnFetchView);
                //进入领用退回关门页面
                SetSubViewInfo(returnFetchView, SubViewType.ReturnFetchClose);
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
            replenishment.LoadingDataEvent += new Replenishment.LoadingDataHandler(onLoadingData);

            ContentFrame.Navigate(replenishment);
            //进入上架页面，将句柄设置成null，类型设置成other，避免错误调用
            SetSubViewInfo(null, SubViewType.Others);
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
            replenishmentDetail.LoadingDataEvent += new ReplenishmentDetail.LoadingDataHandler(onLoadingData);

            ContentFrame.Navigate(replenishmentDetail);
            //进入上架任务单详情页面，将句柄设置成null，类型设置成other，避免错误调用
            SetSubViewInfo(null, SubViewType.Others);
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
            replenishmentDetailOpen.LoadingDataEvent += new ReplenishmentDetailOpen.LoadingDataHandler(onLoadingData);
            FullFrame.Navigate(replenishmentDetailOpen);
            //进入上架任务单详情开门页面，将句柄设置成null，类型设置成DoorOpen，避免错误调用
            SetSubViewInfo(null, SubViewType.DoorOpen);

            OpenCabinet openCabinet = new OpenCabinet();
            openCabinet.HidePopOpenEvent += new OpenCabinet.HidePopOpenHandler(onHidePopOpen);
            onShowPopFrame(openCabinet);
            //MaskView.Visibility = Visibility.Visible;
            //PopFrame.Visibility = Visibility.Visible;
            //PopFrame.Navigate(openCabinet);

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
            List<string> listCom = ApplicationState.GetAllLockerCom();
#else
            List<string> listCom = ApplicationState.GetAllLockerCom();
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

            //HashSet<CommodityEps> hs = RfidHelper.GetEpcDataJsonReplenishment(out bool isGetSuccess);
            HashSet<CommodityEps> hs = new HashSet<CommodityEps>();
            ShelfTask shelfTask = testROPara;
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                ReplenishmentClose replenishmentClose = new ReplenishmentClose(shelfTask, hs);
                replenishmentClose.EnterReplenishmentDetailOpenEvent += new ReplenishmentClose.EnterReplenishmentDetailOpenHandler(onEnterReplenishmentDetailOpen);
                replenishmentClose.EnterPopCloseEvent += new ReplenishmentClose.EnterPopCloseHandler(onEnterPopClose);
                replenishmentClose.LoadingDataEvent += new ReplenishmentClose.LoadingDataHandler(onLoadingData);

                FullFrame.Navigate(replenishmentClose);
                //进入上架任务单关门页面
                SetSubViewInfo(replenishmentClose, SubViewType.ReplenishmentClose);
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
            LogUtils.Debug($"返回开锁状态{isClose}");

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

            //模拟从键盘输入0，空闲时间重新开始计时
            SimulateKeybordInput0();

            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                ReplenishmentClose replenishmentClose = new ReplenishmentClose((ShelfTask)delegateGetMsg.userData, hs);
                replenishmentClose.EnterReplenishmentDetailOpenEvent += new ReplenishmentClose.EnterReplenishmentDetailOpenHandler(onEnterReplenishmentDetailOpen);
                replenishmentClose.EnterPopCloseEvent += new ReplenishmentClose.EnterPopCloseHandler(onEnterPopClose);
                replenishmentClose.LoadingDataEvent += new ReplenishmentClose.LoadingDataHandler(onLoadingData);

                FullFrame.Navigate(replenishmentClose);
                //进入上架任务单关门页面
                SetSubViewInfo(replenishmentClose, SubViewType.ReplenishmentClose);
            }));
        }
#endif

#endregion

#region  ReturnGoods
        /// <summary>
        /// 进入拣货页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterReturnGoods(object sender, RoutedEventArgs e)
        {
            HomePageView.Visibility = Visibility.Hidden;
            btnBackHP.Visibility = Visibility.Visible;

            ReturnGoods returnGoods = new ReturnGoods(false);
            returnGoods.EnterReturnGoodsDetailEvent += new ReturnGoods.EnterReturnGoodsDetailHandler(onEnterReturnGoodsDetail);
            returnGoods.EnterReturnGoodsDetailOpenEvent += new ReturnGoods.EnterReturnGoodsDetailOpenHandler(onEnterReturnGoodsDetailOpen);
            returnGoods.LoadingDataEvent += new ReturnGoods.LoadingDataHandler(onLoadingData);

            ContentFrame.Navigate(returnGoods);
            //进入拣货页面，将句柄设置成null，类型设置成other，避免错误调用
            SetSubViewInfo(null, SubViewType.Others);
        }

        /// <summary>
        /// 进入拣货详情页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterReturnGoodsDetail(object sender, PickTask e)
        {
            ReturnGoodsDetail returnGoodsDetail = new ReturnGoodsDetail(e);
            returnGoodsDetail.EnterReturnGoodsDetailOpenEvent += new ReturnGoodsDetail.EnterReturnGoodsDetailOpenHandler(onEnterReturnGoodsDetailOpen);
            returnGoodsDetail.EnterReturnGoodsEvent += new ReturnGoodsDetail.EnterReturnGoodsHandler(onEnterReturnGoods);
            returnGoodsDetail.LoadingDataEvent += new ReturnGoodsDetail.LoadingDataHandler(onLoadingData);

            ContentFrame.Navigate(returnGoodsDetail);
            //进入拣货任务单详情页面，将句柄设置成null，类型设置成other，避免错误调用
            SetSubViewInfo(null, SubViewType.Others);
        }


        /// <summary>
        /// 进入拣货任务单详情页面-开门状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterReturnGoodsDetailOpen(object sender, PickTask e)
        {
            NaviView.Visibility = Visibility.Hidden;

            ReturnGoodsDetailOpen returnGoodsDetailOpen = new ReturnGoodsDetailOpen(e);
            returnGoodsDetailOpen.LoadingDataEvent += new ReturnGoodsDetailOpen.LoadingDataHandler(onLoadingData);

            FullFrame.Navigate(returnGoodsDetailOpen);
            //进入拣货任务单详情页面，将句柄设置成null，类型设置成DoorOpen
            SetSubViewInfo(null, SubViewType.DoorOpen);

            OpenCabinet openCabinet = new OpenCabinet();
            openCabinet.HidePopOpenEvent += new OpenCabinet.HidePopOpenHandler(onHidePopOpen);
            onShowPopFrame(openCabinet);
            //MaskView.Visibility = Visibility.Visible;
            //PopFrame.Visibility = Visibility.Visible;
            //PopFrame.Navigate(openCabinet);

            SpeakerHelper.Sperker("柜门已开，请您按照要求拣货，拣货完毕请关闭柜门");

#if TESTENV
            testTimer = new System.Timers.Timer(10000);
            testTimer.AutoReset = false;
            testTimer.Enabled = true;
            testTimer.Elapsed += new ElapsedEventHandler(onEnterReturnGoodsCloseTestEvent);
            testPOPara = e;
#else
#if DUALCAB
            List<string> listCom = ApplicationState.GetAllLockerCom();
            if (listCom.Count == 0)
                return;
#else
            List<string> listCom = ApplicationState.GetAllLockerCom();
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
        /// 进入拣货任务单详情页-关门状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterReturnGoodsCloseTestEvent(object sender, EventArgs e)
        {
            //HashSet<CommodityEps> hs = RfidHelper.GetEpcDataJsonReplenishment(out bool isGetSuccess);
            HashSet<CommodityEps> hs = new HashSet<CommodityEps>();
            PickTask pickTask = testPOPara;

            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                ReturnGoodsClose returnGoodsClose = new ReturnGoodsClose(pickTask, hs);
                returnGoodsClose.EnterReturnGoodsDetailOpenEvent += new ReturnGoodsClose.EnterReturnGoodsDetailOpenHandler(onEnterReturnGoodsDetailOpen);
                returnGoodsClose.EnterPopCloseEvent += new ReturnGoodsClose.EnterPopCloseHandler(onEnterPopClose);
                returnGoodsClose.LoadingDataEvent += new ReturnGoodsClose.LoadingDataHandler(onLoadingData);

                FullFrame.Navigate(returnGoodsClose);
            }));
        }
#else
        /// <summary>
        /// 进入拣货任务单详情页-关门状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterReturnGoodsCloseEvent(object sender, bool isClose)
        {
            LockHelper.DelegateGetMsg delegateGetMsg = (LockHelper.DelegateGetMsg)sender;
            LogUtils.Debug($"返回开锁状态{isClose}");

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

            //模拟从键盘输入0，空闲时间重新开始计时
            SimulateKeybordInput0();

            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                ReturnGoodsClose returnGoodsClose = new ReturnGoodsClose((PickTask)delegateGetMsg.userData, hs);
                returnGoodsClose.EnterReturnGoodsDetailOpenEvent += new ReturnGoodsClose.EnterReturnGoodsDetailOpenHandler(onEnterReturnGoodsDetailOpen);
                returnGoodsClose.EnterPopCloseEvent += new ReturnGoodsClose.EnterPopCloseHandler(onEnterPopClose);
                returnGoodsClose.LoadingDataEvent += new ReturnGoodsClose.LoadingDataHandler(onLoadingData);

                FullFrame.Navigate(returnGoodsClose);
                //进入拣货任务单详情页面
                SetSubViewInfo(returnGoodsClose, SubViewType.ReturnGoodsClose);
            }));
        }
#endif

        /// <summary>
        /// 进入回收取货页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterReturnPick(object sender, RoutedEventArgs e)
        {          
            HomePageView.Visibility = Visibility.Hidden;
            btnBackHP.Visibility = Visibility.Visible;

            ReturnQuery returnQuery = new ReturnQuery();
            returnQuery.EnterReturnOpenEvent += new ReturnQuery.EnterReturnOpenHandler(onEnterReturnOpen);
            returnQuery.LoadingDataEvent += new ReturnQuery.LoadingDataHandler(onLoadingData);

            ContentFrame.Navigate(returnQuery);
            //进入回收取货页面，将句柄设置成null，类型设置成other，避免错误调用
            SetSubViewInfo(null, SubViewType.Others);
        }

        /// <summary>
        /// 进入回收取货页面-开门状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterReturnOpen(object sender, CommodityRecovery e)
        {
            NaviView.Visibility = Visibility.Hidden;

            GerFetchState gerFetchState = new GerFetchState(1);
            FullFrame.Navigate(gerFetchState);
            //进入回收取货开门页面，将句柄设置成null，类型设置成DoorOpen
            SetSubViewInfo(null, SubViewType.DoorOpen);

            List<string> com = ApplicationState.GetAllLockerCom();

            LockHelper.DelegateGetMsg delegateGetMsg = LockHelper.GetLockerData(com[0], out bool isGetSuccess);
            delegateGetMsg.DelegateGetMsgEvent += new LockHelper.DelegateGetMsg.DelegateGetMsgHandler(onEnterReturnClose);
            delegateGetMsg.userData = e;

#if DUALCAB
            LockHelper.DelegateGetMsg delegateGetMsg2 = LockHelper.GetLockerData(com[1], out bool isGetSuccess2);
            delegateGetMsg2.DelegateGetMsgEvent += new LockHelper.DelegateGetMsg.DelegateGetMsgHandler(onEnterReturnClose);
            delegateGetMsg.userData = e;
            cabClosedNum = 0;
#endif
            SpeakerHelper.Sperker("柜门已开，请拿取您需要的耗材，拿取完毕请关闭柜门");
        }

        /// <summary>
        /// 进入回收取货-关门状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterReturnClose(object sender, bool isClose)
        {
            LockHelper.DelegateGetMsg delegateGetMsg = (LockHelper.DelegateGetMsg)sender;
            LogUtils.Debug($"返回开锁状态{isClose}");

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

            //模拟从键盘输入0，空闲时间重新开始计时
            SimulateKeybordInput0();

            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                ReturnClose returnClose = new ReturnClose(hs,(CommodityRecovery)delegateGetMsg.userData);
                returnClose.EnterReturnOpenEvent += new ReturnClose.EnterReturnOpenHandler(onEnterReturnOpen);
                returnClose.EnterStockSwitchOpenEvent += new ReturnClose.EnterStockSwitchOpenHandler(onEnterStockSwitch);
                returnClose.EnterPopCloseEvent += new ReturnClose.EnterPopCloseHandler(onEnterPopClose);
                returnClose.LoadingDataEvent += new ReturnClose.LoadingDataHandler(onLoadingData);

                FullFrame.Navigate(returnClose);
                //进入回收取货关门页面
                SetSubViewInfo(returnClose, SubViewType.ReturnClose);
            }));
        }

        /// <summary>
        /// 进入库存调整
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterStockSwitch(object sender, RoutedEventArgs e)
        {
            HomePageView.Visibility = Visibility.Hidden;
            btnBackHP.Visibility = Visibility.Visible;
            NaviView.Visibility = Visibility.Hidden;

            GerFetchState gerFetchState = new GerFetchState(4);
            FullFrame.Navigate(gerFetchState);
            //进入库存调整开门页面，将句柄设置成null，类型设置成DoorOpen
            SetSubViewInfo(null, SubViewType.DoorOpen);

            List<string> com = ApplicationState.GetAllLockerCom();

            LockHelper.DelegateGetMsg delegateGetMsg = LockHelper.GetLockerData(com[0], out bool isGetSuccess);
            delegateGetMsg.DelegateGetMsgEvent += new LockHelper.DelegateGetMsg.DelegateGetMsgHandler(onEnterReturnClose);

#if DUALCAB
            LockHelper.DelegateGetMsg delegateGetMsg2 = LockHelper.GetLockerData(com[1], out bool isGetSuccess2);
            delegateGetMsg2.DelegateGetMsgEvent += new LockHelper.DelegateGetMsg.DelegateGetMsgHandler(onEnterReturnClose);

            cabClosedNum = 0;
#endif
            SpeakerHelper.Sperker("柜门已开，请您根据需要调整耗材，操作完毕请关闭柜门");
        }

        /// <summary>
        /// 进入进入库存调整-关门状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterStockSwitchClose(object sender, bool isClose)
        {
            LockHelper.DelegateGetMsg delegateGetMsg = (LockHelper.DelegateGetMsg)sender;
            LogUtils.Debug($"返回开锁状态{isClose}");

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

            //模拟从键盘输入0，空闲时间重新开始计时
            SimulateKeybordInput0();

            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                ReturnClose returnClose = new ReturnClose(hs,null);
                returnClose.EnterReturnOpenEvent += new ReturnClose.EnterReturnOpenHandler(onEnterReturnOpen);
                returnClose.EnterStockSwitchOpenEvent += new ReturnClose.EnterStockSwitchOpenHandler(onEnterStockSwitch);
                returnClose.EnterPopCloseEvent += new ReturnClose.EnterPopCloseHandler(onEnterPopClose);
                returnClose.LoadingDataEvent += new ReturnClose.LoadingDataHandler(onLoadingData);

                FullFrame.Navigate(returnClose);
                //进入库存调整关门页面
                SetSubViewInfo(returnClose, SubViewType.ReturnClose);
            }));
        }
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
            inventory.SetPopInventoryEvent += new Inventory.SetPopInventoryHandler(onSetPopInventory);
            inventory.LoadingDataEvent += new Inventory.LoadingDataHandler(onLoadingData);

            ContentFrame.Navigate(inventory);
            //进入库存盘点页面，将句柄设置成null，类型设置成other，避免错误调用
            SetSubViewInfo(null, SubViewType.Others);
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
            //进入本地盘点详情，将句柄设置成null，类型设置成other，避免错误调用
            SetSubViewInfo(null, SubViewType.Others);
        }


        /// <summary>
        /// 进入盘点详情页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterInventoryDetail(object sender, InventoryOrder e)
        {
            HomePageView.Visibility = Visibility.Hidden;
            NaviView.Visibility = Visibility.Hidden;

            InventoryDtl inventoryDetail = new InventoryDtl(e);

            inventoryDetail.SetPopInventoryEvent += new InventoryDtl.SetPopInventoryHandler(onSetPopInventory);
            inventoryDetail.BackInventoryEvent += new InventoryDtl.BackInventoryHandler(onBackInventory);
            inventoryDetail.OpenDoorEvent += new InventoryDtl.OpenDoorHandler(onInventoryDoorOpen);
            inventoryDetail.LoadingDataEvent += new InventoryDtl.LoadingDataHandler(onLoadingData);

            FullFrame.Navigate(inventoryDetail);
            //进入盘点详情
            SetSubViewInfo(inventoryDetail, SubViewType.InventoryDtl);
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

            //回到盘点页，将句柄设置成null，类型设置成other，避免错误调用
            SetSubViewInfo(null, SubViewType.Others);
        }


        /// <summary>
        /// 进入添加单品码弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [Obsolete]
        private void onEnterPopAddProduct(object sender, RoutedEventArgs e)
        {
            AddProduct addProduct = new AddProduct();
            addProduct.HidePopAddProductEvent += new AddProduct.HidePopAddProductHandler (onHidePopAddProduct);

            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                onShowPopFrame(addProduct);
                //PopFrame.Visibility = Visibility.Visible;
                //MaskView.Visibility = Visibility.Visible;
                //PopFrame.Navigate(addProduct);
            }));
        }

        /// <summary>
        /// 关闭添加单品码弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [Obsolete]
        private void onHidePopAddProduct(object sender, RoutedEventArgs e)
        {
            ClosePop();
        }

        /// <summary>
        /// 弹出或者关闭库存盘点正在进行中的页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onSetPopInventory(object sender, bool e)
        {
            if(e)
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    InventoryOngoing inventoryOngoing = new InventoryOngoing();
                    onShowPopFrame(inventoryOngoing);
                    //PopFrame.Visibility = Visibility.Visible;
                    //MaskView.Visibility = Visibility.Visible;
                    //PopFrame.Navigate(inventoryOngoing);
                }));
            }
            else
            {
                ClosePop();
            }
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
            string lockerCom = ApplicationState.GetLockerComByCabName((string)e);

            LockHelper.DelegateGetMsg delegateGetMsg = LockHelper.GetLockerData(lockerCom, out bool isGetSuccess);
            delegateGetMsg.DelegateGetMsgEvent += new LockHelper.DelegateGetMsg.DelegateGetMsgHandler(onInventoryDoorClose);

            //盘点开门的时候，句柄还是设置成InventoryDtl使用的句柄，将状态设置成DoorOpen
            SetSubViewInfo(subViewHandler, SubViewType.DoorOpen);
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
            if (subViewHandler == null || subViewType != SubViewType.InventoryDtl) 
                return;
            ((InventoryDtl)subViewHandler).SetButtonVisibility(true);
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
            LogUtils.Debug($"返回开锁状态{isClose}");

            if (!isClose)
                return;

            if (subViewHandler == null || subViewType != SubViewType.DoorOpen) 
                return;

            ((InventoryDtl)subViewHandler).SetButtonVisibility(true);

            //盘点开门的时候，句柄还是设置成InventoryDtl使用的句柄，状态恢复成InventoryDtl
            SetSubViewInfo(subViewHandler, SubViewType.InventoryDtl);
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
            stock.SetPopInventoryEvent += new Stock.SetPopInventoryHandler(onSetPopInventory);
            ContentFrame.Navigate(stock);
            //进入库存盘点页面，将句柄设置成null，类型设置成other，避免错误调用
            SetSubViewInfo(null, SubViewType.Others);
        }

        private void onEnterStockDetailedEvent(object sender, Commodity commodity)
        {
            StockDetailed stockDetailed = new StockDetailed(commodity);
            stockDetailed.EnterStockEvent += new StockDetailed.EnterStockHandler(colseStockDetailedEvent);

            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                onShowPopFrame(stockDetailed);
                //PopFrame.Visibility = Visibility.Visible;
                //MaskView.Visibility = Visibility.Visible;
                //PopFrame.Navigate(stockDetailed);
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

#if TESTENV
#else
#if VEINSERIAL
            vein.Close();
#else
            vein.SetDetectFingerState(true);
#endif
#endif

            BindingVein bindingVein = new BindingVein(mutex);
            bindingVein.HidePopCloseEvent += new BindingVein.HidePopCloseHandler(onHidePopClose);
            bindingVein.UserPwDLoginEvent += new BindingVein.UserPwDLoginHandler(onUserPwDLogin);
            bindingVein.LoadingDataEvent += new BindingVein.LoadingDataHandler(onLoadingData);
            PopFrame.Navigate(bindingVein);
            LogUtils.Debug("EnerBindingVein");
        }

        private void onUserPwDLogin(object sender, User e)
        {
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                onEnterHomePage(e);
                //LoginBkView.Visibility = Visibility.Hidden;
                //PopFrame.Visibility = Visibility.Hidden;

                //ApplicationState.SetUserInfo(e);
                //SetNavBtnVisiblity(e.Role);
                //tbNameText.Text = e.name;
            }));
        }

        private void onEnterSysSetting(object sender, RoutedEventArgs e)
        {
            HomePageView.Visibility = Visibility.Hidden;
            btnBackHP.Visibility = Visibility.Visible;

            SystemSetting systemSetting = new SystemSetting();

            ContentFrame.Navigate(systemSetting);
            //进入系统设置页面，将句柄设置成null，类型设置成other，避免错误调用
            SetSubViewInfo(null, SubViewType.Others);
        }

        /// <summary>
        /// 关门事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterPopClose(object sender, bool e)
        {
            //进入首页
            if (!e)
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    NaviView.Visibility = Visibility.Visible;
                    HomePageView.Visibility = Visibility.Visible;
                    btnBackHP.Visibility = Visibility.Hidden;
                }));
            }
            //弹出退出登录提示框
            else
            {
                CloseCabinet closeCabinet = new CloseCabinet();
                closeCabinet.HidePopCloseEvent += new CloseCabinet.HidePopCloseHandler(onHidePopClose);

                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    onShowPopFrame(closeCabinet);
                    //PopFrame.Visibility = Visibility.Visible;
                    //MaskView.Visibility = Visibility.Visible;
                    //PopFrame.Navigate(closeCabinet);
                }));
            }
        }

        /// <summary>
        /// 操作完成弹出框消失
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onHidePopClose(object sender, RoutedEventArgs e)
        {
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                onReturnToLogin();
//                PopFrame.Visibility = Visibility.Hidden;
//                MaskView.Visibility = Visibility.Hidden;
//                NaviView.Visibility = Visibility.Visible;
//                HomePageView.Visibility = Visibility.Visible;
//                btnBackHP.Visibility = Visibility.Hidden;

//                LoginBkView.Visibility = Visibility.Visible;
//                //回到登陆页
//                SetSubViewInfo(null, SubViewType.Login);

//#if VEINSERIAL
//                vein.ChekVein();
//#else
//                //Task.Factory.StartNew(vein.DetectFinger);
//                ThreadPool.QueueUserWorkItem(new WaitCallback(vein.DetectFinger));
//#endif
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
                onShowPopFrame(inventoryOngoing);
                //PopFrame.Visibility = Visibility.Visible;
                //MaskView.Visibility = Visibility.Visible;
                //PopFrame.Navigate(inventoryOngoing);
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
            onReturnToLogin();
//            //PopFrame.Visibility = Visibility.Hidden;
//            //MaskView.Visibility = Visibility.Hidden;

//            NaviView.Visibility = Visibility.Visible;
//            HomePageView.Visibility = Visibility.Visible;
//            btnBackHP.Visibility = Visibility.Hidden;

//            LoginBkView.Visibility = Visibility.Visible;
//            //回到登陆页
//            SetSubViewInfo(null, SubViewType.Login);

//#if VEINSERIAL
//            vein.ChekVein();
//#else
//            ThreadPool.QueueUserWorkItem(new WaitCallback(vein.DetectFinger));
//            //Task.Factory.StartNew(vein.DetectFinger);
//#endif
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
            //Taskbar.HideTask(true);

            //退出登录，进入首页
            SetSubViewInfo(null,SubViewType.Home);
        }

#region test
        private void MockData()
        {
            //test.InitGoodsInfo();
            //test.InitUsersInfo();
            //test.InitReplenishOrder();
            //test.InitPickingOrder();
            //test.InitSurgerOrder();

#if TESTENV
            TestGoods testGoods = new TestGoods();
            testGoods.GetCurrentRFid();
#endif

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

#region ProcessRing
        /// <summary>
        /// LoadingDataEvent的处理函数，显示或者隐藏精度环
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onLoadingData(object sender, bool e)
        {
            //开启定时器
            if(e)
            {
                processRingTimer.Start();
            }
            else
            {
                processRingTimer.Stop();
            }

            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                LoadingView.Visibility = e ? Visibility.Visible : Visibility.Hidden;
                LoadDataPR.IsActive = e ? true : false;
            }));
        }

        private void onProcessRingTimerExpired(object sender, EventArgs e)
        {
            //定时器超时，隐藏进度环
            onLoadingData(this, false);
            return;
        }
        #endregion

    }

    /// <summary>
    /// 空闲监听定时结构体
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
	struct LASTINPUTINFO
	{
		// 设置结构体块容量
		[MarshalAs(UnmanagedType.U4)]
		public int cbSize;
		// 捕获的时间
		[MarshalAs(UnmanagedType.U4)]
		public uint dwTime;
	}
}
