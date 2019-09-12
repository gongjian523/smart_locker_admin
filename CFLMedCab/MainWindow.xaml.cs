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
using CFLMedCab.View.Allot;
using CFLMedCab.View.Common;

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

        /// <summary>
        /// 货柜开门的个数
        /// </summary>
        private int locOpenNum;

        /// <summary>
        /// 一次完整操作中，所有打开过货位的Rfid串口
        /// </summary>
        private List<string> listOpenLocCom = new List<string>();

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

            DataContext = this;

            processRingTimer = new System.Timers.Timer(1000*60*1);
            processRingTimer.AutoReset = false;
            processRingTimer.Enabled = false;
            processRingTimer.Elapsed += new ElapsedEventHandler(onProcessRingTimerExpired);

            Task.Factory.StartNew(startAutoInventory);
            Task.Factory.StartNew(errorFetchAndInit);

            //Console.ReadKey();

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


#if LOCALSDK

			//执行指静脉相关的逻辑处理
			veinHandleNew();
#else
			//执行指静脉相关的逻辑处理
			veinHandle();

#endif


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

		/// <summary>
		/// 执行指静脉相关的逻辑处理(旧)
		/// </summary>
		private void veinHandle()
		{
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

		}

		/// <summary>
		/// 执行指静脉相关的逻辑处理(新，指静脉本地处理流程)
		/// </summary>
		private void veinHandleNew()
		{
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
						ThreadPool.QueueUserWorkItem(new WaitCallback(detectFinger));
					}
				}
			}

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



#if LOCALSDK
				
                   UserBll userBll = new UserBll();

					List<CurrentUser> userList = userBll.GetAllUsers();

		
					//用来接收找到的用户，如果有的话
					CurrentUser currentUser = null;

					//循环找到是否存在匹配的
					foreach (var itemUser in userList)
					{
						if (itemUser.reg_feature == null)
							continue;

						if (itemUser.ai_feature == null)
							itemUser.ai_feature = itemUser.reg_feature;

						byte[] regfeature = new byte[VeinUtils.FEATURE_COLLECT_CNT * VeinUtils.FV_FEATURE_SIZE];
						regfeature = Convert.FromBase64String(itemUser.reg_feature);

						byte[] aifeature = new byte[VeinUtils.FV_DYNAMIC_FEATURE_CNT * VeinUtils.FV_FEATURE_SIZE];
						aifeature = Convert.FromBase64String(itemUser.ai_feature);

						uint diff = 0;
						uint ailen = VeinUtils.FV_DYNAMIC_FEATURE_CNT * VeinUtils.FV_FEATURE_SIZE;  //输入为动态特征缓冲区大小，输出为动态模板长度

						if (vein.Match(macthfeature, regfeature, VeinUtils.FEATURE_COLLECT_CNT, aifeature, ref diff, (int)VeinUtils.Match_Flg.M_1_1, ref ailen)
							== VeinUtils.FV_ERRCODE_SUCCESS)
						{
							currentUser = itemUser;
							if (ailen > 0)
							{
								//获取ai结果，用于后面的更加智能的匹配结果处理，并入库
								itemUser.ai_feature = Convert.ToBase64String(aifeature);
								userBll.UpdateCurrentUsers(itemUser);
							}
							break;
						}
					}

					//指纹匹配上本地用户
					if (currentUser != null)
					{
						BaseData<User> bdUser = UserLoginBll.GetInstance().GetUserInfo(currentUser.username);
						HttpHelper.GetInstance().ResultCheck(bdUser, out bool isBdUserSuccess);

						//查询到对应线上用户
						if (isBdUserSuccess)
						{

							var currentOnlineUser = bdUser.body.objects[0];

							//签名参数	
							SignInParam siParam = new SignInParam
							{
								//base64解码
								password = BllHelper.DecodeBase64Str(currentOnlineUser.Password),
								//带有+86
								phone = currentOnlineUser.MobilePhone,
								source = "app"
							};


							//获取用户Token
							var bdUserToken = UserLoginBll.GetInstance().GetUserToken(siParam);

							HttpHelper.GetInstance().ResultCheck(bdUserToken, out bool isBdUserTokenSuccess);

							if (isBdUserTokenSuccess)
							{
								//设置token
								HttpHelper.GetInstance().SetHeaders(bdUserToken.body.access_token);
								ApplicationState.SetAccessToken(bdUserToken.body.access_token);
								ApplicationState.SetRefreshToken(bdUserToken.body.refresh_token);
								//设置用户
								user = bdUser.body.objects[0];
							}
							else
							{
								info = "没有找到和当前指静脉匹配的用户";
								info2 = "请先绑定指静脉或者再次尝试";
								LogUtils.Error("没有找到和当前指静脉匹配的用户token：本地线上匹配失败");
							}

						}
						else
						{

							info = "没有找到和当前指静脉匹配的用户";
							info2 = "请先绑定指静脉或者再次尝试";
							LogUtils.Error("没有找到和当前指静脉匹配的用户：本地线上匹配失败");

						}

					}
					else {

						info = "没有找到和当前指静脉匹配的用户";
						info2 = "请先绑定指静脉或者再次尝试";
						LogUtils.Error("没有找到和当前指静脉匹配的用户：本地匹配失败");
					}

#else

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

						//SignInParam siParam = new SignInParam();
						//siParam.password = Convert.FromBase64String(user.Password).ToString();
						//siParam.phone = "+86 " + user.MobilePhone;
						//siParam.source = "app";

						//BaseSinglePostData<UserToken>  bdUserToken = UserLoginBll.GetInstance().GetUserToken(siParam);

						//ApplicationState.SetAccessToken(data.body.accessToken);
						//ApplicationState.SetRefreshToken(data.body.refresh_token);

						//HttpHelper.GetInstance().SetHeaders(data.body.accessToken);

					}
					else
					{
						info = "没有找到和当前指静脉匹配的用户";
						info2 = "请先绑定指静脉或者再次尝试";
						LogUtils.Error("没有找到和当前指静脉匹配的用户：" + data.message);
					}
