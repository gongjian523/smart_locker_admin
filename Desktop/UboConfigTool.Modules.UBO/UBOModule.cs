using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.Regions;
using UboConfigTool.Infrastructure;
using UboConfigTool.Modules.UBO.View;

namespace UboConfigTool.Modules.UBO
{
    [ModuleExport(typeof(UBOModule))]
    public class UBOModule : IModule
    {
        [Import]
        public IRegionManager RegionManager;

        public void Initialize()
        {
            this.RegionManager.RegisterViewWithRegion(RegionNames.MainNavigationBar, typeof(UBONavigationItem));
        }
    }
}
