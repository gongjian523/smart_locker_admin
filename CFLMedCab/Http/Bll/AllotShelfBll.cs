using CFLMedCab.Http.Helper;
using CFLMedCab.Http.Model;
using CFLMedCab.Http.Model.Base;
using CFLMedCab.Http.Model.param;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CFLMedCab.Infrastructure;
using System.Threading.Tasks;
using System.Web;
using CFLMedCab.Http.Enum;
using CFLMedCab.Infrastructure.ToolHelper;
using CFLMedCab.Http.Model.Common;
using CFLMedCab.Http.Model.Enum;

namespace CFLMedCab.Http.Bll
{
    /// <summary>
    /// 调拨上架任务管理
    /// </summary>
    public class AllotShelfBll : BaseBll<AllotShelfBll>
    {
        /// <summary>
        /// 通过【调拨上架任务单 and 操作人（Operator）】从表格 【调拨上架】中查询获取上架任务
        /// </summary>
        /// <param name="allotShelfName"></param>
        /// <returns></returns>
        public BaseData<AllotShelf> GetAllotShelfTask(string allotShelfName)
        {
            //获取待完成上架工单
            BaseData<AllotShelf> allotShelf = HttpHelper.GetInstance().Get<AllotShelf>(new QueryParam
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
                                operands =  {$"'{ HttpUtility.UrlEncode(allotShelfName) }'"}
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

            allotShelf = HttpHelper.GetInstance().ResultCheck(allotShelf, out bool isSuccess);

            if (!isSuccess)
            {
                allotShelf.code = (int)ResultCode.Result_Exception;
                allotShelf.message = ResultCode.Result_Exception.ToString();
            }
            else
            {
                //如果领⽤单作废标识为【是】则弹窗提醒手术单作废，跳转回前⻚
                if ("已完成".Equals(allotShelf.body.objects[0].Status) || "已撤销".Equals(allotShelf.body.objects[0].Status))
                {
                    allotShelf.code = (int)ResultCode.Result_Exception;
                    allotShelf.message = ResultCode.Result_Exception.ToString();
                }
            }

            return allotShelf;

        }


        /// <summary>
        /// 根据上架单号获取任务单详情
        /// </summary>
        /// <param name="shelfTaskName"></param>
        /// <returns></returns>
        public BaseData<AllotShelf> GetAllotShelfTask()
        {
            //获取待完成上架工单
            BaseData<AllotShelf> baseDataShelfTask = HttpHelper.GetInstance().Get<AllotShelf>(new QueryParam
            {
                view_filter =
                {
                    filter =
                    {
                        //logical_relation = "1 AND 2",
                        logical_relation = "1",
                        expressions =
                        {
                            //new QueryParam.Expressions
                            //{
                            //    field = "Status",
                            //    @operator = "==",
                            //    operands = {$"'{ HttpUtility.UrlEncode(ShelfTaskStatus.待上架.ToString()) }'" }
                            //},
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

            //BaseData<AllotShelf> baseDataShelfTask = HttpHelper.GetInstance().Get<AllotShelf>();

            BaseData<AllotShelfCommodity> baseDataShelfTaskCommodityDetail = GetShelfTaskCommodityDetail(baseDataShelfTask);

            //校验是否含有数据
            HttpHelper.GetInstance().ResultCheck(baseDataShelfTaskCommodityDetail, out bool isSuccess);

            if (isSuccess)
            {
                List<AllotShelf> taskList = new List<AllotShelf>();

                var shelfTasks = baseDataShelfTask.body.objects.Where(item=>item.Status != ShelfTaskStatus.已完成.ToString() && item.Status != ShelfTaskStatus.异常.ToString()).ToList();

                shelfTasks.ForEach(it =>
                {
                    it.ShelfNumber = baseDataShelfTaskCommodityDetail.body.objects.Where(sit => sit.AllotShelfId == it.id && sit.Status == CommodityInventoryChangeStatus.未上架.ToString()).Count();
                    if (it.ShelfNumber != 0)
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
        public BaseData<AllotShelfCommodity> GetShelfTaskCommodityDetail(BaseData<AllotShelf> baseDataShelfTask)
        {
            var allotShelfId = baseDataShelfTask.body.objects.Select(it => it.id).ToList();

            //校验是否含有数据，如果含有数据，拼接具体字段
            BaseData<AllotShelfCommodity> baseDataShelfTaskCommodityDetail = HttpHelper.GetInstance().ResultCheck((HttpHelper hh) => {

                return hh.Get<AllotShelfCommodity>(new QueryParam
                {
                    @in =
                    {
                        field = "AllotShelfId",
                        in_list = BllHelper.ParamUrlEncode(allotShelfId)
                    }
                });

            }, baseDataShelfTask);

            return baseDataShelfTaskCommodityDetail;
        }

        /// <summary>
        /// 通过【调拨上架任务】（AllotShelf.id=AllotShelfCommodity.AllotShelfId）从表格 【调拨上架商品明细】中查询获取调拨上架商品的列表信息
        /// </summary>
        /// <param name="allotShelf"></param>
        /// <returns></returns>
        public BaseData<AllotShelfCommodity> GetShelfTaskCommodityDetail(AllotShelf allotShelf)
        {

            BaseData<AllotShelfCommodity> baseDataAllotShelfCommodity = HttpHelper.GetInstance().Get<AllotShelfCommodity>(new QueryParam
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
                                field = "AllotShelfId",
                                @operator = "==",
                                operands =  {$"'{ HttpUtility.UrlEncode(allotShelf.id) }'"}
                            },

                            new QueryParam.Expressions
                            {
                                field = "Status",
                                @operator = "==",
                                operands = {$"'{ HttpUtility.UrlEncode(CommodityInventoryChangeStatus.未上架.ToString()) }'" }
                            }
                        }
                    }
                }

            });

            //校验是否含有数据，如果含有数据，拼接具体字段
            HttpHelper.GetInstance().ResultCheck(baseDataAllotShelfCommodity, out bool isSuccess);

            if (isSuccess)
            {
                baseDataAllotShelfCommodity.body.objects.ForEach(it =>
                {

                    //拼接商品码名称
                    if (!string.IsNullOrEmpty(it.CommodityCodeId))
                    {
                        it.CommodityCodeName = GetNameById<CommodityCode>(it.CommodityCodeId);
                    }

                    //拼接商品名字
                    if (!string.IsNullOrEmpty(it.CommodityId))
                    {
                        it.CommodityName = GetNameById<Commodity>(it.CommodityId);
                    }
                });
            }

            return baseDataAllotShelfCommodity;
        }


        /// <summary>
        /// 根据挑拨上架单中所有货柜下的商品详情
        /// </summary>
        /// <param name="shelfTaskName"></param>
        /// <returns></returns>
        public BaseData<AllotShelfCommodity> GetShelfTaskAllCommodityDetail(AllotShelf allotShelf)
        {
            BaseData<AllotShelfCommodity> baseDataShelfTaskCommodityDetail = HttpHelper.GetInstance().Get<AllotShelfCommodity>(new QueryParam
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
                                field = "AllotShelfId",
                                @operator = "==",
                                operands =  {$"'{ HttpUtility.UrlEncode(allotShelf.id) }'"}
                            }
                        }
                    }
                }

            });

            return baseDataShelfTaskCommodityDetail;
        }
        /// <summary>
        /// 更新调拨上架任务单
        /// </summary>
        /// <param name="allotShelf"></param>
        /// <returns></returns>
        public BasePutData<AllotShelf> PutAllotShelf(AllotShelf allotShelf)
        {
            AllotShelf task = new AllotShelf
            {
                id = allotShelf.id,
                Status = allotShelf.Status,
                version = allotShelf.version
            };

            if(allotShelf.Status == AllotShelfStatusEnum.异常.ToString() && allotShelf.AbnormalCauses != "")
            {
                task.AbnormalCauses = allotShelf.AbnormalCauses;
            }
            
            if(allotShelf.Status == AllotShelfStatusEnum.已完成.ToString())
            {
                task.FinishDate = GetDateTimeNow();
            }

            BasePutData <AllotShelf> basePutData = HttpHelper.GetInstance().Put(task);

            if (basePutData.code != 0)
            {
                LogUtils.Error("AllotShelf " + basePutData.message);
            }

            return basePutData;
        }

