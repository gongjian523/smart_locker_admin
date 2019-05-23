using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UboConfigTool.Domain.Models;

namespace UboConfigTool.Repository
{
    public interface IUBOGroupRepository
    {
        IList<UBOGroup> GetAllUBOs();
    }
}
