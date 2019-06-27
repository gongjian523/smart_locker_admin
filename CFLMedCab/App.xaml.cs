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


			if (!createNew)
			{
				MessageBox.Show("已有一个程序实例运行");
				Current.Shutdown();
				Environment.Exit(0);
			}

			base.OnStartup(e);
            User user = new User
            {
                id = 1111,
                name = "Nathan",
                vein_id = 12323,
            };
            ApplicationState.SetValue((int)ApplicationKey.CurUser, user);
        }



    }
}
