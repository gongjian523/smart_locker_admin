using CFLMedCab.Http.Enum;
using CFLMedCab.Http.Helper;
using CFLMedCab.Http.Model;
using CFLMedCab.Http.Model.Base;
using CFLMedCab.Http.Model.Common;
using CFLMedCab.Http.Model.Enum;
using CFLMedCab.Http.Model.param;
using CFLMedCab.Infrastructure;
using CFLMedCab.Infrastructure.ToolHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CFLMedCab.Http.Bll
{
	/// <summary>
	/// 领用模块相关接口
	/// </summary>
	public class ConsumingBll : BaseBll<ConsumingBll>
	{
        /// <summary>
        /// 根据领用单码查询领用单信息(领用回退不需要操作人,需要根据库房去筛选)
        /// </summary>
        /// <param name="consumingOrderName"></param>
        /// <returns></returns>
        public BaseData<ConsumingOrder> GetConsumingOrder(string consumingOrderName)
		{
			if (null == consumingOrderName)
			{
				return new BaseData<ConsumingOrder>()
				{
					code = (int)ResultCode.Parameter_Exception,
					message = ResultCode.Parameter_Exception.ToString()
				};
			}
            //获取待完成上架工单
            BaseData<ConsumingOrder> bdConsumingOrder;
            bdConsumingOrder = HttpHelper.GetInstance().Get<ConsumingOrder>(new QueryParam
            {
                view_filter =
                {
                    filter =
                    {
                        logical_relation = "1 AND 2",
                        expressions =
                        {
                            new QueryParam.Expressions
                            {
                                field = "name",
                                @operator = "==",
                                operands =  {$"'{ HttpUtility.UrlEncode(consumingOrderName) }'"}
                            },
                            new QueryParam.Expressions
                            {
                                field = "StoreHouseId",
                                @operator = "==",
                                operands = {$"'{ HttpUtility.UrlEncode(ApplicationState.GetValue<string>((int)ApplicationKey.HouseId)) }'" }
                            }
                        }
                    }
                }
            });
          
            //校验是否含有数据，如果含有数据，拼接具体字段
            bdConsumingOrder = HttpHelper.GetInstance().ResultCheck(bdConsumingOrder, out bool isSuccess);

			if (isSuccess)
			{
                bdConsumingOrder.body.objects.ForEach(it =>
				{
					//拼接库房名称
					if (!string.IsNullOrEmpty(it.StoreHouseId))
					{
						it.StoreHouseName = GetNameById<StoreHouse>(it.StoreHouseId);
					}
				});
			}

            if(bdConsumingOrder.body.objects == null)
            {
                bdConsumingOrder.code = (int)ResultCode.Result_Exception;
                bdConsumingOrder.message = ResultCode.Result_Exception.ToString();
            }
            else
            {
                //如果领⽤单作废标识为【是】则弹窗提醒手术单作废，跳转回前⻚
                if ("是".Equals(bdConsumingOrder.body.objects[0].markId) || "已完成".Equals(bdConsumingOrder.body.objects[0].Status) 
                    || "已撤销".Equals(bdConsumingOrder.body.objects[0].Status))
                {
                    bdConsumingOrder.code = (int)ResultCode.Result_Exception;
                    bdConsumingOrder.message = ResultCode.Result_Exception.ToString();
                }
            }

			return bdConsumingOrder;
        }
        /// <summary>
		/// 来源单据解析为【⼿手术单管理理】(ConsumingOrder.SourceBill.object_name=‘OperationOrder’ )：
		/// 通过【领⽤单id】(ConsumingGoodsDetail.ConsumingOrderId =ConsumingOrder.Id）从表格 【领⽤单商品明细】中查询获取领⽤商品的列表信息
        /// </summary>
        /// <param name="baseDataConsumingOrder"></param>
        /// <returns></returns>
        public BaseData<ConsumingGoodsDetail> GetOperationOrderGoodsDetail(BaseData<ConsumingOrder> baseDataConsumingOrder)
		{
			//校验是否含有数据，如果含有数据，拼接具体字段
			HttpHelper.GetInstance().ResultCheck(baseDataConsumingOrder, out bool isSuccess);
			if (!isSuccess)
			{
				return new BaseData<ConsumingGoodsDetail>()
				{
					code = (int)ResultCode.Parameter_Exception,
					message = ResultCode.Parameter_Exception.ToString()
				};
			}
			if (!"OperationOrder".Equals(baseDataConsumingOrder.body.objects[0].SourceBill.object_name))
			{
				return new BaseData<ConsumingGoodsDetail>()
				{
					code = (Int32)ResultCode.Business_Exception,
					message = ResultCode.Business_Exception.ToString()
				};
			}

			//根据领用单ID获取领用上列表信息
			BaseData<ConsumingGoodsDetail> baseOperationOrderGoodsDetail = HttpHelper.GetInstance().ResultCheck((HttpHelper hh) =>
			{
				return hh.Get<ConsumingGoodsDetail>(new QueryParam
				{
                    view_filter =
                    {
                        filter =
                        {
                            logical_relation = "1 AND 2",
                            expressions =
                            {
                                new QueryParam.Expressions
                                {
                                    field = "ConsumingOrderId",
                                    @operator = "==",
                                    operands =  {$"'{ HttpUtility.UrlEncode(baseDataConsumingOrder.body.objects[0].id) }'"}
                                },
                                new QueryParam.Expressions
                                {
                                    field = "EquipmentId",
                                    @operator = "==",
                                    operands = {$"'{ HttpUtility.UrlEncode(ApplicationState.GetEquipId()) }'" }
                                }
                            }
                        }
                    }
                });

			}, baseDataConsumingOrder);

			//校验是否含有数据，如果含有数据，拼接具体字段
			HttpHelper.GetInstance().ResultCheck(baseOperationOrderGoodsDetail, out bool isSuccess2);

			if (isSuccess2)
			{
				baseOperationOrderGoodsDetail.body.objects.ForEach(it =>
				{
					//拼接商品码名称
					if (!string.IsNullOrEmpty(it.CommodityId))
					{
						it.CommodityName = GetNameById<Commodity>(it.CommodityId);
					};

				});
			}
			return baseOperationOrderGoodsDetail;
		}

        public BaseData<ConsumingGoodsDetail> GetAllOperationOrderGoodsDetail(ConsumingOrder consumingOrder)
        {
            //根据领用单ID获取领用上列表信息
            BaseData<ConsumingGoodsDetail> baseOperationOrderGoodsDetail = HttpHelper.GetInstance().Get<ConsumingGoodsDetail>(new QueryParam
            {
                view_filter =
                {
                    filter =
                    {
                        logical_relation = "1",
                        expressions =
                        {
                            new QueryParam.Expressions
                            {
                                field = "ConsumingOrderId",
                                @operator = "==",
                                operands =  {$"'{ HttpUtility.UrlEncode(consumingOrder.id) }'"}
                            }
                        }
                    }
                }
            });

            //校验是否含有数据，如果含有数据，拼接具体字段
            HttpHelper.GetInstance().ResultCheck(baseOperationOrderGoodsDetail, out bool isSuccess2);
            if (isSuccess2)
            {
                baseOperationOrderGoodsDetail.body.objects.ForEach(it =>
                {
                    //拼接商品码名称
                    if (!string.IsNullOrEmpty(it.CommodityId))
                    {
                        it.CommodityName = GetNameById<Commodity>(it.CommodityId);
                    };

                });
            }
            return baseOperationOrderGoodsDetail;
        }


        /// <summary>
        /// 根据医嘱领用单名称，获取PrescriptionBill信息
        /// </summary>
        /// <param name="prescriptionBillName"></param>
        /// <returns></returns>
        public BaseData<PrescriptionBill> GetPrescriptionBill(string prescriptionBillName)
		{
			if (null == prescriptionBillName)
			{
				return new BaseData<PrescriptionBill>()
				{
					code = (int)ResultCode.Parameter_Exception,
					message = ResultCode.Parameter_Exception.ToString()
				};
			}
            BaseData<PrescriptionBill> prescriptionBill = HttpHelper.GetInstance().Get<PrescriptionBill>(new QueryParam
            {
                @in =
                {
                    field = "name",
                    in_list = { HttpUtility.UrlEncode(prescriptionBillName) }
                }
            });
            prescriptionBill = HttpHelper.GetInstance().ResultCheck(prescriptionBill, out bool isSuccess);
            return prescriptionBill;

		}
		/// <summary>
		/// 创建【领用单】，且领用状态为 ‘待领用’。
		/// </summary>
		/// <param name="order"></param>
		/// <returns></returns>
		public BasePostData<ConsumingOrder> CreateConsumingOrder(ConsumingOrder order)
		{
			if (null == order)
			{
				return new BasePostData<ConsumingOrder>()
				{
					code = (int)ResultCode.Parameter_Exception,
					message = ResultCode.Parameter_Exception.ToString()
				};
			}
			return HttpHelper.GetInstance().Post<ConsumingOrder>(new PostParam<ConsumingOrder>()
			{
				objects = new List<ConsumingOrder>() { order }
			});
		}
		/// <summary>
		/// 通过【领用单编号】 查找更更新【领用单】的领用状态为 ‘已完成’
		/// </summary>
		/// <param name="order"></param>
		/// <returns></returns>
		public BasePutData<ConsumingOrder> UpdateConsumingOrderStatus(ConsumingOrder order)
		{

			if (null == order || null == order.id || null == order.Status || null == order.version)
			{
				return new BasePutData<ConsumingOrder>()
				{
					code = (int)ResultCode.Parameter_Exception,
					message = ResultCode.Parameter_Exception.ToString()
				};
			}
            var consumingOrder = new ConsumingOrder()
            {
                id = order.id,//ID
                Status = order.Status,//状态
                //当主单完成状态为已完成时，携带完成时间（FinishDate）进行更新
                FinishDate = order.Status.Equals(ConsumingOrderStatus.已完成.ToString()) ? GetDateTimeNow() : null,
                version = order.version//版本
            };

            return HttpHelper.GetInstance().Put<ConsumingOrder>(consumingOrder);
		}
        /// <summary>
        /// 无单领用提交接口
        /// （2019-08-27 18:08）变更流程为：领用中->创建子表->变更主表状态[异常，已完成]
        /// 当领用过程中放进商品则主单状态异常，领用物品主单状态正常
        /// </summary>
        /// <param name="baseDataCommodityCode"></param>
        /// <returns></returns>
        public BasePostData<CommodityInventoryChange> SubmitConsumingChangeWithoutOrder(BaseData<CommodityCode> baseDataCommodityCode, ConsumingOrderType type, SourceBill  sourceBill = null)
		{
            var normalList = new List<CommodityCode>();//回退商品列表
            var lossList = new List<CommodityCode>();//领用商品列表
            var changeListOut = new List<CommodityInventoryChange>();//商品库存变更记录列表-出库
            var changeListIn = new List<CommodityInventoryChange>();//商品库存变更记录列表-入库

            baseDataCommodityCode.body.objects.ForEach(commodityCode =>
            {
                //为0标识为出库
                if (commodityCode.operate_type == 0) { lossList.Add(commodityCode); } else { normalList.Add(commodityCode); };
            });

            ConsumingOrder consumingOrder = new ConsumingOrder()
            {
                FinishDate = GetDateTimeNow(),//完成时间
                ////当入库数量大于0说明在领用的时候进行了入库操作,变更领用单状态为异常
                //Status = normalList.Count > 0 ? ConsumingOrderStatus.异常.ToString() : ConsumingOrderStatus.已完成.ToString(), //
                Status = ConsumingOrderStatus.领用中.ToString(),
                StoreHouseId = ApplicationState.GetValue<String>((int)ApplicationKey.HouseId),//领用库房
                Type = type.ToString(),//领用类型
                SourceBill = type == ConsumingOrderType.医嘱处方领用 ? sourceBill : null // 需要填写医嘱处方SourceBill
            };

            //创建领用单
            var order = CreateConsumingOrder(consumingOrder);
			//校验数据是否正常
			HttpHelper.GetInstance().ResultCheck(order, out bool isSuccess);

			if (!isSuccess)
			{
                LogUtils.Warn("CreateConsumingOrder 1:" + ResultCode.Result_Exception.ToString());
                return new BasePostData<CommodityInventoryChange>()
				{
					code = (int)ResultCode.Result_Exception,
					message = ResultCode.Result_Exception.ToString()
				};
			}
            //当正常数量大于0说明向智能柜中存放商品，需要创建商品变更记录
            if (normalList.Count > 0)
            {
                normalList.ForEach(normal =>
                {
                    changeListIn.Add(new CommodityInventoryChange()
                    {
                        CommodityCodeId = normal.id,
                        SourceBill = new SourceBill()
                        {
                            object_name = "ConsumingReturnOrder"
                        },
                        ChangeStatus = CommodityInventoryChangeStatus.正常.ToString(),
                        EquipmentId = normal.EquipmentId,
                        StoreHouseId = normal.StoreHouseId,
                        GoodsLocationId = normal.GoodsLocationId
                    });
                });
            }

            //当消耗数量大于0说明向智能柜中取出商品，需要创建商品变更记录
            if (lossList.Count > 0)
            {
                lossList.ForEach(loss =>
                {
                    changeListOut.Add(new CommodityInventoryChange()
                    {
                        CommodityCodeId = loss.id,
                        SourceBill = new SourceBill()
                        {
                            object_name = "ConsumingOrder",
                            object_id = order.body[0].id
                        },
                        ChangeStatus = CommodityInventoryChangeStatus.已消耗.ToString()
                    });
                });
            }

            //出库和入库的库存变更记录分开提交的原因是：商品改变了货柜的位置，会产生一进一出两条变更记录
            //一起提交，有可能主系统会拒绝，在没有创建领用单的情况下，就有回退记录
            var changesOut = CommodityInventoryChangeBll.GetInstance().CreateCommodityInventoryChange(changeListOut);
			//校验数据是否正常
			HttpHelper.GetInstance().ResultCheck(changesOut, out bool isSuccess2);
            if (!isSuccess2)
            {
                LogUtils.Warn("CreateConsumingOrder 2:" + ResultCode.Result_Exception.ToString());
                return changesOut;
            }

            var changesIn = CommodityInventoryChangeBll.GetInstance().CreateCommodityInventoryChange(changeListIn);
            //校验数据是否正常
            HttpHelper.GetInstance().ResultCheck(changesOut, out bool isSuccess3);
            if (!isSuccess3)
            {
                LogUtils.Warn("CreateConsumingOrder 3:" + ResultCode.Result_Exception.ToString());
                return changesIn;
            }

            ////当入库数量大于0说明在领用的时候进行了入库操作, 或者领用商品中有过期商品， 变更领用单状态为异常
            if (normalList.Count > 0 || lossList.Where(item => item.QualityStatus == QualityStatusType.过期.ToString() || item.InventoryStatus == CommodityInventoryChangeStatus.待回收.ToString()).Count() >0)
            {
                order.body[0].Status = ConsumingOrderStatus.异常.ToString();
            }
            else
            {
                order.body[0].Status = ConsumingOrderStatus.已完成.ToString();
            }
            //更新主表状态
            var orderResult = UpdateConsumingOrderStatus(order.body[0]);
            //校验数据是否正常，并记录日志
            HttpHelper.GetInstance().ResultCheck(orderResult, out bool isSuccess4);
            if (!isSuccess4)
            {
                LogUtils.Warn("CreateConsumingOrder 4:" + ResultCode.Result_Exception.ToString());
            }

            changesIn.body.AddRange(changesOut.body);
            return changesIn;
		}

        /// <summary>
        /// 有单领用数据提交
        /// （2019-08-27 18:08）变更提交顺序：
        /// 1）创建变更记录
        /// 2）创建主单状态
        /// 3）变更主单状态【已完成】，完成部分变更【进行中】，商品不在任务单内变更为【异常】
        /// 注：主单没有传完成时间，需要把字段添加上（FinishDate）
        /// </summary>
        /// <param name="baseDataCommodityCode"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public BasePostData<CommodityInventoryChange> SubmitConsumingChangeWithOrder(BaseData<CommodityCode> baseDataCommodityCode, ConsumingOrder order)
		{

			//领用类来源单据
			var sourceBill = new SourceBill()
			{
				object_name = "ConsumingOrder",
				object_id = order.id
			};

			var tempChange = CommodityInventoryChangeBll.GetInstance().CreateCommodityInventoryChange(baseDataCommodityCode, sourceBill);
			//校验数据是否正常
			HttpHelper.GetInstance().ResultCheck(tempChange, out bool isSuccess);
            if (isSuccess)
            {
                //更新领用单状态信息
                var orderResult = UpdateConsumingOrderStatus(order);
                //校验数据是否正常
                HttpHelper.GetInstance().ResultCheck(orderResult, out bool isSuccess2);
                if (!isSuccess2)
                {
                    return new BasePostData<CommodityInventoryChange>()
                    {
                        code = orderResult.code,
                        message = orderResult.message
                    };
                }
            }
           

            return tempChange;
		}
		/// <summary>
		/// 检测并变更手术类【有单领用】领用状态，和商品变动状态明细
		/// </summary>
		/// <param name="baseDataCommodityCode"></param>
		/// <param name="order"></param>
		/// <param name="operationDetail"></param>
		/// <returns></returns>
		public void GetOperationOrderChangeWithOrder(BaseData<CommodityCode> baseDataCommodityCode, ConsumingOrder order, BaseData<ConsumingGoodsDetail> consumingGoodsDetail)
		{
			HttpHelper.GetInstance().ResultCheck(consumingGoodsDetail, out bool isSuccess);

			HttpHelper.GetInstance().ResultCheck(baseDataCommodityCode, out bool isSuccess1);

			if (isSuccess && isSuccess1)
			{
                //获取这个领用单在这个设备下的所有货柜id
                List<string> locIds = consumingGoodsDetail.body.objects.Select(item => item.GoodsLocationId).Distinct().ToList();

                List<string> status = new List<string>();

                //按照货柜来处理
                locIds.ForEach(id =>
                {
                    //手术待领用商品明细
                    var operationDetails = consumingGoodsDetail.body.objects.Where(item => Convert.ToInt32(item.unusedAmount) != 0 && item.GoodsLocationId == id);
                    //获取待领用商品CommodityId列表（去重后）
                    var detailCommodityIds = operationDetails.Select(it => it.CommodityId).Distinct().ToList();
                    //变更后的Id列表
                    var commodityCodes = baseDataCommodityCode.body.objects.Where(item=> item.GoodsLocationId == id).ToList();
                    //是否主单异常
                    var IsException = false;

                    commodityCodes.ForEach(it =>
                    {
                        if (it.operate_type == (int)OperateType.入库)
                        {
                            it.AbnormalDisplay = AbnormalDisplay.异常.ToString();
                            IsException = true;
                        }
                        else
                        {
                            if (detailCommodityIds.Contains(it.CommodityId))
                            {
                                if (it.QualityStatus == QualityStatusType.过期.ToString() || it.InventoryStatus == CommodityInventoryChangeStatus.待回收.ToString())
                                {
                                    it.AbnormalDisplay = AbnormalDisplay.异常.ToString();
                                    IsException = true;
                                }
                                else
                                {
                                    it.AbnormalDisplay = AbnormalDisplay.正常.ToString();
                                }
                            }
                            else
                            {
                                it.AbnormalDisplay = AbnormalDisplay.异常.ToString();
                                IsException = true;
                            }
                        }
                    });

                    //含有异常操作
                    if (IsException)
                    {
                        status.Add(ConsumingOrderStatus.异常.ToString());
                    }
                    else
                    {
                        //变动商品明细CommodityId列表（去重后）
                        var baseDataCommodityIds = commodityCodes.Select(it => it.CommodityId).Distinct().ToList();

                        //是否名称全部一致
                        bool isAllContains = detailCommodityIds.All(baseDataCommodityIds.Contains) && baseDataCommodityIds.Count == detailCommodityIds.Count;
                        //不存在领用的商品数量超过了领用单规定的数量
                        bool isNoOver = true;
                        //所有商品的数量都和领用单规定的一样
                        bool isAllSame = true; 

                        foreach (ConsumingGoodsDetail ccd in operationDetails)
                        {
                            //详情对应的Commodity领用数量
                            var tempCount = commodityCodes.Where(cit => cit.CommodityId == ccd.CommodityId).Count();

                            //任何一种商品的数量不一致
                            if (ccd.unusedAmount != null)
                            {
                                if (Convert.ToInt32(ccd.unusedAmount) < tempCount)
                                {
                                    isNoOver = false;
                                    break;
                                }

                                if (Convert.ToInt32(ccd.unusedAmount) != tempCount)
                                {
                                    isAllSame = false;
                                }
                            }
                        }

                        //只有种类和数量完全一致的情况下，才会修改领用单状态
                        if (isAllContains && isNoOver)
                        {
                            if (isAllSame)
                            {
                                status.Add(ConsumingOrderStatus.已完成.ToString());
                            }
                            else
                            {
                                status.Add(ConsumingOrderStatus.领用中.ToString());
                            }
                        }
                        else
                        {
                            status.Add(ConsumingOrderStatus.异常.ToString());
                        }
                    }
                });

                if(status.Contains(ConsumingOrderStatus.异常.ToString()))
                {
                    order.Status = ConsumingOrderStatus.异常.ToString();
                }
                else if (status.Contains(ConsumingOrderStatus.领用中.ToString()))
                {
                    order.Status = ConsumingOrderStatus.领用中.ToString();
                }
                else
                {
                    BaseData<ConsumingGoodsDetail> bdConsumingGoodsDetail = GetAllOperationOrderGoodsDetail(order);

                    HttpHelper.GetInstance().ResultCheck(bdConsumingGoodsDetail, out bool isSuccess2);

                    if (isSuccess2)
                    {
                        if (bdConsumingGoodsDetail.body.objects.Where(item => Convert.ToInt32(item.unusedAmount) != 0 && item.EquipmentId != ApplicationState.GetEquipId()).Count() == 0)
                        {
                            order.Status = ConsumingOrderStatus.已完成.ToString();
                        }
                        else
                        {
                            order.Status = ConsumingOrderStatus.领用中.ToString();
                        }
                    }
                    else
                    {
                        LogUtils.Error("GetOperationOrderChangeWithOrder: GetAllOperationOrderGoodsDetail" + bdConsumingGoodsDetail.message);
                    }
                }
            }
		}
        /// <summary>
        /// 检测并变更手术类【无单领用】领用状态，和商品变动状态明细
        /// </summary>
        /// <param name="baseDataCommodityCode"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public void GetOperationOrderChangeWithoutOrder(BaseData<CommodityCode> baseDataCommodityCode, ConsumingOrder order)
		{
			HttpHelper.GetInstance().ResultCheck(baseDataCommodityCode, out bool isSuccess);

			if (isSuccess)
			{
				var commodityCodes = baseDataCommodityCode.body.objects;
                //记录订单异常状态
                var IsException = false;

				commodityCodes.ForEach(it =>
				{
					if (it.operate_type == (int)OperateType.入库)
					{
						it.AbnormalDisplay = AbnormalDisplay.异常.ToString();
                        IsException = true;
					}
				});
                //当商品出现入库即判定主单状态为异常
                if (IsException)
                {
                    order.Status = ConsumingOrderStatus.异常.ToString();
                }
                //凡是不存在入库情况就视为已完成
                order.Status = ConsumingOrderStatus.已完成.ToString();
			}
		}
		/// <summary>
		/// 检测并变更医嘱处方单领用状态，和商品变动状态明细
		/// </summary>
		/// <param name="baseDataCommodityCode"></param>
		/// <param name="order"></param>
		/// <param name="operationDetail"></param>
		/// <returns></returns>
		public void GetPrescriptionBillChangeWithOrder(BaseData<CommodityCode> baseDataCommodityCode, ConsumingOrder order, BaseData<PrescriptionOrderGoodsDetail> prescriptionDetail)
		{
			HttpHelper.GetInstance().ResultCheck(prescriptionDetail, out bool isSuccess);

			HttpHelper.GetInstance().ResultCheck(baseDataCommodityCode, out bool isSuccess1);

			if (isSuccess && isSuccess1)
			{
				//手术待领用商品明细
				var operationDetails = prescriptionDetail.body.objects;
				//获取待领用商品CommodityId列表（去重后）
				var detailCommodityIds = operationDetails.Select(it => it.CommodityId).Distinct().ToList();

				var commodityCodes = baseDataCommodityCode.body.objects;
                //记录订单异常状态
                var IsException = false;

				commodityCodes.ForEach(it =>
				{
					if (it.operate_type == (int)OperateType.入库)
					{
						it.AbnormalDisplay = AbnormalDisplay.异常.ToString();
                        IsException = true;
					}
					else
					{
						if (detailCommodityIds.Contains(it.CommodityId))
						{
							it.AbnormalDisplay = AbnormalDisplay.正常.ToString();
						}
						else
						{
							it.AbnormalDisplay = AbnormalDisplay.异常.ToString();
                            IsException = true;
						}
					}
				});
                //当商品出现入库即判定主单状态为异常
                if (IsException)
                {
                    order.Status = ConsumingOrderStatus.异常.ToString();
                }
                else
                {
                    //变动商品明细CommodityId列表（去重后）
                    var baseDataCommodityIds = commodityCodes.Select(it => it.CommodityId).Distinct().ToList();

                    //是否名称全部一致
                    bool isAllContains = detailCommodityIds.All(baseDataCommodityIds.Contains) && baseDataCommodityIds.Count >= detailCommodityIds.Count;

                    if (isAllContains)
                    {

                        bool isAllNormal = true;

                        foreach (PrescriptionOrderGoodsDetail oogd in operationDetails)
                        {
                            var tempCount = commodityCodes.Where(cit => cit.CommodityId == oogd.CommodityId).Count();
                            int.TryParse(oogd.Number, out int number);
                            //当领用数量小于需要领用单上的数量时，状态变更为领用中
                            if (number > tempCount)
                            {
                                order.Status = ConsumingOrderStatus.领用中.ToString();
                                isAllNormal = false;
                                break;

                            }
                        }

                        if (isAllNormal)
                        {
                            order.Status = ConsumingOrderStatus.已完成.ToString();
                        }
                    }
                    if (detailCommodityIds.Count > baseDataCommodityIds.Count)
                    {
                        order.Status = ConsumingOrderStatus.领用中.ToString();

                    }
                }
			}
		}

		/// <summary>
		/// 拼接当前机柜的库存数
		/// </summary>
		/// <param name="baseDataOogd"></param>
		/// <param name="baseDataCommodityCode"></param>
		public void CombinationStockNum(BaseData<ConsumingGoodsDetail> baseDataOogd, BaseData<CommodityCode> baseDataCommodityCode)
		{
			HttpHelper.GetInstance().ResultCheck(baseDataOogd, out bool isSuccess);

			HttpHelper.GetInstance().ResultCheck(baseDataCommodityCode, out bool isSuccess1);

			if (isSuccess && isSuccess1)
			{
				var OogdList = baseDataOogd.body.objects;

				var CommodityCodeList = baseDataCommodityCode.body.objects;

				OogdList.ForEach(it => 
				{
					it.stockNum = CommodityCodeList.Where(cit => cit.CommodityId == it.CommodityId).Count();
                });
			}
		}

		/// <summary>
		/// 拼接当前机柜的库存数
		/// </summary>
		/// <param name="baseDataOogd"></param>
		/// <param name="baseDataCommodityCode"></param>
		public void CombinationStockNum(BaseData<PrescriptionOrderGoodsDetail> baseDataOogd, BaseData<CommodityCode> baseDataCommodityCode)
		{
			HttpHelper.GetInstance().ResultCheck(baseDataOogd, out bool isSuccess);

			HttpHelper.GetInstance().ResultCheck(baseDataCommodityCode, out bool isSuccess1);

			if (isSuccess && isSuccess1)
			{


				var OogdList = baseDataOogd.body.objects;

				var CommodityCodeList = baseDataCommodityCode.body.objects;


				OogdList.ForEach(it =>
				{
					it.stockNum = CommodityCodeList.Where(cit => cit.CommodityId == it.CommodityId).Count();
				});
			}

		}

	}

}
