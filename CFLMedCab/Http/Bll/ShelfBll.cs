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
using CFLMedCab.Infrastructure.ToolHelper;

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
            BaseData<ShelfTask> bdShelfTask = HttpHelper.GetInstance().Get<ShelfTask>(new QueryParam
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
                                field = "Operator",
                                @operator = "==",
                                operands = {$"'{ HttpUtility.UrlEncode(ApplicationState.GetUserInfo().id) }'" }
                            }
                        }
					}
				}
			});

            bdShelfTask = HttpHelper.GetInstance().ResultCheck(bdShelfTask, out bool isSuccess);

            if (!isSuccess)
            {
                bdShelfTask.code = (int)ResultCode.Result_Exception;
                bdShelfTask.message = ResultCode.Result_Exception.ToString();
            }
            else
            {
                //如果领⽤单作废标识为【是】则弹窗提醒手术单作废，跳转回前⻚
                if ("已完成".Equals(bdShelfTask.body.objects[0].Status) || "已撤销".Equals(bdShelfTask.body.objects[0].Status))
                {
                    bdShelfTask.code = (int)ResultCode.Result_Exception;
                    bdShelfTask.message = ResultCode.Result_Exception.ToString();
                }
            }

            return bdShelfTask;

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
						logical_relation = "1 AND 2",
						expressions =
						{
							new QueryParam.Expressions
							{
								field = "Status",
								@operator = "==",
								operands = {$"'{ HttpUtility.UrlEncode(ShelfTaskStatus.待上架.ToString()) }'" }
							},
                            new QueryParam.Expressions
                            {
                                field = "Operator",
                                @operator = "==",
                                operands = {$"'{ HttpUtility.UrlEncode(ApplicationState.GetUserInfo().id) }'" }
                            }

                        }
					}
				}
			});

			BaseData<ShelfTaskCommodityDetail> baseDataShelfTaskCommodityDetail = GetShelfTaskCommodityDetail(baseDataShelfTask);

			//校验是否含有数据
			HttpHelper.GetInstance().ResultCheck(baseDataShelfTaskCommodityDetail, out bool isSuccess);

			if (isSuccess)
			{
                List<string> ids = ApplicationState.GetAllCabIds();

                List<ShelfTask> taskList = new List<ShelfTask>();

				var shelfTasks = baseDataShelfTask.body.objects;
				var shelfTaskCommodityDetails = baseDataShelfTaskCommodityDetail.body.objects;
				shelfTasks.ForEach(it =>
				{
                    ids.ForEach(id => {
                        it.NeedShelfTotalNumber = shelfTaskCommodityDetails.Where(sit => sit.ShelfTaskId == it.id && sit.GoodsLocationId == id).GroupBy(sit => new { sit.ShelfTaskId, sit.GoodsLocationId}).Select(group => group.Sum(sit => (sit.NeedShelfNumber - sit.AlreadyShelfNumber))).Single();
                        if(it.NeedShelfTotalNumber != 0)
                        {
                            it.GoodLocationName = ApplicationState.GetCabNameById(id);
                            taskList.Add(it);
                        }                      
                    });
				});

				baseDataShelfTask.body.objects = taskList;
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
					//@in =
					//{
					//	field = "ShelfTaskId",
					//	in_list = BllHelper.ParamUrlEncode(shelfTaskIds)
					//}
                    view_filter =
                    {
                        filter =
                        {
                            logical_relation = "1 AND 2 AND 3",
                            expressions =
                            {
                                new QueryParam.Expressions
                                {
                                    field = "ShelfTaskId",
                                    @operator = "INRANGE",
                                    operands =  BllHelper.OperandsProcess(shelfTaskIds)
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
						logical_relation = "1 AND 2 AND 3",
						expressions =
						{
							new QueryParam.Expressions
							{
								field = "ShelfTaskId",
								@operator = "==",
								operands =  {$"'{ HttpUtility.UrlEncode(shelfTask.id) }'"}
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
                //上架任务单商品详情列表
				var shelfTaskCommodityDetails = baseDataShelfTaskCommodityDetail.body.objects.Where(item => item.NeedShelfNumber != item.AlreadyShelfNumber);
                //上架任务单商品码
				var sfdCommodityIds = shelfTaskCommodityDetails.Select(it => it.CommodityId).Distinct().ToList();

				var commodityCodes = baseDatacommodityCode.body.objects;

                //商品异常状态回显
                commodityCodes.ForEach(it=> {
					if (it.operate_type == (int)OperateType.出库)
					{
						it.AbnormalDisplay = AbnormalDisplay.异常.ToString();
					}
					else
					{
						if (sfdCommodityIds.Contains(it.CommodityId))
						{
							var shelfTaskCommodityDetail = shelfTaskCommodityDetails.Where(item => item.CommodityId == it.CommodityId).First();

							if ((shelfTaskCommodityDetail.NeedShelfNumber - shelfTaskCommodityDetail.AlreadyShelfNumber) >= ++ shelfTaskCommodityDetail.CountShelfNumber)
							{
								it.AbnormalDisplay = AbnormalDisplay.正常.ToString();
							}
							else
							{
								it.AbnormalDisplay = AbnormalDisplay.异常.ToString();
							}

							
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
                        //种类不相等
						if ((stcd.NeedShelfNumber - stcd.AlreadyShelfNumber) != commodityCodes.Where(cit => cit.CommodityId == stcd.CommodityId).Count())
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
        /// <param name="shelfTask"></param>
        /// <param name="abnormalCauses"></param>
        /// <param name="bAutoSubmit">是否是主动提交</param>
        /// <returns></returns>
        public BasePutData<ShelfTask> PutShelfTask(ShelfTask shelfTask, AbnormalCauses abnormalCauses)
        {

            ShelfTask task = new ShelfTask
            {
                id = shelfTask.id,
                Status = shelfTask.Status,
                version = shelfTask.version
            };

            if (shelfTask.Status == DocumentStatus.异常.ToString() && abnormalCauses != AbnormalCauses.未选)
            {
                task.AbnormalCauses = abnormalCauses.ToString();
            }

            BasePutData<ShelfTask> basePutData = HttpHelper.GetInstance().Put(task);

            if (basePutData.code != 0)
            {
                LogUtils.Error("PutShelfTask 失败！ " + basePutData.message);
            }
            return basePutData;
        }

        /// <summary>
        /// 上架的库存变化
        /// </summary>
        /// <param name="baseDataCommodityCode"></param>
        /// <param name="shelfTask"></param>
        /// <param name="bAutoSubmit">是否是主动提交</param>
        /// <returns></returns>
        public BasePostData<CommodityInventoryChange> CreateShelfTaskCommodityInventoryChange(BaseData<CommodityCode> baseDataCommodityCode, ShelfTask shelfTask, bool bAutoSubmit)
		{

			BasePostData<CommodityInventoryChange> retBaseSinglePostDataCommodityInventoryChange = null;

			//校验是否含有数据，如果含有数据，有就继续下一步
			baseDataCommodityCode = HttpHelper.GetInstance().ResultCheck(baseDataCommodityCode, out bool isSuccess);

			if (isSuccess)
			{
				var CommodityCodes = baseDataCommodityCode.body.objects;
				var CommodityInventoryChanges = new List<CommodityInventoryChange>(CommodityCodes.Count);

                CommodityCodes.ForEach(it =>
                {
                    CommodityInventoryChange cic = new CommodityInventoryChange()
                    {
                        CommodityCodeId = it.id,//商品码【扫描】
                        SourceBill = new SourceBill()//来源单据
                        {
                            object_name = typeof(ShelfTask).Name,
                            object_id = shelfTask.id
                        },
                        //EquipmentId = ApplicationState.GetEquipId(),
                        //StoreHouseId = ApplicationState.GetHouseId(),
                        //GoodsLocationId = it.GoodsLocationId
                    };

                    if (it.operate_type == (int)OperateType.出库)
                    {
                        cic.ChangeStatus = CommodityInventoryChangeStatus.未上架.ToString();
                        cic.StoreHouseId = ApplicationState.GetHouseId();
                    }
                    else
                    {
                        cic.ChangeStatus = CommodityInventoryChangeStatus.正常.ToString();
                        cic.EquipmentId = it.EquipmentId;
                        cic.StoreHouseId = it.StoreHouseId;
                        cic.GoodsLocationId = it.GoodsLocationId;
                    }

                    if (!bAutoSubmit && it.AbnormalDisplay == AbnormalDisplay.异常.ToString())
                    {
                        cic.AdjustStatus = CommodityInventoryChangeAdjustStatus.是.ToString();
                    }

                    CommodityInventoryChanges.Add(cic);
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

                LogUtils.Error("CreateShelfTaskCommodityInventoryChange 失败！ " + baseDataCommodityCode.message);
			}
			return retBaseSinglePostDataCommodityInventoryChange;
		}

	}


}
