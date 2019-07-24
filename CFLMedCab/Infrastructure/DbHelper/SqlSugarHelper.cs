using CFLMedCab.Http.Model;
using CFLMedCab.Model;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Infrastructure.DbHelper
{
    public class SqlSugarHelper
    {

        public SqlSugarClient Db;//用来处理事务多表查询和复杂的操作
        public SqlSugarClient GetDb { get { return Db; } }//用来处理T表的常用操作

        private static string dbFilename = @"CFLMedCab.db";
        private static string ConnectionString = string.Format("Version=3;uri=file:{0}", dbFilename);

		// 定义一个静态变量来保存类的实例
		private static SqlSugarHelper singleton;

		// 定义一个标识确保线程同步
		private static readonly object locker = new object();

		//定义公有方法提供一个全局访问点。
		public static SqlSugarHelper GetInstance()
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
						singleton = new SqlSugarHelper();
					}
				}
			}
			return singleton;
		}

	
		private SqlSugarHelper()
        {
            Db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = ConnectionString,
                DbType = DbType.Sqlite,
                InitKeyType = InitKeyType.Attribute,//从特性读取主键和自增列信息
                IsAutoCloseConnection = true,//开启自动释放模式和EF原理一样我就不多解释了
            });

            //Db.DbMaintenance.CreateDatabase();
            //Db.CodeFirst.InitTables<T>();//Create CodeFirstTable1 

            //调式代码 用来打印SQL 
            Db.Aop.OnLogExecuting = (sql, pars) =>
            {
                Console.WriteLine(sql + "\r\n" +
                    Db.Utilities.SerializeObject(pars.ToDictionary(it => it.ParameterName, it => it.Value)));
                Console.WriteLine();
            };

            //创建领用单表
            Db.CodeFirst.InitTables<FetchOrder>();
            Db.CodeFirst.InitTables<FetchOrderdtl>();

            //创建物品和库存变化表
            Db.CodeFirst.InitTables<Goods>();
            Db.CodeFirst.InitTables<GoodsChageOrder>();
            Db.CodeFirst.InitTables<GoodsChageOrderdtl>();

            //创建补货入库表
            Db.CodeFirst.InitTables<ReplenishOrder>();
            Db.CodeFirst.InitTables<ReplenishSubOrder>();
            Db.CodeFirst.InitTables<ReplenishSubOrderdtl>();

            //创建拣货出库表
            //Db.CodeFirst.InitTables<InventoryOrder>();
            Db.CodeFirst.InitTables<InventoryOrderdtl>();
            Db.CodeFirst.InitTables<InventoryPlanLDB>();

            //创建退货出库表
            Db.CodeFirst.InitTables<PickingOrder>();
            Db.CodeFirst.InitTables<PickingSubOrder>();
            Db.CodeFirst.InitTables<PickingSubOrderdtl>();

            //创建手术表
            Db.CodeFirst.InitTables<SurgeryOrder>();
            Db.CodeFirst.InitTables<SurgeryOrderdtl>();

            //创建用户表
            Db.CodeFirst.InitTables<CurrentUser>();

            //本地库存变化表
            Db.CodeFirst.InitTables<LocalCommodityCode>();

        }

        public static string GetCurrentProjectPath
        {
            get
            {
                return Environment.CurrentDirectory;
            }
        }

    }
}
