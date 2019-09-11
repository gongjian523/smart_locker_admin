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
            userDal.InsertUsers(list);
        }

		public void InsetUserOrUpdate(CurrentUser user)
		{
			CurrentUser currentUser = userDal.GetUserByUsername(user.username);
			if (currentUser == null)
			{
				userDal.InsertUser(user);
			}
			else
			{
				user.id = currentUser.id;
				userDal.UpdateCurrentUser(user);
			}
		}

		/// <summary>
		/// 不存在则插入
		/// </summary>
		/// <param name="user"></param>
		public void InsetUserNotExist(CurrentUser user)
		{
			
			if (!userDal.isExistsByUsername(user.username))
			{
				userDal.InsertUser(user);
			}
		}

		public CurrentUser GetTestUser()
        {
            return userDal.GetUser().First();
        }


        public CurrentUser GetUserByName(string name)
        {
            return userDal.GetUserByName(name);
        }

        public List<CurrentUser> GetAllUsers()
        {
            return userDal.GetUser();
        }

        public void  UpdateCurrentUsers(CurrentUser item)
        {
            userDal.UpdateCurrentUser(item);
        }
    }
}
