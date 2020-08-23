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

			BaseData<ShelfTaskDetail> baseDataShelfTaskDetail = GetShelfTaskDetail(baseDataShelfTask);

			//校验是否含有数据
			HttpHelper.GetInstance().ResultCheck(baseDataShelfTaskDetail, out bool isSuccess);

			if (isSuccess)
			{
                List<ShelfTask> taskList = new List<ShelfTask>();

				var shelfTasks = baseDataShelfTask.body.objects.Where(item=>(item.Status  != ShelfTaskStatus.已完成.ToString() && item.Status != ShelfTaskStatus.已撤销.ToString())).ToList();

                shelfTasks.ForEach(it =>
                {
                    it.NeedShelfTotalNumber = baseDataShelfTaskDetail.body.objects.Where(sit => sit.ShelfTaskId == it.id).GroupBy(sit =>sit.ShelfTaskId).Select(group => group.Sum(sit => sit.Number)).FirstOrDefault();
                    if (it.NeedShelfTotalNumber != 0)
                    {
                        taskList.Add(it);
                    }
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
		public BaseData<ShelfTaskDetail> GetShelfTaskCommodityDetail(string shelfTaskName)
		{
			return GetShelfTaskDetail(GetShelfTask(shelfTaskName));
		}

		/// <summary>
		/// 根据上架单号获取商品详情
		/// </summary>
		/// <param name="shelfTaskName"></param>
		/// <returns></returns>
		public BaseData<ShelfTaskDetail> GetShelfTaskDetail(BaseData<ShelfTask> baseDataShelfTask)
		{

			//校验是否含有数据，如果含有数据，拼接具体字段
			BaseData<ShelfTaskDetail> baseDataShelfTaskDetail = HttpHelper.GetInstance().ResultCheck((HttpHelper hh) => {
				
				return hh.Get<ShelfTaskDetail>(new QueryParam
				{
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
                                //    field = "ShelfTaskId",
                                //    @operator = "INRANGE",
                                //    operands =  BllHelper.OperandsProcess(shelfTaskIds)
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

			}, baseDataShelfTask);

            HttpHelper.GetInstance().ResultCheck(baseDataShelfTask, out bool isSuccess);
            HttpHelper.GetInstance().ResultCheck(baseDataShelfTaskDetail, out bool isSuccess1);

            if (isSuccess && isSuccess1)
            {
                var shelfTaskIds = baseDataShelfTask.body.objects.Select(it => it.id).ToList();
                baseDataShelfTaskDetail.body.objects = baseDataShelfTaskDetail.body.objects.Where(it => shelfTaskIds.Contains(it.ShelfTaskId)).ToList();
            }
            return baseDataShelfTaskDetail;
		}


		/// <summary>
		/// 根据上架单号获取商品详情
		/// </summary>
		/// <param name="shelfTaskName"></param>
		/// <returns></returns>
		public BaseData<ShelfTaskDetail> GetShelfTaskDetail(ShelfTask shelfTask)
		{
			BaseData<ShelfTaskDetail> baseDataShelfTaskDetail = HttpHelper.GetInstance().Get<ShelfTaskDetail>(new QueryParam
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
			HttpHelper.GetInstance().ResultCheck(baseDataShelfTaskDetail, out bool isSuccess);

			if (isSuccess)
			{
				baseDataShelfTaskDetail.body.objects.ForEach(it =>
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
					if (!string.IsNullOrEmpty(it.Commodity))
					{
                        var bdCommodity = GetCommodityById(it.Commodity);

                        HttpHelper.GetInstance().ResultCheck(bdCommodity, out bool isSuccess1);

                        if(isSuccess1)
                        {
                            it.CommodityName = bdCommodity.body.objects[0].name;
                            it.ManufactorName1 = bdCommodity.body.objects[0].ManufactorName1;
                            it.Model1 = bdCommodity.body.objects[0].Model1;
                            it.Spec1 = bdCommodity.body.objects[0].Spec1;
                        }
					}
                });
			}

			return baseDataShelfTaskDetail;
		}


        /// <summary>
        /// 根据上架单中所有货柜下的商品详情
        /// </summary>
        /// <param name="shelfTaskName"></param>
        /// <returns></returns>
        public BaseData<ShelfTaskDetail> GetShelfTaskAllDetail(ShelfTask shelfTask)
        {
            BaseData<ShelfTaskDetail> baseDataShelfTaskDetail = HttpHelper.GetInstance().Get<ShelfTaskDetail>(new QueryParam
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
                            }
                        }
                    }
                }

            });

            return baseDataShelfTaskDetail;
        }

        /// <summary>
        /// 获取变化后的上架单
        /// </summary>
        /// <param name="baseDatacommodityCode"></param>
        /// <param name="baseDataShelfTask"></param>
        /// <param name="baseDataShelfTaskDetail"></param>
        /// <returns></returns>
        public void GetShelfTaskChange(BaseData<CommodityCode> baseDatacommodityCode, ShelfTask shelfTask, BaseData<ShelfTaskDetail> baseDataShelfTaskDetail)
		{
			HttpHelper.GetInstance().ResultCheck(baseDataShelfTaskDetail, out bool isSuccess);
            HttpHelper.GetInstance().ResultCheck(baseDatacommodityCode, out bool isSuccess1);

            if (isSuccess && isSuccess1)
			{
                List<string> locIds = baseDataShelfTaskDetail.body.objects.Select(item => item.GoodsLocationId).Distinct().ToList();
                List<string> status = new List<string>();

                locIds.ForEach(id => {
                    //上架任务单商品详情列表
                    var shelfTaskDetails = baseDataShelfTaskDetail.body.objects.Where(item => item.Number != 0 && item.GoodsLocationId == id);
                    //上架任务单商品码
                    var sfdCommodityIds = shelfTaskDetails.Select(it => it.Commodity).Distinct().ToList();

                    var commodityCodes = baseDatacommodityCode.body.objects.Where(item => item.GoodsLocationId == id).ToList();

                    //商品异常状态回显
                    commodityCodes.ForEach(it => {
                        if (it.operate_type == (int)OperateType.出库)
                        {
                            it.AbnormalDisplay = AbnormalDisplay.异常.ToString();
                        }
                        else
                        {
                            if (sfdCommodityIds.Contains(it.CommodityId))
                            {
                                var shelfTaskDetail = shelfTaskDetails.Where(item => item.Commodity == it.CommodityId).First();

                                if (shelfTaskDetail.Number  >= ++shelfTaskDetail.CountShelfNumber)
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
                        //不存在领用的商品数量超过了领用单规定的数量
                        bool isNoOver = true;
                        //所有商品的数量都和领用单规定的一样
                        bool isAllSame = true;

                        foreach (ShelfTaskDetail stcd in shelfTaskDetails)
                        {
                            //数量超出
                            if (stcd.Number < commodityCodes.Where(cit => cit.CommodityId == stcd.Commodity).Count())
                            {
                                isNoOver = false;
                                break;
                            }

                            //数量不相等
                            if (stcd.Number != commodityCodes.Where(cit => cit.CommodityId == stcd.Commodity).Count())
                            {
                                isAllSame = false;
                            }
                        }

                        if (isNoOver)
                        {
                            if (isAllSame)
                            {
                                status.Add(DocumentStatus.已完成.ToString());
                            }
                            else
                            {
                                status.Add(DocumentStatus.进行中.ToString());
                            }
                        }
                        else
                        {
                            status.Add(DocumentStatus.异常.ToString());
                        }
                    }
                    else
                    {
                        status.Add(DocumentStatus.异常.ToString());
                    }

                });

                if(status.Contains(DocumentStatus.异常.ToString()))
                {
                    shelfTask.Status = DocumentStatus.异常.ToString();
                }
                else if (status.Contains(DocumentStatus.进行中.ToString()))
                {
                    shelfTask.Status = DocumentStatus.进行中.ToString();
                }
                else
                {
                    //获取这个任务单中所有的商品详情
                    BaseData<ShelfTaskDetail> bdAllstcd = GetShelfTaskAllDetail(shelfTask);

                    HttpHelper.GetInstance().ResultCheck(bdAllstcd, out bool isSuccess2);

                    if (isSuccess2)
                    {
                        //只有所有商品都完成了上架，不管在那个货架上，才能将这个任务单的状态改为“已完成”
                        if (bdAllstcd.body.objects.Where(it => it.Number != 0 && it.EquipmentId != ApplicationState.GetEquipId()).Count() == 0)
                        {
                            shelfTask.Status = DocumentStatus.已完成.ToString();
                        }
                        else
                        {
                            shelfTask.Status = DocumentStatus.进行中.ToString();
                        }
                    }
                    else
                    {
                        LogUtils.Error("GetShelfTaskChange: GetShelfTaskAllDetail" + bdAllstcd.message);
                    }
                }

                foreach (ShelfTaskDetail stcd in baseDataShelfTaskDetail.body.objects)
                {
                    stcd.CurShelfNumber = baseDatacommodityCode.body.objects.Where(cit => cit.CommodityId == stcd.Commodity).Count();
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
        public BasePutData<ShelfTask> PutShelfTask(ShelfTask shelfTask)
        {

            ShelfTask task = new ShelfTask
            {
                id = shelfTask.id,
                Status = shelfTask.Status,
                version = shelfTask.version
            };

            if (shelfTask.Status == DocumentStatus.异常.ToString() && shelfTask.AbnormalCauses != "")
            {
                task.AbnormalCauses = shelfTask.AbnormalCauses;
            }

            //当任务单状态为已完成时，携带完成时间进行更新
            if (shelfTask.Status == DocumentStatus.已完成.ToString())
            {
                task.FinishDate = GetDateTimeNow();
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
				var CommodityInventoryChanges = new List<CommodityInventoryChange>();

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
                        operate_type = it.operate_type
                    };

                    if (it.operate_type == (int)OperateType.出库)
                    {
                        cic.ChangeStatus = CommodityInventoryChangeStatus.未上架.ToString();
                        cic.StoreHouseId = ApplicationState.GetHouseId();
                        //只有出库的时候才会设置调整状态
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
