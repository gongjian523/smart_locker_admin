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
	public class ShelfFastBll : BaseBll<ShelfFastBll>
	{

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
						logical_relation = "1 AND 2 AND 3",
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

					//拼接商品名字
					if (!string.IsNullOrEmpty(it.CommodityId))
					{
						it.CommodityName = GetNameById<Commodity>(it.CommodityId);
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
        /// <param name="baseDatacommodityCode"></param>
        /// <param name="baseDataShelfTask"></param>
        /// <param name="baseDataShelfTaskFastDetail"></param>
        /// <returns></returns>
        public void GetShelfTaskChange(BaseData<CommodityCode> baseDatacommodityCode, ShelfTaskFast shelfTaskFast, BaseData<ShelfTaskFastDetail> baseDataShelfTaskFastDetail)
		{
			HttpHelper.GetInstance().ResultCheck(baseDataShelfTaskFastDetail, out bool isSuccess);
            HttpHelper.GetInstance().ResultCheck(baseDatacommodityCode, out bool isSuccess1);

            if (isSuccess && isSuccess1)
			{
                List<string> locIds = baseDataShelfTaskFastDetail.body.objects.Select(item => item.GoodsLocationId).Distinct().ToList();
                List<string> status = new List<string>();

                locIds.ForEach(id => {
                    //上架任务单商品详情列表
                    var shelfTaskCommodityDetails = baseDataShelfTaskFastDetail.body.objects.Where(item => item.GoodsLocationId == id);
                    //var shelfTaskCommodityDetails = baseDataShelfTaskFastDetail.body.objects.Where(item => item.NeedShelfNumber != item.AlreadyShelfNumber && item.GoodsLocationId == id);
                    //上架任务单商品码
                    var sfdCommodityIds = shelfTaskCommodityDetails.Select(it => it.CommodityId).Distinct().ToList();

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
                                var shelfTaskCommodityDetail = shelfTaskCommodityDetails.Where(item => item.CommodityId == it.CommodityId).First();

                                //if ((shelfTaskCommodityDetail.NeedShelfNumber - shelfTaskCommodityDetail.AlreadyShelfNumber) >= ++shelfTaskCommodityDetail.CountShelfNumber)
                                {
                                    it.AbnormalDisplay = AbnormalDisplay.正常.ToString();
                                }
                                //else
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

                        //foreach (ShelfTaskCommodityDetail stcd in shelfTaskCommodityDetails)
                        //{
                        //    //数量超出
                        //    if ((stcd.NeedShelfNumber - stcd.AlreadyShelfNumber) < commodityCodes.Where(cit => cit.CommodityId == stcd.CommodityId).Count())
                        //    {
                        //        isNoOver = false;
                        //        break;
                        //    }

                        //    //数量不相等
                        //    if ((stcd.NeedShelfNumber - stcd.AlreadyShelfNumber) != commodityCodes.Where(cit => cit.CommodityId == stcd.CommodityId).Count())
                        //    {
                        //        isAllSame = false;
                        //    }
                        //}

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
                    shelfTaskFast.Status = DocumentStatus.异常.ToString();
                }
                else if (status.Contains(DocumentStatus.进行中.ToString()))
                {
                    shelfTaskFast.Status = DocumentStatus.进行中.ToString();
                }
                else
                {
                    //获取这个任务单中所有的商品详情
                    //BaseData<ShelfTaskFastDetail> bdAllstcd = GetShelfTaskAllFastDetail(shelfTask);

                    //HttpHelper.GetInstance().ResultCheck(bdAllstcd, out bool isSuccess2);

                    //if (isSuccess2)
                    //{
                    //    //只有所有商品都完成了上架，不管在那个货架上，才能将这个任务单的状态改为“已完成”
                    //    //if (bdAllstcd.body.objects.Where(it => it.NeedShelfNumber != it.AlreadyShelfNumber && it.EquipmentId != ApplicationState.GetEquipId()).Count() == 0)
                    //    {
                    //        shelfTask.Status = DocumentStatus.已完成.ToString();
                    //    }
                    //    //else
                    //    {
                    //        shelfTask.Status = DocumentStatus.进行中.ToString();
                    //    }
                    //}
                    //else
                    //{
                    //    LogUtils.Error("GetShelfTaskChange: GetShelfTaskAllCommodityDetail" + bdAllstcd.message);
                    //}
                }

                //foreach (ShelfTaskCommodityDetail stcd in baseDataShelfTaskFastDetail.body.objects)
                //{
                //    stcd.CurShelfNumber = baseDatacommodityCode.body.objects.Where(cit => cit.CommodityId == stcd.CommodityId).Count();
                //    stcd.PlanShelfNumber = stcd.NeedShelfNumber - stcd.AlreadyShelfNumber;
                //}
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

            if (shelfTaskFast.Status == DocumentStatus.异常.ToString() && shelfTaskFast.AbnormalCauses != "")
            {
                task.AbnormalCauses = shelfTaskFast.AbnormalCauses;
            }

            //当任务单状态为已完成时，携带完成时间进行更新
            if (shelfTaskFast.Status == DocumentStatus.已完成.ToString())
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

                    if (!bAutoSubmit && it.AbnormalDisplay == AbnormalDisplay.异常.ToString())
                    {
                        cic.AdjustStatus = CommodityInventoryChangeAdjustStatus.是.ToString();
                    }

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
