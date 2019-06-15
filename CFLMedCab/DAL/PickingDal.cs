using CFLMedCab.APO;
using CFLMedCab.DTO;
using CFLMedCab.DTO.Picking;
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
	/// <summary>
	/// 补货入库dao层
	/// </summary>
	public class PickingDal
	{

		//Db
		public SqlSugarClient Db;

		// 定义一个静态变量来保存类的实例
		private static PickingDal singleton;

		// 定义一个标识确保线程同步
		private static readonly object locker = new object();

		//定义公有方法提供一个全局访问点。
		public static PickingDal GetInstance()
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
						singleton = new PickingDal();
					}
				}
			}
			return singleton;
		}

		// 定义私有构造函数，使外界不能创建该类实例
		private PickingDal()
		{
			Db = SqlSugarHelper.GetInstance().Db;
		}


		/// <summary>
		/// 获取待完成上架工单
		/// </summary>
		/// <returns></returns>
		public List<PickingSubOrderDto> GetPickingSubOrderDto(BasePageDataApo pageDataApo, out int totalCount)
		{

			totalCount = 0;
			List<PickingSubOrderDto> data;

			//查询语句
			var queryable = Db.Queryable<PickingSubOrder>()
				.Where(it => it.status == 0)
				.OrderBy(it => it.create_time, OrderByType.Desc)
				.Select(it => new PickingSubOrderDto
				{
					id = it.id,
					code = it.code,
					picking_order_code = it.picking_order_code,
					create_time = it.create_time,
					status = it.status,
					picked_goods_num = SqlFunc.Subqueryable<PickingSubOrderdtl>()
													  .Where(itsub => itsub.picking_sub_orderid == it.id && itsub.status == 0)
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
		public List<PickingSubOrderdtlDto> GetPickingSubOrderdtlDto(PickingSubOrderdtlApo pageDataApo, out int totalCount)
		{
			totalCount = 0;
			List<PickingSubOrderdtlDto> data;

			//查询语句
			var queryable = Db.Queryable<PickingSubOrderdtl>()
				.Where(it => it.status == 0 && it.picking_sub_orderid == pageDataApo.picking_sub_orderid )
				.OrderBy(it => it.birth_date, OrderByType.Desc)
				.Select<PickingSubOrderdtlDto>();
			

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


		

	}
}
