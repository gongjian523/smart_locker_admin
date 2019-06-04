using CFLMedCab.Infrastructure.SqliteHelper;
using CFLMedCab.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.DAL
{
    public class UserDal
    {
        /// <summary>
        /// 数据库没有表时创建
        /// </summary>
        public void CreateTable_User()
        {
            string commandText = @"CREATE TABLE if not exists user ('id' INTEGER PRIMARY KEY AUTOINCREMENT, 
                                                                   'name' VARCHAR(20)),
                                                                   'role' INTEGER,
                                                                   'vein_id' VARCHAR(50));";
            SqliteHelper.Instance.ExecuteNonQuery(commandText);
            return;
        }

        /// <summary>
        /// 新增用户数据
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public int InsertNewUser(User user)
        {
            string commandText = string.Format(@"INSERT INTO user (name, role, vein_id) VALUES 
                                                ('{0}', '{1}', '{2}')",
                                                user.name, user.role, user.vein_id);

            if (!SqliteHelper.Instance.ExecuteNonQuery(commandText))
                return 0;


            return LastInsertRowId();
        }

        private int LastInsertRowId()
        {
            return Convert.ToInt16(SqliteHelper.Instance.ExecuteScalar("SELECT last_insert_rowid();"));
        }
    }
}
