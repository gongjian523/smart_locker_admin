using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace UboConfigTool.Infrastructure.Models
{
    [DataContract]
    public class DashboardSummaryInfo
    {
        public DashboardSummaryInfo()
        {

        }

        [DataMember( Name = "ubogroup_count" )]
        public int UBOGroupCount
        {
            get;
            set;
        }

        [DataMember( Name = "ubo_count" )]
        public int UBOCount
        {
            get;
            set;
        }

        [DataMember( Name = "usb_count" )]
        public int USBCount
        {
            get;
            set;
        }

        [DataMember( Name = "rules_count" )]
        public int RulesCount
        {
            get;
            set;
        }

        [DataMember( Name = ( "logs_total_count" ) )]
        public int LogsTotalCount
        {
            get;
            set;
        }

        [DataMember( Name = ( "logs_unread_count" ) )]
        public int LogsUnreadCount
        {
            get;
            set;
        }
    }
}
