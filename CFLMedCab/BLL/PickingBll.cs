using AutoMapper;
using CFLMedCab.APO;
using CFLMedCab.DAL;
using CFLMedCab.DTO;
using CFLMedCab.DTO.Goodss;
using CFLMedCab.DTO.Picking;
using CFLMedCab.Infrastructure;
using CFLMedCab.Infrastructure.ToolHelper;
using CFLMedCab.Model;
using CFLMedCab.Model.Enum;
using System;
using System.Collections;
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

		/// <summary>
		/// 获取库存变化操作类
		/// </summary>
		private readonly GoodsChangeOrderDal goodsChageOrderDal;
        private readonly GoodsDal  goodsDal;

        public PickingBll()
		{
			pickingDal = PickingDal.GetInstance();
			goodsChageOrderDal = GoodsChangeOrderDal.GetInstance();
            goodsDal = GoodsDal.GetInstance();
        }


		/// <summary>
		/// 获取待完成拣货工单
		/// </summary>
		/// <returns></returns>
		[Obsolete]
		public BasePageDataDto<PickingSubOrderDto> GetPickingSubOrderDto(BasePageDataApo basePageDataApo)
		{
			return new BasePageDataDto<PickingSubOrderDto>()
			{
				PageIndex = basePageDataApo.PageIndex,
				PageSize = basePageDataApo.PageSize,
				Data = pickingDal.GetPickingSubOrderDto(basePageDataApo, out int totalCount).Where(item => item.picked_goods_num > 0).ToList(),
				TotalCount = totalCount
			};
		}

		/// <summary>
		/// 获取待完成拣货商品列表
		/// </summary>
		/// <returns></returns>
		[Obsolete]
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
		/// 确认时，修改工单数据
		/// </summary>
		/// <param name="pickingSubOrderid">拣货单id</param>
		/// <param name="datasDto">当前操作数据dto</param>
		/// <returns></returns>
		[Obsolete]
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
					it.status = (int)RPOStatusType.已完成;
				}
			});

			bool ret = pickingDal.UpdatePickingStatus(pickingSubOrderid, pickingSubOrderdtlDtos);

			//生成库存变化信息
			if (ret)
			{
				goodsChageOrderDal.InsertGoodsChageOrderInfo(datasDto, RequisitionType.退货出库);
			}

			return ret;
		}

		/// <summary>
		/// 获取待完成拣货工单
		/// </summary>
		/// <returns></returns>
		public BasePageDataDto<PickingOrderDto> GetPickingOrderDto(BasePageDataApo basePageDataApo)
		{
			return new BasePageDataDto<PickingOrderDto>()
			{
				PageIndex = basePageDataApo.PageIndex,
				PageSize = basePageDataApo.PageSize,
				Data = pickingDal.GetPickingOrderDto(basePageDataApo, out int totalCount).ToList(),
				TotalCount = totalCount
			};
		}

		/// <summary>
		/// 获取拣货操作情况
		/// </summary>
		/// <param name="pickingSubOrderid"></param>
		/// <param name="goodsDtos"></param>
		/// <returns></returns>
		public List<GoodsDto> GetPickingSubOrderdtlOperateDto(int pickingSubOrderid, string pickingOrderCode, List<GoodsDto> goodsDtos, out int operateGoodsNum, out int storageGoodsExNum, out int outStorageGoodsExNum)
		{

			//获取当前工单商品
			var pickingSubOrderdtlDtos = pickingDal.GetPickingOrderdtlDto(
				new PickingSubOrderdtlApo
				{
					picking_order_code = pickingOrderCode
				}, out int totalCount);



			goodsDtos.ForEach(it =>
			{
				//获取业务单号
				it.business_order_code = pickingOrderCode;
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
					if (!pickingSubOrderdtlDtos.Exists(rsoDto => rsoDto.code.Equals(it.code) && rsoDto.status == (int)PSOStatusType.待拣货))
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
			return goodsDtos.OrderBy(it => it.exception_flag).ThenBy(it => it.expire_date).ToList();
		}

		/// <summary>
		/// 确认时，修改工单数据
		/// </summary>
		/// <param name="pickingSubOrderid">拣货单id</param>
		/// <param name="datasDto">当前操作数据dto</param>
		/// <returns></returns>
		public bool UpdatePickingStatus(string pickingOrderCode, List<GoodsDto> datasDto)
		{

			//获取当前工单商品
			var pickingSubOrderdtlDtos = pickingDal.GetPickingOrderdtlDto(
				new PickingSubOrderdtlApo
				{
					picking_order_code = pickingOrderCode
				}, out int totalCount);


			//修改工单数据
			pickingSubOrderdtlDtos.ForEach(it =>
			{
				if (datasDto.Exists(dataDto => dataDto.exception_flag == (int)ExceptionFlag.正常 && it.code.Equals(dataDto.code)))
				{
					it.status = (int)RPOStatusType.已完成;
				}
			});

			bool ret = pickingDal.UpdatePickingStatus(pickingOrderCode, pickingSubOrderdtlDtos);

			//生成库存变化信息
			if (ret)
			{
				goodsChageOrderDal.InsertGoodsChageOrderInfo(datasDto, RequisitionType.退货出库);
			}

			return ret;

		}

		/// <summary>
		/// 模拟拣货单 
		/// </summary>
		/// <param name="rfid">单品码的RFID</param>
		/// <returns></returns>
		public void InitPickingOrder(string poCode, string psoCode, Hashtable rfid, int principalId)
        {
            pickingDal.InsertPickingOrder(new PickingOrder
            {
                code = poCode,
                create_time = DateTime.Now,
				end_time = DateTime.Now.AddDays(25),
				principal_id = principalId,
                status = (int)RPOStatusType.待完成
            });

            int i = 0;
            foreach (HashSet<string> list in rfid.Values)
            {
                i++;

                List<GoodsDto> goodsList = goodsDal.GetGoodsDto(list);

                var config = new MapperConfiguration(x => x.CreateMap<GoodsDto, PickingSubOrderdtl>()
                                            .ForMember(d => d.goods_id, o => o.MapFrom(s => s.id)));
                IMapper mapper = new Mapper(config);
                var psoDtls = mapper.Map<List<PickingSubOrderdtl>>(goodsList);

                int psoId = pickingDal.InsertPickingSubOrder(new PickingSubOrder
                {
                    code = psoCode + i.ToString(),
                    create_time = DateTime.Now,
					end_time = DateTime.Now.AddDays(25),
					position = psoDtls.First().position,
                    picking_order_code = poCode,
                    status = (int)PSOStatusType.待拣货,
                });

                psoDtls.ForEach(item =>
                {
                    item.picking_sub_orderid = psoId;
                    item.status = (int)PSOStatusType.待拣货;
                });

                pickingDal.InsertPickingSubOrderDetails(psoDtls);
            }
        }

        /// <summary>
        ///获取拣货单号的个数
        /// </summary>
        /// <returns></returns>
        public int GettPickingOrderNum()
        {
            return pickingDal.GetPickingOrderNum();
        }

    }
}
