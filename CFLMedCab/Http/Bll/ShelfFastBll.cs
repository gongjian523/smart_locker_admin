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
using System;

namespace CFLMedCab.Http.Bll
{
	/// <summary>
	/// 上架业务
	/// </summary>
	public class ShelfFastBll : BaseBll<ShelfFastBll>
	{
        public BaseData<ProcessTask> GetProcessTask(string name)
        {
            //获取待完成上架工单
            BaseData<ProcessTask> bdShelfTaskFast = HttpHelper.GetInstance().Get<ProcessTask>(new QueryParam
            {
                view_filter =
                {
                    filter =
                    {
                        //logical_relation = "1 AND 2",
                        logical_relation = "1",

                        expressions =
                        {
                            new QueryParam.Expressions
                            {
                                field = "name",
                                @operator = "==",
                                operands =  {$"'{ HttpUtility.UrlEncode(name) }'"}
                            },
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

            bdShelfTaskFast = HttpHelper.GetInstance().ResultCheck(bdShelfTaskFast, out bool isSuccess);

            if (!isSuccess)
            {
                bdShelfTaskFast.code = (int)ResultCode.Result_Exception;
                bdShelfTaskFast.message = ResultCode.Result_Exception.ToString();
            }
            else
            {
                //如果领⽤单作废标识为【是】则弹窗提醒手术单作废，跳转回前⻚
                if (!"已完成".Equals(bdShelfTaskFast.body.objects[0].Status))
                {
                    bdShelfTaskFast.code = (int)ResultCode.Result_Exception;
                    bdShelfTaskFast.message = ResultCode.Result_Exception.ToString();
                }
            }

            return bdShelfTaskFast;
        }

        public BaseData<ProcessDoneCommodity> GetProcessDoneCommodity(ProcessTask processTask)
        {
            BaseData<ProcessDoneCommodity> baseDataProcessDoneCommodity = HttpHelper.GetInstance().Get<ProcessDoneCommodity>(new QueryParam
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
                                field = "ProcessTaskId",
                                @operator = "==",
                                operands =  {$"'{ HttpUtility.UrlEncode(processTask.id) }'"}
                            },
                        }
                    }
                }

            });

            HttpHelper.GetInstance().ResultCheck(baseDataProcessDoneCommodity, out bool isSuccess);
            return baseDataProcessDoneCommodity;
        }

