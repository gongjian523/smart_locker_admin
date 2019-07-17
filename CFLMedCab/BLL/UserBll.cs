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
            userDal = UserDal.GetInstance();
        }

        public CurrentUser  GetUserByVeinId(int veinId)
        {
            return userDal.GetUserByVeinId(veinId);
        }

        public int GetUserNum()
        {
            return userDal.GetUserNum();
        }

        public void InsetUsers(List<CurrentUser> list)
        {
            userDal.InsertUser(list);
        }

        public CurrentUser GetTestUser()
        {
            return userDal.GetUser().First();
        }


        public CurrentUser GetUserByName(string name)
        {
            return userDal.GetUserByName(name);
        }
    }
}