#endif
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
                }));
            }
            else
            {
                SimulateKeybordInput0();

                // 进入首页
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    onEnterHomePage(user);
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


        private void SetNavBtnVisiblity(string role)
        {
            bool isMedicalStuff = (role == "医院医护人员") ? true : false;

			NavBtnEnterGerFetch.Visibility = isMedicalStuff ? Visibility.Visible : Visibility.Hidden;
            NavBtnEnterSurgery.Visibility = isMedicalStuff ? Visibility.Visible : Visibility.Hidden;
            NavBtnEnterPrescription.Visibility = isMedicalStuff ? Visibility.Visible : Visibility.Hidden;
            NavBtnEnterReturnFetch.Visibility = isMedicalStuff ? Visibility.Visible : Visibility.Hidden;

            NavBtnEnterReplenishment.Visibility = (!isMedicalStuff) ? Visibility.Visible : Visibility.Hidden;
            NavBtnEnterAllotShlef.Visibility = (!isMedicalStuff) ? Visibility.Visible : Visibility.Hidden;
            NavBtnEnterReturnGoods.Visibility = (!isMedicalStuff) ? Visibility.Visible : Visibility.Hidden;
            NavBtnEnterReturnAll.Visibility = (!isMedicalStuff) ? Visibility.Visible : Visibility.Hidden;
            NavBtnEnterStockSwitch.Visibility = (!isMedicalStuff) ? Visibility.Visible : Visibility.Hidden;
            NavBtnEnterInvtory.Visibility = Visibility.Visible;
            NavBtnEnterStock.Visibility = Visibility.Visible;
            NavBtnEnterSysSetting.Visibility = (!isMedicalStuff) ? Visibility.Visible : Visibility.Hidden;

            if(isMedicalStuff)
            {
                NavBtnEnterInvtory.SetValue(Grid.RowProperty,1);
                NavBtnEnterInvtory.SetValue(Grid.ColumnProperty,0);
                NavBtnEnterStock.SetValue(Grid.RowProperty, 1);
                NavBtnEnterStock.SetValue(Grid.ColumnProperty, 1);
            }
            else
            {
                NavBtnEnterInvtory.SetValue(Grid.RowProperty, 1);
                NavBtnEnterInvtory.SetValue(Grid.ColumnProperty, 1);
                NavBtnEnterStock.SetValue(Grid.RowProperty, 1);
                NavBtnEnterStock.SetValue(Grid.ColumnProperty, 2);
            }
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
        /// 设置子页面类型，主要用于区分进入开门页面后，柜门是否开启的场景
        /// </summary>
        /// <param name="type"></param>
        private void SetSubViewType(SubViewType type)
        {
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

            //处于登录页，不用处理：当货柜门打开的时候，不能强制退出登录
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
                case SubViewType.AllotShelfClose:
                    ((AllotShelfClose)subViewHandler).onExitTimerExpired();
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

            //回到登录页
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
            ThreadPool.QueueUserWorkItem(new WaitCallback(detectFinger));
#endif
#endif
        }

        //登录，进入首页
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
            HashSet<CommodityEps> hs = RfidHelper.GetEpcDataJson(out bool isGetSuccess, ApplicationState.GetAllRfidCom());
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

		/// <summary>
		/// 增加的开机故障领用的逻辑，如果有的话
		/// </summary>
        private void errorFetchAndInit()
        {
            #region 处理开机（即应用启动时）需要对比库存变化上传的逻辑

            //获取当前机柜所有商品数据
            HashSet<CommodityEps> currentCommodityEps = RfidHelper.GetEpcDataJson(out bool isGetSuccess, ApplicationState.GetAllRfidCom());

			//判断是否是初次使用本地库存上次，如果是则不上传
			bool isInitLocalCommodityEpsInfo = CommodityCodeBll.GetInstance().isInitLocalCommodityEpsInfo();

            if (isGetSuccess && !isInitLocalCommodityEpsInfo)
            {

                //获取数据库记录的以前所有商品数据
                HashSet<CommodityEps> lastCommodityEps = CommodityCodeBll.GetInstance().GetLastLocalCommodityEpsInfo();

                //比对
                List<CommodityCode> commodityCodeList = CommodityCodeBll.GetInstance().GetCompareSimpleCommodity(lastCommodityEps, currentCommodityEps);

                //有不同的情况，则需要处理上传逻辑
                if (commodityCodeList != null && commodityCodeList.Count > 0)
                {
                    //根据商品码集合获取完整商品属性集合(已对比后结果)
                    var bdCommodityCodeList = CommodityCodeBll.GetInstance().GetCommodityCode(commodityCodeList);

                    var checkbdCommodityCodeList = HttpHelper.GetInstance().ResultCheck(bdCommodityCodeList, out bool isSuccess);

                    if (isSuccess)
                    {
                        //按照类似无单一般领用的方式（故障领用）
                        var bdBasePostData = ConsumingBll.GetInstance().SubmitConsumingChangeWithoutOrder(bdCommodityCodeList, ConsumingOrderType.故障领用);

                        //校验是否含有数据
                        HttpHelper.GetInstance().ResultCheck(bdBasePostData, out bool isSuccess1);
                        if (!isSuccess1)
                        {
                            LogUtils.Error("提交故障领用结果失败！" + bdBasePostData.message);
                        }
                        else
                        {
                            LogUtils.Info("提交故障领用结果成功！" + bdBasePostData.message);
                        }
                    }
                    else
                    {
                        LogUtils.Info("提交故障领用结果成功！");
                    }
                }

            }


			//初始化
			ApplicationState.SetGoodsInfo(currentCommodityEps);
			#endregion
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

            GerFetchState gerFetchState = new GerFetchState(OpenDoorViewType.Fetch);
            gerFetchState.openDoorBtnBoard.OpenDoorEvent += new OpenDoorBtnBoard.OpenDoorHandler(onGerFectchOpenDoorEvent);

            FullFrame.Navigate(gerFetchState);
            //进入一般领用开门页面，类型设置成Others，表明柜门还没有开启
            SetSubViewInfo(gerFetchState, SubViewType.Others);

            locOpenNum = 0;

            //只有从主页进来，才能清空此列表；从关门页面进来，不用
            if ((sender as Control).Name == "NavBtnEnterGerFetch")
            {
                listOpenLocCom.Clear();
            }

            List<Locations> locs = ApplicationState.GetLocations();
            
            //只有一个货位，直接开门
            if (locs.Count == 1)
            {
                onGerFectchOpenDoorEvent(this, locs[0].Code);
            }
        }

        /// <summary>
        /// 一般领用开门事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onGerFectchOpenDoorEvent(object sender, string e)
        {
            //listOpenLocCom记录一次操作中，所有开过门的货柜的串口
            string rfidCom = ApplicationState.GetRfidComByLocCode(e);
            string lockerCom = ApplicationState.GetLockerComByLocCode(e);

            if (rfidCom == "" || lockerCom == "")
            {
                return;
            }

            if(!listOpenLocCom.Contains(rfidCom))
            {
                listOpenLocCom.Add(rfidCom);
            }

#if TESTENV
#else
            LockHelper.DelegateGetMsg delegateGetMsg = LockHelper.GetLockerData(lockerCom, out bool isGetSuccess);
            delegateGetMsg.DelegateGetMsgEvent += new LockHelper.DelegateGetMsg.DelegateGetMsgHandler(onEnterGerFectchLockerEvent);
            delegateGetMsg.userData = e;
#endif
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                if (subViewHandler != null)
                {
                    ((GerFetchState)subViewHandler).onDoorOpen();
                }
            }));

            locOpenNum++;

            SpeakerHelper.Sperker("柜门已开，请拿取您需要的耗材，拿取完毕请关闭柜门");

            //柜门实际打开后，类型设置成DoorOpen
            SetSubViewType(SubViewType.DoorOpen);
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

            locOpenNum--;

            if (locOpenNum > 0)
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    if(subViewHandler != null)
                    {
                        LockHelper.DelegateGetMsg delegateGetMsg = (LockHelper.DelegateGetMsg)sender;
                        ((GerFetchState)subViewHandler).onDoorClosed((string)delegateGetMsg.userData);
                    }
                }));
                return;
            }

            //弹出盘点中弹窗
            EnterInvotoryOngoing();

            HashSet<CommodityEps> hs = RfidHelper.GetEpcDataJson(out bool isGetSuccess,listOpenLocCom);

            //关闭盘点中弹窗
            ClosePop();

            //模拟从键盘输入0，空闲时间重新开始计时
            SimulateKeybordInput0();
            
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                GerFetchView gerFetchView = new GerFetchView(hs, listOpenLocCom);
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
        /// 进入医嘱处方领用-开门状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterSurgeryNoNumOpen(object sender, ConsumingOrder e)
        {
            NaviView.Visibility = Visibility.Hidden;

            GerFetchState gerFetchState = new GerFetchState(OpenDoorViewType.Fetch, e);
            gerFetchState.openDoorBtnBoard.OpenDoorEvent += new OpenDoorBtnBoard.OpenDoorHandler(onSurgeryNoNumOpenDoorEvent);

            FullFrame.Navigate(gerFetchState);
            //进入一般领用开门页面，类型设置成Others，表明柜门还没有开启
            SetSubViewInfo(gerFetchState, SubViewType.Others);

            locOpenNum = 0;

            //只有从主页进来，才能清空此列表；从关门页面进来，不用
            if ((sender as Control).Name != "CtrlSurgeryNoNumClose")
            {
                listOpenLocCom.Clear();
            }

            List<Locations> locs = ApplicationState.GetLocations();

            //只有一个货位，直接开门
            if (locs.Count == 1)
            {
                onSurgeryNoNumOpenDoorEvent(this, locs[0].Code);
            }
        }

        /// <summary>
        /// 手术无单领用和医嘱处方领用的开门事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onSurgeryNoNumOpenDoorEvent(object sender, string e)
        {
            string rfidCom = ApplicationState.GetRfidComByLocCode(e);
            string lockerCom = ApplicationState.GetLockerComByLocCode(e);

            if (rfidCom == "" || lockerCom == "")
            {
                return;
            }

            //listOpenLocCom记录一次操作中，所有开过门的货柜的串口
            if (!listOpenLocCom.Contains(rfidCom))
            {
                listOpenLocCom.Add(rfidCom);
            }

#if TESTENV
#else
            LockHelper.DelegateGetMsg delegateGetMsg = LockHelper.GetLockerData(lockerCom, out bool isGetSuccess);
            delegateGetMsg.DelegateGetMsgEvent += new LockHelper.DelegateGetMsg.DelegateGetMsgHandler(onEnterSurgeryNoNumLockerEvent);
            delegateGetMsg.userData = e;
#endif
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                if (subViewHandler != null)
                {
                    ((GerFetchState)subViewHandler).onDoorOpen();
                }
            }));

            locOpenNum++;

            SpeakerHelper.Sperker("柜门已开，请拿取您需要的耗材，拿取完毕请关闭柜门");

            //柜门实际打开后，类型设置成DoorOpen
            SetSubViewType(SubViewType.DoorOpen);
        }

		/// <summary>
		///进入手术无单领用-开门状态
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void onEnterSurgeryNoNumOpen(object sender, RoutedEventArgs e)
		{
			HomePageView.Visibility = Visibility.Hidden;
			btnBackHP.Visibility = Visibility.Visible;
			NaviView.Visibility = Visibility.Hidden;

			GerFetchState gerFetchState = new GerFetchState(OpenDoorViewType.Fetch);
            gerFetchState.openDoorBtnBoard.OpenDoorEvent += new OpenDoorBtnBoard.OpenDoorHandler(onSurgeryNoNumOpenDoorEvent);

            FullFrame.Navigate(gerFetchState);
			//进入一般领用开门页面，类型设置成Others，表明柜门还没有开启
			SetSubViewInfo(gerFetchState, SubViewType.Others);

			locOpenNum = 0;

			//只有从主页进来，才能清空此列表；从关门页面进来，不用
			if ((sender as Control).Name != "CtrlSurgeryNoNumClose")
			{
				listOpenLocCom.Clear();
			}

			List<Locations> locs = ApplicationState.GetLocations();

			//只有一个货位，直接开门
			if (locs.Count == 1)
			{
				onSurgeryNoNumOpenDoorEvent(this, locs[0].Code);
			}
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

            locOpenNum--;

            if (locOpenNum > 0)
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    if (subViewHandler != null)
                    {
                        LockHelper.DelegateGetMsg delegateGetMsg = (LockHelper.DelegateGetMsg)sender;
                        ((GerFetchState)subViewHandler).onDoorClosed((string)delegateGetMsg.userData);
                    }
                }));

                return;
            }


            //弹出盘点中弹窗
            EnterInvotoryOngoing();

            HashSet<CommodityEps> hs = RfidHelper.GetEpcDataJson(out bool isGetSuccess, listOpenLocCom);

            ConsumingOrder consumingOrder = ((GerFetchState)subViewHandler).GetConsumingOrder();

            //关闭盘点中弹窗
            ClosePop();

            //模拟从键盘输入0，空闲时间重新开始计时
            SimulateKeybordInput0();

            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                SurgeryNoNumClose surgeryNoNumClose;
                if (consumingOrder == null)
                {
                    surgeryNoNumClose  = new SurgeryNoNumClose(hs, listOpenLocCom, ConsumingOrderType.手术领用);
                }
                else
                {
                    surgeryNoNumClose = new SurgeryNoNumClose(hs, listOpenLocCom, ConsumingOrderType.医嘱处方领用, consumingOrder);
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
            if ((sender as Button).Name == "NavBtnEnterSurgery")
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
            surgeryNumOpen.openDoorBtnBoard.OpenDoorEvent += new OpenDoorBtnBoard.OpenDoorHandler(onSurgeryOpenDoorEvent);

            FullFrame.Navigate(surgeryNumOpen);
            //进入手术有单领用开门页面，类型设置成Others，表明柜门还没有开启
            SetSubViewInfo(surgeryNumOpen, SubViewType.Others);

            locOpenNum = 0;

            //只有从手术列表或者详情页面发出的开门事件，才能清空此列表；
            //从关门页面发出的开门事件，不能清除次列表
            if ((sender as Control).Name != "CtrlSurgeryNumClose")
            {
                listOpenLocCom.Clear();
            }

            List<Locations> locs = ApplicationState.GetLocations();

            //只有一个货位，直接开门
            if (locs.Count == 1)
            {
                onSurgeryOpenDoorEvent(this, locs[0].Code);

                OpenCabinet openCabinet = new OpenCabinet();
                openCabinet.HidePopOpenEvent += new OpenCabinet.HidePopOpenHandler(onHidePopOpen);
                onShowPopFrame(openCabinet);
            }
        }

        /// <summary>
        /// 手术有单领用开门事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onSurgeryOpenDoorEvent(object sender, string e)
        {
            string rfidCom = ApplicationState.GetRfidComByLocCode(e);
            string lockerCom = ApplicationState.GetLockerComByLocCode(e);

            if (rfidCom == "" || lockerCom == "")
            {
                return;
            }

            //listOpenLocCom记录一次操作中，所有开过门的货柜的串口
            if (!listOpenLocCom.Contains(rfidCom))
            {
                listOpenLocCom.Add(rfidCom);
            }

#if TESTENV
#else
            LockHelper.DelegateGetMsg delegateGetMsg = LockHelper.GetLockerData(lockerCom, out bool isGetSuccess);
            delegateGetMsg.DelegateGetMsgEvent += new LockHelper.DelegateGetMsg.DelegateGetMsgHandler(onEnterSurgeryNumLockerEvent);
            delegateGetMsg.userData = e;
#endif

            locOpenNum++;

            SpeakerHelper.Sperker("柜门已开，请拿取您需要的耗材，拿取完毕请关闭柜门");

            //柜门实际打开后，类型设置成DoorOpen
            SetSubViewType(SubViewType.DoorOpen);
        }

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

            locOpenNum--;

            if (locOpenNum > 0)
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    if (subViewHandler != null)
                    {
                        LockHelper.DelegateGetMsg delegateGetMsg = (LockHelper.DelegateGetMsg)sender;
                        ((SurgeryNumOpen)subViewHandler).onDoorClosed((string)delegateGetMsg.userData);
                    }
                }));

                return;
            }

            //弹出盘点中弹窗
            EnterInvotoryOngoing();

            HashSet<CommodityEps> ht = RfidHelper.GetEpcDataJson(out bool isGetSuccess, listOpenLocCom);

            //关闭盘点中弹窗
            ClosePop();

            //模拟从键盘输入0，空闲时间重新开始计时
            SimulateKeybordInput0();

            FetchParam fetchParam = ((SurgeryNumOpen)subViewHandler).GetFetchPara();

            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                SurgeryNumClose surgeryNumClose = new SurgeryNumClose(fetchParam, ht, listOpenLocCom);
                surgeryNumClose.EnterPopCloseEvent += new SurgeryNumClose.EnterPopCloseHandler(onEnterPopClose);
                surgeryNumClose.EnterSurgeryNumOpenEvent += new SurgeryNumClose.EnterSurgeryNumOpenHandler(EnterSurgeryNumOpenEvent);
                surgeryNumClose.LoadingDataEvent += new SurgeryNumClose.LoadingDataHandler(onLoadingData);
                FullFrame.Navigate(surgeryNumClose);
                //进入手术有单领用关门页面，
                SetSubViewInfo(surgeryNumClose, SubViewType.SurFetchWOrderClose);
            }));
        }
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

            GerFetchState gerFetchState = new GerFetchState(OpenDoorViewType.FetchReturn);
            gerFetchState.openDoorBtnBoard.OpenDoorEvent += new OpenDoorBtnBoard.OpenDoorHandler(onReturnFetchOpenDoorEvent);

            FullFrame.Navigate(gerFetchState);

            //进入领用退回开门页面，类型设置成Others，表明柜门还没有开启
            SetSubViewInfo(gerFetchState, SubViewType.Others);

            locOpenNum = 0;

            //只有从主页进来，才能清空此列表；从关门页面进来，不用
            if ((sender as Control).Name == "NavBtnEnterReturnFetch")
            {
                listOpenLocCom.Clear();
            }

            List<Locations> locs = ApplicationState.GetLocations();

            //只有一个货位，直接开门
            if (locs.Count == 1)
            {
                onReturnFetchOpenDoorEvent(this, locs[0].Code);
            }
        }

        /// <summary>
        /// 领用回退开门事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onReturnFetchOpenDoorEvent(object sender, string e)
        {
            //listOpenLocCom记录一次操作中，所有开过门的货柜的串口
            string rfidCom = ApplicationState.GetRfidComByLocCode(e);
            string lockerCom = ApplicationState.GetLockerComByLocCode(e);

            if (rfidCom == "" || lockerCom == "")
            {
                return;
            }

            if (!listOpenLocCom.Contains(rfidCom))
            {
                listOpenLocCom.Add(rfidCom);
            }

#if TESTENV
#else
            LockHelper.DelegateGetMsg delegateGetMsg = LockHelper.GetLockerData(lockerCom, out bool isGetSuccess);
            delegateGetMsg.DelegateGetMsgEvent += new LockHelper.DelegateGetMsg.DelegateGetMsgHandler(onEnterReturnFetchLockerEvent);
            delegateGetMsg.userData = e;
#endif
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                if (subViewHandler != null)
                {
                    ((GerFetchState)subViewHandler).onDoorOpen();
                }
            }));

            locOpenNum++;

            SpeakerHelper.Sperker("柜门已开，请拿取您需要的耗材，拿取完毕请关闭柜门");

            //柜门实际打开后，类型设置成DoorOpen
            SetSubViewType(SubViewType.DoorOpen);
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

            locOpenNum--;

            if (locOpenNum > 0)
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    LockHelper.DelegateGetMsg delegateGetMsg = (LockHelper.DelegateGetMsg)sender;
                    ((GerFetchState)subViewHandler).onDoorClosed((string)delegateGetMsg.userData);
                }));

                return;
            }

            //弹出盘点中弹窗
            EnterInvotoryOngoing();

            HashSet<CommodityEps> hs = RfidHelper.GetEpcDataJson(out bool isGetSuccess, listOpenLocCom);

            //关闭盘点中弹窗
            ClosePop();

            //模拟从键盘输入0，空闲时间重新开始计时
            SimulateKeybordInput0();

            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                ReturnFetchView returnFetchView = new ReturnFetchView(hs, listOpenLocCom);
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
            replenishmentDetailOpen.openDoorBtnBoard.OpenDoorEvent += new OpenDoorBtnBoard.OpenDoorHandler(onReplenishmentOpenDoorEvent);

            FullFrame.Navigate(replenishmentDetailOpen);
            //进入上架任务单详情开门页面，类型设置成Others，表明柜门还没有开启
            SetSubViewInfo(replenishmentDetailOpen, SubViewType.Others);

            locOpenNum = 0;

            //只有从上架列表或者详情页面发出的开门事件，才能清空此列表；
            //从关门页面发出的开门事件，不能清除此列表
            if ((sender as Control).Name != "CtrlReplenishmentClose")
            {
                listOpenLocCom.Clear();
            }

            List<Locations> locs = ApplicationState.GetLocations();

            //只有一个货位，直接开门
            if (locs.Count == 1)
            {
                onReplenishmentOpenDoorEvent(this, locs[0].Code);

                OpenCabinet openCabinet = new OpenCabinet();
                openCabinet.HidePopOpenEvent += new OpenCabinet.HidePopOpenHandler(onHidePopOpen);
                onShowPopFrame(openCabinet);
            }
        }

        /// <summary>
        /// 上架开门事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onReplenishmentOpenDoorEvent(object sender, string e)
        {
            string rfidCom = ApplicationState.GetRfidComByLocCode(e);
            string lockerCom = ApplicationState.GetLockerComByLocCode(e);

            if (rfidCom == "" || lockerCom == "")
            {
                return;
            }

            //listOpenLocCom记录一次操作中，所有开过门的货柜的串口
            if (!listOpenLocCom.Contains(rfidCom))
            {
                listOpenLocCom.Add(rfidCom);
            }

#if TESTENV
#else
            LockHelper.DelegateGetMsg delegateGetMsg = LockHelper.GetLockerData(lockerCom, out bool isGetSuccess);
            delegateGetMsg.DelegateGetMsgEvent += new LockHelper.DelegateGetMsg.DelegateGetMsgHandler(onEnterReplenishmentCloseEvent);
            delegateGetMsg.userData = e;
#endif

            locOpenNum++;

            SpeakerHelper.Sperker("柜门已开，请拿取您需要的耗材，拿取完毕请关闭柜门");

            //柜门实际打开后，类型设置成DoorOpen
            SetSubViewType(SubViewType.DoorOpen);
        }

        /// <summary>
        /// 进入上架单详情页-关门状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterReplenishmentCloseEvent(object sender, bool isClose)
        {
            LogUtils.Debug($"返回开锁状态{isClose}");

            if (!isClose)
                return;

            locOpenNum--;

            if (locOpenNum > 0)
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    if (subViewHandler != null)
                    {
                        LockHelper.DelegateGetMsg delegateGetMsg = (LockHelper.DelegateGetMsg)sender;
                        ((ReplenishmentDetailOpen)subViewHandler).onDoorClosed((string)delegateGetMsg.userData);
                    }
                }));

                return;
            }

            //弹出盘点中弹窗
            EnterInvotoryOngoing();

            HashSet<CommodityEps> hs = RfidHelper.GetEpcDataJson(out bool isGetSuccess, listOpenLocCom);

            //关闭盘点中弹窗
            ClosePop();

            //模拟从键盘输入0，空闲时间重新开始计时
            SimulateKeybordInput0();

            ShelfTask shelfTask = ((ReplenishmentDetailOpen)subViewHandler).GetShelfTask();

            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                ReplenishmentClose replenishmentClose = new ReplenishmentClose(shelfTask, hs, listOpenLocCom);
                replenishmentClose.EnterReplenishmentDetailOpenEvent += new ReplenishmentClose.EnterReplenishmentDetailOpenHandler(onEnterReplenishmentDetailOpen);
                replenishmentClose.EnterPopCloseEvent += new ReplenishmentClose.EnterPopCloseHandler(onEnterPopClose);
                replenishmentClose.LoadingDataEvent += new ReplenishmentClose.LoadingDataHandler(onLoadingData);

                FullFrame.Navigate(replenishmentClose);
                //进入上架任务单关门页面
                SetSubViewInfo(replenishmentClose, SubViewType.ReplenishmentClose);
            }));
        }

        #endregion

        #region AllotShelf
        /// <summary>
        /// 进入调拨上架单列表页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterAllotShelfView(object sender, RoutedEventArgs e)
        {
            HomePageView.Visibility = Visibility.Hidden;
            btnBackHP.Visibility = Visibility.Visible;

            AllotShelfView allotShelfView = new AllotShelfView();
            allotShelfView.EnterAllotShelfDetailEvent += new AllotShelfView.EnterAllotShelfDetailHandler(onEnterAllotShelfDetail);
            allotShelfView.EnterAllotShelfDetailOpenEvent += new AllotShelfView.EnterAllotShelfDetailOpenHandler(onEnterAllotShelfDetailOpen);
            allotShelfView.LoadingDataEvent += new AllotShelfView.LoadingDataHandler(onLoadingData);

            ContentFrame.Navigate(allotShelfView);
            //进入上架页面，将句柄设置成null，类型设置成other，避免错误调用
            SetSubViewInfo(null, SubViewType.Others);
        }

        /// <summary>
        /// 进入调拨上架单详情页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterAllotShelfDetail(object sender, AllotShelf e)
        {
            AllotShelfDetail allotShelfDetail = new AllotShelfDetail(e);
            allotShelfDetail.EnterAllotShelfDetailOpenEvent += new AllotShelfDetail.EnterAllotShelfDetailOpenHandler(onEnterAllotShelfDetailOpen);
            allotShelfDetail.EnterAllotShelfViewEvent += new AllotShelfDetail.EnterAllotShelfViewHandler(onEnterAllotShelfView);
            allotShelfDetail.LoadingDataEvent += new AllotShelfDetail.LoadingDataHandler(onLoadingData);

            ContentFrame.Navigate(allotShelfDetail);
            //进入上架任务单详情页面，将句柄设置成null，类型设置成other，避免错误调用
            SetSubViewInfo(null, SubViewType.Others);
        }

        /// <summary>
        /// 进入上架单详情页-开门状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterAllotShelfDetailOpen(object sender, AllotShelf e)
        {
            NaviView.Visibility = Visibility.Hidden;

            AllotShelfDetailOpen allotShelfDetailOpen = new AllotShelfDetailOpen(e);
            allotShelfDetailOpen.LoadingDataEvent += new AllotShelfDetailOpen.LoadingDataHandler(onLoadingData);
            allotShelfDetailOpen.openDoorBtnBoard.OpenDoorEvent += new OpenDoorBtnBoard.OpenDoorHandler(onAllotShelfOpenDoorEvent);

            FullFrame.Navigate(allotShelfDetailOpen);
            //进入调拨上架开门页面，类型设置成Others，表明柜门还没有开启
            SetSubViewInfo(allotShelfDetailOpen, SubViewType.Others);

            locOpenNum = 0;

            //只有从上架列表或者详情页面发出的开门事件，才能清空此列表；
            //从关门页面发出的开门事件，不能清除此列表
            if ((sender as Control).Name != "CtrlAllotShelfClose")
            {
                listOpenLocCom.Clear();
            }

            List<Locations> locs = ApplicationState.GetLocations();

            //只有一个货位，直接开门
            if (locs.Count == 1)
            {
                onAllotShelfOpenDoorEvent(this, locs[0].Code);

                OpenCabinet openCabinet = new OpenCabinet();
                openCabinet.HidePopOpenEvent += new OpenCabinet.HidePopOpenHandler(onHidePopOpen);
                onShowPopFrame(openCabinet);
            }
        }

        /// <summary>
        /// 调拨上架开门事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onAllotShelfOpenDoorEvent(object sender, string e)
        {
            string rfidCom = ApplicationState.GetRfidComByLocCode(e);
            string lockerCom = ApplicationState.GetLockerComByLocCode(e);

            if (rfidCom == "" || lockerCom == "")
            {
                return;
            }

            //listOpenLocCom记录一次操作中，所有开过门的货柜的串口
            if (!listOpenLocCom.Contains(rfidCom))
            {
                listOpenLocCom.Add(rfidCom);
            }

#if TESTENV
#else
            LockHelper.DelegateGetMsg delegateGetMsg = LockHelper.GetLockerData(lockerCom, out bool isGetSuccess);
            delegateGetMsg.DelegateGetMsgEvent += new LockHelper.DelegateGetMsg.DelegateGetMsgHandler(onEnterAllotShelfCloseEvent);
            delegateGetMsg.userData = e;
#endif

            locOpenNum++;

            SpeakerHelper.Sperker("柜门已开，请拿取您需要的耗材，拿取完毕请关闭柜门");

            //柜门实际打开后，类型设置成DoorOpen
            SetSubViewType(SubViewType.DoorOpen);
        }

        /// <summary>
        /// 进入上架单详情页-关门状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterAllotShelfCloseEvent(object sender, bool isClose)
        {
            LogUtils.Debug($"返回开锁状态{isClose}");

            if (!isClose)
                return;

            locOpenNum--;

            if (locOpenNum > 0)
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    if (subViewHandler != null)
                    {
                        LockHelper.DelegateGetMsg delegateGetMsg = (LockHelper.DelegateGetMsg)sender;
                        ((AllotShelfDetailOpen)subViewHandler).onDoorClosed((string)delegateGetMsg.userData);
                    }
                }));

                return;
            }

            //弹出盘点中弹窗
            EnterInvotoryOngoing();

            HashSet<CommodityEps> hs = RfidHelper.GetEpcDataJson(out bool isGetSuccess, listOpenLocCom);

            //关闭盘点中弹窗
            ClosePop();

            //模拟从键盘输入0，空闲时间重新开始计时
            SimulateKeybordInput0();

            AllotShelf allotShelf = ((AllotShelfDetailOpen)subViewHandler).GetShelfTask();

            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                AllotShelfClose allotShelfClose = new AllotShelfClose(allotShelf, hs, listOpenLocCom);
                allotShelfClose.EnterAllotShelfDetailOpenEvent += new AllotShelfClose.EnterAllotShelfDetailOpenHandler(onEnterAllotShelfDetailOpen);
                allotShelfClose.EnterPopCloseEvent += new AllotShelfClose.EnterPopCloseHandler(onEnterPopClose);
                allotShelfClose.LoadingDataEvent += new AllotShelfClose.LoadingDataHandler(onLoadingData);

                FullFrame.Navigate(allotShelfClose);
                //进入调拨上架关门页面
                SetSubViewInfo(allotShelfClose, SubViewType.AllotShelfClose);
            }));
        }

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
            returnGoodsDetailOpen.openDoorBtnBoard.OpenDoorEvent += new OpenDoorBtnBoard.OpenDoorHandler(onReturnGoodsOpenDoorEvent);

            FullFrame.Navigate(returnGoodsDetailOpen);
            //进入拣货任务单详情页面，类型设置成Others，表明柜门还没有开启
            SetSubViewInfo(returnGoodsDetailOpen, SubViewType.Others);

            locOpenNum = 0;

            //只有从拣货列表或者详情页面发出的开门事件，才能清空此列表；
            //从关门页面发出的开门事件，不能清除此列表
            if ((sender as Control).Name != "CtrlReturnGoodsClose")
            {
                listOpenLocCom.Clear();
            }

            List<Locations> locs = ApplicationState.GetLocations();

            //只有一个货位，直接开门
            if (locs.Count == 1)
            {
                onReturnGoodsOpenDoorEvent(this, locs[0].Code);

                OpenCabinet openCabinet = new OpenCabinet();
                openCabinet.HidePopOpenEvent += new OpenCabinet.HidePopOpenHandler(onHidePopOpen);
                onShowPopFrame(openCabinet);
            }

        }

        /// <summary>
        /// 拣货开门事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onReturnGoodsOpenDoorEvent(object sender, string e)
        {
            string rfidCom = ApplicationState.GetRfidComByLocCode(e);
            string lockerCom = ApplicationState.GetLockerComByLocCode(e);

            if (rfidCom == "" || lockerCom == "")
            {
                return;
            }

            //listOpenLocCom记录一次操作中，所有开过门的货柜的串口
            if (!listOpenLocCom.Contains(rfidCom))
            {
                listOpenLocCom.Add(rfidCom);
            }

#if TESTENV
#else
            LockHelper.DelegateGetMsg delegateGetMsg = LockHelper.GetLockerData(lockerCom, out bool isGetSuccess);
            delegateGetMsg.DelegateGetMsgEvent += new LockHelper.DelegateGetMsg.DelegateGetMsgHandler(onEnterReplenishmentCloseEvent);
            delegateGetMsg.userData = e;
#endif

            locOpenNum++;

            SpeakerHelper.Sperker("柜门已开，请拿取您需要的耗材，拿取完毕请关闭柜门");

            //柜门实际打开后，类型设置成DoorOpen
            SetSubViewType(SubViewType.DoorOpen);
        }



        /// <summary>
        /// 进入拣货任务单详情页-关门状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterReturnGoodsCloseEvent(object sender, bool isClose)
        {
            LogUtils.Debug($"返回开锁状态{isClose}");

            if (!isClose)
                return;

            locOpenNum--;

            if (locOpenNum > 0)
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    if (subViewHandler != null)
                    {
                        LockHelper.DelegateGetMsg delegateGetMsg = (LockHelper.DelegateGetMsg)sender;
                        ((ReturnGoodsDetailOpen)subViewHandler).onDoorClosed((string)delegateGetMsg.userData);
                    }
                }));

                return;
            }

            //弹出盘点中弹窗
            EnterInvotoryOngoing();

            HashSet<CommodityEps> hs = RfidHelper.GetEpcDataJson(out bool isGetSuccess, listOpenLocCom);

            //关闭盘点中弹窗
            ClosePop();

            //模拟从键盘输入0，空闲时间重新开始计时
            SimulateKeybordInput0();

            PickTask pickTask = ((ReturnGoodsDetailOpen)subViewHandler).GetPickTask();

            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                ReturnGoodsClose returnGoodsClose = new ReturnGoodsClose(pickTask, hs, listOpenLocCom);
                returnGoodsClose.EnterReturnGoodsDetailOpenEvent += new ReturnGoodsClose.EnterReturnGoodsDetailOpenHandler(onEnterReturnGoodsDetailOpen);
                returnGoodsClose.EnterPopCloseEvent += new ReturnGoodsClose.EnterPopCloseHandler(onEnterPopClose);
                returnGoodsClose.LoadingDataEvent += new ReturnGoodsClose.LoadingDataHandler(onLoadingData);

                FullFrame.Navigate(returnGoodsClose);
                //进入拣货任务单详情页面
                SetSubViewInfo(returnGoodsClose, SubViewType.ReturnGoodsClose);
            }));
        }

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

            GerFetchState gerFetchState = new GerFetchState(OpenDoorViewType.Fetch,null,e);
            gerFetchState.openDoorBtnBoard.OpenDoorEvent += new OpenDoorBtnBoard.OpenDoorHandler(onReturnOpenDoorEvent);

            FullFrame.Navigate(gerFetchState);
            //进入回收取货开门页面，类型设置成Others，表明柜门还没有开启
            SetSubViewInfo(gerFetchState, SubViewType.Others);

            //只有从回收取货查询页面发出的开门事件，才能清空此列表；
            //从关门页面发出的开门事件，不能清除此列表
            if ((sender as Control).Name != "CtrlReturnClose")
            {
                listOpenLocCom.Clear();
            }

            List<Locations> locs = ApplicationState.GetLocations();

            //只有一个货位，直接开门
            if (locs.Count == 1)
            {
                onReturnOpenDoorEvent(this, locs[0].Code);
            }
        }

        /// <summary>
        /// 回收取货开门事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onReturnOpenDoorEvent(object sender, string e)
        {
            //listOpenLocCom记录一次操作中，所有开过门的货柜的串口
            string rfidCom = ApplicationState.GetRfidComByLocCode(e);
            string lockerCom = ApplicationState.GetLockerComByLocCode(e);

            if (rfidCom == "" || lockerCom == "")
            {
                return;
            }

            if (!listOpenLocCom.Contains(rfidCom))
            {
                listOpenLocCom.Add(rfidCom);
            }

#if TESTENV
#else
            LockHelper.DelegateGetMsg delegateGetMsg = LockHelper.GetLockerData(lockerCom, out bool isGetSuccess);
            delegateGetMsg.DelegateGetMsgEvent += new LockHelper.DelegateGetMsg.DelegateGetMsgHandler(onEnterGerFectchLockerEvent);
            delegateGetMsg.userData = e;
#endif
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                if (subViewHandler != null)
                {
                    ((GerFetchState)subViewHandler).onDoorOpen();
                }
            }));

            locOpenNum++;

            SpeakerHelper.Sperker("柜门已开，请拿取您需要的耗材，拿取完毕请关闭柜门");

            //柜门实际打开后，类型设置成DoorOpen
            SetSubViewType(SubViewType.DoorOpen);
        }

        /// <summary>
        /// 进入回收取货-关门状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterReturnClose(object sender, bool isClose)
        {
            LogUtils.Debug($"返回开锁状态{isClose}");

            if (!isClose)
                return;

            locOpenNum--;

            if (locOpenNum > 0)
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    if (subViewHandler != null)
                    {
                        LockHelper.DelegateGetMsg delegateGetMsg = (LockHelper.DelegateGetMsg)sender;
                        ((GerFetchState)subViewHandler).onDoorClosed((string)delegateGetMsg.userData);
                    }
                }));
                return;
            }


            //弹出盘点中弹窗
            EnterInvotoryOngoing();

            HashSet<CommodityEps> hs = RfidHelper.GetEpcDataJson(out bool isGetSuccess, listOpenLocCom);

            CommodityRecovery commodityRecovery = ((GerFetchState)subViewHandler).GetCommodityRecovery();

            //关闭盘点中弹窗
            ClosePop();

            //模拟从键盘输入0，空闲时间重新开始计时
            SimulateKeybordInput0();

            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                ReturnClose returnClose = new ReturnClose(hs, listOpenLocCom, commodityRecovery);
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

            GerFetchState gerFetchState = new GerFetchState(OpenDoorViewType.StockSwitch);
            gerFetchState.openDoorBtnBoard.OpenDoorEvent += new OpenDoorBtnBoard.OpenDoorHandler(onStockSwitchOpenDoorEvent);

            FullFrame.Navigate(gerFetchState);
            //进入库存调整开门页面，类型设置成Others，表明柜门还没有开启
            SetSubViewInfo(gerFetchState, SubViewType.Others);

            locOpenNum = 0;

            //只有从主页进来，才能清空此列表；从关门页面进来，不用
            if ((sender as Control).Name == "NavBtnEnterStockSwitch")
            {
                listOpenLocCom.Clear();
            }

            List<Locations> locs = ApplicationState.GetLocations();

            //只有一个货位，直接开门
            if (locs.Count == 1)
            {
                onStockSwitchOpenDoorEvent(this, locs[0].Code);
            }
        }

        /// <summary>
        /// 库存调整开门事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onStockSwitchOpenDoorEvent(object sender, string e)
        {
            //listOpenLocCom记录一次操作中，所有开过门的货柜的串口
            string rfidCom = ApplicationState.GetRfidComByLocCode(e);
            string lockerCom = ApplicationState.GetLockerComByLocCode(e);

            if (rfidCom == "" || lockerCom == "")
            {
                return;
            }

            if (!listOpenLocCom.Contains(rfidCom))
            {
                listOpenLocCom.Add(rfidCom);
            }

#if TESTENV
#else
            LockHelper.DelegateGetMsg delegateGetMsg = LockHelper.GetLockerData(lockerCom, out bool isGetSuccess);
            delegateGetMsg.DelegateGetMsgEvent += new LockHelper.DelegateGetMsg.DelegateGetMsgHandler(onEnterGerFectchLockerEvent);
            delegateGetMsg.userData = e;
#endif
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                if (subViewHandler != null)
                {
                    ((GerFetchState)subViewHandler).onDoorOpen();
                }
            }));

            locOpenNum++;

            SpeakerHelper.Sperker("柜门已开，请拿取您需要的耗材，拿取完毕请关闭柜门");

            //柜门实际打开后，类型设置成DoorOpen
            SetSubViewType(SubViewType.DoorOpen);
        }


        /// <summary>
        /// 进入进入库存调整-关门状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterStockSwitchClose(object sender, bool isClose)
        {
            LogUtils.Debug($"返回开锁状态{isClose}");

            if (!isClose)
                return;

            locOpenNum--;

            if (locOpenNum > 0)
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    if (subViewHandler != null)
                    {
                        LockHelper.DelegateGetMsg delegateGetMsg = (LockHelper.DelegateGetMsg)sender;
                        ((GerFetchState)subViewHandler).onDoorClosed((string)delegateGetMsg.userData);
                    }
                }));
                return;
            }

            //弹出盘点中弹窗
            EnterInvotoryOngoing();

            HashSet<CommodityEps> hs = RfidHelper.GetEpcDataJson(out bool isGetSuccess, listOpenLocCom);

            //关闭盘点中弹窗
            ClosePop();

            //模拟从键盘输入0，空闲时间重新开始计时
            SimulateKeybordInput0();

            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                ReturnClose returnClose = new ReturnClose(hs, listOpenLocCom,null);
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
        private void onEnterInventoryDetail(object sender, List<InventoryOrder> e)
        {
            HomePageView.Visibility = Visibility.Hidden;
            NaviView.Visibility = Visibility.Hidden;

            InventoryDtl inventoryDetail = new InventoryDtl(e);

            inventoryDetail.SetPopInventoryEvent += new InventoryDtl.SetPopInventoryHandler(onSetPopInventory);
            inventoryDetail.BackInventoryEvent += new InventoryDtl.BackInventoryHandler(onBackInventory);
            inventoryDetail.LoadingDataEvent += new InventoryDtl.LoadingDataHandler(onLoadingData);
            inventoryDetail.openDoorBtnBoard.OpenDoorEvent += new OpenDoorBtnBoard.OpenDoorHandler(onInventoryDoorOpenEvent);

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
        private void onInventoryDoorOpenEvent(object sender, string e)
        {

#if TESTENV
            testTimer = new System.Timers.Timer(3000);
            testTimer.AutoReset = false;
            testTimer.Enabled = true;
            testTimer.Elapsed += new ElapsedEventHandler(onInventoryDoorCloseTest);
#else
            string rfidCom = ApplicationState.GetRfidComByLocCode(e);
            string lockerCom = ApplicationState.GetLockerComByLocCode(e);

            if (rfidCom == "" || lockerCom == "")
            {
                return;
            }

            //listOpenLocCom记录一次操作中，所有开过门的货柜的串口
            if (!listOpenLocCom.Contains(rfidCom))
            {
                listOpenLocCom.Add(rfidCom);
            }

#if TESTENV
#else
            LockHelper.DelegateGetMsg delegateGetMsg = LockHelper.GetLockerData(lockerCom, out bool isGetSuccess);
            delegateGetMsg.DelegateGetMsgEvent += new LockHelper.DelegateGetMsg.DelegateGetMsgHandler(onInventoryDoorClose);
            delegateGetMsg.userData = e;
#endif

            locOpenNum++;

            SpeakerHelper.Sperker("柜门已开，请检查相关耗材，检查完毕请关闭柜门");

            ((InventoryDtl)subViewHandler).SetButtonVisibility(false);

            //盘点开门的时候，将状态设置成DoorOpen
            SetSubViewType(SubViewType.DoorOpen);
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
            LogUtils.Debug($"返回开锁状态{isClose}");

            if (!isClose)
                return;

            locOpenNum--;

            if (locOpenNum > 0)
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    if (subViewHandler != null)
                    {
                        LockHelper.DelegateGetMsg delegateGetMsg = (LockHelper.DelegateGetMsg)sender;
                        ((GerFetchState)subViewHandler).onDoorClosed((string)delegateGetMsg.userData);
                    }
                }));
                return;
            }

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
        /// 弹出退出登录提示框
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
                
                SetSubViewInfo(null, SubViewType.Home);
            }
            //弹出退出登录提示框
            else
            {
                CloseCabinet closeCabinet = new CloseCabinet();
                closeCabinet.HidePopCloseEvent += new CloseCabinet.HidePopCloseHandler(onHidePopClose);

                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    onShowPopFrame(closeCabinet);
                }));
                SetSubViewInfo(null, SubViewType.Login);
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
