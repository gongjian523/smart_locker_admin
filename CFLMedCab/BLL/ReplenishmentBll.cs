using CFLMedCab.APO;
using CFLMedCab.DAL;
using CFLMedCab.DTO;
using CFLMedCab.DTO.Goodss;
using CFLMedCab.DTO.Replenish;
using CFLMedCab.Infrastructure.ToolHelper;
using CFLMedCab.Model;
using System.Collections.Generic;
using System.Linq;

namespace CFLMedCab.BLL
{
	/// <summary>
	/// 补货入库业务层
	/// </summary>
	public class ReplenishmentBll
	{
		
		/// <summary>
		/// 获取操作实体
		/// </summary>
		private readonly ReplenishDal replenishDal;

		public ReplenishmentBll()
		{
			replenishDal = ReplenishDal.GetInstance();
		}

		/// <summary>
		/// 获取待完成上架工单
		/// </summary>
		/// <returns></returns>
		public BasePageDataDto<ReplenishSubOrderDto> GetReplenishSubOrderDto(BasePageDataApo basePageDataApo)
		{
			return new BasePageDataDto<ReplenishSubOrderDto>()
			{
				PageIndex = basePageDataApo.PageIndex,
				PageSize = basePageDataApo.PageSize,
				Data = replenishDal.GetReplenishSubOrderDto(basePageDataApo, out int totalCount),
				TotalCount = totalCount
			};
		}

		/// <summary>
		/// 获取待完成上架商品列表
		/// </summary>
		/// <returns></returns>
		public BasePageDataDto<ReplenishSubOrderdtlDto> GetReplenishSubOrderdtlDto(ReplenishSubOrderdtlApo pageDataApo)
		{
			
			return new BasePageDataDto<ReplenishSubOrderdtlDto>()
			{
				PageIndex = pageDataApo.PageIndex,
				PageSize = pageDataApo.PageSize,
				Data = replenishDal.GetReplenishSubOrderdtlDto(pageDataApo, out int totalCount),
				TotalCount = totalCount
			};
		}

		/// <summary>
		/// 获取补货操作情况
		/// </summary>
		/// <param name="replenishSubOrderid"></param>
		/// <param name="goodsDtos"></param>
		/// <returns></returns>
		public List<ReplenishSubOrderdtlOperateDto> GetReplenishSubOrderdtlOperateDto(int replenishSubOrderid, List<GoodsDto> goodsDtos)
		{


			//获取当前工单商品
			var replenishSubOrderdtlDtos = replenishDal.GetReplenishSubOrderdtlDto(
				new ReplenishSubOrderdtlApo {
					replenish_sub_orderid = replenishSubOrderid
				}, out int totalCount);


			var datasDto = goodsDtos.MapToList<GoodsDto, ReplenishSubOrderdtlOperateDto>();

			datasDto.ForEach(it =>
			{


				if (it.operate_type == 1)
				{
					//当前出库操作的商品是否在工单中存在
					if (replenishSubOrderdtlDtos.Exists(rsoDto => rsoDto.code.Equals(it.code)))
					{

					}
				}
				else if(it.operate_type == 0)
				{
					it.operate_type_description = OperateType.出库.ToString();
					it.exception_flag = ExceptionFlag.异常.ToString();
					it.exception_description = ExceptionDescription.操作与业务类型冲突.ToString();
				}
			});

		

			return null;
		}



	}
}
