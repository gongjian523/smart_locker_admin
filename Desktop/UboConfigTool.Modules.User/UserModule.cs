using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UboConfigTool.Infrastructure;
using UboConfigTool.Modules.User.View;

namespace UboConfigTool.Modules.User
{
    [ModuleExport(typeof(UserModule))]
    public class UserModule : IModule
    {
        [Import]
        public IRegionManager RegionManager;

        public void Initialize()
        {
            this.RegionManager.RegisterViewWithRegion( RegionNames.MainVavigationBarOthers, typeof( UserNavigationItem ) );
            this.RegionManager.RegisterViewWithRegion( RegionNames.LoginContentRegion, typeof( LoginView) );
        }
    }
}
