using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using UboConfigTool.Infrastructure.DomainBase;

namespace UboConfigTool.Modules.Logging.Model
{
    public enum LogLevelType
    {
        None = 0,
        Fatal = 1,
        Error = 2,
        Warning = 3,
        Info = 4
    }

    public class DeviceLogItem
    {
        public DeviceLogItem()
        {

        }

        public string msg
        {
            get;
            set;
        }

        public string ubo
        {
            get;
            set;
        }

        public string ubo_group
        {
            get;
            set;
        }

        public string datetime
        {
            get;
            set;
        }

        public string level
        {
            get;
            set;
        }
    }
}