        /// <summary>
        /// 处理调拨上架任务商品变更和初始化任务单状态
        /// </summary>
        /// <param name="baseDataCommodityCode"></param>
        /// <param name="order"></param>
        /// <param name="baseAllotShelfCommodity"></param>
        public void HandleAllotShelfChangeWithOrder(BaseData<CommodityCode> baseDataCommodityCode, AllotShelf order, BaseData<AllotShelfCommodity> baseAllotShelfCommodity)
        {
            HttpHelper.GetInstance().ResultCheck(baseAllotShelfCommodity, out bool isSuccess);

            HttpHelper.GetInstance().ResultCheck(baseDataCommodityCode, out bool isSuccess1);

            if (isSuccess && isSuccess1)
            {
                //调拨上架商品明细
                var allotShelfCommodities = baseAllotShelfCommodity.body.objects;
                //获取待上架商品CommodityCodeName列表（去重后）
                var detailCommodityCodes = allotShelfCommodities.Select(it => it.CommodityCodeName).Distinct().ToList();

                var commodityCodes = baseDataCommodityCode.body.objects;
                //记录任务单异常状态
                var IsException = false;

                var number = 0;
                //遍历变化后的商品码列表
                commodityCodes.ForEach(it =>
                {
                    //向智能柜里面放东西
                    if (it.operate_type == (int)OperateType.入库)
                    {
                        //商品在任务单中
                        if (detailCommodityCodes.Contains(it.name))
                        {
                            it.AbnormalDisplay = AbnormalDisplay.正常.ToString();
                            number++;

                        }
                        //商品不在任务单中【待确认】
                        else
                        {
                            it.AbnormalDisplay = AbnormalDisplay.异常.ToString();
                            IsException = true;
                        }

                    }
                    //从智能柜中取东西
                    else
                    {
                        it.AbnormalDisplay = AbnormalDisplay.异常.ToString();
                        IsException = true;
                    }
                });

                //当商品出现出库或放入异常商品时以及正常数量和预定数量不相等时判定主单状态为异常

                if (!IsException)
                {
                    //当正常数量小于待上架商品。主单状态为进行中
                    if (number < allotShelfCommodities.Count)
                    {
                        order.Status = AllotShelfStatusEnum.进行中.ToString();
                    }
                    //当正常数量等于待上架商品，主单状态为已完成
                    else
                    {
                        order.Status = AllotShelfStatusEnum.已完成.ToString();
                    }
                    //由于CommodityId唯一，所以不存在第三种情况

                }
                else
                {
                    order.Status = AllotShelfStatusEnum.异常.ToString();
                }
            }
        }


