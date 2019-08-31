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
    public class AllotShelfBll : BaseBll<AllotShelf>
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
                        logical_relation = "1 AND 2 AND 3",
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
            HttpHelper.GetInstance().ResultCheck(baseDataAllotShelfCommodity, out bool isSuccess);

            if (isSuccess)
            {
                baseDataAllotShelfCommodity.body.objects.ForEach(it =>
                {
                    ///按理说根据该条件查询出来的商品设备名称和库房名称已经固定
                    ////拼接设备名字
                    //if (!string.IsNullOrEmpty(it.EquipmentId))
                    //{
                    //    it.EquipmentName = GetNameById<Equipment>(it.EquipmentId);
                    //}
                    it.EquipmentName = HttpUtility.UrlEncode(ApplicationState.GetEquipName());


                    ////拼接库房名字
                    //if (!string.IsNullOrEmpty(it.StoreHouseId))
                    //{
                    //    it.StoreHouseName = GetNameById<StoreHouse>(it.StoreHouseId);
                    //}
                    it.StoreHouseName = HttpUtility.UrlEncode(ApplicationState.GetHouseName());

                    //拼接商品码名称
                    if (!string.IsNullOrEmpty(it.CommodityCodeId))
                    {
                        it.CommodityCodeName = GetNameById<CommodityCode>(it.CommodityCodeId);
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

            return baseDataAllotShelfCommodity;
        }
        /// <summary>
        /// 更新调拨上架任务单
        /// </summary>
        /// <param name="allotShelf"></param>
        /// <returns></returns>
        public BasePutData<AllotShelf> PutAllotShelf(AllotShelf allotShelf)
        {

            BasePutData<AllotShelf> basePutData = HttpHelper.GetInstance().Put(new AllotShelf
            {
                id = allotShelf.id,
                Status = allotShelf.Status,
                version = allotShelf.version
            });

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
                //获取待上架商品CommodityId列表（去重后）
                var detailCommodityIds = allotShelfCommodities.Select(it => it.CommodityId).Distinct().ToList();

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
                        if (detailCommodityIds.Contains(it.CommodityId))
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
                    if (number < detailCommodityIds.Count)
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
        /// <param name="bAutoSubmit"></param>
        /// <returns></returns>

        public BasePostData<CommodityInventoryChange> SubmitAllotShelfChangeWithOrder(BaseData<CommodityCode> baseDataCommodityCode, AllotShelf allotShelf, BaseData<AllotShelfCommodity> baseAllotShelfCommodity,bool bAutoSubmit)
        {
            BasePostData<CommodityInventoryChange> retBaseSinglePostDataCommodityInventoryChange = null;

            //校验是否含有数据，如果含有数据，有就继续下一步
            baseDataCommodityCode = HttpHelper.GetInstance().ResultCheck(baseDataCommodityCode, out bool isSuccess);

            baseAllotShelfCommodity = HttpHelper.GetInstance().ResultCheck(baseAllotShelfCommodity, out bool isSuccess2);
            if (isSuccess && isSuccess2)
            {
                var CommodityCodes = baseDataCommodityCode.body.objects;
                var CommodityInventoryChanges = new List<CommodityInventoryChange>(CommodityCodes.Count);

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
