using CFLMedCab.Http.Helper;
using CFLMedCab.Http.Model;
using CFLMedCab.Http.Model.Base;
using CFLMedCab.Http.Model.param;
using CFLMedCab.Http.Model.Enum;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using CFLMedCab.Http.Enum;
using CFLMedCab.Http.Model.Common;
using CFLMedCab.Infrastructure;

namespace CFLMedCab.Http.Bll
{
	/// <summary>
	/// 上架业务
	/// </summary>
	public class ShelfBll : BaseBll<ShelfBll>
	{

		/// <summary>
		/// 根据上架单号获取任务单详情
		/// </summary>
		/// <param name="shelfTaskName"></param>
		/// <returns></returns>
		public BaseData<ShelfTask> GetShelfTask(string shelfTaskName)
		{
			//获取待完成上架工单
			return HttpHelper.GetInstance().Get<ShelfTask>(new QueryParam
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
								operands =  {$"'{ HttpUtility.UrlEncode(shelfTaskName) }'"}
							},
							new QueryParam.Expressions
							{
								field = "Status",
								@operator = "==",
								operands = {$"'{ HttpUtility.UrlEncode(ShelfTaskStatus.待上架.ToString()) }'" }
							}
                            //new QueryParam.Expressions
                            //{
                            //    field = "Operator",
                            //    @operator = "==",
                            //    operands = {$"'{ HttpUtility.UrlEncode(ApplicationState.GetUserInfo().id) }'" }
                            //}
                        }
					}
				}
			});
		}

		/// <summary>
		/// 根据上架单号获取任务单详情
		/// </summary>
		/// <param name="shelfTaskName"></param>
		/// <returns></returns>
		public BaseData<ShelfTask> GetShelfTask()
		{

			
			//获取待完成上架工单
			BaseData<ShelfTask> baseDataShelfTask = HttpHelper.GetInstance().Get<ShelfTask>(new QueryParam
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
								field = "Status",
								@operator = "==",
								operands = {$"'{ HttpUtility.UrlEncode(ShelfTaskStatus.待上架.ToString()) }'" }
							}
							//new QueryParam.Expressions
							//{
							//	field = "Operator",
							//	@operator = "==",
							//	operands = {$"'{ HttpUtility.UrlEncode(ApplicationState.GetValue<string>((int)ApplicationKey.CurUser)) }'" }
							//}

						}
					}
				}
			});

			BaseData<ShelfTaskCommodityDetail> baseDataShelfTaskCommodityDetail = GetShelfTaskCommodityDetail(baseDataShelfTask);

			//校验是否含有数据
			HttpHelper.GetInstance().ResultCheck(baseDataShelfTaskCommodityDetail, out bool isSuccess);

			if (isSuccess)
			{
				var shelfTasks = baseDataShelfTask.body.objects;
				var shelfTaskCommodityDetails = baseDataShelfTaskCommodityDetail.body.objects;
				shelfTasks.ForEach(it =>
				{
					it.NeedShelfTotalNumber = shelfTaskCommodityDetails.Where(sit => sit.ShelfTaskId == it.id).GroupBy(sit => sit.ShelfTaskId).Select(group => group.Sum(sit => sit.NeedShelfNumber)).Single();
				});

				baseDataShelfTask.body.objects = shelfTasks.Where(it=>it.NeedShelfTotalNumber != 0).ToList();
			}

			return baseDataShelfTask;
		}
		/// <summary>
		/// 根据上架单号获取商品详情
		/// </summary>
		/// <param name="shelfTaskName"></param>
		/// <returns></returns>
		public BaseData<ShelfTaskCommodityDetail> GetShelfTaskCommodityDetail(string shelfTaskName)
		{
			return GetShelfTaskCommodityDetail(GetShelfTask(shelfTaskName));
		}

		/// <summary>
		/// 根据上架单号获取商品详情
		/// </summary>
		/// <param name="shelfTaskName"></param>
		/// <returns></returns>
		public BaseData<ShelfTaskCommodityDetail> GetShelfTaskCommodityDetail(BaseData<ShelfTask> baseDataShelfTask)
		{


			//校验是否含有数据，如果含有数据，拼接具体字段
			BaseData<ShelfTaskCommodityDetail> baseDataShelfTaskCommodityDetail = HttpHelper.GetInstance().ResultCheck((HttpHelper hh) => {

				var shelfTaskIds = baseDataShelfTask.body.objects.Select(it => it.id).ToList();
				

				return hh.Get<ShelfTaskCommodityDetail>(new QueryParam
				{
					@in =
					{
						field = "ShelfTaskId",
						in_list = BllHelper.ParamUrlEncode(shelfTaskIds)
					}
				});

			}, baseDataShelfTask);

			//baseDataShelfTaskCommodityDetail.body.objects = baseDataShelfTaskCommodityDetail.body.objects.Where(it => it.EquipmentId == ApplicationState.GetValue<string>((int)ApplicationKey.EquipId)).ToList();

			return baseDataShelfTaskCommodityDetail;

		}


		/// <summary>
		/// 根据上架单号获取商品详情
		/// </summary>
		/// <param name="shelfTaskName"></param>
		/// <returns></returns>
		public BaseData<ShelfTaskCommodityDetail> GetShelfTaskCommodityDetail(ShelfTask shelfTask)
		{

			BaseData<ShelfTaskCommodityDetail> baseDataShelfTaskCommodityDetail = HttpHelper.GetInstance().Get<ShelfTaskCommodityDetail>(new QueryParam
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
								field = "ShelfTaskId",
								@operator = "==",
								operands =  {$"'{ HttpUtility.UrlEncode(shelfTask.id) }'"}
							},
							//new QueryParam.Expressions
							//{
							//	field = "EquipmentId",
							//	@operator = "==",
							//	operands = {$"'{ HttpUtility.UrlEncode(ApplicationState.GetValue<string>((int)ApplicationKey.EquipId)) }'" }
							//}

						}
					}
				}
		
			});

			//校验是否含有数据，如果含有数据，拼接具体字段
			HttpHelper.GetInstance().ResultCheck(baseDataShelfTaskCommodityDetail, out bool isSuccess);

			if (isSuccess)
			{
				baseDataShelfTaskCommodityDetail.body.objects.ForEach(it =>
				{
					//拼接设备名字
					if (!string.IsNullOrEmpty(it.EquipmentId))
					{
						it.EquipmentName = GetNameById<Equipment>(it.EquipmentId);
					}

					//拼接库房名字
					if (!string.IsNullOrEmpty(it.StoreHouseId))
					{
						it.StoreHouseName = GetNameById<StoreHouse>(it.StoreHouseId);
					}

					//拼接货位名字
					if (!string.IsNullOrEmpty(it.GoodsLocationId))
					{
						it.GoodsLocationName = GetNameById<GoodsLocation>(it.GoodsLocationId);
					}

					//拼接商品名字
					if (!string.IsNullOrEmpty(it.CommodityId))
					{
						it.CommodityName = GetNameById<Commodity>(it.CommodityId);
					}

				});
			}

			return baseDataShelfTaskCommodityDetail;

		}

		/// <summary>
		/// 获取变化后的上架单
		/// </summary>
		/// <param name="baseDatacommodityCode"></param>
		/// <param name="baseDataShelfTask"></param>
		/// <param name="baseDataShelfTaskCommodityDetail"></param>
		/// <returns></returns>
		public void GetShelfTaskChange(BaseData<CommodityCode> baseDatacommodityCode, ShelfTask shelfTask, BaseData<ShelfTaskCommodityDetail> baseDataShelfTaskCommodityDetail)
		{

			HttpHelper.GetInstance().ResultCheck(baseDataShelfTaskCommodityDetail, out bool isSuccess);

            HttpHelper.GetInstance().ResultCheck(baseDatacommodityCode, out bool isSuccess1);

            if (isSuccess && isSuccess1)
			{
				var shelfTaskCommodityDetails = baseDataShelfTaskCommodityDetail.body.objects;

				var sfdCommodityIds = shelfTaskCommodityDetails.Select(it => it.CommodityId).Distinct().ToList();

				var commodityCodes = baseDatacommodityCode.body.objects;

					commodityCodes.ForEach(it=> {
						if (it.operate_type == (int)OperateType.出库)
						{
							it.AbnormalDisplay = AbnormalDisplay.异常.ToString();
						}
						else
						{
							if (sfdCommodityIds.Contains(it.CommodityId))
							{
								it.AbnormalDisplay = AbnormalDisplay.正常.ToString();
							}
							else
							{
								it.AbnormalDisplay = AbnormalDisplay.异常.ToString();
							}
						}
					});

				var cccIds = commodityCodes.Select(it => it.CommodityId).Distinct().ToList();

				//是否名称全部一致
				bool isAllContains = sfdCommodityIds.All(cccIds.Contains) && sfdCommodityIds.Count == cccIds.Count;

                if (isAllContains)
				{

					bool isAllNormal = true;

					foreach (ShelfTaskCommodityDetail stcd in shelfTaskCommodityDetails)
					{
						if ((int)stcd.NeedShelfNumber != commodityCodes.Where(cit => cit.CommodityId == stcd.CommodityId).Count())
						{
							shelfTask.Status = DocumentStatus.异常.ToString();
							isAllNormal = false;
							break;

						}
					}

					if (isAllNormal)
					{
						shelfTask.Status = DocumentStatus.已完成.ToString();
					}

				}
				else
				{
					shelfTask.Status = DocumentStatus.异常.ToString();
					
				}

                foreach (ShelfTaskCommodityDetail stcd in shelfTaskCommodityDetails)
                {
                    stcd.CurShelfNumber = commodityCodes.Where(cit => cit.CommodityId == stcd.CommodityId).Count();
                    stcd.PlanShelfNumber = stcd.NeedShelfNumber - stcd.AlreadyShelfNumber;
                }
            }

		}


		/// <summary>
		/// 更新上架任务单
		/// </summary>
		/// <param name="baseDataShelfTask">最后结果集</param>
		/// <returns></returns>
		public BasePutData<ShelfTask> PutShelfTask(ShelfTask shelfTask, AbnormalCauses abnormalCauses)
		{
			if (shelfTask.Status == DocumentStatus.异常.ToString())
			{
				shelfTask.AbnormalCauses = abnormalCauses.ToString();
			}

			return HttpHelper.GetInstance().Put(new ShelfTask
			{
				id = shelfTask.id,
				Status = shelfTask.Status,
				AbnormalCauses = shelfTask.AbnormalCauses,
				version = shelfTask.version
			});
		}

		/// <summary>
		/// 上架的库存变化
		/// </summary>
		/// <param name="baseDatacommodityCode"></param>
		/// <param name="shelfTask"></param>
		/// <returns></returns>
		public BasePostData<CommodityInventoryChange> CreateShelfTaskCommodityInventoryChange(BaseData<CommodityCode> baseDataCommodityCode, ShelfTask shelfTask)
		{

			BasePostData<CommodityInventoryChange> retBaseSinglePostDataCommodityInventoryChange = null;

			//校验是否含有数据，如果含有数据，有就继续下一步
			baseDataCommodityCode = HttpHelper.GetInstance().ResultCheck(baseDataCommodityCode, out bool isSuccess);

			if (isSuccess)
			{

				var CommodityCodes = baseDataCommodityCode.body.objects;

				var CommodityInventoryChanges = new List<CommodityInventoryChange>(CommodityCodes.Count);

				CommodityCodes.ForEach(it=> {

					string changeStatus;

					if (it.operate_type == (int)OperateType.出库)
					{
						changeStatus = CommodityInventoryChangeStatus.已消耗.ToString();
					}
					else
					{
						changeStatus = CommodityInventoryChangeStatus.正常.ToString();
					}


					CommodityInventoryChanges.Add(new CommodityInventoryChange()
					{
						CommodityCodeId = it.id,//商品码【扫描】
						SourceBill = new SourceBill()//来源单据
						{
							object_name = typeof(ShelfTask).Name,
							object_id = shelfTask.id
						},
						ChangeStatus = changeStatus
					});
				});

				retBaseSinglePostDataCommodityInventoryChange = CommodityInventoryChangeBll.GetInstance().CreateCommodityInventoryChange(CommodityInventoryChanges);
			}
			else
			{
				retBaseSinglePostDataCommodityInventoryChange = new BasePostData<CommodityInventoryChange>
				{
					code = baseDataCommodityCode.code,
					message = baseDataCommodityCode.message
				};
			}
			return retBaseSinglePostDataCommodityInventoryChange;
		}

	}


}
