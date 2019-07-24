﻿using CFLMedCab.Http.Helper;
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

namespace CFLMedCab.Http.Bll
{
	/// <summary>
	/// 拣货业务
	/// </summary>
	public class PickBll : BaseBll<PickBll>
	{
		/// <summary>
		/// 根据拣货单号获取任务单详情
		/// </summary>
		/// <param name="pickTaskName"></param>
		/// <returns></returns>
		public BaseData<PickTask> GetPickTask(string pickTaskName)
		{
			//获取待完成拣货工单
			return HttpHelper.GetInstance().Get<PickTask>(new QueryParam
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
								operands =  {$"'{ HttpUtility.UrlEncode(pickTaskName) }'"}
							},
							new QueryParam.Expressions
							{
								field = "BillStatus",
								@operator = "==",
								operands = {$"'{ HttpUtility.UrlEncode(PickTaskStatus.待拣货.ToString()) }'" }
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
		}

		/// <summary>
		/// 根据拣货单号获取任务单详情
		/// </summary>
		/// <param name="pickTaskName"></param>
		/// <returns></returns>
		public BaseData<PickTask> GetPickTask()
		{

			
			//获取待完成拣货工单
			BaseData<PickTask> baseDataPickTask = HttpHelper.GetInstance().Get<PickTask>(new QueryParam
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
								field = "BillStatus",
								@operator = "==",
								operands = {$"'{ HttpUtility.UrlEncode(PickTaskStatus.待拣货.ToString()) }'" }
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

			BaseData<PickCommodity> baseDataPickTaskCommodityDetail = GetPickTaskCommodityDetail(baseDataPickTask);

			//校验是否含有数据
			HttpHelper.GetInstance().ResultCheck(baseDataPickTaskCommodityDetail, out bool isSuccess);

			if (isSuccess)
			{
				var pickTasks = baseDataPickTask.body.objects;
				var pickTaskCommodityDetails = baseDataPickTaskCommodityDetail.body.objects;
				pickTasks.ForEach(it =>
				{
					it.NeedPickTotalNumber = pickTaskCommodityDetails.Where(sit => sit.PickTaskId == it.id).GroupBy(sit => sit.PickTaskId).Select(group => group.Sum(sit => sit.Number)).Single();
				});

				baseDataPickTask.body.objects = pickTasks.Where(it=>it.NeedPickTotalNumber != 0).ToList();
			}

			return baseDataPickTask;
		}
		/// <summary>
		/// 根据拣货单号获取商品详情
		/// </summary>
		/// <param name="pickTaskName"></param>
		/// <returns></returns>
		public BaseData<PickCommodity> GetPickTaskCommodityDetail(string pickTaskName)
		{
			return GetPickTaskCommodityDetail(GetPickTask(pickTaskName));
		}

		/// <summary>
		/// 根据拣货单号获取商品详情
		/// </summary>
		/// <param name="pickTaskName"></param>
		/// <returns></returns>
		public BaseData<PickCommodity> GetPickTaskCommodityDetail(BaseData<PickTask> baseDataPickTask)
		{


			//校验是否含有数据，如果含有数据，拼接具体字段
			BaseData<PickCommodity> baseDataPickTaskCommodityDetail = HttpHelper.GetInstance().ResultCheck((HttpHelper hh) => {

				var pickTaskIds = baseDataPickTask.body.objects.Select(it => it.id).ToList();
				

				return hh.Get<PickCommodity>(new QueryParam
				{
					@in =
					{
						field = "PickTaskId",
						in_list = BllHelper.ParamUrlEncode(pickTaskIds)
					}
				});

			}, baseDataPickTask);

			//baseDataPickTaskCommodityDetail.body.objects = baseDataPickTaskCommodityDetail.body.objects.Where(it => it.EquipmentId == ApplicationState.GetValue<string>((int)ApplicationKey.EquipId)).ToList();

			return baseDataPickTaskCommodityDetail;

		}


		/// <summary>
		/// 根据拣货单号获取商品详情
		/// </summary>
		/// <param name="pickTaskName"></param>
		/// <returns></returns>
		public BaseData<PickCommodity> GetPickTaskCommodityDetail(PickTask pickTask)
		{

			BaseData<PickCommodity> baseDataPickTaskCommodityDetail = HttpHelper.GetInstance().Get<PickCommodity>(new QueryParam
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
								field = "PickTaskId",
								@operator = "==",
								operands =  {$"'{ HttpUtility.UrlEncode(pickTask.id) }'"}
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
			HttpHelper.GetInstance().ResultCheck(baseDataPickTaskCommodityDetail, out bool isSuccess);

			if (isSuccess)
			{
				baseDataPickTaskCommodityDetail.body.objects.ForEach(it =>
				{
					//拼接设备名字
					if (!string.IsNullOrEmpty(it.EquipmentId))
					{
						it.EquipmentName = GetNameById<Equipment>(it.EquipmentId);
					}

					//拼接库房名字
					if (!string.IsNullOrEmpty(it.EquipmentId))
					{
						it.StoreHouseName = GetNameById<StoreHouse>(it.StoreHouseId);
					}

				});
			}

			return baseDataPickTaskCommodityDetail;

		}

		/// <summary>
		/// 获取变化后的拣货单
		/// </summary>
		/// <param name="baseDatacommodityCode"></param>
		/// <param name="baseDataPickTask"></param>
		/// <param name="baseDataPickTaskCommodityDetail"></param>
		/// <returns></returns>
		public BaseData<PickTask> GetPickTaskChange(BaseData<CommodityCode> baseDatacommodityCode, BaseData<PickTask> baseDataPickTask, BaseData<PickCommodity> baseDataPickTaskCommodityDetail)
		{

			//校验是否含有数据，如果含有数据，有就继续下一步
			BaseData<PickTask> retBaseDataPickTask = HttpHelper.GetInstance().ResultCheck(baseDataPickTask, out bool isSuccess1);

			HttpHelper.GetInstance().ResultCheck(baseDataPickTask, out bool isSuccess2);

			if (isSuccess1 && isSuccess2)
			{
				var pickTaskCommodityDetails = baseDataPickTaskCommodityDetail.body.objects;

				var pickTask = baseDataPickTask.body.objects[0];

				var sfdCommodityIds = pickTaskCommodityDetails.Select(it => it.CommodityId).ToList();

				HttpHelper.GetInstance().ResultCheck(baseDatacommodityCode, out bool isSuccess3);

				var commodityCodes = new List<CommodityCode>();

				if (isSuccess3)
				{
					commodityCodes = baseDatacommodityCode.body.objects;
				}

				var cccIds = commodityCodes.Select(it => it.CommodityId).ToList();

				//是否名称全部一致
				bool isAllContains = sfdCommodityIds.All(cccIds.Contains) && sfdCommodityIds.Count == cccIds.Count;

				if (isAllContains)
				{

					bool isAllNormal = true;

					foreach (PickCommodity stcd in pickTaskCommodityDetails)
					{
						if (stcd.Number != commodityCodes.Where(cit => cit.CommodityId == stcd.CommodityId).Count())
						{
							pickTask.BillStatus = DocumentStatus.异常.ToString();
							isAllNormal = false;
							break;

						}
					}

					if (isAllNormal)
					{
						pickTask.BillStatus = DocumentStatus.已完成.ToString();
					}

				}
				else
				{
					pickTask.BillStatus = DocumentStatus.异常.ToString();
					
				}

				
			}

			return retBaseDataPickTask;
		}

		/// <summary>
		/// 更新拣货任务单
		/// </summary>
		/// <param name="baseDataPickTask">最后结果集</param>
		/// <returns></returns>
		public BasePutData<PickTask> PutPickTask(BaseData<PickTask> baseDataPickTask, AbnormalCauses abnormalCauses)
		{

			//校验是否含有数据，如果含有数据，有就继续下一步
			BasePutData<PickTask> retBasePutDataPickTask = HttpHelper.GetInstance().ResultCheckPutByBase(baseDataPickTask, out bool isSuccess1);

			if (isSuccess1)
			{
				var pickTask = baseDataPickTask.body.objects[0];
				//if (pickTask.BillStatus == DocumentStatus.异常.ToString())
				//{
				//	pickTask.AbnormalCauses = abnormalCauses.ToString();
				//}

				//put修改拣货工单
				retBasePutDataPickTask = HttpHelper.GetInstance().Put(new PickTask
				{
					id = pickTask.id,
					BillStatus = pickTask.BillStatus,
					//AbnormalCauses = pickTask.AbnormalCauses,
					version = pickTask.version
				});
			}

			return retBasePutDataPickTask;
		}

		/// <summary>
		/// 更新拣货任务单
		/// </summary>
		/// <param name="baseDataPickTask">最后结果集</param>
		/// <returns></returns>
		public BasePutData<PickTask> PutPickTask(BaseData<PickTask> baseDataPickTask)
		{

			//校验是否含有数据，如果含有数据，有就继续下一步
			BasePutData<PickTask> retBasePutDataPickTask = HttpHelper.GetInstance().ResultCheckPutByBase(baseDataPickTask, out bool isSuccess1);

			if (isSuccess1)
			{
				var pickTask = baseDataPickTask.body.objects[0];
				//put修改拣货工单
				retBasePutDataPickTask = HttpHelper.GetInstance().Put(new PickTask
				{
					id = pickTask.id,
					BillStatus = pickTask.BillStatus,
					version = pickTask.version
				});
			}

			return retBasePutDataPickTask;
		}

		/// <summary>
		/// 拣货的库存变化
		/// </summary>
		/// <param name="baseDatacommodityCode"></param>
		/// <param name="pickTask"></param>
		/// <returns></returns>
		public BasePostData<CommodityInventoryChange> CreatePickTaskCommodityInventoryChange(BaseData<CommodityCode> baseDataCommodityCode, BaseData<PickTask> baseDataPickTask)
		{

			BasePostData<CommodityInventoryChange> retBaseSinglePostDataCommodityInventoryChange = null;

			//校验是否含有数据，如果含有数据，有就继续下一步
			baseDataCommodityCode = HttpHelper.GetInstance().ResultCheck(baseDataCommodityCode, out bool isSuccess1);

			//校验是否含有数据，如果含有数据，有就继续下一步
			baseDataPickTask = HttpHelper.GetInstance().ResultCheck(baseDataPickTask, out bool isSuccess2);

			if (isSuccess1 && isSuccess2)
			{

				var CommodityCodes = baseDataCommodityCode.body.objects;

				var CommodityInventoryChanges = new List<CommodityInventoryChange>(CommodityCodes.Count);

				CommodityCodes.ForEach(it=> {

					string changeStatus;

					if (it.operate_type == (int)OperateType.出库)
					{
						changeStatus = CommodityInventoryChangeStatus.拣货作业.ToString();
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
							object_name = typeof(PickTask).Name,
							object_id = baseDataPickTask.body.objects[0].id
						},
						ChangeStatus = changeStatus
					});
				});

				retBaseSinglePostDataCommodityInventoryChange = CommodityInventoryChangeBll.GetInstance().createCommodityInventoryChange(CommodityInventoryChanges);
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
