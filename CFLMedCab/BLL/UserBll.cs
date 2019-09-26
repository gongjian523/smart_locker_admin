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
		/// 普通插入
		/// </summary>
		/// <param name="user"></param>
		public void InsertUser(CurrentUser user)
		{
			userDal.InsertUser(user);
		}


		/// <summary>
		/// 是否存在该fid
		/// </summary>
		/// <param name="user"></param>
		public bool IsExistsByFid(int fid)
		{
			return userDal.isExistsByFid(fid);
		}

		/// <summary>
		/// 不存在则插入
		/// </summary>
		/// <param name="user"></param>
		public void InsetUserNotExist(CurrentUser user)
		{
			
			if (!userDal.isExistsByRegfeature(user.username))
			{
				userDal.InsertUser(user);
			}
		}

		public CurrentUser GetTestUser()
        {
            return userDal.GetUser().First();
        }

		/// <summary>
		/// 获取最大id值
		/// </summary>
		/// <param name="user"></param>
		public int getUserMaxId()
		{
			return userDal.GetUserMaxId();
		}

		/// <summary>
		/// 根据fid获取用户名（目前只有手机号）
		/// </summary>
		/// <param name="fid"></param>
		/// <returns></returns>
		public string GetUserNameByFid(int fid)
		{
			return userDal.GetUserNameByFid(fid);
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
