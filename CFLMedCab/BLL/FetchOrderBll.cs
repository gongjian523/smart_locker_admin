using CFLMedCab.APO.Surgery;
using CFLMedCab.DAL;
using CFLMedCab.DTO;
using CFLMedCab.DTO.Fetch;
using CFLMedCab.DTO.Goodss;
using CFLMedCab.DTO.Surgery;
using CFLMedCab.Model.Enum;
using System.Collections.Generic;
using System.Linq;

namespace CFLMedCab.BLL
{

    public class FetchOrderBll
    {
        private readonly FetchOrderDal FetchOrderDal  = FetchOrderDal.GetInstance();

		private readonly GoodsChangeOrderDal GoodsChangeOrderDal = GoodsChangeOrderDal.GetInstance();

		#region 通用业务处理代码


		/// <summary>
		/// 生成领用单信息和库存变化单信息
		/// </summary>
		/// <param name="goodsDtos">操作变化数据</param>
		/// <param name="requisitionType">业务类型</param>
		/// <param name="surgeryOrderCode">手术单号，如果是有单手术业务的情况</param>
		/// <returns></returns>
		public bool InsertFetchAndGoodsChangeInfo(List<GoodsDto> operateGoodsDtos, RequisitionType requisitionType, string surgeryOrderCode)
		{
			bool ret = false;

			string goodsChangeBusinessOrderCode = null;

			switch (requisitionType)
			{
				case RequisitionType.一般领用:
					ret = FetchOrderDal.InsertFetchOrderInfo(operateGoodsDtos, new GoodsChageAttribute
					{
						RequisitionType = requisitionType,
						RequisitionStatus = RequisitionStatus.已认领,
						ConsumablesStatus = ConsumablesStatus.已领用,
					}, surgeryOrderCode, out goodsChangeBusinessOrderCode);

					//生成存库变化单
					if (ret)
					{
						ret = GoodsChangeOrderDal.InsertGoodsChageOrderInfo(operateGoodsDtos, goodsChangeBusinessOrderCode, requisitionType);
					}

					break;
				case RequisitionType.无单手术领用:
					ret = FetchOrderDal.InsertFetchOrderInfo(operateGoodsDtos, new GoodsChageAttribute
					{
						RequisitionType = requisitionType,
						RequisitionStatus = RequisitionStatus.待认领,
						ConsumablesStatus = ConsumablesStatus.已领用,
					}, null, out goodsChangeBusinessOrderCode);

					//生成存库变化单
					if (ret)
					{
						ret = GoodsChangeOrderDal.InsertGoodsChageOrderInfo(operateGoodsDtos, goodsChangeBusinessOrderCode, requisitionType);
					}
					break;
				case RequisitionType.有单手术领用:
					ret = FetchOrderDal.InsertFetchOrderInfo(operateGoodsDtos, new GoodsChageAttribute
					{
						RequisitionType = requisitionType,
						RequisitionStatus = RequisitionStatus.已认领,
						ConsumablesStatus = ConsumablesStatus.已领用,
					}, null, out goodsChangeBusinessOrderCode);

					//生成存库变化单
					if (ret)
					{
						ret = GoodsChangeOrderDal.InsertGoodsChageOrderInfo(operateGoodsDtos, goodsChangeBusinessOrderCode, requisitionType);
					}
					break;
				default:
					break;
			}

			

			

			return ret;
		}
		#endregion

		#region 一般领用业务

