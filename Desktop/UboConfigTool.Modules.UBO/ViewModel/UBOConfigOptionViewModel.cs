using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using UboConfigTool.Infrastructure;

namespace UboConfigTool.Modules.UBO.ViewModel
{
    [Export( typeof( UBOConfigOptionViewModel ) )]
    [PartCreationPolicy( CreationPolicy.NonShared )]
    public class UBOConfigOptionViewModel : PopupViewModelBase
    {
        public UBOConfigOptionViewModel()
        {
        }

        public override bool Validate()
        {
            return true;
        }
    }
}
