using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using UboConfigTool.Infrastructure;

namespace UboConfigTool.Modules.User.ViewModel
{
    [Export( typeof( UserAddViewModel ) )]
    [PartCreationPolicy( CreationPolicy.NonShared )]
    public class UserAddViewModel : PopupViewModelBase
    {
        public UserAddViewModel( bool bSysAdmin )
        {
            IsSysAdmin = bSysAdmin;
        }

        public bool IsSysAdmin
        {
            get;
            set;
        }

        public string RoleString
        {
            get
            {
                if( IsSysAdmin )
                    return UserGroupViewModel.GROUP_SYS_ADMIN;
                else
                    return UserGroupViewModel.GROUP_LOG_USER;
            }
        }

        public override bool Validate()
        {
            return true;
        }
    }
}
