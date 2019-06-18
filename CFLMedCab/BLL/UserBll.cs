using CFLMedCab.DAL;
using CFLMedCab.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.BLL
{
    class UserBll
    {
        private readonly UserDal userDal;

        public UserBll()
        {
            userDal = new UserDal();
        }

        public User  GetUserByVeinId(int veinId)
        {
            return userDal.GetUserByVeinId(veinId);
        }

        public int GetUserNum()
        {
            return userDal.GetUserNum();
        }

        public void InsetUsers(List<User> list)
        {
            userDal.InsertUser(list);
        }

        public User GetTestUser()
        {
            return userDal.GetUser().First();
        }

    }
}
