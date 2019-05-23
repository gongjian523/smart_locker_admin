using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UboConfigTool.Infrastructure.Models;

namespace UboConfigTool.Infrastructure.Interfaces
{
    interface IVirusLibVersionService
    {
        IList<VirusLibVersionItem> AllVirusLibVersions();
        VirusLibVersionItem GetVersion( string virusVersionName );
        bool AddVersionItem( VirusLibVersionItem vItem );
        bool RemoveVersionItem( VirusLibVersionItem vItem );
    }
}
