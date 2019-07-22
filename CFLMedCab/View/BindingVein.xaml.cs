using CFLMedCab.BLL;
using CFLMedCab.Controls;
using CFLMedCab.Http.Bll;
using CFLMedCab.Http.Model;
using CFLMedCab.Http.Model.Base;
using CFLMedCab.Http.Model.param;
using CFLMedCab.Infrastructure;
using CFLMedCab.Infrastructure.DeviceHelper;
using CFLMedCab.Model;
using System;
using System.Collections.Generic;
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
#if LOCALSDK
            user = userBll.GetUserByName(tbInputName.Text);

            if (user == null)
            {
                WarnInfo.Content = "无法获取用户信息，请重新输入！";
                return;
            }
#else
            var data1 = UserLoginBll.GetInstance().GetUserToken(new Account
            {
                //Phone = tbInputName.Text,
                //Password = tbInputPsw.Text
                Phone = "18628293148",
                Password = "cfy12345"
            });
#endif

            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(240, 80);
            var data2 = UserLoginBll.GetInstance().GetCaptchaImage();

            //byte[] byteArray = System.Text.Encoding.Default.GetBytes(data2);
            byte[] byteArray = new byte[256];
            System.IO.MemoryStream ms = new System.IO.MemoryStream(byteArray);
            //System.Drawing.Image img = System.Drawing.Image.FromStream(ms);

            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            ms.Position = 0;

            var bi = new BitmapImage();
            bi.BeginInit();
            bi.CacheOption = BitmapCacheOption.OnLoad;
            bi.StreamSource = ms;
            bi.EndInit();
            bi.Freeze();

            imageAuth.Source = bi;


            loginView.Visibility = Visibility.Collapsed;
            BindingView.Visibility = Visibility.Visible;

            rebindingBtn.Visibility = Visibility.Hidden;

            Task.Factory.StartNew(Binding);
        }

        private void Binding()
        {
            int ret;
            return;


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
            BaseData<string> data = UserLoginBll.GetInstance().VeinmatchBinding(new VeinmatchPostParam
            {
                regfeature = Convert.ToBase64String(regfeature),
                finger_name = "fingerSSS"
            });

            if(data.code == 0)
                this.Dispatcher.BeginInvoke(new Action(() => GuidInfo.Content = "指静脉采集成功！"));
            else
                this.Dispatcher.BeginInvoke(new Action(() => GuidInfo.Content = data.message));
#endif

        }
    }
}