        /// <summary>
        /// 提交变更信息
        /// </summary>
        /// <param name="baseDataCommodityCode"></param>
        /// <param name="allotShelf"></param>
        /// <param name="baseAllotShelfCommodity"></param>
        /// <returns></returns>

        public BasePostData<CommodityInventoryChange> SubmitAllotShelfChangeWithOrder(BaseData<CommodityCode> baseDataCommodityCode, AllotShelf allotShelf, BaseData<AllotShelfCommodity> baseAllotShelfCommodity)
        {
            BasePostData<CommodityInventoryChange> retBaseSinglePostDataCommodityInventoryChange = null;

            //校验是否含有数据，如果含有数据，有就继续下一步
            baseDataCommodityCode = HttpHelper.GetInstance().ResultCheck(baseDataCommodityCode, out bool isSuccess);

            baseAllotShelfCommodity = HttpHelper.GetInstance().ResultCheck(baseAllotShelfCommodity, out bool isSuccess2);
            if (isSuccess && isSuccess2)
            {
                var CommodityCodes = baseDataCommodityCode.body.objects;
                var CommodityInventoryChanges = new List<CommodityInventoryChange>();

                //调拨上架商品明细
                var allotShelfCommodities = baseAllotShelfCommodity.body.objects;
                //获取待上架商品CommodityId列表（去重后）
                var detailCommodityIds = allotShelfCommodities.Select(it => it.CommodityId).Distinct().ToList();

                CommodityCodes.ForEach(it =>
                {
                    CommodityInventoryChange cic = new CommodityInventoryChange()
                    {
                        CommodityCodeId = it.id,//商品码【扫描】
                        SourceBill = new SourceBill()//来源单据
                        {
                            object_name = typeof(AllotShelf).Name,
                            object_id = allotShelf.id
                        },
                        operate_type = it.operate_type
                        //EquipmentId = ApplicationState.GetEquipId(),
                        //StoreHouseId = ApplicationState.GetHouseId(),
                        //GoodsLocationId = it.GoodsLocationId
                    };

                    //【待确认】出库
                    if (it.operate_type == (int)OperateType.出库)
                    {
                        cic.ChangeStatus = CommodityInventoryChangeStatus.未上架.ToString();
                        cic.StoreHouseId = ApplicationState.GetHouseId();
                        cic.AdjustStatus = CommodityInventoryChangeAdjustStatus.是.ToString();
                    }
                    //入库
                    else
                    {
                        cic.ChangeStatus = CommodityInventoryChangeStatus.正常.ToString();
                        cic.EquipmentId = it.EquipmentId;
                        cic.StoreHouseId = it.StoreHouseId;
                        cic.GoodsLocationId = it.GoodsLocationId;
                        //【待确认】
                        if (!detailCommodityIds.Contains(it.CommodityId))
                        {
                            cic.AdjustStatus = CommodityInventoryChangeAdjustStatus.是.ToString();
                        }
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
