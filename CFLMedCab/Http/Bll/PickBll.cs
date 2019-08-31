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
						logical_relation = "1 AND 2",
						expressions =
						{
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
                    //WARING 这种做法只在单柜下才是完全正确，在多柜中需要修改
                    string id = ApplicationState.GetAllLocIds().ToList().First();
                    List<PickTask> taskList = new List<PickTask>();

                    var pickTasks = baseDataPickTask.body.objects;
                    var pickTaskCommodityDetails = baseDataPickTaskCommodityDetail.body.objects.Where(item => item.GoodsLocationId == id);
                    pickTasks.ForEach(it =>
                    {
                        //ids.ForEach(id => {
                        //    it.NeedPickTotalNumber = pickTaskCommodityDetails.Where(sit => sit.PickTaskId == it.id && sit.GoodsLocationId == id).GroupBy(sit => new { sit.PickTaskId, sit.GoodsLocationId }).Select(group => group.Sum(sit => (sit.Number - sit.PickNumber))).Single();
                        //    if (it.NeedPickTotalNumber != 0)
                        //    {
                        //        it.GoodLocationName = ApplicationState.GetLocCodeById(id);
                        //        taskList.Add(it);
                        //    }
                        //});

                        it.NeedPickTotalNumber = pickTaskCommodityDetails.Where(sit => sit.PickTaskId == it.id).GroupBy(sit => sit.PickTaskId).Select(group => group.Sum(sit => (sit.Number-sit.PickNumber))).FirstOrDefault();
                        if(it.NeedPickTotalNumber != 0)
                        {
                            it.GoodLocationName = ApplicationState.GetLocCodeById(id);
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

                //var pickTaskIds = baseDataPickTask.body.objects.Select(it => it.id).ToList();


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
								field = "EquipmentId",
								@operator = "==",
								operands = {$"'{ HttpUtility.UrlEncode(ApplicationState.GetValue<string>((int)ApplicationKey.EquipId)) }'" }
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
        /// <param name="baseDatacommodityCode"></param>
        /// <param name="pickTask"></param>
        /// <param name="baseDataPickTaskCommodityDetail"></param>
        /// <returns></returns>
        public void GetPickTaskChange(BaseData<CommodityCode> baseDatacommodityCode, PickTask pickTask, BaseData<PickCommodity> baseDataPickTaskCommodityDetail)
		{

			HttpHelper.GetInstance().ResultCheck(baseDataPickTaskCommodityDetail, out bool isSuccess);

            HttpHelper.GetInstance().ResultCheck(baseDatacommodityCode, out bool isSuccess1);

            if (isSuccess && isSuccess1)
			{
				var pickTaskCommodityDetails = baseDataPickTaskCommodityDetail.body.objects.Where(item => item.Number != item.PickNumber);

				var sfdCommodityIds = pickTaskCommodityDetails.Select(it => it.CommodityId).Distinct().ToList();

                //WARING 这种取法的的前提是pickTaskCommodityDetails中全是一个货柜的产品，前面获取数据的做法是错误的，没有将货位id加进去，
                //在后续多柜中改正
                var goodsLocationId = pickTaskCommodityDetails.First().GoodsLocationId;

                var commodityCodes = new List<CommodityCode>();

				commodityCodes = baseDatacommodityCode.body.objects;

				commodityCodes.ForEach(it => {
					if (it.operate_type == (int)OperateType.入库)
					{
						it.AbnormalDisplay = AbnormalDisplay.异常.ToString();
					}
					else
					{
                        if (sfdCommodityIds.Contains(it.CommodityId))
						{
                            var pickTaskCommodityDetail = pickTaskCommodityDetails.Where(item => item.CommodityId == it.CommodityId).First();

                            if ((pickTaskCommodityDetail.Number - pickTaskCommodityDetail.PickNumber) >= ++pickTaskCommodityDetail.CountPickNumber)
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

					foreach (PickCommodity stcd in pickTaskCommodityDetails)
					{
						if ((stcd.Number - stcd.PickNumber) != commodityCodes.Where(cit => cit.CommodityId == stcd.CommodityId).Count())
						{
							isAllNormal = false;
							break;
						}
					}

					if (isAllNormal)
					{
                        //获取这个任务单中所有的商品详情
                        BaseData<PickCommodity> bdAllpc = GetPickTaskAllCommodityDetail(pickTask);

                        HttpHelper.GetInstance().ResultCheck(bdAllpc, out bool isSuccess2);

                        if (isSuccess2)
                        {
                            //只有所有商品都完成了拣货，不管在那个货架上，才能将这个任务单的状态改为“已完成”
                            if (bdAllpc.body.objects.Where(it => it.Number != it.PickNumber && it.GoodsLocationId != goodsLocationId).Count() == 0)
                            {
                                pickTask.BillStatus = DocumentStatus.已完成.ToString();
                            }
                        }
                        else
                        {
                            LogUtils.Error("GetPickTaskChange: GetShelfTaskAllCommodityDetail" + bdAllpc.message);
                        }
					}
				}
				//else
				//{
				//	pickTask.BillStatus = DocumentStatus.异常.ToString();
					
				//}

                foreach (PickCommodity stcd in pickTaskCommodityDetails)
                {
                    stcd.CurShelfNumber = commodityCodes.Where(cit => cit.CommodityId == stcd.CommodityId).Count();
                    stcd.PlanShelfNumber = stcd.Number - stcd.PickNumber;
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

            //put修改拣货工单
            BasePutData<PickTask> basePutData =  HttpHelper.GetInstance().Put(new PickTask
			{
				id = pickTask.id,
				BillStatus = pickTask.BillStatus,
                FinishDate = pickTask.BillStatus.Equals(PickTaskStatus.已完成.ToString())?GetDateTimeNow() : null,
				version = pickTask.version
			});

            if(basePutData.code != 0)
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

				var CommodityInventoryChanges = new List<CommodityInventoryChange>(CommodityCodes.Count);

				CommodityCodes.ForEach(it=> {

                    CommodityInventoryChange cic = new CommodityInventoryChange()
                    {
                        CommodityCodeId = it.id,//商品码【扫描】
                        SourceBill = new SourceBill()//来源单据
                        {
                            object_name = typeof(PickTask).Name,
                            object_id = pickTask.id
                        }
                    };

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

                    if(!bAutoSubmit && it.AbnormalDisplay == AbnormalDisplay.异常.ToString())
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
			}

			return retBaseSinglePostDataCommodityInventoryChange;

		}

	}


}
