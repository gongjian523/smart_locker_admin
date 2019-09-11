using CFLMedCab.BLL;
using CFLMedCab.Controls;
using CFLMedCab.Http.Bll;
using CFLMedCab.Http.Enum;
using CFLMedCab.Http.Helper;
using CFLMedCab.Http.Model;
using CFLMedCab.Http.Model.Base;
using CFLMedCab.Http.Model.login;
using CFLMedCab.Http.Model.param;
using CFLMedCab.Infrastructure;
using CFLMedCab.Infrastructure.DeviceHelper;
using CFLMedCab.Infrastructure.ToolHelper;
using CFLMedCab.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace CFLMedCab.View.Common
{
    /// <summary>
    /// CloseCabinet.xaml 的交互逻辑
    /// </summary>
    public partial class BindingVein : UserControl
    {
        //private Timer timer;

        public delegate void HidePopCloseHandler(object sender, RoutedEventArgs e);
        public event HidePopCloseHandler HidePopCloseEvent;

        public delegate void UserPwDLoginHandler(object sender, User e);
        public event UserPwDLoginHandler UserPwDLoginEvent;

        //显示加载数据的进度条
        public delegate void LoadingDataHandler(object sender, bool e);
        public event LoadingDataHandler LoadingDataEvent;

        private string CaptchaToken { get; set; }

        VeinUtils vein = VeinUtils.GetInstance();

        //从主页面传来的互斥锁，用来防止在同时时间调用sdk;
        Mutex mt;

        public BindingVein(Mutex mutex)
        {
            InitializeComponent();
            mt = mutex;

#if TESTENV
            tbInputName.Text = "18628293148";
            tbInputPsw.Password = "cfy12345";
#endif
            //timer = new Timer(1000 * 10 * 60);
            //timer.AutoReset = false;
            //timer.Enabled = true;
            //timer.Elapsed += new ElapsedEventHandler(onHidePopClose);
        }

        /// <summary>
        /// 三秒之后显示提示消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void onHidePopClose(object sender, EventArgs e)
        {
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                HidePopCloseEvent(this, null);
            }));
        }

        public void onExitApp(object sender, EventArgs e)
        {
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                Taskbar.HideTask(false);
                Application.Current.Shutdown();
                //System.Environment.Exit(0);
            }));
        }

        /// <summary>
        /// 绑定指静脉
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void onBindingVein(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() => {
                WarnInfo.Content = "";
            }));

            BaseSinglePostData<UserToken> bdUserToken = GetUserToken(out string warningString, out BitmapImage bi);
            HttpHelper.GetInstance().ResultCheck(bdUserToken, out bool isSuccess);

            if (!isSuccess)
            {
                Dispatcher.BeginInvoke(new Action(() => {

                    WarnInfo.Content = warningString;
                    if (bi != null)
                    {
                        lbInputAuth.Visibility = Visibility.Visible;
                        tbInputAuth.Visibility = Visibility.Visible;
                        imageAuth.Visibility = Visibility.Visible;
                        imageAuth.Source = bi;
                    }
                }));
                return;
            }

            SetTokens(bdUserToken.body.access_token, bdUserToken.body.refresh_token);

            loginView.Visibility = Visibility.Collapsed;
            bindingView.Visibility = Visibility.Visible;
            rebindingBtn.Visibility = Visibility.Hidden;

            Task task =  Task.Factory.StartNew(a => {
                Binding();
            }, true);
        }

        /// <summary>
        /// 绑定指静脉
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void onRebindingVein(object sender, EventArgs e)
        {
            Task.Factory.StartNew(a => {
                Binding();
                }, false);
        }

        /// <summary>
        /// 用户名密码登录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void onPswLogin(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() => {
                WarnInfo.Content = "";
            }));

            BaseSinglePostData<UserToken> bdUserToken = GetUserToken(out string warningString,  out BitmapImage bi);
            HttpHelper.GetInstance().ResultCheck(bdUserToken, out bool isSuccess);

            if (!isSuccess)
            {
                Dispatcher.BeginInvoke(new Action(() => {

                    WarnInfo.Content = warningString;
                    if (bi != null)
                    {
                        lbInputAuth.Visibility = Visibility.Visible;
                        tbInputAuth.Visibility = Visibility.Visible;
                        imageAuth.Visibility = Visibility.Visible;
                        imageAuth.Source = bi;
                    }
                }));
                return;    
            }

            SetTokens(bdUserToken.body.access_token, bdUserToken.body.refresh_token);

            LoadingDataEvent(this, true);
            BaseData<User> bdUser = UserLoginBll.GetInstance().GetUserInfo(("+86 " + tbInputName.Text));
            LoadingDataEvent(this, false);
            HttpHelper.GetInstance().ResultCheck(bdUser, out bool isSuccess2);

            if (!isSuccess2)
            {
                WarnInfo.Content = "获取用户信息失败！";
            }
            else
            {
                UserPwDLoginEvent(this, bdUser.body.objects[0]);
            }

        }

        private void SetTokens(string accessToken, string refreshToken)
        {
            HttpHelper.GetInstance().SetHeaders(accessToken);

            ApplicationState.SetAccessToken(accessToken);
            ApplicationState.SetRefreshToken(refreshToken);
        }

        private BaseSinglePostData<UserToken> GetUserToken(out string warningString, out BitmapImage bi)
        {
            BaseSinglePostData<UserToken> bdUserToken = new BaseSinglePostData<UserToken>();

            if (tbInputName.Text == "" || tbInputPsw.Password.ToString() == "")
            {

                //Dispatcher.BeginInvoke(new Action(() => {
                    if (tbInputName.Text == "")
                        warningString = "请输入用户名！";
                    else
                        warningString = "请输入密码！";
                //}));
                bi = null;
                HttpHelper.GetInstance().ResultCheck(bdUserToken, out bool isSuccess);
                return bdUserToken;
            }

            SignInParam siParam = new SignInParam();
            if (tbInputAuth.Visibility == Visibility.Visible)
            {
                siParam.captcha_token = CaptchaToken;
                siParam.captcha_value = tbInputAuth.Text;
            }
            siParam.password = tbInputPsw.Password.ToString();
            siParam.phone = "+86 " + tbInputName.Text;
            siParam.source = "app";

            //获取用户Token
            LoadingDataEvent(this, true);
            bdUserToken = UserLoginBll.GetInstance().GetUserToken(siParam);
            LoadingDataEvent(this, false);
            //获得用户Token后，返回true， 进入绑定指静脉或者用户名密码登录的下一步
            HttpHelper.GetInstance().ResultCheck(bdUserToken, out bool isSuccess1);
            if (isSuccess1)
            {
                warningString = "";
                bi = null;
                return bdUserToken;
            }


            //获得用户Token失败，首先获取图形验证的Toke
            LoadingDataEvent(this, true);
            var bdCaptchaToken = UserLoginBll.GetInstance().GetCaptchaImageToken();
            LoadingDataEvent(this, false);
            HttpHelper.GetInstance().ResultCheck(bdCaptchaToken, out bool isSuccess2);
            if (!isSuccess2)
            {
                //Dispatcher.BeginInvoke(new Action(() => {
                warningString = "获取验证码Token失败，请重试!";
                //}));
                bi = null;
                bdUserToken.code = bdCaptchaToken.code;
                bdUserToken.message = bdCaptchaToken.message;
                return bdUserToken;
            }

            //赋值给全局变量，下一次使用
            CaptchaToken = bdCaptchaToken.body.captcha_token;

            // 联网异步获取验证码图片
            LoadingDataEvent(this, true);
            //Task<System.Drawing.Image> result = ImageUtils.GetBitmapFromWebServerAsync(url);
            System.Drawing.Image image = ImageUtils.GetBitmapFromWebServer(HttpHelper.GetCaptchaImageUrl() + "?captcha_token=" + bdCaptchaToken.body.captcha_token);
            LoadingDataEvent(this, false);

            //转型：Image --> Bitmap --> BitmapImage
            Bitmap bitmap = new Bitmap(image);
            //BitmapImage bitmapImage = ImageUtils.BitmapToBitmapImage(bitmap);
            bi = ImageUtils.BitmapToBitmapImage(bitmap);

            //Dispatcher.BeginInvoke(new Action(() => {
            //lbInputAuth.Visibility = Visibility.Visible;
            //tbInputAuth.Visibility = Visibility.Visible;
            //imageAuth.Visibility = Visibility.Visible;

            //imageAuth.Source = bitmapImage;
            warningString = "获取账号信息失败，请输入用户信息、密码和验证码，重新尝试!";
            //}));
            bdUserToken.code = (int)ResultCode.Result_Exception;
            bdUserToken.message = ResultCode.Result_Exception.ToString();
            return bdUserToken;
        }
        
        /// <summary>
        /// 绑定流程
        /// </summary>
        /// <param name="bInitial"></param>
        private void Binding(bool bInitial = true)
        {
            int ret;

            mt.WaitOne();

            Dispatcher.BeginInvoke(new Action(() => {
                WarnInfo2.Content = "";
                rebindingBtn.Visibility = Visibility.Hidden;
                bindingExitBtn.Visibility = Visibility.Hidden;
            }));

            if(bInitial)
            {
                ret = vein.LoadingDevice();

                if (ret != 0 && ret != VeinUtils.FV_ERRCODE_EXISTING)
                {
                    LogUtils.Error("初始化指静脉设备失败: " + ret);
                    Dispatcher.BeginInvoke(new Action(() => {
                        WarnInfo2.Content = "初始化指静脉设备失败，请联系工作人员!";
                        rebindingBtn.Visibility = Visibility.Visible;
                        bindingExitBtn.Visibility = Visibility.Visible;
                    }));
                    mt.ReleaseMutex();
                    return;
                }

                //vein.EnumDevice();

                if (ret != VeinUtils.FV_ERRCODE_EXISTING)
                {
                    ret = vein.OpenDevice();
                    if (ret != 0)
                    {
                        LogUtils.Error("无法打开指静脉设备: " + ret);
                        Dispatcher.BeginInvoke(new Action(() => {
                            WarnInfo2.Content = "无法打开指静脉设备，请联系工作人员!";
                            rebindingBtn.Visibility = Visibility.Visible;
                            bindingExitBtn.Visibility = Visibility.Visible;
                        }));
                        mt.ReleaseMutex();
                        return;
                    }
                }
            }

            byte[] regfeature = new byte[VeinUtils.FEATURE_COLLECT_CNT * VeinUtils.FV_FEATURE_SIZE];

            int errCnt = 0;

            for (int i = 0; i < VeinUtils.FEATURE_COLLECT_CNT; )
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    if(errCnt == 0)
                    {
                        GuidInfo.Content = "请将手指放在指静脉模块上，采集第" + (i + 1) + "个指静脉特征";
                    }
                    else
                    {
                        GuidInfo.Content = "请将手指放在指静脉模块上，重新采集第" + (i + 1) + "个指静脉特征";
                    }
                    WarnInfo2.Content = "";
                    }));

                //byte[] subregfeature = new byte[VeinUtils.FV_FEATURE_SIZE];

                ////等待手指放置
                //if (vein.WaitState(0x03, out info) < 0)
                //{
                //    Dispatcher.BeginInvoke(new Action(() =>{
                //        WarnInfo2.Content = info;
                //        rebindingBtn.Visibility = Visibility.Visible;
                //    }));
                //    return;
                //}

                //if (vein.RegisterProcess(i, regfeature, out subregfeature, out info) != 0)
                //{
                //    Dispatcher.BeginInvoke(new Action(() => {
                //        WarnInfo2.Content = info;
                //        rebindingBtn.Visibility = Visibility.Visible;
                //    }));
                //    return;
                //}

                //this.Dispatcher.BeginInvoke(new Action(() => GuidInfo.Content = "请将手指从指静脉模块上移开！"));

                ////等待手指放置
                //if (vein.WaitState(0x00, out info) < 0)
                //{
                //    Dispatcher.BeginInvoke(new Action(() =>
                //    {
                //        WarnInfo2.Content = info;
                //        rebindingBtn.Visibility = Visibility.Visible;
                //    }));
                //    return;
                //}

                if (Sample(i, regfeature, out byte[] subregfeature))
                {
                    Array.Copy(subregfeature, 0, regfeature, i * VeinUtils.FV_FEATURE_SIZE, VeinUtils.FV_FEATURE_SIZE);
                    i++;
                    errCnt = 0;
                }
                else
                {
                    errCnt++;
                    if(errCnt == 3)
                    {
                        Dispatcher.BeginInvoke(new Action(() => {
                            WarnInfo2.Content = "采集指静脉失误3次，请重新开始指静脉绑定流程!";
                            rebindingBtn.Visibility = Visibility.Visible;
                            bindingExitBtn.Visibility = Visibility.Visible;
                        }));

                        mt.ReleaseMutex();
                        return;
                    }
                    else
                    {
                        //睡眠2s后重新开始采集指静脉
                        Thread.Sleep(1000 * 2);
                    }
                }
            }
            mt.ReleaseMutex();



