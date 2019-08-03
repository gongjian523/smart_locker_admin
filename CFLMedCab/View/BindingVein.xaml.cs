using CFLMedCab.BLL;
using CFLMedCab.Controls;
using CFLMedCab.Http.Bll;
using CFLMedCab.Http.Helper;
using CFLMedCab.Http.Model;
using CFLMedCab.Http.Model.Base;
using CFLMedCab.Http.Model.param;
using CFLMedCab.Infrastructure;
using CFLMedCab.Infrastructure.DeviceHelper;
using CFLMedCab.Infrastructure.ToolHelper;
using CFLMedCab.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace CFLMedCab.View
{
    /// <summary>
    /// CloseCabinet.xaml 的交互逻辑
    /// </summary>
    public partial class BindingVein : UserControl
    {
        private Timer timer;

        public delegate void HidePopCloseHandler(object sender, RoutedEventArgs e);
        public event HidePopCloseHandler HidePopCloseEvent;

        public delegate void UserPwDLoginHandler(object sender, User e);
        public event UserPwDLoginHandler UserPwDLoginEvent;

        private string CaptchaToken { get; set; }

#if LOCALSDK
        private UserBll userBll = new UserBll();
        private CurrentUser user = new CurrentUser();
#endif

        VeinUtils vein = VeinUtils.GetInstance();

        public BindingVein()
        {
            InitializeComponent();

            timer = new Timer(1000 * 10 * 60);
            timer.AutoReset = false;
            timer.Enabled = true;
            timer.Elapsed += new ElapsedEventHandler(onHidePopClose);
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
                System.Environment.Exit(0);
            }));
        }

        public void onBindingVein(object sender, EventArgs e)
        {
            bool bBinding  = ((Button)sender).Name == "btnBinding" ? true : false;

            Dispatcher.BeginInvoke(new Action(() => {
                WarnInfo.Content = "";
            }));

#if LOCALSDK
            user = userBll.GetUserByName(tbInputName.Text);

            if (user == null)
            {
                WarnInfo.Content = "无法获取用户信息，请重新输入！";
                return;
            }
#else

            if (tbInputName.Text == "" || tbInputPsw.Password.ToString() == "")
            {
                Dispatcher.BeginInvoke(new Action(() => {
                    if(tbInputName.Text == "")
                        WarnInfo.Content = "请输入用户名！";
                    else
                        WarnInfo.Content = "请输入密码！";
                }));
                return;
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

            var data1 = UserLoginBll.GetInstance().GetUserToken(siParam);

            if(data1.code != 0)
            {
                var data = UserLoginBll.GetInstance().GetCaptchaImageToken();

                if(data.code != 0)
                {
                    Dispatcher.BeginInvoke(new Action(() => {
                        WarnInfo.Content = "获取账号失败，请再次输入用户信息、密码以及验证，并重新" + (bBinding?"绑定" :"登录") + "!";
                    }));
                    return;
                }

                CaptchaToken = data.body.captcha_token;
                // 4、再联网异步获取头像图片
                //Task<System.Drawing.Image> result = ImageUtils.GetBitmapFromWebServerAsync(url);
                System.Drawing.Image image = ImageUtils.GetBitmapFromWebServer(HttpHelper.GetCaptchaImageUrl() + "?captcha_token=" + data.body.captcha_token);

                // 5、转型：Image --> Bitmap --> BitmapImage
                Bitmap bitmap = new Bitmap(image);
                BitmapImage bitmapImage = ImageUtils.BitmapToBitmapImage(bitmap);

                Dispatcher.BeginInvoke(new Action(() => {
                    lbInputAuth.Visibility = Visibility.Visible;
                    tbInputAuth.Visibility = Visibility.Visible;
                    imageAuth.Visibility = Visibility.Visible;

                    imageAuth.Source = bitmapImage;
                    WarnInfo.Content = "获取账号失败，请再次输入用户信息、密码以及验证，并重新" + (bBinding ? "绑定" : "登录") + "!";
                }));
            }
            else
            {

                if(bBinding)
                {
                    loginView.Visibility = Visibility.Collapsed;
                    BindingView.Visibility = Visibility.Visible;

                    rebindingBtn.Visibility = Visibility.Hidden;

                    HttpHelper.GetInstance().SetHeaders(data1.body.access_token);

                    ApplicationState.SetAccessToken(data1.body.access_token);
                    ApplicationState.SetRefreshToken(data1.body.refresh_token);

                    Task.Factory.StartNew(Binding);
                }
                else
                {
                    HttpHelper.GetInstance().SetHeaders(data1.body.access_token);

                    ApplicationState.SetAccessToken(data1.body.access_token);
                    ApplicationState.SetRefreshToken(data1.body.refresh_token);

                    BaseData<User> bdData = UserLoginBll.GetInstance().GetUserInfo(("+86 " + tbInputName.Text), tbInputPsw.Password);
                    if (bdData.code != 0 || (bdData.code == 0 && bdData.body.objects == null))
                    {
                        WarnInfo2.Content = "获取用户信息失败！" + bdData.message;
                    }
                    else
                    {
                        UserPwDLoginEvent(this, bdData.body.objects[0]);
                    }
                }
            }
#endif
        }

        private void Binding()
        {
            int ret;

            ret = vein.LoadingDevice();

            if (ret != 0 && ret != VeinUtils.FV_ERRCODE_EXISTING)
            {
                Dispatcher.BeginInvoke(new Action(() => {
                    WarnInfo2.Content = "初始化指静脉设备失败，请联系工作人员" + ret;
                    rebindingBtn.Visibility = Visibility.Visible;
                }));
                return;
            }

            //vein.EnumDevice();

            if(ret != VeinUtils.FV_ERRCODE_EXISTING)
            {
                ret = vein.OpenDevice();
                if (ret != 0)
                {
                    Dispatcher.BeginInvoke(new Action(() => {
                        WarnInfo2.Content = "无法打开指静脉设备，请联系工作人员" + ret;
                        rebindingBtn.Visibility = Visibility.Visible;
                    }));
                    return;
                }
            }

            byte[] regfeature = new byte[VeinUtils.FEATURE_COLLECT_CNT * VeinUtils.FV_FEATURE_SIZE];
            string info = "";

            for (int i = 0; i < VeinUtils.FEATURE_COLLECT_CNT; i++)
            {
                Dispatcher.BeginInvoke(new Action(() => GuidInfo.Content = "请将手指放在指静脉模块上！第" + (i+1) + "次"));

                byte[] subregfeature = new byte[VeinUtils.FV_FEATURE_SIZE];

                //等待手指放置
                if (vein.WaitState(0x03, out info) < 0)
                {
                    Dispatcher.BeginInvoke(new Action(() =>{
                        WarnInfo2.Content = info;
                        rebindingBtn.Visibility = Visibility.Visible;
                    }));
                    return;
                }

                if (vein.RegisterProcess(i, regfeature, out subregfeature, out info) != 0)
                {
                    Dispatcher.BeginInvoke(new Action(() => {
                        WarnInfo2.Content = info;
                        rebindingBtn.Visibility = Visibility.Visible;
                    }));
                    return;
                }

                this.Dispatcher.BeginInvoke(new Action(() => GuidInfo.Content = "请将手指从指静脉模块上移开！"));

                //等待手指放置
                if (vein.WaitState(0x00, out info) < 0)
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        WarnInfo2.Content = info;
                        rebindingBtn.Visibility = Visibility.Visible;
                    }));
                    return;
                }

                Array.Copy(subregfeature, 0, regfeature, i* VeinUtils.FV_FEATURE_SIZE,VeinUtils.FV_FEATURE_SIZE);
            }

