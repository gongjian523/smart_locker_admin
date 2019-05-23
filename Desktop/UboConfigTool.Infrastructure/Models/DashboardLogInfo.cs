using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace UboConfigTool.Infrastructure.Models
{
    [DataContract]
    public class DashboardLogInfo
    {
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
        public DateTime datetime
        {
            get;
            set;
        }
        public string level
        {
            get;
            set;
        }
        public string type
        {
            get;
            set;
        }
    }
}
