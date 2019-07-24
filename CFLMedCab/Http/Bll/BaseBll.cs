using CFLMedCab.Http.Helper;
using CFLMedCab.Http.Model;
using CFLMedCab.Http.Model.Base;
using CFLMedCab.Http.Model.param;
using CFLMedCab.Infrastructure.DbHelper;
using CFLMedCab.Infrastructure.ToolHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CFLMedCab.Http.Bll
{
	/// <summary>
	/// 通用业务处理代码封装
	/// </summary>
	public class BaseBll<T> where T : new()
	{
		// 定义一个静态变量来保存类的实例
		private static T singleton;
		// 定义一个标识确保线程同步
		private static readonly object locker = new object();


		//定义公有方法提供一个全局访问点。
		public static T GetInstance()
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
						singleton = new T();
					}
				}
			}
			return singleton;
		}

		/// <summary>
		/// 通用业务，通过id查询名称,如果出错，返回出错信息
		/// </summary>
		public string GetNameById<K>(string id) where K : BaseModel 
		{
			BaseData<K> baseData = HttpHelper.GetInstance().Get<K>(new QueryParam
			{
				@in =
				{
				field = "id",
				in_list =  { HttpUtility.UrlEncode(id) }
				}
			});

			baseData = HttpHelper.GetInstance().ResultCheck(baseData, out bool isSuccess);

			if (isSuccess)
			{
				return baseData.body.objects[0].name;
			}
			else
			{
				return baseData.message;
			}
		}

		/// <summary>
		/// 插入变化后的商品信息
		/// </summary>
		/// <param name="baseDataCommodityCode">所有数据</param>
		/// <param name="sourceBill">业务类型</param>
		/// <returns></returns>
		public bool InsertLocalCommodityCodeInfo(BaseData<CommodityCode> baseDataCommodityCode, string sourceBill)
		{
			var result = false;

			//校验是否含有数据，如果含有数据，有就继续下一步
			baseDataCommodityCode = HttpHelper.GetInstance().ResultCheck(baseDataCommodityCode, out bool isSuccess);

			if (isSuccess)
			{
				List<LocalCommodityCode> localCommodityCodes = baseDataCommodityCode.body.objects.MapToListIgnoreId<CommodityCode, LocalCommodityCode>();

				localCommodityCodes.ForEach(it =>
				{
					it.sourceBill = sourceBill;
				});

				//事务防止多插入产生脏数据
				result = SqlSugarHelper.GetInstance().Db.Ado.UseTran(() =>
				{

					SqlSugarHelper.GetInstance().Db.Insertable(localCommodityCodes).ExecuteCommand();

				}).IsSuccess;

			}

			return result;
		}


	}

}
