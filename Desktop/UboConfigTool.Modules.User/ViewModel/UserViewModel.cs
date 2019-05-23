using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using UboConfigTool.Infrastructure;
using UboConfigTool.Infrastructure.Interfaces;

namespace UboConfigTool.Modules.User.ViewModel
{
    public class UserViewModel : NotificationObject
    {
        private static Uri userEditViewUri = new Uri( "UserEditView", UriKind.Relative );
        private static Uri userAddViewUri = new Uri( "UserAddView", UriKind.Relative );
        private readonly string NORMAL_VM = "NormalViewModel";
        private readonly string ADD_VM = "AddViewModel";
        private IRegionManager _regionMgr;
        private Model.User _user;

        public UserViewModel( Model.User user )
        {
            _user = user;
            DeleteUserCommand = new DelegateCommand<object>( DeleteUser );
            EditUserCommand = new DelegateCommand<object>( EditUser );
            _vmType = NORMAL_VM;

            _regionMgr = ServiceLocator.Current.GetInstance<IRegionManager>();
        }

        public UserViewModel(bool bSysAdmin = true)
        {
            _isAddAsSysAdmin = bSysAdmin;
            _vmType = ADD_VM;
            _regionMgr = ServiceLocator.Current.GetInstance<IRegionManager>();
            AddUserCommand = new DelegateCommand( AddUser );
        }

        private string _vmType;
        public string VMType
        {
            get
            {
                return _vmType;
            }
        }

        public Model.User AssociatedUser
        {
            get
            {
                return _user;
            }
        }

        private bool _isAddAsSysAdmin;
        
        public bool IsSysAdmin
        {
            get
            {
                return AssociatedUser.IsSysAdminUser();
            }
        }

        private string _userRoleStr = string.Empty;
        public string UserRoleStr
        {
            get
            {
                if( _user == null )
                    return string.Empty;

                if( _user.UserRole.Type == Model.Role.Log_Checker )
                    return "日志管理员";
                else if( _user.UserRole.Type != Model.Role.None )
                    return "系统管理员";

                return string.Empty;
            }
        }

        public ICommand DeleteUserCommand
        {
            get;
            private set;
        }


        public ICommand EditUserCommand
        {
            get;
            private set;
        }

        public ICommand AddUserCommand
        {
            get;
            private set;
        }

        private void EditUser( object obj )
        {
            _regionMgr.Regions[RegionNames.SecondaryRegion].Context = new UserEditViewModel( AssociatedUser );
            _regionMgr.RequestNavigate( RegionNames.SecondaryRegion, userEditViewUri );
        }

        private void DeleteUser( object obj )
        {
            IMessageService msgService = ServiceLocator.Current.GetInstance<IMessageService>();
            if( msgService == null )
                return;

            if( msgService.ShowMessage( "删除", "确认删除当前选中用户?" ) != DialogResponse.Ok )
                return;
        }

        private void AddUser()
        {
            _regionMgr.Regions[RegionNames.SecondaryRegion].Context = new UserAddViewModel( _isAddAsSysAdmin );
            _regionMgr.RequestNavigate( RegionNames.SecondaryRegion, userAddViewUri );
        }
    }
}
