using CFLMedCab.Infrastructure.DbHelper;
using CFLMedCab.Model;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.DAL
{
    public class UserDal 
    {       
        //Db
        public SqlSugarClient Db;

        // 定义一个静态变量来保存类的实例
        private static UserDal singleton;

        // 定义一个标识确保线程同步
        private static readonly object locker = new object();

        //定义公有方法提供一个全局访问点。
        public static UserDal GetInstance()
        {
            //这里的lock其实使用的原理可以用一个词语来概括“互斥”这个概念也是操作系统的精髓
            //其实就是当一个进程进来访问的时候，其他进程便先挂起状态
            if (singleton == null)
            {
                lock (locker)
                {
                    // 如果类的实例不存在则创建，否则直接返回
                    if (singleton == null)
                    {
                        singleton = new UserDal();
                    }
                }
            }
            return singleton;
        }

        // 定义私有构造函数，使外界不能创建该类实例
        private UserDal()
        {
            Db = SqlSugarHelper.GetInstance().Db;
        }

        public User GetUserByVeinId(int veinId)
        {
            return Db.Queryable<User>().Where(user => user.vein_id == veinId).First();
        }

        public int GetUserNum()
        {
            return Db.Queryable<User>().Select<User>().ToList().Count();
        }

        public void InsertUser(List<User> list)
        {
            Db.Insertable<User>(list).ExecuteCommand();
        }

        public List<User> GetUser()
        {
            return Db.Queryable<User>().Select<User>().ToList();
        }

        public User GetUserByName(string name)
        {
            return Db.Queryable<User>().Where(user => user.name.ToUpper() == name.ToUpper()).First();
        }
    }
}
