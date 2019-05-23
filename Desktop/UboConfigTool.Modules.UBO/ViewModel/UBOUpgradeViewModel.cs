using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UboConfigTool.Infrastructure;

namespace UboConfigTool.Modules.UBO.ViewModel
{
    public class UBOUpgradeViewModel : PopupViewModelBase
    {
        public UBOUpgradeViewModel()
        {
        }

        public override bool Validate()
        {
            return true;
        }
    }
}
