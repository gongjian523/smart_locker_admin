using CFLMedCab.APO;
using CFLMedCab.DTO;
using CFLMedCab.DTO.Picking;
using CFLMedCab.Infrastructure;
using CFLMedCab.Infrastructure.DbHelper;
using CFLMedCab.Infrastructure.ToolHelper;
using CFLMedCab.Model;
using CFLMedCab.Model.Enum;
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
		/// 获取待完成拣货工单
		/// </summary>
		/// <returns></returns>
		[Obsolete]
		public List<PickingSubOrderDto> GetPickingSubOrderDto(BasePageDataApo pageDataApo, out int totalCount)
		{

			totalCount = 0;
			List<PickingSubOrderDto> data;

			//查询语句
			var queryable = Db.Queryable<PickingSubOrder, PickingOrder>((pso, po) =>new object[] {
            JoinType.Left, pso.picking_order_code == po.code})
				.Where((pso, po) => (pso.status == (int)PSOStatusType.待拣货 || pso.status == (int)PSOStatusType.部分拣货)  &&  po.principal_id == ApplicationState.GetValue<User>((int)ApplicationKey.CurUser).id)
				.OrderBy(pso => pso.create_time, OrderByType.Desc)
				.Select((pso, po) => new PickingSubOrderDto
				{
					id = pso.id,
					code = pso.code,
					picking_order_code = pso.picking_order_code,
					create_time = pso.create_time,
					status = pso.status,
					distribute_time = po.create_time,
                    position = pso.position,
                    picked_goods_num = SqlFunc.Subqueryable<PickingSubOrderdtl>()
													  .Where(itsub => itsub.picking_sub_orderid == pso.id && itsub.status == (int)RPOStatusType.待完成)
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
		/// 获取待完成拣货商品列表
		/// </summary>
		/// <returns></returns>
		[Obsolete]
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

		/// <summary>
		/// 确认时，修改工单数据
		/// </summary>
		/// <param name="pickingSubOrderid">拣货单id</param>
		/// <param name="datasDto">当前操作数据dto</param>
		/// <returns></returns>
		[Obsolete]
		public bool UpdatePickingStatus(int pickingSubOrderid, List<PickingSubOrderdtlDto> pickingSubOrderdtlDtos)
		{


			//事务防止多修改产生脏数据
			var result = Db.Ado.UseTran(() =>
			{

			
				bool isSuccess = Db.Updateable(pickingSubOrderdtlDtos.MapToList<PickingSubOrderdtlDto, PickingSubOrderdtl>()).ExecuteCommand() > 0;

				if (isSuccess)
				{
					int currentStatus;

					if (!pickingSubOrderdtlDtos.Exists(it => it.status == (int)RPOStatusType.待完成))
					{
						currentStatus = (int)PSOStatusType.已拣货;
					}
					if (!pickingSubOrderdtlDtos.Exists(it => it.status == (int)RPOStatusType.已完成))
					{
						currentStatus = (int)PSOStatusType.待拣货;
					}
					else
					{
						currentStatus = (int)PSOStatusType.部分拣货;
					}

					Db.Updateable<PickingSubOrder>()
						.SetColumns(it => new PickingSubOrder() { status = currentStatus })
						.Where(it => it.id == pickingSubOrderid)
						.ExecuteCommand();
				}

			});

			return result.IsSuccess;

		}


		/// <summary>
		/// 获取待完成拣货工单
		/// </summary>
		/// <returns></returns>
		public List<PickingOrderDto> GetPickingOrderDto(BasePageDataApo pageDataApo, out int totalCount)
		{

			totalCount = 0;
			List<PickingOrderDto> data;

			//查询语句
			var queryable = Db.Queryable<PickingOrder, PickingSubOrder>((ro, rso) => new object[] {
			JoinType.Left, rso.picking_order_code == ro.code})
				.Where((ro, rso) => (SqlFunc.Subqueryable<PickingSubOrderdtl>()
													  .Where(itsub => itsub.picking_sub_orderid == rso.id && itsub.status == (int)RPOStatusType.待完成)
													  .Count() > 0) && ro.principal_id == ApplicationState.GetValue<User>((int)ApplicationKey.CurUser).id)
				.OrderBy((ro, rso) => ro.create_time, OrderByType.Desc)
				.Select((ro, rso) => new PickingOrderDto
				{
					id = ro.id,
					code = ro.code,
					status = ro.status,
					distribute_time = ro.create_time,
					picked_goods_num = SqlFunc.Subqueryable<PickingSubOrderdtl>()
													  .Where(itsub => itsub.picking_sub_orderid == rso.id && itsub.status == (int)RPOStatusType.待完成)
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
		/// 获取待完成拣货商品列表(通过总的拣货单)
		/// </summary>
		/// <returns></returns>
		public List<PickingSubOrderdtlDto> GetPickingOrderdtlDto(PickingSubOrderdtlApo pageDataApo, out int totalCount)
		{
			totalCount = 0;
			List<PickingSubOrderdtlDto> data;

			//查询语句
			var queryable = Db.Queryable<PickingSubOrderdtl, PickingSubOrder>((psod, pso) => new object[] {
			JoinType.Left, psod.picking_sub_orderid == pso.id})
				.Where((psod, pso) => psod.status == (int)RPOStatusType.待完成 && pso.picking_order_code == pageDataApo.picking_order_code)
				.OrderBy((psod, pso) => psod.birth_date, OrderByType.Desc)
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

		/// <summary>
		/// 确认时，修改工单数据(通过总的拣货单)
		/// </summary>
		/// <param name="pickingSubOrderid">拣货单id</param>
		/// <param name="datasDto">当前操作数据dto</param>
		/// <returns></returns>
		public bool UpdatePickingStatus(string pickingSubOrderCode, List<PickingSubOrderdtlDto> pickingSubOrderdtlDtos)
		{

			return Db.Updateable(pickingSubOrderdtlDtos.MapToList<PickingSubOrderdtlDto, PickingSubOrderdtl>()).ExecuteCommand() > 0;

		}

		/// <summary>
		/// 生成拣货单号
		/// </summary>
		/// <param name="po">拣货单</param>
		/// <returns></returns>
		public string InsertPickingOrder(PickingOrder po)
        {
            return Db.Insertable(po).ExecuteReturnEntity().code;
        }

        /// <summary>
        /// 生成拣货单工号
        /// </summary>
        /// <param name="po">拣货工单</param>
        /// <returns></returns>
        public int InsertPickingSubOrder(PickingSubOrder po)
        {
            return Db.Insertable(po).ExecuteReturnEntity().id;
        }

        /// <summary>
        /// 生成拣货单号详情
        /// </summary>
        /// <param name="psodtlList">拣货工单</param>
        /// <returns></returns>
        public void InsertPickingSubOrderDetails(List<PickingSubOrderdtl> psodtlList)
        {
            Db.Insertable(psodtlList).ExecuteCommand();
        }

        /// <summary>
        /// 获得所有拣货单的个数
        /// </summary>
        /// <returns></returns>
        public int GetPickingOrderNum()
        {
            return Db.Queryable<PickingOrder>().ToList().Count;
        }

    }
}
