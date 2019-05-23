using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UboConfigTool.Infrastructure.Repository;

namespace UboConfigTool.Modules.User.Repository
{
    public class UserRepository : RepositoryBase<Model.User>
    {
        public UserRepository()
        {
            RestResource = "auth/sign_in";
        }
    }
}