#if LOCALSDK
    
			LoadingDataEvent(this, true);
			BasePostData<string> data = UserLoginBll.GetInstance().VeinmatchBinding(new VeinbindingPostParam
			{
				regfeature = Convert.ToBase64String(regfeature),
				finger_name = "finger1"
			});

			LoadingDataEvent(this, false);

            if (data.code == 0)
            {
                this.Dispatcher.BeginInvoke(new Action(() => {
                    GuidInfo.Content = "指静脉绑定成功！";
                    bindingExitBtn.Visibility = Visibility.Visible;
                }));
            }  
            else
            {
                LogUtils.Error("向主系统绑定指静脉特征失败：" + data.message);
                this.Dispatcher.BeginInvoke(new Action(() => {
                    GuidInfo.Content = "指静脉绑定失败";
                    rebindingBtn.Visibility = Visibility.Visible;
                    bindingExitBtn.Visibility = Visibility.Visible;
                }));
            }                
#else

			LoadingDataEvent(this, true);

			//本地指静脉绑定流程（实际是入库）
			bool isBindingSucess = false;

			if(regfeature != null && regfeature.Length > 0) {
				if (tbInputName.Text != null && tbInputName.Text != "")
				{
					BaseData<User> bdUser = UserLoginBll.GetInstance().GetUserInfo(("+86 " + tbInputName.Text));
					HttpHelper.GetInstance().ResultCheck(bdUser, out bool bdUserIsSucess);

					if (bdUserIsSucess)
					{
						CurrentUser currentUser = new CurrentUser(bdUser.body.objects[0], regfeature);
						new UserBll().InsetUserNotExist(currentUser);
						isBindingSucess = true;
					}

				}
				
			}

			LoadingDataEvent(this, false);

			if (isBindingSucess)
			{
				this.Dispatcher.BeginInvoke(new Action(() => {
					GuidInfo.Content = "指静脉绑定成功！";
					bindingExitBtn.Visibility = Visibility.Visible;
				}));
			}
			else
			{
				LogUtils.Error("本地绑定指静脉特征失败：" );
				this.Dispatcher.BeginInvoke(new Action(() => {
					GuidInfo.Content = "指静脉绑定失败";
					rebindingBtn.Visibility = Visibility.Visible;
					bindingExitBtn.Visibility = Visibility.Visible;
				}));
			}

#endif


		}

		/// <summary>
		/// 单次采集和比对流程
		/// </summary>
		/// <param name="feature_getCnt"></param>
		/// <param name="regfeature"></param>
		/// <param name="subregfeature"></param>
		/// <returns></returns>
		private bool Sample(int feature_getCnt, byte[] regfeature, out byte[] subregfeature)
        {
            //等待手指放置
            if (vein.WaitState(0x03, out string info) < 0)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    WarnInfo2.Content = info;
                }));
                subregfeature = null;
                return false;
            }

            if (vein.RegisterProcess(feature_getCnt, regfeature, out subregfeature, out info) != 0)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    WarnInfo2.Content = info;
                }));
                return false;
            }

            this.Dispatcher.BeginInvoke(new Action(() => GuidInfo.Content = "请将手指从指静脉模块上移开！"));

            //等待手指放置
            if (vein.WaitState(0x00, out info) < 0)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    WarnInfo2.Content = info;
                }));
                return false;
            }

            return true;
        }

        /// <summary>
        /// 聚焦事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TbInputPsw_GotFocus(object sender, RoutedEventArgs e)
		{
			Process.Start(@"C:\Program Files\Common Files\Microsoft Shared\Ink\TabTip.exe");
		}
    }
}
