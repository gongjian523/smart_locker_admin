using CFLMedCab.DAL;
using CFLMedCab.Infrastructure;
using CFLMedCab.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using CFLMedCab.Infrastructure.ToolHelper;
using log4net;
using System.Windows.Threading;
using CFLMedCab.Http.Model;

namespace CFLMedCab
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
		public EventWaitHandle ProgramStarted { get; set; }

		protected override void OnStartup(StartupEventArgs e)
        {
			bool createNew;
			ProgramStarted = new EventWaitHandle(false, EventResetMode.AutoReset, "乘法云", out createNew);

            LogUtils.InitLog4Net();

            if (!createNew)
			{
				MessageBox.Show("已有一个程序实例运行");
				Current.Shutdown();
				Environment.Exit(0);
			}

			base.OnStartup(e);
            CurrentUser user = new CurrentUser
            {
                id = 1111,
                name = "Nathan",
                vein_id = 12323,
            };
            ApplicationState.SetValue((int)ApplicationKey.CurUser, user);

            ApplicationState.SetEquipName("E00000010");
            ApplicationState.SetEquipId("AQACQqweDg8BAAAA1G-F5jgPsxWCFwQA");
            Console.WriteLine("SetEquipId");

            ApplicationState.SetHouseName("SR00000008");
            ApplicationState.SetHouseId("AQACQqweDg8BAAAAdoUd3g4PsxV3FwQA");

            ApplicationState.SetMCabName("L00000012");
            ApplicationState.SetMCabId("AQACQqweDg8BAAAA6XuBbV0PsxWJFwQA");
#if DUALCAB
            ApplicationState.SetSCabName("L00000012");
            ApplicationState.SetSCabId("AQACQqweDg8BAAAA6XuBbV0PsxWJFwQA");
#endif
            ApplicationState.SetMLockerCOM("COM2"); //"COM2"
            ApplicationState.SetSLockerCOM("COM5"); //"COM5"

            ApplicationState.SetMRfidCOM("COM1"); //"COM1"
            ApplicationState.SetSRfidCOM("COM4"); //"COM4"

            ApplicationState.SetMVeinCOM("COM9"); //"COM9"

            ApplicationState.SetGoodsInfo(new HashSet<CommodityEps>());

            // 注册Application_Error
            DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(App_DispatcherUnhandledException);
        }

        //异常处理逻辑
         void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
         {
            //处理完后，我们需要将Handler=true表示已此异常已处理过
            LogUtils.Error(e.ToString());
            e.Handled = true;
         }
    }
}
