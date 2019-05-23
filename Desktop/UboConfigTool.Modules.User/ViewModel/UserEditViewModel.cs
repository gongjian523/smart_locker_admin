using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UboConfigTool.Infrastructure;

namespace UboConfigTool.Modules.User.ViewModel
{
    public class UserEditViewModel : PopupViewModelBase
    {
        private Model.User _editUser;
        public UserEditViewModel( Model.User editUser )
        {
            _editUser = editUser;
            _userName = _editUser.Name;
            _IsSysAdmin = _editUser.IsSysAdminUser();
            _userRoleName = _IsSysAdmin ? UserGroupViewModel.GROUP_SYS_ADMIN : UserGroupViewModel.GROUP_LOG_USER;
        }


        private string _userName = string.Empty;
        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                _userName = value;
                this.RaisePropertyChanged( () => UserName );
            }
        }

        private bool _IsSysAdmin;
        public bool IsSysAdmin
        {
            get
            {
                return _IsSysAdmin;
            }
            set
            {
                _IsSysAdmin = value;
                this.RaisePropertyChanged( () => IsSysAdmin );
            }
        }

        private string _userRoleName;
        public string UserRoleName
        {
            get
            {
                return _userRoleName;
            }
            set
            {
                if( _userRoleName != value )
                {
                    _userRoleName = value;
                    this.RaisePropertyChanged( () => UserRoleName );
                    IsSysAdmin = ( value == UserGroupViewModel.GROUP_SYS_ADMIN ) ? true : false;
                }

            }
        }

        public override bool Validate()
        {
            return true;
        }
    }
}
