using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UboConfigTool.Infrastructure.DomainBase
{
    public interface IEntity
    {
        object Key
        {
            get;
        }
    }
}
