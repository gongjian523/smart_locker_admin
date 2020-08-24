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
using CFLMedCab.Infrastructure.ToolHelper;
using CFLMedCab.Infrastructure;

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
                        logical_relation = "1 AND 2 AND 3",
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
                            },
                            new QueryParam.Expressions
                            {
                                field = "Operator",
                                @operator = "==",
                                operands = {$"'{ HttpUtility.UrlEncode(ApplicationState.GetUserInfo().id)}'"}
                            }

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
								field = "Operator",
								@operator = "==",
								operands = {$"'{ HttpUtility.UrlEncode(ApplicationState.GetUserInfo().id) }'" }
							}

						}
					}
				}
			});
            //校验是否含有数据
            HttpHelper.GetInstance().ResultCheck(baseDataPickTask, out bool isSuccess0);

            if (isSuccess0)
            {
                BaseData<PickCommodity> baseDataPickTaskCommodityDetail = GetPickTaskCommodityDetail(baseDataPickTask);

                //校验是否含有数据
                HttpHelper.GetInstance().ResultCheck(baseDataPickTaskCommodityDetail, out bool isSuccess);

                if (isSuccess)
                {
                    List<PickTask> taskList = new List<PickTask>();

                    var pickTasks = baseDataPickTask.body.objects.Where(item => (item.BillStatus != PickTaskStatus.已完成.ToString() && item.BillStatus != PickTaskStatus.已撤销.ToString())).ToList(); ;
                    var pickTaskCommodityDetails = baseDataPickTaskCommodityDetail.body.objects.ToList();

                    pickTasks.ForEach(it =>
                    {
                        //it.NeedPickTotalNumber = pickTaskCommodityDetails.Where(sit => sit.PickTaskId == it.id).GroupBy(sit => sit.PickTaskId).Select(group => group.Sum(sit => (sit.Number-sit.PickNumber))).FirstOrDefault();
                        it.NeedPickTotalNumber = pickTaskCommodityDetails.Where(sit => sit.PickTaskId == it.id).GroupBy(sit => sit.PickTaskId).Select(group => group.Sum(sit => sit.Number)).FirstOrDefault();
                        if (it.NeedPickTotalNumber != 0)
                        {
                            taskList.Add(it);
                        }
                    });

                    baseDataPickTask.body.objects = taskList;
                }
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

                return hh.Get<PickCommodity>(new QueryParam
				{
					//@in =
					//{
					//	field = "PickTaskId",
					//	in_list = BllHelper.ParamUrlEncode(pickTaskIds)
					//}
                    view_filter =
                    {
                        filter =
                        {
                            //logical_relation = "1 AND 2 AND 3",
                            logical_relation = "1 AND 2",
                            expressions =
                            {
                                //这种写法有问题，暂时把这个条件删除，在后面过滤
                                //new QueryParam.Expressions
                                //{
                                //    field = "PickTaskId",
                                //    @operator = "INRANGE",
                                //    operands =  BllHelper.OperandsProcess(pickTaskIds)
                                //},
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

			}, baseDataPickTask);

            //baseDataPickTaskCommodityDetail.body.objects = baseDataPickTaskCommodityDetail.body.objects.Where(it => it.EquipmentId == ApplicationState.GetValue<string>((int)ApplicationKey.EquipId)).ToList();

            HttpHelper.GetInstance().ResultCheck(baseDataPickTask, out bool isSuccess);
            HttpHelper.GetInstance().ResultCheck(baseDataPickTaskCommodityDetail, out bool isSuccess1);

            if (isSuccess && isSuccess1)
            {
                var pickTaskIds = baseDataPickTask.body.objects.Select(it => it.id).ToList();
                baseDataPickTaskCommodityDetail.body.objects = baseDataPickTaskCommodityDetail.body.objects.Where(it => pickTaskIds.Contains(it.PickTaskId)).ToList();
            }

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
						logical_relation = "1 AND 2",
						expressions =
						{
							new QueryParam.Expressions
							{
								field = "PickTaskId",
								@operator = "==",
								operands =  {$"'{ HttpUtility.UrlEncode(pickTask.id) }'"}
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
			HttpHelper.GetInstance().ResultCheck(baseDataPickTaskCommodityDetail, out bool isSuccess);

			if (isSuccess)
			{
				baseDataPickTaskCommodityDetail.body.objects.ForEach(it =>
				{
					//拼接库房名字
					if (!string.IsNullOrEmpty(it.StoreHouseId))
					{
						it.StoreHouseName = GetNameById<StoreHouse>(it.StoreHouseId);
					}

					//拼接商品名字
					if (!string.IsNullOrEmpty(it.CommodityId))
					{
						it.CommodityName = GetNameById<Commodity>(it.CommodityId);
					}

				});
			}

			return baseDataPickTaskCommodityDetail;

		}

        /// <summary>
        /// 根据拣货任务单获取所有货柜下的商品详情
        /// </summary>
        /// <param name="pickTask"></param>
        /// <returns></returns>
        public BaseData<PickCommodity> GetPickTaskAllCommodityDetail(PickTask pickTask)
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
                            }
                        }
                    }
                }

            });

            return baseDataPickTaskCommodityDetail;
        }

        /// <summary>
        /// 获取变化后的拣货单
        /// </summary>
        /// <param name="bdcommodityCode"></param>
        /// <param name="pickTask"></param>
        /// <param name="bdPickTaskCommodityDetail"></param>
        /// <returns></returns>
        public void GetPickTaskChange(BaseData<CommodityCode> bdcommodityCode, PickTask pickTask, BaseData<PickCommodity> bdPickTaskCommodityDetail)
		{

			HttpHelper.GetInstance().ResultCheck(bdPickTaskCommodityDetail, out bool isSuccess);
            HttpHelper.GetInstance().ResultCheck(bdcommodityCode, out bool isSuccess1);

            if (isSuccess && isSuccess1)
			{
                List<string> locIds = bdcommodityCode.body.objects.Select(item => item.GoodsLocationId).Distinct().ToList();
                List<string> status = new List<string>();

                locIds.ForEach(id =>
                {

                    var commodityCodes = bdcommodityCode.body.objects.Where(item => item.GoodsLocationId == id).ToList();

                    commodityCodes.ForEach(it =>
                    {
                        if (it.operate_type == (int)OperateType.入库)
                        {
                            it.AbnormalDisplay = AbnormalDisplay.异常.ToString();
                        }
                        else
                        {
                            it.AbnormalDisplay = AbnormalDisplay.正常.ToString();
                        }
                    });
                });

                var pickTaskCommodityDetails = bdPickTaskCommodityDetail.body.objects.Where(item => item.Number != 0);
                var sfdCommodityIds = pickTaskCommodityDetails.Select(it => it.CommodityId).Distinct().ToList();


                foreach (PickCommodity stcd in pickTaskCommodityDetails)
                {
                    stcd.CurPickNumber = bdcommodityCode.body.objects.Where(cit => cit.CommodityId == stcd.CommodityId && cit.operate_type == (int)OperateType.入库).Count();

                    if (stcd.Number < stcd.CurPickNumber)
                    {
                        status.Add(DocumentStatus.进行中.ToString());
                    }
                    else
                    {
                        status.Add(DocumentStatus.已完成.ToString());
                    }
                }

                if(bdcommodityCode.body.objects.Where(item => item.AbnormalDisplay == AbnormalDisplay.异常.ToString()).Count() > 0)
                {
                    pickTask.BillStatus = DocumentStatus.异常.ToString();
                }
                else if (status.Contains(DocumentStatus.进行中.ToString()))
                {
                    pickTask.BillStatus = DocumentStatus.进行中.ToString();
                }
                else
                {
                    //获取这个任务单中所有的商品详情
                    BaseData<PickCommodity> bdAllpc = GetPickTaskAllCommodityDetail(pickTask);
                    HttpHelper.GetInstance().ResultCheck(bdAllpc, out bool isSuccess2);
                    
                    if (isSuccess2)
                    {
                        //只有所有商品都完成了拣货，不管在那个货架上，才能将这个任务单的状态改为“已完成”
                        if (bdAllpc.body.objects.Where(it => it.Number == 0 && it.StoreHouseId != ApplicationState.GetHouseId()).Count() == 0)
                        {
                            pickTask.BillStatus = DocumentStatus.已完成.ToString();
                        }
                        else
                        {
                            pickTask.BillStatus = DocumentStatus.进行中.ToString();
                        }
                    }
                    else
                    {
                        LogUtils.Error("GetPickTaskChange: GetShelfTaskAllCommodityDetail" + bdAllpc.message);
                    }
                }
            }
		}

		/// <summary>
		/// 更新拣货任务单
		/// </summary>
		/// <param name="baseDataPickTask">最后结果集</param>
		/// <returns></returns>
		public BasePutData<PickTask> PutPickTask(PickTask pickTask)
		{
            PickTask task = new PickTask
            {
                id = pickTask.id,
                BillStatus = pickTask.BillStatus,
                version = pickTask.version
            };

            if (pickTask.BillStatus == DocumentStatus.异常.ToString() && pickTask.AbnormalCauses != "")
            {
                task.AbnormalCauses = pickTask.AbnormalCauses;
            }

            //当任务单状态为已完成时，携带完成时间进行更新
            if (pickTask.BillStatus == DocumentStatus.已完成.ToString())
            {
                task.FinishDate = GetDateTimeNow();
            }

            BasePutData<PickTask> basePutData = HttpHelper.GetInstance().Put(task);

            if (basePutData.code != 0)
            {
                LogUtils.Error("PutPickTask " + basePutData.message);
            }

            return basePutData;
        }


        /// <summary>
        /// 拣货的库存变化
        /// </summary>
        /// <param name="baseDatacommodityCode"></param>
        /// <param name="pickTask"></param>
        /// <param name="bAutoSubmit">是否是主动提交</param>
        /// <returns></returns>
        public BasePostData<CommodityInventoryChange> CreatePickTaskCommodityInventoryChange(BaseData<CommodityCode> baseDataCommodityCode, PickTask pickTask, bool bAutoSubmit)
		{

			BasePostData<CommodityInventoryChange> retBaseSinglePostDataCommodityInventoryChange = null;

			//校验是否含有数据，如果含有数据，有就继续下一步
			baseDataCommodityCode = HttpHelper.GetInstance().ResultCheck(baseDataCommodityCode, out bool isSuccess);

			if (isSuccess)
			{
				var CommodityCodes = baseDataCommodityCode.body.objects;
                 
                var CommodityInventoryChanges = new List<CommodityInventoryChange>();

                CommodityCodes.ForEach(it=> {

                    CommodityInventoryChange cic = new CommodityInventoryChange()
                    {
                        CommodityCodeId = it.id,//商品码【扫描】
                        SourceBill = new SourceBill()//来源单据
                        {
                            object_name = typeof(PickTask).Name,
                            object_id = pickTask.id
                        },
                        operate_type = it.operate_type
                    };

                    if (!bAutoSubmit && it.AbnormalDisplay == AbnormalDisplay.异常.ToString())
                    {
                        cic.AdjustStatus = CommodityInventoryChangeAdjustStatus.是.ToString();
                    }

                    if (it.operate_type == (int)OperateType.出库)
                    {
                        cic.ChangeStatus = CommodityInventoryChangeStatus.拣货作业.ToString();
                        cic.StoreHouseId = it.StoreHouseId;
                    }
                    else
                    {
                        cic.ChangeStatus = CommodityInventoryChangeStatus.正常.ToString();
                        cic.EquipmentId = it.EquipmentId;
                        cic.StoreHouseId = it.StoreHouseId;
                        cic.GoodsLocationId = it.GoodsLocationId;
                    }
                    CommodityInventoryChanges.Add(cic);
                });

                return CommodityInventoryChangeBll.GetInstance().CreateCommodityInventoryChangeSeparately(CommodityInventoryChanges);
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
