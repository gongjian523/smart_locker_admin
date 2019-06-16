﻿using CFLMedCab.Infrastructure.DbHelper;
using CFLMedCab.Model;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.DAL
{
    public class FetchOrderDal
    {
		//Db
		public SqlSugarClient Db;

		// 定义一个静态变量来保存类的实例
		private static FetchOrderDal singleton;

		// 定义一个标识确保线程同步
		private static readonly object locker = new object();

		//定义公有方法提供一个全局访问点。
		public static FetchOrderDal GetInstance()
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
						singleton = new FetchOrderDal();
					}
				}
			}
			return singleton;
		}

		// 定义私有构造函数，使外界不能创建该类实例
		private FetchOrderDal()
		{
			Db = SqlSugarHelper.GetInstance().Db;
		}




	}
}
