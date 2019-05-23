using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using UboConfigTool.Infrastructure.DomainBase;

namespace UboConfigTool.Modules.Logging.Model
{
    public enum OperationType
    {
        None = 0,
        Create = 1,
        Update = 2,
        Delete = 3
    }

    public enum OperatoinTargetType
    {
        None = 0,
        UBO_Group = 1,
        UBO = 2,
        Usb = 3,
        User = 4
    }

    [DataContract]
    public class SystemLogItem
    {
        public SystemLogItem()
        {

        }

        //[DataMember]
        //public string id
        //{
        //    get;
        //    set;
        //}

        //[DataMember]
        //public string userId
        //{
        //    get;
        //    set;
        //}

        //[DataMember]
        //public string userRole
        //{
        //    get;
        //    set;
        //}

        //[DataMember]
        //public OperatoinTargetType target
        //{
        //    get;
        //    set;
        //}

        //[DataMember]
        //public OperationType type
        //{
        //    get;
        //    set;
        //}

        //[DataMember]
        //public string info
        //{
        //    get;
        //    set;
        //}

        //[DataMember]
        //public DateTime logTime
        //{
        //    get;
        //    set;
        //}

        //public object Key
        //{
        //    get
        //    {
        //        return id;
        //    }
        //}

        public string msg
        {
            get;
            set;
        }

        public string user
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

        public string module
        {
            get;
            set;
        }

        public string ip
        {
            get;
            set;
        }
    }
}
