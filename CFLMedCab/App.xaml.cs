using CFLMedCab.DAL;
using CFLMedCab.Infrastructure;
using CFLMedCab.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace CFLMedCab
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Hashtable ht = new Hashtable();
            HashSet<string> hs1 = new HashSet<string> { "E20000176012027919504D98", "E20000176012025319504D67", "E20000176012025619504D70", "E20000176012028119504DA5", "E20000176012023919504D48" };
            ht.Add("COM1", hs1);
            HashSet<string> hs4 = new HashSet<string> { "E20000176012028219504DAD", "E20000176012026619504D8D", "E20000176012026319404F98", "E20000176012028019504DA0", "E20000176012026519504D85" };
            ht.Add("COM4", hs4);
            ApplicationState.SetValue((int)ApplicationKey.CurGoods, ht);
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