        [Obsolete]
        public BaseData<Allot> GetAllot(string name)
        {
            //获取待完成上架工单
            BaseData<Allot> bdShelfTaskFast = HttpHelper.GetInstance().Get<Allot>(new QueryParam
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
                                field = "name",
                                @operator = "==",
                                operands =  {$"'{ HttpUtility.UrlEncode(name) }'"}
                            }
                        }
                    }
                }
            });

            bdShelfTaskFast = HttpHelper.GetInstance().ResultCheck(bdShelfTaskFast, out bool isSuccess);

            if (!isSuccess)
            {
                bdShelfTaskFast.code = (int)ResultCode.Result_Exception;
                bdShelfTaskFast.message = ResultCode.Result_Exception.ToString();
            }
            else
            {
                if (!"待拣货".Equals(bdShelfTaskFast.body.objects[0].Status))
                {
                    bdShelfTaskFast.code = (int)ResultCode.Result_Exception;
                    bdShelfTaskFast.message = ResultCode.Result_Exception.ToString();
                }
            }

            return bdShelfTaskFast;
        }
        [Obsolete]
        public BaseData<AllotDetail> GetAllotDetail(Allot allot)
        {
            BaseData<AllotDetail> baseDataProcessDoneCommodity = HttpHelper.GetInstance().Get<AllotDetail>(new QueryParam
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
                                field = "AllotId",
                                @operator = "==",
                                operands =  {$"'{ HttpUtility.UrlEncode(allot.id) }'"}
                            },
                        }
                    }
                }

            });

            HttpHelper.GetInstance().ResultCheck(baseDataProcessDoneCommodity, out bool isSuccess);
            return baseDataProcessDoneCommodity;
        }

        public BaseData<DistributionTask> GetDistributionTask(string name)
        {
            BaseData<DistributionTask> bdShelfTaskFast = HttpHelper.GetInstance().Get<DistributionTask>(new QueryParam
            {
                view_filter =
                {
                    filter =
                    {
                        //logical_relation = "1 AND 2",
                        logical_relation = "1",
                        expressions =
                        {
                            new QueryParam.Expressions
                            {
                                field = "name",
                                @operator = "==",
                                operands =  {$"'{ HttpUtility.UrlEncode(name) }'"}
                            },
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

            bdShelfTaskFast = HttpHelper.GetInstance().ResultCheck(bdShelfTaskFast, out bool isSuccess);

            if (!isSuccess)
            {
                bdShelfTaskFast.code = (int)ResultCode.Result_Exception;
                bdShelfTaskFast.message = ResultCode.Result_Exception.ToString();
            }
            else
            {
                //如果领⽤单作废标识为【是】则弹窗提醒手术单作废，跳转回前⻚
                //if ("已完成".Equals(bdShelfTaskFast.body.objects[0].Status) || "已撤销".Equals(bdShelfTaskFast.body.objects[0].Status))
                if (!"已完成".Equals(bdShelfTaskFast.body.objects[0].Status))
                {
                    bdShelfTaskFast.code = (int)ResultCode.Result_Exception;
                    bdShelfTaskFast.message = ResultCode.Result_Exception.ToString();
                }
            }

            return bdShelfTaskFast;
        }

        public BaseData<AllotAcceptance> GetAllotAcceptanceByDistributionTaskId(string distributionTaskId)
        {
            BaseData<AllotAcceptance> bdShelfTaskFast = HttpHelper.GetInstance().Get<AllotAcceptance>(new QueryParam
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
                                field = "DistributionTaskId",
                                @operator = "==",
                                operands =  {$"'{ HttpUtility.UrlEncode(distributionTaskId) }'"}
                            }
                        }
                    }
                }
            });

            bdShelfTaskFast = HttpHelper.GetInstance().ResultCheck(bdShelfTaskFast, out bool isSuccess);

            return bdShelfTaskFast;
        }

        public BaseData<AcceptanceCommodity> GetAcceptanceCommodity(AllotAcceptance allotAcceptance)
        {
            BaseData<AcceptanceCommodity> bdAcceptanceCommodity = HttpHelper.GetInstance().Get<AcceptanceCommodity>(new QueryParam
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
                                field = "AllotAcceptanceId",
                                @operator = "==",
                                operands =  {$"'{ HttpUtility.UrlEncode(allotAcceptance.id) }'"}
                            },
                        }
                    }
                }

            });

            HttpHelper.GetInstance().ResultCheck(bdAcceptanceCommodity, out bool isSuccess);
            return bdAcceptanceCommodity;
        }

        public BasePostData<ShelfTaskFast> CreateShelfTaskFask(ShelfTaskFast task)
        {
            if (null == task)
            {
                return new BasePostData<ShelfTaskFast>()
                {
                    code = (int)ResultCode.Parameter_Exception,
                    message = ResultCode.Parameter_Exception.ToString()
                };
            }
            return HttpHelper.GetInstance().Post<ShelfTaskFast>(new PostParam<ShelfTaskFast>()
            {
                objects = new List<ShelfTaskFast>() { task }
            });
        }

        public BasePostData<ShelfTaskFastDetail> CreateShelfTaskFaskDetail( List<ShelfTaskFastDetail> detailList)
        {
            if (null == detailList)
            {
                return new BasePostData<ShelfTaskFastDetail>()
                {
                    code = (int)ResultCode.Parameter_Exception,
                    message = ResultCode.Parameter_Exception.ToString()
                };
            }
            return HttpHelper.GetInstance().Post<ShelfTaskFastDetail>(new PostParam<ShelfTaskFastDetail>()
            {
                objects = detailList
            });  
        }


        /// <summary>
        /// 根据加工/调拨任务单获取快捷上架任务单
        /// </summary>
        /// <param name="business"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public BaseData<ShelfTaskFast> GetShelfTaskFast(string  business, string id)
        {
            //获取待完成上架工单
            BaseData<ShelfTaskFast> bdShelfTaskFast = HttpHelper.GetInstance().Get<ShelfTaskFast>(new QueryParam
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
                                field = "SourceBill.object_id",
                                @operator = "==",
                                operands =  {$"'{ HttpUtility.UrlEncode(id) }'"}
                            },
                            new QueryParam.Expressions
                            {
                                field = "SourceBill.object_name",
                                @operator = "==",
                                operands =  {$"'{ HttpUtility.UrlEncode(business) }'"}
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

            return bdShelfTaskFast;
        }


		/// <summary>
		/// 根据上架单号获取任务单详情
		/// </summary>
		/// <param name="shelfTaskFastName"></param>
		/// <returns></returns>
		public BaseData<ShelfTaskFast> GetShelfTaskFast(string shelfTaskFastName)
		{
            //获取待完成上架工单
            BaseData<ShelfTaskFast> bdShelfTaskFast = HttpHelper.GetInstance().Get<ShelfTaskFast>(new QueryParam
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
								operands =  {$"'{ HttpUtility.UrlEncode(shelfTaskFastName) }'"}
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

            bdShelfTaskFast = HttpHelper.GetInstance().ResultCheck(bdShelfTaskFast, out bool isSuccess);

            if (!isSuccess)
            {
                bdShelfTaskFast.code = (int)ResultCode.Result_Exception;
                bdShelfTaskFast.message = ResultCode.Result_Exception.ToString();
            }
            else
            {
                //如果领⽤单作废标识为【是】则弹窗提醒手术单作废，跳转回前⻚
                if ("已完成".Equals(bdShelfTaskFast.body.objects[0].Status) || "已撤销".Equals(bdShelfTaskFast.body.objects[0].Status))
                {
                    bdShelfTaskFast.code = (int)ResultCode.Result_Exception;
                    bdShelfTaskFast.message = ResultCode.Result_Exception.ToString();
                }
            }

            return bdShelfTaskFast;
        }

		/// <summary>
		/// 根据上架单号获取任务单详情
		/// </summary>
		/// <param name="shelfTaskName"></param>
		/// <returns></returns>
		public BaseData<ShelfTaskFast> GetShelfTaskFast()
		{
			//获取待完成上架工单
			BaseData<ShelfTaskFast> baseDataShelfTaskFast = HttpHelper.GetInstance().Get<ShelfTaskFast>(new QueryParam
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

			BaseData<ShelfTaskFastDetail> baseDataShelfTaskFastDetail = GetShelfTaskFastDetail(baseDataShelfTaskFast);

			//校验是否含有数据
			HttpHelper.GetInstance().ResultCheck(baseDataShelfTaskFastDetail, out bool isSuccess);

			if (isSuccess)
			{
                List<ShelfTaskFast> taskList = new List<ShelfTaskFast>();

				var shelfTasks = baseDataShelfTaskFast.body.objects.Where(item=>(item.Status  != ShelfTaskStatus.已完成.ToString() && item.Status != ShelfTaskStatus.已撤销.ToString())).ToList();

                //shelfTasks.ForEach(it =>
                //{
                //    it.NeedShelfTotalNumber = baseDataShelfTaskFastDetail.body.objects.Where(sit => sit.ShelfTaskId == it.id).GroupBy(sit =>sit.ShelfTaskId).Select(group => group.Sum(sit => (sit.NeedShelfNumber - sit.AlreadyShelfNumber))).FirstOrDefault();
                //    if (it.NeedShelfTotalNumber != 0)
                //    {
                //        taskList.Add(it);
                //    }
                //});

                baseDataShelfTaskFast.body.objects = taskList;
			}

			return baseDataShelfTaskFast;
		}
		/// <summary>
		/// 根据上架单号获取商品详情
		/// </summary>
		/// <param name="shelfTaskFastName"></param>
		/// <returns></returns>
		public BaseData<ShelfTaskFastDetail> GetShelfTaskFastDetail(string shelfTaskFastName)
		{
			return GetShelfTaskFastDetail(GetShelfTaskFast(shelfTaskFastName));
		}

		/// <summary>
		/// 根据上架单号获取商品详情
		/// </summary>
		/// <param name="shelfTaskName"></param>
		/// <returns></returns>
		public BaseData<ShelfTaskFastDetail> GetShelfTaskFastDetail(BaseData<ShelfTaskFast> baseDataShelfTaskFast)
		{

			//校验是否含有数据，如果含有数据，拼接具体字段
			BaseData<ShelfTaskFastDetail> baseDataShelfTaskFastDetail = HttpHelper.GetInstance().ResultCheck((HttpHelper hh) => {
				
				return hh.Get<ShelfTaskFastDetail>(new QueryParam
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

			}, baseDataShelfTaskFast);

            HttpHelper.GetInstance().ResultCheck(baseDataShelfTaskFast, out bool isSuccess);
            HttpHelper.GetInstance().ResultCheck(baseDataShelfTaskFastDetail, out bool isSuccess1);

            if (isSuccess && isSuccess1)
            {
                var shelfTaskFastIds = baseDataShelfTaskFast.body.objects.Select(it => it.id).ToList();
                baseDataShelfTaskFastDetail.body.objects = baseDataShelfTaskFastDetail.body.objects.Where(it => shelfTaskFastIds.Contains(it.ShelfTaskFastId)).ToList();
            }
            return baseDataShelfTaskFastDetail;
		}


		/// <summary>
		/// 根据上架单号获取商品详情
		/// </summary>
		/// <param name="shelfTaskName"></param>
		/// <returns></returns>
		public BaseData<ShelfTaskFastDetail> GetShelfTaskFastDetail(ShelfTaskFast shelfTaskFast)
		{

			BaseData<ShelfTaskFastDetail> baseDataShelfTaskFastDetail = HttpHelper.GetInstance().Get<ShelfTaskFastDetail>(new QueryParam
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
								field = "ShelfTaskFastId",
								@operator = "==",
								operands =  {$"'{ HttpUtility.UrlEncode(shelfTaskFast.id) }'"}
							},
                            new QueryParam.Expressions
                            {
                                field = "Status",
                                @operator = "==",
                                operands = {$"'{ HttpUtility.UrlEncode("待上架") }'" }
                            },
                        }
					}
				}
		
			});

			//校验是否含有数据，如果含有数据，拼接具体字段
			HttpHelper.GetInstance().ResultCheck(baseDataShelfTaskFastDetail, out bool isSuccess);

			if (isSuccess)
			{
				baseDataShelfTaskFastDetail.body.objects.ForEach(it =>
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

                    //拼接货位名字
                    if (!string.IsNullOrEmpty(it.CommodityCodeId))
                    {
                        it.CommodityCodeName = GetNameById<CommodityCode>(it.CommodityCodeId);
                    }

                    //拼接商品名字
                    if (!string.IsNullOrEmpty(it.CommodityId))
					{
                        var bdCommodity = GetCommodityById(it.CommodityId);

                        HttpHelper.GetInstance().ResultCheck(bdCommodity, out bool isSuccess1);

                        if (isSuccess1)
                        {
                            it.CommodityName = bdCommodity.body.objects[0].name;
                            it.ManufactorName1 = bdCommodity.body.objects[0].ManufactorName1;
                            it.Model1 = bdCommodity.body.objects[0].Model1;
                            it.Spec1 = bdCommodity.body.objects[0].Spec1;
                        }
                    }
				});
			}

			return baseDataShelfTaskFastDetail;
		}


        /// <summary>
        /// 根据上架单中所有货柜下的商品详情
        /// </summary>
        /// <param name="shelfTaskName"></param>
        /// <returns></returns>
        public BaseData<ShelfTaskFastDetail> GetShelfTaskAllFastDetail(ShelfTaskFast shelfTaskFast)
        {
            BaseData<ShelfTaskFastDetail> baseDataShelfTaskFastDetail = HttpHelper.GetInstance().Get<ShelfTaskFastDetail>(new QueryParam
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
                                field = "ShelfTaskFastId",
                                @operator = "==",
                                operands =  {$"'{ HttpUtility.UrlEncode(shelfTaskFast.id) }'"}
                            }
                        }
                    }
                }

            });

            return baseDataShelfTaskFastDetail;
        }

        /// <summary>
        /// 获取变化后的上架单
        /// </summary>
        /// <param name="bdCommodityCode"></param>
        /// <param name="baseDataShelfTask"></param>
        /// <param name="bdShelfTaskFastDetail"></param>
        /// <returns></returns>
        public void GetShelfTaskChange(BaseData<CommodityCode> bdCommodityCode, ShelfTaskFast shelfTaskFast, BaseData<ShelfTaskFastDetail> bdShelfTaskFastDetail)
		{
			HttpHelper.GetInstance().ResultCheck(bdShelfTaskFastDetail, out bool isSuccess);
            HttpHelper.GetInstance().ResultCheck(bdCommodityCode, out bool isSuccess1);

            if (isSuccess && isSuccess1)
			{
                List<string> locIds = bdCommodityCode.body.objects.Select(item => item.GoodsLocationId).Distinct().ToList();
                List<string> status = new List<string>();

                locIds.ForEach(id => {

                    //上架任务单商品RF码id
                    var sfdCommodityCodeIds = bdShelfTaskFastDetail.body.objects.Select(it => it.CommodityCodeId).Distinct().ToList();

                    var commodityCodes = bdCommodityCode.body.objects.Where(item => item.GoodsLocationId == id).ToList();

                    //商品异常状态回显
                    commodityCodes.ForEach(it => {
                        if (it.operate_type == (int)OperateType.出库)
                        {
                            it.AbnormalDisplay = AbnormalDisplay.异常.ToString();
                        }
                        else
                        {
                            if (sfdCommodityCodeIds.Contains(it.id))
                            {
                                it.AbnormalDisplay = AbnormalDisplay.正常.ToString();
                            }
                            else
                            {
                                it.AbnormalDisplay = AbnormalDisplay.异常.ToString();
                            }
                        }
                    });
                });


                if(bdCommodityCode.body.objects.Where(item => item.operate_type == (int)OperateType.出库).Count() > 0)
                {
                    //包含了出库商品，状态单就显示异常
                    shelfTaskFast.Status = ShelfTaskFastStatusEnum.异常.ToString();
                }
                else if(bdCommodityCode.body.objects.Where(item => item.operate_type == (int)OperateType.入库 && item.AbnormalDisplay == AbnormalDisplay.异常.ToString()).Count() > 0)
                {
                    //入库的不在任务单上面的商品，状态单就显示异常
                    shelfTaskFast.Status = ShelfTaskFastStatusEnum.异常.ToString();
                }
                else 
                {
                    int needShelfNum = bdShelfTaskFastDetail.body.objects.Where(item => item.Status == ShelfTaskFastDetailStatusType.待上架.ToString()).Count();
                    int normalShelfNum = bdCommodityCode.body.objects.Where(item => item.operate_type == (int)OperateType.入库 && item.AbnormalDisplay == AbnormalDisplay.正常.ToString()).Count();
                    
                    if(normalShelfNum >= needShelfNum)
                    {   
                        //正常上架的商品数量超过或者等于（实际上不应该超过）需要上架的商品数量，状态设置成已完成
                        //便捷上架不用考虑其他设备，已经完成状态可以直接上传
                        shelfTaskFast.Status = ShelfTaskFastStatusEnum.已完成.ToString();
                    }
                    else
                    {
                        shelfTaskFast.Status = ShelfTaskFastStatusEnum.待上架.ToString();
                    }
                }
            }
		}

        /// <summary>
        /// 更新上架任务单
        /// </summary>
        /// <param name="shelfTaskFast"></param>
        /// <param name="abnormalCauses"></param>
        /// <param name="bAutoSubmit">是否是主动提交</param>
        /// <returns></returns>
        public BasePutData<ShelfTaskFast> PutShelfTaskFast(ShelfTaskFast shelfTaskFast)
        {

            ShelfTaskFast task = new ShelfTaskFast
            {
                id = shelfTaskFast.id,
                Status = shelfTaskFast.Status,
                version = shelfTaskFast.version
            };

            if (shelfTaskFast.Status == ShelfTaskFastStatusEnum.异常.ToString() && shelfTaskFast.AbnormalCauses != "")
            {
                task.AbnormalCauses = shelfTaskFast.AbnormalCauses;
            }

            //当任务单状态为已完成时，携带完成时间进行更新
            if (shelfTaskFast.Status == ShelfTaskFastStatusEnum.已完成.ToString())
            {
                task.FinishDate = GetDateTimeNow();
            }

            BasePutData<ShelfTaskFast> basePutData = HttpHelper.GetInstance().Put(task);

            if (basePutData.code != 0)
            {
                LogUtils.Error("PutShelfTaskFast 失败！ " + basePutData.message);
            }
            return basePutData;
        }

        /// <summary>
        /// 上架的库存变化
        /// </summary>
        /// <param name="baseDataCommodityCode"></param>
        /// <param name="shelfTaskFast"></param>
        /// <param name="bAutoSubmit">是否是主动提交</param>
        /// <returns></returns>
        public BasePostData<CommodityInventoryChange> CreateShelfTaskCommodityInventoryChange(BaseData<CommodityCode> baseDataCommodityCode, ShelfTaskFast shelfTaskFast, bool bAutoSubmit)
		{

			BasePostData<CommodityInventoryChange> retBaseSinglePostDataCommodityInventoryChange = null;

			//校验是否含有数据，如果含有数据，有就继续下一步
			baseDataCommodityCode = HttpHelper.GetInstance().ResultCheck(baseDataCommodityCode, out bool isSuccess);

			if (isSuccess)
			{
				var CommodityCodes = baseDataCommodityCode.body.objects;
				var CommodityInventoryChanges = new List<CommodityInventoryChange>();

                CommodityCodes.ForEach(it =>
                {
                    CommodityInventoryChange cic = new CommodityInventoryChange()
                    {
                        CommodityCodeId = it.id,//商品码【扫描】
                        SourceBill = new SourceBill()//来源单据
                        {
                            object_name = typeof(ShelfTaskFast).Name,
                            object_id = shelfTaskFast.id
                        },
                        operate_type = it.operate_type
                        //EquipmentId = ApplicationState.GetEquipId(),
                        //StoreHouseId = ApplicationState.GetHouseId(),
                        //GoodsLocationId = it.GoodsLocationId
                    };

                    if (it.operate_type == (int)OperateType.出库)
                    {
                        cic.ChangeStatus = CommodityInventoryChangeStatus.未上架.ToString();
                        cic.StoreHouseId = ApplicationState.GetHouseId();
                        cic.AdjustStatus = CommodityInventoryChangeAdjustStatus.是.ToString();
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

                LogUtils.Error("CreateShelfTaskCommodityInventoryChange 失败！ " + baseDataCommodityCode.message);
			}
			return retBaseSinglePostDataCommodityInventoryChange;
		}

	}


}
