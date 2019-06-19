
using CFLMedCab.APO.Surgery;
using CFLMedCab.DTO.Fetch;
using CFLMedCab.DTO.Goodss;
using CFLMedCab.DTO.Surgery;
using CFLMedCab.Infrastructure;
using CFLMedCab.Infrastructure.DbHelper;
using CFLMedCab.Infrastructure.ToolHelper;
using CFLMedCab.Model;
using CFLMedCab.Model.Enum;
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


		/// <summary>
		///  生成领用信息(领用单号和领用详情)
		/// </summary>
		/// <param name="goodsDtos">操作数据</param>
		/// <returns></returns>
		public bool InsertFetchOrderInfo(List<GoodsDto> goodsDtos, GoodsChageAttribute goodsChageAttribute, string surgeryOrderCode, out string goodsChangeBusinessOrderCode)
		{
			goodsChangeBusinessOrderCode = null;

			if (goodsDtos.Count <= 0)
			{
				return false;
			}

			//修改商品里的数据为有单领用
			goodsDtos.ForEach(it => 
			{
				if (it.exception_flag == (int)ExceptionFlag.正常 && it.fetch_type == (int)RequisitionAttribute.无单领用)
				{
					it.fetch_type = (int)RequisitionAttribute.有单领用;
				}
			});
			

			List<GoodsDto> goodsDtosNotEx = goodsDtos.Where(it => it.exception_flag == (int)ExceptionFlag.正常).ToList();

			if (goodsDtosNotEx.Count <= 0)
			{
				return false;
			}

			//生成领用单的业务单号，根据实际情况
			string business_order_code;

			switch (goodsChageAttribute.RequisitionType)
			{
				case RequisitionType.一般领用:
					business_order_code = "-";
					break;
				case RequisitionType.无单手术领用:
					business_order_code = "";
					break;
				case RequisitionType.有单手术领用:
					business_order_code = surgeryOrderCode;
					break;
				default:
					business_order_code = "";
					break;
			}

			//事务防止多插入产生脏数据
			var result = Db.Ado.UseTran(() =>
			{

				//领用单id
				FetchOrder fetchOrder = Db.Insertable(new FetchOrder
				{
					//用uuid当作新生成的领用单单号
					code = Guid.NewGuid().ToString("N"),
					create_time = DateTime.Now,
				    operator_id = ApplicationState.GetValue<User>((int)ApplicationKey.CurUser).id,
					type = (int)goodsChageAttribute.RequisitionType,
					status = (int)goodsChageAttribute.RequisitionStatus,
					business_order_code = business_order_code
					

				}).ExecuteReturnEntity();

				

				List<FetchOrderdtl> fetchOrderdtls = goodsDtosNotEx.MapToListIgnoreId<GoodsDto, FetchOrderdtl>();

				fetchOrderdtls.ForEach(it =>
				{
					it.is_add = 0;
					it.related_order_id = fetchOrder.id;
					it.status = (int)goodsChageAttribute.ConsumablesStatus;
				});

				Db.Insertable(fetchOrderdtls).ExecuteCommand();

				return fetchOrder;

			});

			//返回用于生成库存变化单的业务单号
			if (result.IsSuccess)
			{
				goodsChangeBusinessOrderCode = result.Data.code;
			}


			return result.IsSuccess;
		}


		/// <summary>
		/// 根据手术单号查询对应手术单号
		/// </summary>
		/// <param name="pageDataApo">带分页的pageDataApo</param>
		/// <param name="totalCount">返回的总数</param>
		/// <returns></returns>
		public List<SurgeryOrderDto> GetSurgeryOrderDto(SurgeryOrderApo pageDataApo, out int totalCount)
		{

			totalCount = 0;
			List<SurgeryOrderDto> data;

			//查询语句
			var queryable = Db.Queryable<SurgeryOrder>()
				.Where(it => it.code.StartsWith(pageDataApo.SurgeryOrderCode))
				.Select<SurgeryOrderDto>();


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
		/// 根据手术单号查询耗材详情
		/// </summary>
		/// <param name="surgeryOrderCode"></param>
		/// <returns></returns>
		public List<SurgeryOrderdtlDto> GetSurgeryOrderdtlDto(SurgeryOrderApo pageDataApo, out int totalCount)
		{

			totalCount = 0;
			List<SurgeryOrderdtlDto> data;

			//查询语句(查询出手术单待领用的耗材，已领用好的不显示)
			var queryable = Db.Queryable<SurgeryOrderdtl>()
				.Where(it => it.surgery_order_code == pageDataApo.SurgeryOrderCode && it.not_fetch_num > 0)
				.Select<SurgeryOrderdtlDto>();


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
		/// 根据手术单号查询耗材详情（全部）
		/// </summary>
		/// <param name="surgeryOrderCode"></param>
		/// <returns></returns>
		public List<SurgeryOrderdtlDto> GetSurgeryOrderdtlAllDto(SurgeryOrderApo pageDataApo, out int totalCount)
		{

			totalCount = 0;
			List<SurgeryOrderdtlDto> data;

			//查询语句(查询全部出手术单待领用的耗材)
			var queryable = Db.Queryable<SurgeryOrderdtl>()
				.Where(it => it.surgery_order_code == pageDataApo.SurgeryOrderCode)
				.Select<SurgeryOrderdtlDto>();


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
		/// 确认时，修改手术领用详情数据
		/// </summary>
		/// <param name="datasDto">当前操作数据dto</param>
		/// <returns></returns>
		public bool UpdateSurgeryOrderdtl(List<SurgeryOrderdtlDto> datasDto)
		{
            return Db.Updateable(datasDto.MapToList<SurgeryOrderdtlDto, SurgeryOrderdtl>()).ExecuteCommand() > 0;
		}

		/// <summary>
		/// 获取领取单信息，根据商品code
		/// </summary>
		/// <param name="code">商品的单品号</param>
		/// <returns></returns>
		public FetchOrderDto GetFetchOrderCode(string code)
		{
			return Db.Queryable<FetchOrderdtl, FetchOrder>((fod,fo) => new object[] { JoinType.Left,fod.related_order_id == fo.id})
				.Where(fod=>fod.code == code)
				.Select<FetchOrderDto>().First();
		}

		/// <summary>
		/// 根据库存变化信息查询对应领用单里的详情
		/// </summary>
		/// <param name="goodsDtos"></param>
		/// <returns></returns>
		public List<FetchOrderdtl> GetFetchOrderDtlsByGoBackGoodsDtos(List<GoodsDto> goBackGoodsDtos)
		{
			var goodsCodes = goBackGoodsDtos.Where(it => it.exception_flag == (int)ExceptionFlag.正常).Select(it => it.code).ToArray();
			return Db.Queryable<FetchOrderdtl>().Where(it => goodsCodes.Contains(it.code)).ToList();
		}

		/// <summary>
		/// 更新领用单的详情的回退信息
		/// </summary>
		/// <param name="fetchOrderdtls">已修改的集合</param>
		/// <returns></returns>
		public bool UpdateGoBackFetchOrderdtls(List<FetchOrderdtl> fetchOrderdtls)
		{
			if (fetchOrderdtls == null) return false;
			if (fetchOrderdtls.Count() <= 0) return false;
			return Db.Updateable(fetchOrderdtls).ExecuteCommand() > 0;
		}


	}
}
