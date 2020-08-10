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
    class LoginDal
    {
        //Db
        public SqlSugarClient Db = SqlSugarHelper.GetInstance().Db;

        // 定义一个静态变量来保存类的实例
        private static LoginDal singleton;
        // 定义一个标识确保线程同步
        private static readonly object locker = new object();


        //定义公有方法提供一个全局访问点。
        public static LoginDal GetInstance()
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
                        singleton = new LoginDal();
                    }
                }
            }
            return singleton;
        }


        /// <summary>
        /// 新建登录记录
        /// </summary>
        /// <returns></returns>
        public int NewLoginRecode(LoginRecord loginRecord)
        {
            return Db.Insertable<LoginRecord>(loginRecord).ExecuteReturnEntity().id;
        }


        /// <summary>
        /// 更新登录记录
        /// </summary>
        public void UpdateLoginRecode(LoginRecord loginRecord)
        {
            Db.Updateable<LoginRecord>(loginRecord).ExecuteCommand();
        }


        public LoginRecord GetLoginRecordById(int id)
        {
            var item = Db.Queryable<LoginRecord>()
                .Where(it => it.id == id)
                .OrderBy(it => it.login_time, OrderByType.Desc)
                .Select<LoginRecord>()
                .First();
            return item;
        }

        public List<LoginRecord> GetLoginRecordByUserName(string userName)
        {
            var list = Db.Queryable<LoginRecord>()
                .Where(it => it.user_name == userName)
                .OrderBy(it => it.login_time, OrderByType.Desc)
                .Select<LoginRecord>()
                .ToList();
            return list;
        }


        public List<LoginRecord> GetAllLoginRecord()
        {
            var list = Db.Queryable<LoginRecord>()
                .OrderBy(it => it.login_time, OrderByType.Desc)
                .Select<LoginRecord>()
                .ToList();
            return list;
        }
    }
}
