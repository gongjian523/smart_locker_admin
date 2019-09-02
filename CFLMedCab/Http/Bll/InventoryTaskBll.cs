using CFLMedCab.Http.Enum;
using CFLMedCab.Http.Helper;
using CFLMedCab.Http.Model;
using CFLMedCab.Http.Model.Base;
using CFLMedCab.Http.Model.param;
using CFLMedCab.Infrastructure;
using CFLMedCab.Infrastructure.QuartzHelper.scheduler;
using CFLMedCab.Infrastructure.ToolHelper;
using Quartz;
using Quartz.Impl.Matchers;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CFLMedCab.Http.Bll
{
	public class InventoryTaskBll : BaseBll<InventoryTaskBll>
	{
		/// <summary>
		/// 根据盘点任务单号查询盘点任务信息
		/// </summary>
		/// <param name="taskName"></param>
		/// <returns></returns>
		public BaseData<InventoryTask> GetInventoryTaskByInventoryTaskName(string taskName)
		{
			if (null == taskName)
			{
				return new BaseData<InventoryTask>()
				{
					code = (int)ResultCode.Parameter_Exception,
					message = ResultCode.Parameter_Exception.ToString()
				};
			}
			//获取待盘点列表
			var task = HttpHelper.GetInstance().Get<InventoryTask>(new QueryParam
			{
				view_filter =
				{
					filter =
					{
                        logical_relation = "1 AND 2 AND (3 OR 4) ",
                        expressions =
						{
							new QueryParam.Expressions
							{
								field = "name",
								@operator = "==",
								operands =  {$"'{ HttpUtility.UrlEncode(taskName) }'"}
							},
                            new QueryParam.Expressions
                            {
                                field = "Operator",
                                @operator = "==",
                                operands = {$"'{ HttpUtility.UrlEncode(ApplicationState.GetUserInfo().id)}'"}
                            },
                            new QueryParam.Expressions
                            {
                                field = "Status",
                                @operator = "==",
                                operands = {$"'{ HttpUtility.UrlEncode(InventoryTaskStatus.待盘点.ToString()) }'" }
                            },
                            new QueryParam.Expressions
                            {
                                field = "Status",
                                @operator = "==",
                                operands = {$"'{ HttpUtility.UrlEncode(InventoryTaskStatus.盘点中.ToString()) }'" }
                            }
                        }
					}
				}
			});
			//校验是否含有数据，如果含有数据，拼接具体字段
			HttpHelper.GetInstance().ResultCheck(task, out bool isSuccess);

			return task;
		}
		/// <summary>
		/// 【手动盘点】获取盘点任务相关信息
		/// 通过【盘点任务名称】从表格InvoentoryTask【盘点任务】中查询获取盘点任务id。
		/// 通过【盘点任务单】（InventoryTask.id =InventoryOrder.InventoryTaskId）从表格【盘点单】中查询获得盘点单列列表
		/// taskName 盘点任务名称
		/// </summary>
		/// <param name="taskName"></param>
		/// <returns></returns>
		public BaseData<InventoryOrder> GetInventoryOrdersByInventoryTaskName(string taskName)
		{
			var task = GetInventoryTaskByInventoryTaskName(taskName);

			//校验是否含有数据，如果含有数据，拼接具体字段
			HttpHelper.GetInstance().ResultCheck(task, out bool isSuccess);

			var orders = new BaseData<InventoryOrder>();

			if (isSuccess)
			{
				//确认数据只有一条
				if (!string.IsNullOrEmpty(task.body.objects[0].id))//id = AQACQqweJ4wBAAAAXRA4vCD_sxWaDwQA
				{
					orders = HttpHelper.GetInstance().Get<InventoryOrder>(new QueryParam
					{
						view_filter =
						{
							filter =
							{
								logical_relation = "1 AND 2 AND 3 AND 4",
								expressions =
								{
									new QueryParam.Expressions
									{
										field = "InventoryTaskId",
										@operator = "==",
										operands =  {$"'{ HttpUtility.UrlEncode(task.body.objects[0].id) }'"}
									},
									new QueryParam.Expressions
									{
										field = "Status",
										@operator = "==",
										operands = {$"'{ HttpUtility.UrlEncode(InventoryTaskStatus.待盘点.ToString()) }'" }
									},
                                    new QueryParam.Expressions
                                    {
                                        field = "StoreHouseId",
                                        @operator = "==",
                                        operands = {$"'{ HttpUtility.UrlEncode(ApplicationState.GetHouseId()) }'" }
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
					//校验是否含有数据，如果含有数据，拼接具体字段
					HttpHelper.GetInstance().ResultCheck(orders, out bool isSuccess2);
					if (isSuccess2)
					{
						orders.body.objects.ForEach(order =>
						{
							//根据所在盘点货位获取所在货位名称
							if (!string.IsNullOrEmpty(order.GoodsLocationId))
							{
								order.GoodsLocationName = GetNameById<GoodsLocation>(order.GoodsLocationId);
							}
							//根据所在设备编号查询设备名称
							if (!string.IsNullOrEmpty(order.EquipmentId))
							{
								order.EquipmentName = GetNameById<Equipment>(order.EquipmentId);
							}
							//根据盘点库房编号查询盘点库房名称
							if (!string.IsNullOrEmpty(order.StoreHouseId))
							{
								order.StoreHouseName = GetNameById<StoreHouse>(order.StoreHouseId);
							}
						});
					}
				}
			}
			else
			{
				orders.code = task.code;
				orders.message = task.message;
			}
			return orders;
		}

		/// <summary>
		/// 【手动盘点】 更新【盘点单管理理】的盘点状态为 ‘已完成’
		/// InventoryOrder关键字段：
		/// id 主键
		/// Status 盘点状态
		/// version 数据版本号【必须和当前数据保持一致】
		/// </summary>
		/// <param name="order"></param>
		/// <returns></returns>
		public BasePutData<InventoryOrder> UpdateInventoryOrderStatus(InventoryOrder order)
		{
			if (null == order || null == order.id || null == order.Status || null == order.version)
			{
				return new BasePutData<InventoryOrder>()
				{
					code = (int)ResultCode.Parameter_Exception,
					message = ResultCode.Parameter_Exception.ToString()
				};
			}
			var inventoryOrder = HttpHelper.GetInstance().Put<InventoryOrder>(new InventoryOrder()
			{
				id = order.id,
				Status = InventoryOrderStatus.已完成.ToString(),
				version = order.version
			});
            if (inventoryOrder.code != 0)
            {
                LogUtils.Error("PutInventoryOrders " + inventoryOrder.message);
            }

            return inventoryOrder;
		}

		/// <summary>
		/// 【手动盘点】 逐一创建【盘点商品明细】。
		/// InventoryDetail所需字段：
		/// CommodityInventoryId 商品编码
		/// InventoryOrderId 关联盘点单
		/// Statis 质量状态 【正常 损坏】
		/// Type 类型 【账面存在 盘点缺失 盘点新增】
		/// </summary>
		/// <param name="details"></param>
		/// <returns></returns>
		public BasePostData<InventoryDetail> CreateInventoryDetail(List<InventoryDetail> details)
		{
			if (null == details || details.Count <= 0)
			{
				return new BasePostData<InventoryDetail>()
				{
					code = (int)ResultCode.Parameter_Exception,
					message = ResultCode.Parameter_Exception.ToString()
				};
			}
			var inventoryDetails = HttpHelper.GetInstance().Post<InventoryDetail>(new PostParam<InventoryDetail>()
			{
				objects = details
			});

			return inventoryDetails;
		}

		/// <summary>
		/// 【手动盘点】 逐一创建【盘点商品明细】。
		/// InventoryDetail所需字段：
		/// CommodityInventoryId 商品编码
		/// InventoryOrderId 关联盘点单
		/// Statis 质量状态 【正常 损坏】
		/// Type 类型 【账面存在 盘点缺失 盘点新增】
		/// </summary>
		/// <param name="details"></param>
		/// <returns></returns>
		public BasePostData<InventoryDetail> CreateInventoryDetail(List<CommodityCode> commodityCodes, List<InventoryOrder> inventoryOrders)
		{
			if (null == commodityCodes || commodityCodes.Count <= 0)
			{
				return new BasePostData<InventoryDetail>()
				{
					code = (int)ResultCode.Parameter_Exception,
					message = ResultCode.Parameter_Exception.ToString()
				};
			}

			BaseData<CommodityInventoryDetail> CommodityInventoryDetails = null;

			if (commodityCodes.Count > 0)
			{
				var commodityCodeIds = commodityCodes.Select(it => it.id).Distinct().ToList();

				CommodityInventoryDetails = HttpHelper.GetInstance().Get<CommodityInventoryDetail>(new QueryParam
				{
					@in =
						{
							field = "CommodityCodeId",
							in_list = BllHelper.ParamUrlEncode(commodityCodeIds)
						}
				});
			}

			if (CommodityInventoryDetails == null)
			{
				return new BasePostData<InventoryDetail>()
				{
					code = (int)ResultCode.Business_Exception,
					message = ResultCode.Business_Exception.ToString()
				};
			}
			else
			{
				HttpHelper.GetInstance().ResultCheck(CommodityInventoryDetails, out bool isSuccess);

				if (isSuccess)
				{
					commodityCodes.ForEach(it =>
					{
						it.CommodityInventoryId = CommodityInventoryDetails.body.objects.Where(cit => cit.CommodityCodeId == it.id).First().id;
					});
				}
			}

			List<InventoryDetail> inventoryDetailList = new List<InventoryDetail>();

			commodityCodes.ForEach(it =>
			{
				inventoryDetailList.Add(new InventoryDetail
				{
					CommodityInventoryId = it.CommodityInventoryId,
					InventoryOrderId = inventoryOrders.Where(item => item.GoodsLocationId == it.GoodsLocationId).First().id,
					CommodityCodeId = it.id,
                    Status = it.QStatus
                });
			});

			return HttpHelper.GetInstance().Post(new PostParam<InventoryDetail>()
			{
				objects = inventoryDetailList
			});
		}
		/// <summary>
		/// 根据设备名称或Id查询设备信息
		/// </summary>
		/// <param name="equipmentNameOrId"></param>
		/// <returns></returns> 
		public BaseData<Equipment> GetEquipmentByEquipmentNameOrId(string equipmentNameOrId)
		{
			if (null == equipmentNameOrId)
			{
				return new BaseData<Equipment>()
				{
					code = (int)ResultCode.Parameter_Exception,
					message = ResultCode.Parameter_Exception.ToString()
				};
			}
			//根据设备号或设备ID获取设备信息
			var equipment = HttpHelper.GetInstance().Get<Equipment>(new QueryParam
			{
				view_filter =
				{
					filter =
					{
						logical_relation = "1 OR 2",
						expressions =
						{
							new QueryParam.Expressions
							{
								field = "name",
								@operator = "==",
								operands =  {$"'{ HttpUtility.UrlEncode(equipmentNameOrId) }'"}
							},
							new QueryParam.Expressions
							{
								field = "id",
								@operator = "==",
								operands = {$"'{ HttpUtility.UrlEncode(equipmentNameOrId) }'" }
							}
						}
					}
				}
			});
			//校验是否含有数据，如果含有数据，拼接具体字段
			HttpHelper.GetInstance().ResultCheck(equipment, out bool isSuccess);

			return equipment;
		}
		/// <summary>
		/// 自动盘点：
		/// • 通过当前【设备编码/id】从表格 【设备管理】（Equipment）中查询获取设备的’自动盘点计划’（InventoryPlanId）。
		/// • 通过当前【id】（InventoryPlan.id=Equipment.InventoryPlanId）从表格【盘点计划管理理】（InventoryPlan）中查询获取相关盘点信息。
		/// </summary>
		/// <param name="equipmentNameOrId"></param>
		/// <returns></returns>
		public BaseData<InventoryPlan> GetInventoryPlanByEquipmnetNameOrId(string equipmentNameOrId)
		{
			//根据设备编码或ID获取设备信息
			var equipment = GetEquipmentByEquipmentNameOrId(equipmentNameOrId);
			//校验是否含有数据，如果含有数据，进行后续操作
			HttpHelper.GetInstance().ResultCheck(equipment, out bool isSuccess);
            BaseData<InventoryPlan> plans = new BaseData<InventoryPlan>();
			if (isSuccess)
			{
				//根据自动盘点计划Id查询盘点计划相关信息
				if (null != equipment.body.objects[0].InventoryPlanId && equipment.body.objects[0].InventoryPlanId.Count > 0)
				{
					plans = HttpHelper.GetInstance().Get<InventoryPlan>(new QueryParam
					{
						@in =
						{
							field = "id",
							in_list =  BllHelper.ParamUrlEncode(equipment.body.objects[0].InventoryPlanId)
						}
					});
				}
                //校验是否含有数据，如果含有数据，拼接具体字段
                plans = HttpHelper.GetInstance().ResultCheck(plans, out bool isSuccess2);
            }
			else
			{
				plans.code = equipment.code;
				plans.message = equipment.message;
			}

			return plans;
		}
		/// <summary>
		/// 【智能柜】创建【盘点单管理理】返回获得id
		/// </summary>
		/// <param name="orders"></param>
		/// <returns></returns>
		public BasePostData<InventoryOrder> CreateInventoryOrder(List<InventoryOrder> orders)
		{
			if (null == orders || orders.Count <= 0)
			{
				return new BasePostData<InventoryOrder>()
				{
					code = (int)ResultCode.Parameter_Exception,
					message = ResultCode.Parameter_Exception.ToString()
				};
			}
			var inventoryOrders = HttpHelper.GetInstance().Post(new PostParam<InventoryOrder>()
			{
				objects = orders
			});

			return inventoryOrders;
		}

		/// <summary>
		/// 【智能柜】 自动盘点更新盘点单管理和其商品明细 ,post请求为admintoken
		/// </summary>
		/// <param name="orders"></param>
		/// <returns></returns>
		public BasePostData<InventoryDetail> CreateInventoryOrderAndDetail(List<CommodityCode> commodityCodes)
		{

			BasePostData<InventoryDetail> inventoryDetailRet;

			if (null == commodityCodes || commodityCodes.Count <= 0)
			{
				inventoryDetailRet = new BasePostData<InventoryDetail>()
				{
					code = (int)ResultCode.Parameter_Exception,
					message = ResultCode.Parameter_Exception.ToString()
				};

				return inventoryDetailRet;

			}

			//创建盘点任务单
			var inventoryTasks = HttpHelper.GetInstance().PostByAdminToken(new PostParam<InventoryTask>()
			{
				objects = { new InventoryTask { Status = InventoryTaskStatus.待确认.ToString() } }
			});

			HttpHelper.GetInstance().ResultCheck(inventoryTasks, out bool isSuccess);

			if (isSuccess)
			{
				string now = GetDateTimeNow();
				List<InventoryOrder> inventoryOrderList = new List<InventoryOrder>();

                //分柜创建盘点任务
				commodityCodes.Select(it => it.GoodsLocationId).Distinct().ToList().ForEach(goodsLocationId =>
				{

					inventoryOrderList.Add(new InventoryOrder
					{
						ConfirmDate = now,
						InventoryTaskId = inventoryTasks.body[0].id,
						Status = InventoryOrderStatus.待盘点.ToString(),//创建盘点单状态为[待盘点]
						GoodsLocationId = goodsLocationId,
						EquipmentId = ApplicationState.GetEquipId(),
						StoreHouseId = ApplicationState.GetHouseId(),
						Type = "自动创建"
					});

				});

				//创建盘点单
				var inventoryOrders = HttpHelper.GetInstance().PostByAdminToken(new PostParam<InventoryOrder>()
				{
					objects = inventoryOrderList
				});

				inventoryDetailRet = HttpHelper.GetInstance().ResultCheck((HttpHelper hh) =>
				{

					BaseData<CommodityInventoryDetail> CommodityInventoryDetails = null;

					if (commodityCodes.Count > 0)
					{
						var commodityCodeIds = commodityCodes.Select(it => it.id).Distinct().ToList();

						CommodityInventoryDetails = hh.Get<CommodityInventoryDetail>(new QueryParam
						{
							@in =
						{
							field = "CommodityCodeId",
							in_list =  BllHelper.ParamUrlEncode(commodityCodeIds)
						}
						});
					}
					if (CommodityInventoryDetails != null)
					{
						hh.ResultCheck(CommodityInventoryDetails, out bool isSuccessq);

						if (isSuccessq)
						{
							commodityCodes.ForEach(it =>
							{
								it.CommodityInventoryId = CommodityInventoryDetails.body.objects.Where(cit => cit.CommodityCodeId == it.id).First().id;
							});

						}
					}

					List<InventoryDetail> inventoryDetailList = new List<InventoryDetail>();
					commodityCodes.ForEach(it =>
					{
						inventoryDetailList.Add(new InventoryDetail
						{
							CommodityInventoryId = it.CommodityInventoryId,
							InventoryOrderId = inventoryOrders.body.Where(iit => iit.GoodsLocationId == it.GoodsLocationId).Select(iit => iit.id).First(),
							CommodityCodeId = it.id
						});
					});
                    //创建盘名单明细列表
					return hh.PostByAdminToken(new PostParam<InventoryDetail>()
					{
						objects = inventoryDetailList
					});

				}, inventoryOrders);

                //更新盘点单状态
                if (inventoryDetailRet != null)
                {
                    var orderIds = inventoryDetailRet.body.Select(it => it.InventoryOrderId).Distinct().ToList();

                    orderIds.ForEach(id =>
                    {
                        var temp = inventoryOrders.body.Where(it => it.id.Equals(id)).First();
                        //temp.Status = InventoryOrderStatus.已完成.ToString();

                        //执行更新操作，异常状态记录日志，详情见方法体内部
                        UpdateInventoryOrderStatus(temp);

                    });


                }
			}
			else
			{
			    inventoryDetailRet = new BasePostData<InventoryDetail>()
				{
					code = (int)ResultCode.Result_Exception,
					message = ResultCode.Result_Exception.ToString()
				};
				
			}

			return inventoryDetailRet;
		}

	}
}
