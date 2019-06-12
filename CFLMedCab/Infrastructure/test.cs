using CFLMedCab.Model;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Infrastructure
{
    public class hehaixia
    {
        public static hehaixia Instance
        {
            get
            {
                return iInstance;
            }

        }

        static private hehaixia iInstance = new hehaixia();
        public void SqlSugarClient()
        {
            DbContext context = new DbContext();

            context.Db.CodeFirst.InitTables<InventoryOrderdal>();//Create Tables
        }
    }

    /// <summary>
    /// DbContext Example 1
    /// </summary>
    public class DbContext
    {

        public SqlSugarClient Db;
        public DbContext()
        {
            Db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = "Version=3;uri=file:CFLMedCab1.db",//Master Connection
                DbType = DbType.Sqlite,
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute,
                AopEvents = new AopEvents()
                {
                    OnLogExecuting = (sql, p) =>
                    {
                        Console.WriteLine(sql);
                    }
                }
            });
        }
    }
}
