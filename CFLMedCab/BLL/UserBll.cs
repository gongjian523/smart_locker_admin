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
        private UserDal userDal;

        public UserBll()
        {
            userDal = new UserDal();
        }

        public User  GetUserByVeinId(int veinId)
        {
            return userDal.GetUserByVeinId(veinId);
        }

    }
}
