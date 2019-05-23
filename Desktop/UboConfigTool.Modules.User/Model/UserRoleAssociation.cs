using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UboConfigTool.Infrastructure.DomainBase;
using UboConfigTool.Infrastructure.Models;

namespace UboConfigTool.Modules.User.Model
{
    public class UserRoleAssociation : IEntity
    {
        public UserRoleAssociation(DTOUserRole userRole)
        {
            Id = userRole.id;
            Type = (Role)userRole.role_type;
            User_Id = userRole.user_id;
            CreateAt = userRole.created_at;
            UpdateAt = userRole.updated_at;
        }

        public static DTOUserRole TODTOModel( UserRoleAssociation userAssociation )
        {
            if( userAssociation == null )
                return null;

            DTOUserRole dtoRole = new DTOUserRole()
            {
                id = userAssociation.Id,
                role_type = (int)userAssociation.Type,
                user_id = userAssociation.User_Id,
                created_at = userAssociation.CreateAt,
                updated_at = userAssociation.UpdateAt
            };

            return dtoRole;
        }

        public int Id { get; private set; }

        public Role Type { get; private set; }
 
        public int User_Id { get; private set; }

        public DateTime? CreateAt { get; private set; }

        public DateTime? UpdateAt  { get; private set; }

        public object Key
        {
            get
            {
                return Id;
            }
        }
    }
}
