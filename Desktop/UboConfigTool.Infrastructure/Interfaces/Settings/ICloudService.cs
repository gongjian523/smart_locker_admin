using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UboConfigTool.Infrastructure.Models;

namespace UboConfigTool.Infrastructure.Interfaces
{
    interface ICloudService
    {
        CloudOption GetOption();
        bool SetOption( CloudOption opt );
    }
}
