using CFLMedCab.APO;
using CFLMedCab.DAL;
using CFLMedCab.DTO;
using CFLMedCab.DTO.Goodss;
using CFLMedCab.DTO.Replenish;
using CFLMedCab.Infrastructure.ToolHelper;
using CFLMedCab.Model;
using CFLMedCab.Model.Enum;
using System.Collections.Generic;
using System.Linq;

namespace CFLMedCab.BLL
{
	/// <summary>
	/// 补货入库业务层
	/// </summary>
	public class ReplenishBll
	{
		
		/// <summary>
		/// 获取操作实体
		/// </summary>
		private readonly ReplenishDal replenishDal;

		public ReplenishBll()
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
                //Data = replenishDal.GetReplenishSubOrderDto(basePageDataApo, out int totalCount).ToList().Where(item => item.not_picked_goods_num > 0).ToList(),
                Data = replenishDal.GetReplenishSubOrderDto(basePageDataApo, out int totalCount).ToList(),
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
		public List<GoodsDto> GetReplenishSubOrderdtlOperateDto(int replenishSubOrderid, List<GoodsDto> goodsDtos, out int operateGoodsNum, out int storageGoodsExNum, out int outStorageGoodsExNum)
		{


			//获取当前工单商品
			var replenishSubOrderdtlDtos = replenishDal.GetReplenishSubOrderdtlDto(
				new ReplenishSubOrderdtlApo {
					replenish_sub_orderid = replenishSubOrderid
				}, out int totalCount);



			goodsDtos.ForEach(it =>
			{

				//入库
				if (it.operate_type == (int)OperateType.入库)
				{
					it.operate_type_description = OperateType.入库.ToString();
					//当前入库操作的商品是否在工单中存在
					if (!replenishSubOrderdtlDtos.Exists(rsoDto => rsoDto.code.Equals(it.code)))
					{
						it.exception_flag = (int)ExceptionFlag.异常;
						it.exception_flag_description = ExceptionFlag.异常.ToString();
						it.exception_description = ExceptionDescription.上架商品不在工单目录.ToString();
					}
					else
					{
						it.exception_flag = (int)ExceptionFlag.正常;
					}
				}
				//出库
				else if (it.operate_type == (int)OperateType.出库)
				{
					it.operate_type_description = OperateType.出库.ToString();
					it.exception_flag = (int)ExceptionFlag.异常;
					it.exception_flag_description = ExceptionFlag.异常.ToString();
					it.exception_description = ExceptionDescription.操作与业务类型冲突.ToString();
				}
			});

			//统计数量
			operateGoodsNum = goodsDtos.Count;
			storageGoodsExNum = goodsDtos.Where(it => it.operate_type == (int)OperateType.入库 && it.exception_flag == (int)ExceptionFlag.异常).Count();
			outStorageGoodsExNum = goodsDtos.Where(it => it.operate_type == (int)OperateType.出库 && it.exception_flag == (int)ExceptionFlag.异常).Count();

			//均升序排列
			return goodsDtos.OrderBy(it => it.exception_flag).ThenBy(it => it.expire_date).ToList();
		}



		/// <summary>
		/// 确认时，修改工单数据
		/// </summary>
		/// <param name="replenishSubOrderid">上架单id</param>
		/// <param name="datasDto">当前操作数据dto</param>
		/// <returns></returns>
		public bool UpdateReplenishStatus(int replenishSubOrderid, List<GoodsDto> datasDto)
		{

			//获取当前工单商品
			var replenishSubOrderdtlDtos = replenishDal.GetReplenishSubOrderdtlDto(
				new ReplenishSubOrderdtlApo
				{
					replenish_sub_orderid = replenishSubOrderid
				}, out int totalCount);


			//修改工单数据
			replenishSubOrderdtlDtos.ForEach(it=> 
			{
				if (datasDto.Exists(dataDto => dataDto.exception_flag == (int)ExceptionFlag.正常 && it.code.Equals(dataDto.code)))
				{
					it.status = 1;
				}
			});


			return replenishDal.UpdateReplenishStatus(replenishSubOrderid, replenishSubOrderdtlDtos);

		}

	}
}
