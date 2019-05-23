using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UboConfigTool.Infrastructure.Interfaces;
using UboConfigTool.Infrastructure.Models;

namespace UboConfigTool.Modules.User.Services
{
    public interface IUserService
    {
        event DataBeginLoading DataBeginLoadingEvent;
        event DataEndLoading DataEndLoadingEvent;

        bool Authenticate( string name, string password );
        UserDescription Login( string name, string password );
        UserDescription GetCurrentUser();
        bool ChangePassword( UserDescription user, string oldPassword, string newPassword );
        UserDescription CurrentUserCreateNew( string name, string password, Model.Role role );
        List<Model.User> GetCurrentManageableUserAccordingRole();
        
        void ReloadData();

        IList<Model.User> GetAllUsers();
    }
}