		/// <summary>
		///  获取一般领用操作情况
		/// </summary>
		/// <param name="goodsDtos"></param>
		/// <returns></returns>
		public List<GoodsDto> GetGeneralFetchOrderdtlOperateDto(List<GoodsDto> goodsDtos)
		{

			//组装当前状态
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
					it.operate_type_description = OperateType.出库.ToString();

					if (it.fetch_type == (int)RequisitionAttribute.无单领用)
					{
						it.exception_flag = (int)ExceptionFlag.正常;
					}
					else
					{
						it.exception_flag = (int)ExceptionFlag.异常;
						it.exception_flag_description = ExceptionFlag.异常.ToString();
						it.exception_description = ExceptionDescription.领用属性与业务类型冲突.ToString();
					}
				}
			});


			//均升序排列
			return goodsDtos.OrderBy(it => it.exception_flag).ThenBy(it => it.expire_date).ToList();
		}

		#endregion

		#region 手术无单领用业务

		/// <summary>
		///  获取一般领用操作情况
		/// </summary>
		/// <param name="goodsDtos"></param>
		/// <returns></returns>
		public List<GoodsDto> GetSurgeryFetchOrderdtlOperateDto(List<GoodsDto> goodsDtos)
		{

			//组装当前状态
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
					it.operate_type_description = OperateType.出库.ToString();

					if (it.fetch_type == (int)RequisitionAttribute.无单领用)
					{
						it.exception_flag = (int)ExceptionFlag.正常;
					}
					else
					{
						it.exception_flag = (int)ExceptionFlag.异常;
						it.exception_flag_description = ExceptionFlag.异常.ToString();
						it.exception_description = ExceptionDescription.领用属性与业务类型冲突.ToString();
					}
				}
			});


			//均升序排列
			return goodsDtos.OrderBy(it => it.exception_flag).ThenBy(it => it.expire_date).ToList();
		}

		#endregion

		#region 手术领用有单业务

		/// <summary>
		/// 根据手术单号查询对应手术单号
		/// </summary>
		/// <param name="pageDataApo">带分页的pageDataApo</param>
		/// <returns></returns>
		public BasePageDataDto<SurgeryOrderDto> GetSurgeryOrderDto(SurgeryOrderApo pageDataApo)
		{
			return new BasePageDataDto<SurgeryOrderDto>()
			{
				PageIndex = pageDataApo.PageIndex,
				PageSize = pageDataApo.PageSize,
				Data = FetchOrderDal.GetSurgeryOrderDto(pageDataApo, out int totalCount),
				TotalCount = totalCount
			};
		}


		/// <summary>
		/// 根据手术单号查询耗材详情
		/// </summary>
		/// <param name="pageDataApo"></param>
		/// <param name="stockGoodsNum">商品有库存的数量</param>
		/// <param name="notStockGoodsNum">商品库存不足的数量</param>
		/// <returns></returns>
		public BasePageDataDto<SurgeryOrderdtlDto> GetSurgeryOrderdtlDto(SurgeryOrderApo pageDataApo,  out int stockGoodsNum, out int notStockGoodsNum)
		{
			
			BasePageDataDto<SurgeryOrderdtlDto> ret = GetSurgeryOrderdtlDto(pageDataApo);

			List<SurgeryOrderdtlDto> surgeryOrderdtlDtos = ret.Data;
		

			//手术单耗材详情拼接当前柜子对应商品库存数量
			surgeryOrderdtlDtos.ForEach(it => 
			{
				it.stock_num = pageDataApo.GoodsDtos.Where(goodsDto => goodsDto.fetch_type == (int)RequisitionAttribute.有单领用 && goodsDto.goods_code == it.goods_code).Count();
			});

			//库存满足的商品的数量
			stockGoodsNum = surgeryOrderdtlDtos.Where(surgeryOrderdtlDto =>
				surgeryOrderdtlDto.not_fetch_num <= surgeryOrderdtlDto.stock_num
			).Count();

			//库存不满足满足的商品的数量
			notStockGoodsNum = surgeryOrderdtlDtos.Count() - stockGoodsNum;

			return ret;

		}

		/// <summary>
		/// 根据手术单号查询耗材详情（全部显示）
		/// </summary>
		/// <param name="pageDataApo"></param>
		/// <param name="stockGoodsNum">商品有库存的数量</param>
		/// <param name="notStockGoodsNum">商品库存不足的数量</param>
		/// <returns></returns>
		public BasePageDataDto<SurgeryOrderdtlDto> GetSurgeryOrderdtlDto(SurgeryOrderApo pageDataApo)
		{

			List<SurgeryOrderdtlDto> surgeryOrderdtlDtos = FetchOrderDal.GetSurgeryOrderdtlDto(pageDataApo, out int totalCount);

			return new BasePageDataDto<SurgeryOrderdtlDto>()
			{
				PageIndex = pageDataApo.PageIndex,
				PageSize = pageDataApo.PageSize,
				Data = surgeryOrderdtlDtos,
				TotalCount = totalCount
			};

		}


		/// <summary>
		/// 获取手术有单领用操作情况
		/// </summary>
		/// <param name="goodsDtos">一次操作变化信息</param>
		/// <param name="surgeryOrderCode">手术单号</param>
		/// <param name="currentOperateNum">当前操作的数量</param>
		/// <param name="storageOperateExNum">异常入库数量</param>
		/// <param name="notStorageOperateExNum">异常出库数量</param>
		/// <returns></returns>
		public List<GoodsDto> GetSurgeryGoodsOperateDto(List<GoodsDto> goodsDtos, string surgeryOrderCode, out int currentOperateNum, out int storageOperateExNum, out int notStorageOperateExNum)
		{
			//获取该手术单号下所有商品
			List<SurgeryOrderdtlDto> surgeryOrderdtlDtos = FetchOrderDal.GetSurgeryOrderdtlDto(new SurgeryOrderApo {
				SurgeryOrderCode = surgeryOrderCode
			}, out int totalCount);

			//组装当前状态
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
					it.operate_type_description = OperateType.出库.ToString();

					if (it.fetch_type == (int)RequisitionAttribute.有单领用)
					{
						
						//注意：此逻辑无法控制是否超过待取数量，仅能控制是否在待领用的领用单里
						if (surgeryOrderdtlDtos.Exists(surgeryOrderdtlDto => it.goods_code == surgeryOrderdtlDto.goods_code))
						{
							it.exception_flag = (int)ExceptionFlag.正常;
						}
						else
						{
							it.exception_flag = (int)ExceptionFlag.异常;
							it.exception_flag_description = ExceptionFlag.异常.ToString();
							it.exception_description = ExceptionDescription.商品不在待领目录.ToString();
						}
					}
					else
					{
						it.exception_flag = (int)ExceptionFlag.异常;
						it.exception_flag_description = ExceptionFlag.异常.ToString();
						it.exception_description = ExceptionDescription.领用属性与业务类型冲突.ToString();
					}
				}
			});

			currentOperateNum = goodsDtos.Count();

			storageOperateExNum = goodsDtos.Where(it => it.exception_flag == (int)ExceptionFlag.异常 && it.operate_type == (int)OperateType.入库).Count();

			notStorageOperateExNum = goodsDtos.Where(it => it.exception_flag == (int)ExceptionFlag.异常 && it.operate_type == (int)OperateType.出库).Count();

			//均升序排列
			return goodsDtos.OrderBy(it => it.exception_flag).ThenBy(it => it.expire_date).ToList();
		}


		/// <summary>
		/// 根据手术单号查询耗材详情（临时显示，不做入库逻辑）
		/// </summary>
		/// <param name="pageDataApo"></param>
		/// <param name="not_fetch_goods_num">仍然待领取商品的数量</param>
		/// <returns></returns>
		public BasePageDataDto<SurgeryOrderdtlDto> GetSurgeryOrderdtlOperateDto(SurgeryOrderApo pageDataApo, out int notFetchGoodsNum)
		{
			//获取手术单详情
			List<SurgeryOrderdtlDto> surgeryOrderdtlDtos = FetchOrderDal.GetSurgeryOrderdtlDto(pageDataApo, out int totalCount);

			//获取一次操作变化的商品集
			List<GoodsDto> operateGoodsDtos = pageDataApo.OperateGoodsDtos;

			//获取当前库存商品集
			List<GoodsDto> goodsDtos = pageDataApo.GoodsDtos;

			//手术单耗材详情拼接当前柜子对应商品库存数量
			surgeryOrderdtlDtos.ForEach(it =>
			{
				//对应正常领取商品数量
				int normalOperateCount = operateGoodsDtos.Where(goodsDto => goodsDto.goods_code == it.goods_code && goodsDto.exception_flag == (int)ExceptionFlag.正常).Count();

				//对应商品的库存数
				int goodsStockCount = goodsDtos.Where(goodsDto => goodsDto.fetch_type == (int)RequisitionAttribute.有单领用 && goodsDto.goods_code == it.goods_code).Count();

				//已经领用数量
				it.already_fetch_num = normalOperateCount;

				//根据操作的商品数据临时调整代取数量(会有多取的，所以默认多取，也满足正常取)
				it.not_fetch_num = it.not_fetch_num - it.already_fetch_num >= 0 ? it.not_fetch_num - it.already_fetch_num : 0;

				it.stock_num = goodsStockCount;
			});

			notFetchGoodsNum = surgeryOrderdtlDtos.Where(it => it.not_fetch_num > 0).Count();

			return new BasePageDataDto<SurgeryOrderdtlDto>()
			{
				PageIndex = pageDataApo.PageIndex,
				PageSize = pageDataApo.PageSize,
				//过滤无需代取的商品，此步骤留给前端做，保证处理后数据的完整性，以便可直接做入库修改逻辑
				//Data = surgeryOrderdtlDtos.Where(it=>it.not_fetch_num > 0).ToList(),
				Data = surgeryOrderdtlDtos,
				TotalCount = totalCount
			};

		}

		/// <summary>
		/// 确认时，修改手术领用详情数据
		/// </summary>
		/// <param name="replenishSubOrderid">上架单id</param>
		/// <param name="datasDto">当前操作数据dto</param>
		/// <returns></returns>
		public bool UpdateSurgeryOrderdtl(SurgeryOrderApo pageDataApo, List<SurgeryOrderdtlDto> datasDto)
		{
			bool ret = FetchOrderDal.UpdateSurgeryOrderdtl(datasDto);


			//生成有单领用单信息和库存变化单信息
			if (ret)
			{
				ret = InsertFetchAndGoodsChangeInfo(pageDataApo.OperateGoodsDtos, RequisitionType.有单手术领用, pageDataApo.SurgeryOrderCode);
			}
			return ret;

		}

		/// <summary>
		/// 修改操作变化dto里,某异常商品为正常
		/// </summary>
		/// <param name="operateGoodsDtos">一次操作变化的商品集</param>
		/// <param name="code">该商品单品号</param>
		/// <returns></returns>
		public bool UpdateSurgeryOrderdtl(List<GoodsDto> operateGoodsDtos, string code)
		{

			bool ret = false;

			var currentGoodDtos = operateGoodsDtos.Where(it => it.code == code && it.exception_flag == (int)ExceptionFlag.异常).ToList();

			operateGoodsDtos.ForEach(it => 
			{
				if (it.code == code && it.exception_flag == (int)ExceptionFlag.异常)
				{
					it.exception_flag = (int)ExceptionFlag.正常;
					it.exception_flag_description = "";
					it.exception_description = "";
					ret = true;
				}
			});

			return ret;

		}

		#endregion

		#region 领用退回

		/// <summary>
		///  获取一般领用操作情况
		/// </summary>
		/// <param name="goodsDtos"></param>
		/// <returns></returns>
		public List<GoodsDto> GetGoBackFetchOrderdtlOperateDto(List<GoodsDto> goodsDtos)
		{

			//组装当前状态
			goodsDtos.ForEach(it =>
			{
				FetchOrderDto fetchOrderDto = FetchOrderDal.GetFetchOrderCode(it.code);

				//入库
				if (it.operate_type == (int)OperateType.入库)
				{
					it.operate_type_description = OperateType.入库.ToString();
					//查到商品的对应的领用单号
					if (fetchOrderDto != null)
					{
						it.business_order_code = fetchOrderDto.code;
						it.fetch_type = (int)RequisitionAttribute.有单领用;
						if (fetchOrderDto.type == (int)RequisitionType.无单手术领用 || fetchOrderDto.type == (int)RequisitionType.有单手术领用 || fetchOrderDto.type == (int)RequisitionType.一般领用)
						{
							it.exception_flag = (int)ExceptionFlag.正常;
						}
					}
					else
					{
						it.exception_flag = (int)ExceptionFlag.异常;
						it.exception_flag_description = ExceptionFlag.异常.ToString();
						it.exception_description = ExceptionDescription.未查到领用记录.ToString();
					}
				}
				//出库
				else if (it.operate_type == (int)OperateType.出库)
				{
					it.operate_type_description = OperateType.出库.ToString();

					//查到商品的对应的领用单号,改变领用属性
					if (fetchOrderDto != null)
					{
						it.fetch_type = (int)RequisitionAttribute.有单领用;
					}

					it.exception_flag = (int)ExceptionFlag.异常;
					it.exception_flag_description = ExceptionFlag.异常.ToString();
					it.exception_description = ExceptionDescription.操作与业务类型冲突.ToString();

				}
			});

			//均升序排列
			return goodsDtos.OrderBy(it => it.exception_flag).ThenBy(it => it.expire_date).ToList();
		}

		/// <summary>
		/// 更新领用单的详情的回退信息(确定时)
		/// </summary>
		/// <param name="datasDto">当前操作数据dto</param>
		/// <returns></returns>
		public bool UpdateGoBackFetchOrder(List<GoodsDto> datasDto)
		{
			//获取对应领用单的详情数据
			var fetchOrderDtls = FetchOrderDal.GetFetchOrderDtlsByGoBackGoodsDtos(datasDto);

			if (fetchOrderDtls == null || fetchOrderDtls.Count() <= 0) return false;

			fetchOrderDtls.ForEach(it=> 
			{
				it.status = (int)ConsumablesStatus.已退回;
			});

			bool ret = false;

			ret = FetchOrderDal.UpdateGoBackFetchOrderdtls(fetchOrderDtls);

			//如果成功，修改库存变化
			if (ret)
			{
				GoodsChangeOrderDal.InsertGoodsChageOrderInfo(datasDto, RequisitionType.领用回退);
			}

			return ret; 

		}

		#endregion

	}
}
