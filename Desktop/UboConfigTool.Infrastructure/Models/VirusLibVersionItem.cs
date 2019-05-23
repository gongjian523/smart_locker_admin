using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UboConfigTool.Infrastructure.DomainBase;

namespace UboConfigTool.Infrastructure.Models
{
    public class VirusLibVersionItem : IEntity
    {
        public string VersionName { get;   private set; }
        public string Description { get;   set; }
        public string FilePath { get;   set; }

        public object Key
        {
            get
            {
                return VersionName;
            }
        }
    }
}
