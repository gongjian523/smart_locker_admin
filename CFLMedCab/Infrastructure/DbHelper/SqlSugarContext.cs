using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Infrastructure.DbHelper
{
    public class SqlSugarContext<T> where T : class, new()
    {


        public SqlSugarClient Db;//用来处理事务多表查询和复杂的操作
        public SimpleClient<T> CurrentDb { get { return new SimpleClient<T>(Db); } }//用来处理T表的常用操作

        private static string dbFilename = @"CFLMedCab1.db";
        private static string ConnectionString = string.Format("Version=3;uri=file:{0}", dbFilename);
        //public static string ConnectionString = @"DataSource=" + GetCurrentProjectPath + @"CFLMedCab1.db";

        public SqlSugarContext()
        {
            Db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = ConnectionString,
                DbType = DbType.Sqlite,
                InitKeyType = InitKeyType.Attribute,//从特性读取主键和自增列信息
                IsAutoCloseConnection = true,//开启自动释放模式和EF原理一样我就不多解释了
            });

            //Db.DbMaintenance.CreateDatabase();
            Db.CodeFirst.InitTables<T>();//Create CodeFirstTable1 

            //调式代码 用来打印SQL 
            //Db.Aop.OnLogExecuting = (sql, pars) =>
            //{
            //    Console.WriteLine(sql + "\r\n" +
            //        Db.Utilities.SerializeObject(pars.ToDictionary(it => it.ParameterName, it => it.Value)));
            //    Console.WriteLine();
            //};

        }

        /// <summary>
        /// 获取所有
        /// </summary>
        /// <returns></returns>
        public virtual List<T> GetList()
        {
            return CurrentDb.GetList();
        }

        /// <summary>
        /// 根据主键删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual bool Delete(dynamic id)
        {
            return CurrentDb.Delete(id);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual bool Update(T obj)
        {
            return CurrentDb.Update(obj);
        }

        public virtual int Insert(T obj)
        {
            return CurrentDb.InsertReturnIdentity(obj);
        }

        public static string GetCurrentProjectPath
        {
            get
            {
                return Environment.CurrentDirectory;
            }
        }

        public virtual List<T> SqlQuery(string sql, params SugarParameter[] parameters)
        {
            return Db.Ado.SqlQuery<T>(sql, parameters);
        }
    }
}
