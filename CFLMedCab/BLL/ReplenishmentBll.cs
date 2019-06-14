using CFLMedCab.APO;
using CFLMedCab.DAL;
using CFLMedCab.DTO;
using CFLMedCab.DTO.Replenish;

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
			return new BasePageDataDto<ReplenishSubOrderDto>() {
				PageIndex = basePageDataApo.PageIndex,
				PageSize = basePageDataApo.PageSize,
				Data = replenishDal.GetReplenishSubOrderDto(basePageDataApo, out int totalCount),
				TotalCount = totalCount
			};
		}
	}
}
