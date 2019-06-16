﻿using CFLMedCab.APO;
using CFLMedCab.DAL;
using CFLMedCab.DTO;
using CFLMedCab.DTO.Goodss;
using CFLMedCab.DTO.Picking;
using CFLMedCab.Infrastructure.ToolHelper;
using CFLMedCab.Model;
using CFLMedCab.Model.Enum;
using System.Collections.Generic;
using System.Linq;

namespace CFLMedCab.BLL
{
	/// <summary>
	/// 拣货出库业务层
	/// </summary>
	public class PickingBll
	{
		
		/// <summary>
		/// 获取操作实体
		/// </summary>
		private readonly PickingDal pickingDal;

		public PickingBll()
		{
			pickingDal = PickingDal.GetInstance();
		}

		/// <summary>
		/// 获取待完成拣货工单
		/// </summary>
		/// <returns></returns>
		public BasePageDataDto<PickingSubOrderDto> GetPickingSubOrderDto(BasePageDataApo basePageDataApo)
		{
			return new BasePageDataDto<PickingSubOrderDto>()
			{
				PageIndex = basePageDataApo.PageIndex,
				PageSize = basePageDataApo.PageSize,
				Data = pickingDal.GetPickingSubOrderDto(basePageDataApo, out int totalCount),
				TotalCount = totalCount
			};
		}

		/// <summary>
		/// 获取待完成拣货商品列表
		/// </summary>
		/// <returns></returns>
		public BasePageDataDto<PickingSubOrderdtlDto> GetPickingSubOrderdtlDto(PickingSubOrderdtlApo pageDataApo)
		{
			
			return new BasePageDataDto<PickingSubOrderdtlDto>()
			{
				PageIndex = pageDataApo.PageIndex,
				PageSize = pageDataApo.PageSize,
				Data = pickingDal.GetPickingSubOrderdtlDto(pageDataApo, out int totalCount),
				TotalCount = totalCount
			};
		}

		/// <summary>
		/// 获取补货操作情况
		/// </summary>
		/// <param name="pickingSubOrderid"></param>
		/// <param name="goodsDtos"></param>
		/// <returns></returns>
		public List<GoodsDto> GetPickingSubOrderdtlOperateDto(int pickingSubOrderid, List<GoodsDto> goodsDtos, out int operateGoodsNum, out int storageGoodsExNum, out int outStorageGoodsExNum)
		{

	

			//获取当前工单商品
			var pickingSubOrderdtlDtos = pickingDal.GetPickingSubOrderdtlDto(
				new PickingSubOrderdtlApo {
					picking_sub_orderid = pickingSubOrderid
				}, out int totalCount);

			goodsDtos.ForEach(it =>
			{

				//入库
				if (it.operate_type == (int)OperateType.入库)
				{
					it.operate_type_description = OperateType.入库.ToString();
					it.exception_flag = (int)ExceptionFlag.异常;
					it.exception_flag_description = ExceptionFlag.异常.ToString();
					it.exception_description = ExceptionDescription.操作与业务类型冲突.ToString();
				}
				//出库
				else if (it.operate_type == (int)OperateType.出库)
				{
					it.operate_type_description = OperateType.入库.ToString();
					//当前出库操作的商品是否在工单中存在
					if (!pickingSubOrderdtlDtos.Exists(rsoDto => rsoDto.code.Equals(it.code)))
					{
						it.exception_flag = (int)ExceptionFlag.异常;
						it.exception_flag_description = ExceptionFlag.异常.ToString();
						it.exception_description = ExceptionDescription.拣货商品不在工单目录.ToString();
					}
					else
					{
						it.exception_flag = (int)ExceptionFlag.正常;
					}
				}
			});

			//统计数量
			operateGoodsNum = goodsDtos.Count;
			storageGoodsExNum = goodsDtos.Where(it => it.operate_type == (int)OperateType.入库 && it.exception_flag == (int)ExceptionFlag.异常).Count();
			outStorageGoodsExNum = goodsDtos.Where(it => it.operate_type == (int)OperateType.出库 && it.exception_flag == (int)ExceptionFlag.异常).Count();
			
			//均升序排列
			return goodsDtos.OrderBy (it => it.exception_flag).ThenBy(it => it.expire_date).ToList();
		}

		/// <summary>
		/// 确认时，修改工单数据
		/// </summary>
		/// <param name="pickingSubOrderid">拣货单id</param>
		/// <param name="datasDto">当前操作数据dto</param>
		/// <returns></returns>
		public bool UpdatePickingStatus(int pickingSubOrderid, List<GoodsDto> datasDto)
		{

			//获取当前工单商品
			var pickingSubOrderdtlDtos = pickingDal.GetPickingSubOrderdtlDto(
				new PickingSubOrderdtlApo
				{
					picking_sub_orderid = pickingSubOrderid
				}, out int totalCount);


			//修改工单数据
			pickingSubOrderdtlDtos.ForEach(it =>
			{
				if (datasDto.Exists(dataDto => dataDto.exception_flag == (int)ExceptionFlag.正常 && it.code.Equals(dataDto.code)))
				{
					it.status = 1;
				}
			});


			return pickingDal.UpdatePickingStatus(pickingSubOrderid, pickingSubOrderdtlDtos);

		}

	}
}