#if LOCALSDK
            user.reg_feature = Convert.ToBase64String(regfeature);
            user.ai_feature = Convert.ToBase64String(regfeature);
            userBll.UpdateCurrentUsers(user);
            this.Dispatcher.BeginInvoke(new Action(() => GuidInfo.Content = "指静脉采集成功！"));
#else
			BasePostData<string> data = UserLoginBll.GetInstance().VeinmatchBinding(new VeinbindingPostParam
            {
                regfeature = Convert.ToBase64String(regfeature),
                finger_name = "finger1"
            });

            if(data.code == 0)
                this.Dispatcher.BeginInvoke(new Action(() => GuidInfo.Content = "指静脉采集成功！"));
            else
                this.Dispatcher.BeginInvoke(new Action(() => GuidInfo.Content = data.message));
#endif

        }


        private void Login()
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                BaseData<User> bdData = UserLoginBll.GetInstance().GetUserInfo(("+86 " + tbInputName.Text), tbInputPsw.Password);
                if (bdData.code != 0 || (bdData.code == 0 && bdData.body.objects == null) )
                {
                    WarnInfo2.Content = "获取用户信息失败！" + bdData.message;
                }
                else
                {
                    UserPwDLoginEvent(this, bdData.body.objects[0]);
                }
            }));
        }

    }
}
