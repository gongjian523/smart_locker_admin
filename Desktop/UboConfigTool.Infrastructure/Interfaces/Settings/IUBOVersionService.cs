using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UboConfigTool.Infrastructure.Models;

namespace UboConfigTool.Infrastructure.Interfaces
{
    interface IUBOVersionService
    {
        IList<UBOVersionItem> AllUBOFirmwareVersions();
        UBOVersionItem GetVersion( string versionName );
        bool AddVersionItem( UBOVersionItem vItem );
        bool RemoveVersionItem( UBOVersionItem vItem );
    }
}
