using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UboConfigTool.Infrastructure.Models
{
    public class CloudOption
    {
        public bool UseCloudBackup { get;   set; }
        public string BackupServer { get;   set; }

        public bool UseLogMonitoring { get;   set; }
        public string LogMonitoringServer { get;   set; }
    }
}
