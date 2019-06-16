using CFLMedCab.APO;
using CFLMedCab.DTO;
using CFLMedCab.DTO.Replenish;
using CFLMedCab.Infrastructure.DbHelper;
using CFLMedCab.Infrastructure.ToolHelper;
using CFLMedCab.Model;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.DAL
{
	/// <summary>
	/// 补货入库dao层
	/// </summary>
	public class ReplenishDal
	{

		//Db
		public SqlSugarClient Db;

		// 定义一个静态变量来保存类的实例
		private static ReplenishDal singleton;

		// 定义一个标识确保线程同步
		private static readonly object locker = new object();

		//定义公有方法提供一个全局访问点。
		public static ReplenishDal GetInstance()
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
						singleton = new ReplenishDal();
					}
				}
			}
			return singleton;
		}

		// 定义私有构造函数，使外界不能创建该类实例
		private ReplenishDal()
		{
			Db = SqlSugarHelper.GetInstance().Db;
		}


		/// <summary>
		/// 获取待完成上架工单
		/// </summary>
		/// <returns></returns>
		public List<ReplenishSubOrderDto> GetReplenishSubOrderDto(BasePageDataApo pageDataApo, out int totalCount)
		{

			totalCount = 0;
			List<ReplenishSubOrderDto> data;

			//查询语句
			var queryable = Db.Queryable<ReplenishSubOrder>()
				.Where(it => it.status == 0)
				.OrderBy(it => it.create_time, OrderByType.Desc)
				.Select(it => new ReplenishSubOrderDto
				{
					id = it.id,
					code = it.code,
					replenish_order_code = it.replenish_order_code,
					create_time = it.create_time,
					status = it.status,
					distribute_time = SqlFunc.Subqueryable<ReplenishOrder>()
													  .Where(itpo => itpo.code == it.replenish_order_code).Select(itpo => itpo.create_time),
					not_picked_goods_num = SqlFunc.Subqueryable<ReplenishSubOrderdtl>()
													  .Where(itsub => itsub.replenish_sub_orderid == it.id && itsub.status == 0)
													  .Count()
				});

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
		/// 获取待完成上架商品列表
		/// </summary>
		/// <returns></returns>
		public List<ReplenishSubOrderdtlDto> GetReplenishSubOrderdtlDto(ReplenishSubOrderdtlApo pageDataApo, out int totalCount)
		{
			totalCount = 0;
			List<ReplenishSubOrderdtlDto> data;

			//查询语句
			var queryable = Db.Queryable<ReplenishSubOrderdtl>()
				.Where(it => it.status == 0 && it.replenish_sub_orderid == pageDataApo.replenish_sub_orderid )
				.OrderBy(it => it.birth_date, OrderByType.Desc)
				.Select<ReplenishSubOrderdtlDto>();
			

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
		/// 确认时，修改工单数据
		/// </summary>
		/// <param name="replenishSubOrderid">上架单id</param>
		/// <param name="datasDto">当前操作数据dto</param>
		/// <returns></returns>
		public bool UpdateReplenishStatus(int replenishSubOrderid, List<ReplenishSubOrderdtlDto> replenishSubOrderdtlDtos)
		{
			

			//事务防止多插入产生脏数据
			var result = Db.Ado.UseTran(() =>
			{

				Db.Updateable(replenishSubOrderdtlDtos.MapToList<ReplenishSubOrderdtlDto, ReplenishSubOrderdtl>()).ExecuteCommand();
				if (!replenishSubOrderdtlDtos.Exists(it => it.status == 0))
				{
					Db.Updateable<ReplenishSubOrder>()
					.SetColumns(it => new ReplenishSubOrder() { status = 1 })
					.Where(it => it.id == replenishSubOrderid)
					.ExecuteCommand();
				}
				
			});

			return result.IsSuccess;

		}
		

	}
}
