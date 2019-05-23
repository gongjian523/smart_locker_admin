using Microsoft.Practices.Prism.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace UboConfigTool.Modules.User.ViewModel
{
    public class UserGroupViewModel : NotificationObject
    {
        public static string GROUP_SYS_ADMIN = "系统管理员";
        public static string GROUP_LOG_USER = "日志管理员";

        private List<Model.User> _modelUsers;

        public UserGroupViewModel(string grpName, List<Model.User> users  )
        {
            GroupName = grpName;
            _modelUsers = users;

            _modelUsers.ForEach( aUser =>
                {
                    Users.Add( new UserViewModel( aUser ) );
                } );


            if( Users.Count == 0 )
            {
                for( int i=0; i < 20; i++ )
                {
                    List<Infrastructure.Models.DTOUserRole> roles = new List<Infrastructure.Models.DTOUserRole>();
                    roles.Add( new Infrastructure.Models.DTOUserRole()
                        {
                            id = i,
                            role_type = i % 2 ==0 ? 1: 2,
                            user_id = i
                        } );
                    Users.Add( new UserViewModel( new Model.User( new Infrastructure.Models.DTOUser()
                    {
                        id = i,
                        email = "test@123.com",
                        username = "test-" + i.ToString(),
                        uid = "test-" + i.ToString(),
                        user_roles = roles
                    } ) ) );
                }

            }

            Users.Add( new UserViewModel(GroupName == GROUP_SYS_ADMIN) );//addtional as UI adding viewmodel
        }

        public string GroupName
        {
            get;
            private set;
        }

        private ObservableCollection<UserViewModel> _users = new ObservableCollection<UserViewModel>();
        public ObservableCollection<UserViewModel> Users
        {
            get
            {
                return _users;
            }
        }
    }
}
