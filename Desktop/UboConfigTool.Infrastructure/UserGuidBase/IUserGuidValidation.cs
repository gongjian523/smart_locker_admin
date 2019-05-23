using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UboConfigTool.Infrastructure.UserGuidBase
{
    public enum WizardProcedureType : short
    {
        NONE = 0,
        AddUBOGroup = 1,
        AddUBO = 2,
        AddRules = 3,
        AddUSBs = 4,
        Finish = 5
    }

    public interface IUserGuidValidation
    {
        bool ValidateData();
    }
}
