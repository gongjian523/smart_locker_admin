using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using UboConfigTool.Infrastructure.DomainBase;
using UboConfigTool.Infrastructure.Interfaces;
using UboConfigTool.Infrastructure.Models;
using UboConfigTool.Modules.User.Services;

namespace UboConfigTool.Modules.User.Model
{
    public enum Role
    {
        None = 0,
        Admin = 1,
        Log_Checker = 2,
        Super_Admin = 3
    }

    [DataContract]
    public class User : IAggregateRoot
    {
        [Import]
        public IUserService userService;

        public User()
        {

        }

        public User( DTOUser dtoUser )
        {
            Id = dtoUser.id;
            UId = dtoUser.uid;
            Name = dtoUser.username;
            EmailAdd = dtoUser.email;
            CreatedAt = dtoUser.created_at;
            if(dtoUser.user_roles.Count > 0)
                UserRole = new UserRoleAssociation(dtoUser.user_roles[0]);
        }

        public static DTOUser TODTOModel( User user )
        {
            if( user == null )
                return null;

            DTOUser dtoUser = new DTOUser()
            {
                id = user.Id,
                uid = user.UId,
                username = user.Name,
                email = user.EmailAdd,
                created_at = user.CreatedAt,
                user_roles = new List<DTOUserRole>()
            };

            dtoUser.user_roles.Add( UserRoleAssociation.TODTOModel( user.UserRole ) );

            return dtoUser;
        }

        public int Id { get; private set; }

        public string UId { get; private set; }

        public string Name { get; set; }

        public string EmailAdd   { get; set; }

        public DateTime? CreatedAt { get; private set; }

        public UserRoleAssociation UserRole { get; private set; }
     
        public bool ChangePassword( string oldPassword, string newPassword )
        {
            //if( string.IsNullOrEmpty( oldPassword ) || string.IsNullOrEmpty( newPassword ) )
            //{
            //    return false;
            //}

            //if( this.Password != oldPassword )
            //{
            //    return false;
            //}

            //bool bSuc = this.userService.ChangePassword( new UserDescription( this.UserId, this.Name ), oldPassword, newPassword );
            //if( bSuc )
            //{
            //    this.Password = newPassword;
            //}

            //return bSuc;

            return true;
        }

        public bool IsSysAdminUser()
        {
            if( UserRole == null )
                return false;

            return ( UserRole.Type == Model.Role.Admin || UserRole.Type == Model.Role.Super_Admin );
        }

        public bool IsLogUser()
        {
            if( UserRole == null )
                return false;

            return UserRole.Type == Model.Role.Log_Checker;
        }

        public object Key
        {
            get
            {
                return Id;
            }
        }
    }
}
