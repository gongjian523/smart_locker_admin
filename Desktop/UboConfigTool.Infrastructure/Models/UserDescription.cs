using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UboConfigTool.Infrastructure.Models
{
    public class UserDescription
    {
        public UserDescription()
        {
        }

        public UserDescription(string Id, string name)
        {
            UserId = Id;
            UserName = name;
        }

        public string UserId
        {
            get;
            set;
        }

        public string UserName
        {
            get;
            set;
        }

        //public Role UserRole
        //{
        //    get;
        //    set;
        //}
     }
}
