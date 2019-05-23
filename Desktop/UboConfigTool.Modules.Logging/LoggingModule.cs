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
using UboConfigTool.Modules.Logging.View;

namespace UboConfigTool.Modules.Logging
{
    [ModuleExport( typeof( LoggingModule ) )]
    public class LoggingModule : IModule
    {
        [Import]
        public IRegionManager RegionManager;

        public void Initialize()
        {
            this.RegionManager.RegisterViewWithRegion( RegionNames.MainNavigationBar, typeof( LoggingNavigationItem ) );
            this.RegionManager.RegisterViewWithRegion( RegionNames.MainContentFlyout, typeof( DeviceLogFilterView ) );
            this.RegionManager.RegisterViewWithRegion( RegionNames.MainContentFlyout, typeof( SystemLogFilterView ) );
        }
    }
}
