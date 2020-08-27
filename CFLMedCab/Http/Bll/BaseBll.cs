using CFLMedCab.APO.Inventory;
using CFLMedCab.Http.Helper;
using CFLMedCab.Http.Model;
using CFLMedCab.Http.Model.Base;
using CFLMedCab.Http.Model.param;
using CFLMedCab.Infrastructure;
using CFLMedCab.Infrastructure.DbHelper;
using CFLMedCab.Infrastructure.ToolHelper;
using Newtonsoft.Json;
using SqlSugar;
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
		/// 通用业务，通过id查询名称,如果出错，返回出错信息
		/// </summary>
		public BaseData<K> GetNameById<K>(string id, out bool isSuccess1) where K : BaseModel
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

			isSuccess1 = isSuccess;

			return baseData;
		}


		/// <summary>
		/// 通用业务，通过id查询对象,如果出错，返回出错信息
		/// </summary>
		public BaseData<K> GetObjectByIds<K>(List<string> ids, out bool isSuccess1) where K : BaseModel
		{
			BaseData<K> baseData = HttpHelper.GetInstance().Get<K>(new QueryParam
			{
				@in =
				{
				field = "id",
				in_list =  ids
				}
			});

			baseData = HttpHelper.GetInstance().ResultCheck(baseData, out bool isSuccess);

			isSuccess1 = isSuccess;

			return baseData;
		}

		/// <summary>
		/// 通用业务，通过id查询名称,如果出错，返回出错信息
		/// </summary>
		public string GetStoreHouseCodeById<K>(string id) where K : StoreHouse
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
				return baseData.body.objects[0].StoreHouseCode;
			}
			else
			{
				return baseData.message;
			}
		}


		/// <summary>
		/// 通用业务，通过name查询id,如果出错，返回出错信息
		/// </summary>
		public BaseData<string> GetIdByName<K>(string name) where K : BaseModel
		{
			BaseData<string> baseDataRet = new BaseData<string>()
			{
				body = new BaseBody<string>()
				{
					objects = new List<string>() { "" }
				}
			};

			BaseData<K> baseData = HttpHelper.GetInstance().Get<K>(new QueryParam
			{
				@in =
				{
				field = "name",
				in_list =  { HttpUtility.UrlEncode(name) }
				}
			});

			baseData = HttpHelper.GetInstance().ResultCheck(baseData, out bool isSuccess);
			baseDataRet.code = baseData.code;
			baseDataRet.message = baseData.message;

			if (isSuccess)
			{
				baseDataRet.body.objects[0] = baseData.body.objects[0].id;
			}
			return baseDataRet;
		}

		/// <summary>
		/// 通用业务，通过name查询id,如果出错，返回出错信息
		/// </summary>
		public BaseData<string> GetIdByStoreHouseCode<K>(string storeHouseCode) where K : StoreHouse
		{
			BaseData<string> baseDataRet = new BaseData<string>()
			{
				body = new BaseBody<string>()
				{
					objects = new List<string>() { "" }
				}
			};

			BaseData<K> baseData = HttpHelper.GetInstance().Get<K>(new QueryParam
			{
				@in =
				{
				field = "StoreHouseCode",
				in_list =  { HttpUtility.UrlEncode(storeHouseCode) }
				}
			});

			baseData = HttpHelper.GetInstance().ResultCheck(baseData, out bool isSuccess);
			baseDataRet.code = baseData.code;
			baseDataRet.message = baseData.message;

			if (isSuccess)
			{
				baseDataRet.body.objects[0] = baseData.body.objects[0].id;
			}
			return baseDataRet;
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

				var createTime = DateTime.Now;
				var operater = ApplicationState.GetUserInfo().name;

				localCommodityCodes.ForEach(it =>
				{
					it.sourceBill = sourceBill;
					it.create_time = createTime;
					it.operater = operater;
				});

				//事务防止多插入产生脏数据
				result = SqlSugarHelper.GetInstance().Db.Ado.UseTran(() =>
				{
					SqlSugarHelper.GetInstance().Db.Insertable(localCommodityCodes).ExecuteCommand();

				}).IsSuccess;
			}

			if (!result)
			{
				LogUtils.Warn("InsertLocalCommodityCodeInfo" + sourceBill);
			}

			return result;
		}


		/// <summary>
		/// 获取变化后的商品信息
		/// </summary>
		/// <returns></returns>
		public List<LocalCommodityCode> GetLocalCommodityCodeChange(InventoryChangesApo pageDataApo, out int totalCount)
		{
			totalCount = 0;
			List<LocalCommodityCode> data;

			//查询语句
			var queryable = SqlSugarHelper.GetInstance().Db.Queryable<LocalCommodityCode>()
				.Where((lcc) => lcc.operate_type == pageDataApo.operate_type)
				.WhereIF(pageDataApo.startTime.HasValue, (lcc) => lcc.create_time >= pageDataApo.startTime)
				.WhereIF(pageDataApo.endTime.HasValue, (lcc) => lcc.create_time <= pageDataApo.endTime)
				.WhereIF((!string.IsNullOrEmpty(pageDataApo.name) && !string.IsNullOrWhiteSpace(pageDataApo.name)), (lcc) => lcc.CommodityName.Contains(pageDataApo.name))
				.OrderBy((lcc) => lcc.create_time, OrderByType.Desc)
				.Select<LocalCommodityCode>();


			//如果小于0，默认查全部
			if (pageDataApo.PageSize > 0)
			{
				data = queryable.ToPageList(pageDataApo.PageIndex, pageDataApo.PageSize, ref totalCount);
			}
			else
			{
				data = queryable.ToList();
				totalCount = data.Count();
			}
			return data;
		}

		/// <summary>
		/// 插入扫描的所有商品信息
		/// </summary>
		/// <param name="currentCommodityEps">当前扫描出来的所有数据</param>
		/// <returns></returns>
		public bool InsertLocalCommodityEpsInfo(HashSet<CommodityEps> currentCommodityEps)
		{

			var result = false;

			if (currentCommodityEps == null)
			{
				return result;
			}

			//组装最新的扫描结果，一组数据
			var localCommodityEps = new LocalCommodityEps
			{
				commodityEpsList = JsonConvert.SerializeObject(new List<CommodityEps>(currentCommodityEps)),
				create_time = DateTime.Now
			};

			//事务防止多插入产生脏数据
			result = SqlSugarHelper.GetInstance().Db.Ado.UseTran(() =>
			{
				SqlSugarHelper.GetInstance().Db.Insertable(localCommodityEps).ExecuteCommand();

			}).IsSuccess;


			if (!result)
			{
				LogUtils.Warn("InsertLocalCommodityEpsInfo 失败" + DateTime.Now);
			}

			return result;
		}

		/// <summary>
		/// 判断是否是初次使用本地库存上次，如果是则不上传
		/// </summary>
		/// <returns></returns>
		public bool isInitLocalCommodityEpsInfo()
		{
			//查询语句
			var localCommodityEpsCount = SqlSugarHelper.GetInstance().Db.Queryable<LocalCommodityEps>()
				.Select<LocalCommodityEps>().Count();

			return (localCommodityEpsCount <= 0);
		}

		/// <summary>
		/// 获取上次扫描记录的商品信息
		/// </summary>
		/// <returns></returns>
		public HashSet<CommodityEps> GetLastLocalCommodityEpsInfo()
		{
			var ret = new HashSet<CommodityEps>();

			//查询语句
			var localCommodityEps = SqlSugarHelper.GetInstance().Db.Queryable<LocalCommodityEps>()
				.OrderBy((lcc) => lcc.create_time, OrderByType.Desc)
				.Select<LocalCommodityEps>().First();

			if (localCommodityEps.commodityEpsList != null)
			{
				ret = new HashSet<CommodityEps>(JsonConvert.DeserializeObject<List<CommodityEps>>(localCommodityEps.commodityEpsList));
			}

			return ret;

		}

		/// <summary>
		/// 获取变化后的商品信息的名称集合
		/// </summary>
		/// <returns></returns>
		public List<string> GetLocalCommodityName()
		{
			//查询语句
			var queryable = SqlSugarHelper.GetInstance().Db.Queryable<LocalCommodityCode>()
				.Distinct()
				.OrderBy((lcc) => lcc.create_time, OrderByType.Desc)
				.Select(it => it.CommodityName).ToList();

			return queryable;
		}


		public string GetDateTimeNow()
		{
			//return DateTime.Now.ToString("s") + "Z";
			return DateTime.UtcNow.ToString("s") + "Z";
		}

		//
		public BaseData<Commodity> GetCommodityById(string id)
		{
			BaseData<Commodity> baseData = HttpHelper.GetInstance().Get<Commodity>(new QueryParam
			{
				@in =
				{
				field = "id",
				in_list =  { HttpUtility.UrlEncode(id) }
				}
			});

			baseData = HttpHelper.GetInstance().ResultCheck(baseData, out bool isSuccess);
			return baseData;
		}

		/// <summary>
		/// 根据商品码获取完整商品属性集合
		/// </summary>
		/// <returns></returns>
		public BaseData<CommodityCode> GetCommodityCode(string commodityCodeId)
		{
			BaseData<CommodityCode> baseData = HttpHelper.GetInstance().Get<CommodityCode>(new QueryParam
			{
				@in =
				{
					field = "id",
					in_list =  { HttpUtility.UrlEncode(commodityCodeId) }
				}
			});

			return HttpHelper.GetInstance().ResultCheck(baseData);
		}


	}

}
