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
    /// 反向调拨任务管理
    /// </summary>
    public class AllotReverseBll : BaseBll<AllotReverseBll>
    {
        /// <summary>
        /// 通过【反向调拨任务单 and 操作人（Operator）】从表格 【反向调拨】中查询获取上架任务
        /// </summary>
        /// <param name="allotReverseName"></param>
        /// <returns></returns>
        public BaseData<AllotReverse> GetAllotReverseTask(string allotReverseName)
        {
            //获取待完成上架工单
            BaseData<AllotReverse> AllotReverse = HttpHelper.GetInstance().Get<AllotReverse>(new QueryParam
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
                                operands =  {$"'{ HttpUtility.UrlEncode(allotReverseName) }'"}
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

            AllotReverse = HttpHelper.GetInstance().ResultCheck(AllotReverse, out bool isSuccess);

            if (!isSuccess)
            {
                AllotReverse.code = (int)ResultCode.Result_Exception;
                AllotReverse.message = ResultCode.Result_Exception.ToString();
            }
            else
            {
                //如果领⽤单作废标识为【是】则弹窗提醒任务单作废，跳转回前⻚
                if ("已完成".Equals(AllotReverse.body.objects[0].Status) || "已撤销".Equals(AllotReverse.body.objects[0].Status))
                {
                    AllotReverse.code = (int)ResultCode.Result_Exception;
                    AllotReverse.message = ResultCode.Result_Exception.ToString();
                }
            }
            return AllotReverse;
        }


        /// <summary>
        /// 获取用户名下所有未完成的反向调拨单
        /// </summary>
        /// <returns></returns>
        public BaseData<AllotReverse> GetAllotReverseTask()
        {
            //获取待完成上架工单
            BaseData<AllotReverse> bdAllotReverse = HttpHelper.GetInstance().Get<AllotReverse>(new QueryParam
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

            BaseData<AllotReverseCommodity> bdAllotReverseCommodity = GetAllotReverseCommodity(bdAllotReverse);

            //校验是否含有数据
            HttpHelper.GetInstance().ResultCheck(bdAllotReverseCommodity, out bool isSuccess);

            if (isSuccess)
            {
                List<AllotReverse> taskList = new List<AllotReverse>();

                var allotReverses = bdAllotReverse.body.objects.Where(item=>item.Status != AllotReverseStatus.已完成.ToString()).ToList();

                allotReverses.ForEach(it =>
                {
                    it.PickNumber = bdAllotReverseCommodity.body.objects.Where(rcit => rcit.AllotReverse == it.id).GroupBy(rcit => rcit.AllotReverse).Select(group => group.Sum(rcit => rcit.Number)).FirstOrDefault();
                    if (it.PickNumber != 0)
                    {
                        taskList.Add(it);
                    }
                });

                bdAllotReverse.body.objects = taskList;
            }

            return bdAllotReverse;
        }


        /// <summary>
        /// 根据上架单号获取商品详情
        /// </summary>
        /// <param name="shelfTaskName"></param>
        /// <returns></returns>
        public BaseData<AllotReverseCommodity> GetAllotReverseCommodity(BaseData<AllotReverse> bdAllotReverse)
        {
            var AllotReverseId = bdAllotReverse.body.objects.Select(it => it.id).ToList();

            //校验是否含有数据，如果含有数据，拼接具体字段
            BaseData<AllotReverseCommodity> bdAllotReverseCommodity = HttpHelper.GetInstance().ResultCheck((HttpHelper hh) => {

                return hh.Get<AllotReverseCommodity>(new QueryParam
                {
                    @in =
                    {
                        field = "AllotReverse",
                        in_list = BllHelper.ParamUrlEncode(AllotReverseId)
                    }
                });

            }, bdAllotReverse);

            return bdAllotReverseCommodity;
        }

        /// <summary>
        /// 通过反向调拨单查询此任务单中商品明细
        /// </summary>
        /// <param name="AllotReverse"></param>
        /// <returns></returns>
        public BaseData<AllotReverseCommodity> GetAllotReverseCommodity(AllotReverse AllotReverse)
        {
            BaseData<AllotReverseCommodity> bdAllotReverseCommodity = HttpHelper.GetInstance().Get<AllotReverseCommodity>(new QueryParam
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
                                field = "AllotReverse",
                                @operator = "==",
                                operands =  {$"'{ HttpUtility.UrlEncode(AllotReverse.id) }'"}
                            },
                        }
                    }
                }
            });

            //校验是否含有数据，如果含有数据，拼接具体字段
            HttpHelper.GetInstance().ResultCheck(bdAllotReverseCommodity, out bool isSuccess);

            if (isSuccess)
            {
                bdAllotReverseCommodity.body.objects = bdAllotReverseCommodity.body.objects.Where(it => it.Number != 0).ToList();
                bdAllotReverseCommodity.body.objects.ForEach(it =>
                {
                    //拼接商品名字
                    if (!string.IsNullOrEmpty(it.Commodity))
                    {
                        var bdCommodity = GetCommodityById(it.Commodity);

                        HttpHelper.GetInstance().ResultCheck(bdCommodity, out bool isSuccess1);

                        if (isSuccess1)
                        {
                            it.CommodityName = bdCommodity.body.objects[0].name;
                            it.ManufactorNameStr = bdCommodity.body.objects[0].ManufactorName1;
                            it.ModelStr = bdCommodity.body.objects[0].Model1;
                            it.SpecStr = bdCommodity.body.objects[0].Spec1;
                        }

                    }
                });
            }

            return bdAllotReverseCommodity;
        }


        /// <summary>
        /// 更新反向调拨任务单
        /// </summary>
        /// <param name="AllotReverse"></param>
        /// <returns></returns>
        public BasePutData<AllotReverse> PutAllotReverse(AllotReverse AllotReverse)
        {
            AllotReverse task = new AllotReverse
            {
                id = AllotReverse.id,
                Status = AllotReverse.Status,
                version = AllotReverse.version
            };

            BasePutData <AllotReverse> basePutData = HttpHelper.GetInstance().Put(task);

            if (basePutData.code != 0)
            {
                LogUtils.Error("AllotReverse " + basePutData.message);
            }

            return basePutData;
        }

        /// <summary>
        /// 处理反向调拨任务商品变更和初始化任务单状态
        /// </summary>
        /// <param name="bdCommodityCode"></param>
        /// <param name="order"></param>
        /// <param name="bdAllotReverseCommodity"></param>
        public void GetAllotReverseChange(BaseData<CommodityCode> bdCommodityCode, AllotReverse order, BaseData<AllotReverseCommodity> bdAllotReverseCommodity)
        {
            HttpHelper.GetInstance().ResultCheck(bdAllotReverseCommodity, out bool isSuccess);
            HttpHelper.GetInstance().ResultCheck(bdCommodityCode, out bool isSuccess1);

            if (isSuccess && isSuccess1)
            {
                //反向调拨商品明细
                var allotReverseCommodities = bdAllotReverseCommodity.body.objects;
                var detailCommodityIds = allotReverseCommodities.Select(it => it.Commodity).Distinct().ToList();

                var commodityCodes = bdCommodityCode.body.objects;

                List<string> status = new List<string>();

                commodityCodes.ForEach(it => {
                    if (it.operate_type == (int)OperateType.入库)
                    {
                        it.AbnormalDisplay = AbnormalDisplay.异常.ToString();
                    }
                    else
                    {
                        if (detailCommodityIds.Contains(it.CommodityId))
                        {
                            var allotReverseCommodity = allotReverseCommodities.Where(item => item.Commodity == it.CommodityId).First();

                            if (allotReverseCommodity.Number >= ++allotReverseCommodity.PickCount)
                            {
                                it.AbnormalDisplay = AbnormalDisplay.正常.ToString();
                            }
                            else
                            {
                                //it.AbnormalDisplay = AbnormalDisplay.异常.ToString();
                                it.AbnormalDisplay = AbnormalDisplay.正常.ToString();
                            }
                        }
                        else
                        {
                            it.AbnormalDisplay = AbnormalDisplay.异常.ToString();
                        }
                    }
                });

                foreach (AllotReverseCommodity arc in allotReverseCommodities)
                {
                    arc.PickNumber = commodityCodes.Where(cit => cit.CommodityId == arc.Commodity).Count();
                    if (arc.PickNumber < arc.Number)
                    {
                        status.Add(DocumentStatus.进行中.ToString());
                    }
                    else
                    {
                        status.Add(DocumentStatus.已完成.ToString());
                    }
                }

                if (commodityCodes.Where(cci => cci.AbnormalDisplay.Contains(AbnormalDisplay.异常.ToString())).Count() > 0)
                {
                    order.Status = DocumentStatus.异常.ToString();
                }
                else if (status.Contains(DocumentStatus.进行中.ToString()))
                {
                    order.Status = DocumentStatus.进行中.ToString();
                }
                else
                {
                    order.Status = DocumentStatus.已完成.ToString();
                }
            }
        }


        /// <summary>
        /// 提交变更信息
        /// </summary>
        /// <param name="baseDataCommodityCode"></param>
        /// <param name="allotReverse"></param>
        /// <param name="baseAllotReverseCommodity"></param>
        /// <returns></returns>
        public BasePostData<AllotReverseDetail> CreateAllotReverseDetail(BaseData<CommodityCode> baseDataCommodityCode, AllotReverse allotReverse)
        {
            //校验是否含有数据，如果含有数据，有就继续下一步
            baseDataCommodityCode = HttpHelper.GetInstance().ResultCheck(baseDataCommodityCode, out bool isSuccess);

            if (isSuccess)
            {
                var CommodityCodes = baseDataCommodityCode.body.objects.Where(item=> item.operate_type == (int)OperateType.出库).ToList();
                var AllotReverseDetails = new List<AllotReverseDetail>();

                CommodityCodes.ForEach(it =>
                {
                    AllotReverseDetail ard = new AllotReverseDetail()
                    {
                        CommodityCodeId = it.id,
                        //CommodityId = it.CommodityId,
                        Status = "拣货作业",
                        AllotReverseId = allotReverse.id,
                    };

                    AllotReverseDetails.Add(ard);

                });

                return AllotReverseDetail(AllotReverseDetails);
            }
            else
            {
                return  new BasePostData<AllotReverseDetail>
                {
                    code = (int)ResultCode.Parameter_Exception,
                    message = ResultCode.Parameter_Exception.ToString()
                };
            }
        }


        public BasePostData<AllotReverseDetail> AllotReverseDetail(List<AllotReverseDetail> allotReverseDetails)
        {
            if (null == allotReverseDetails || allotReverseDetails.Count <= 0)
            {
                return new BasePostData<AllotReverseDetail>()
                {
                    code = (int)ResultCode.Parameter_Exception,
                    message = ResultCode.Parameter_Exception.ToString()
                };
            }

            return HttpHelper.GetInstance().Post<AllotReverseDetail>(new PostParam<AllotReverseDetail>()
            {
                objects = allotReverseDetails
            });
        }


        public BasePostData<CommodityInventoryChange> CreateCommodityInventoryChange(BaseData<CommodityCode> baseDataCommodityCode, AllotReverse allotReverse)
        {
            //校验是否含有数据，如果含有数据，有就继续下一步
            baseDataCommodityCode = HttpHelper.GetInstance().ResultCheck(baseDataCommodityCode, out bool isSuccess);
            if (isSuccess)
            {
                var CommodityCodes = baseDataCommodityCode.body.objects.Where(item => item.operate_type == (int)OperateType.入库).ToList();
                var CommodityInventoryChanges = new List<CommodityInventoryChange>();

                CommodityCodes.ForEach(it =>
                {
                    CommodityInventoryChange cic = new CommodityInventoryChange()
                    {
                        CommodityCodeId = it.id,//商品码【扫描】
                        SourceBill = new SourceBill()//来源单据
                        {
                            object_name = typeof(AllotReverse).Name,
                            object_id = allotReverse.id
                        },
                        operate_type = it.operate_type,
                        ChangeStatus = CommodityInventoryChangeStatus.正常.ToString(),
                        EquipmentId = it.EquipmentId,
                        StoreHouseId = it.StoreHouseId,
                        GoodsLocationId = it.GoodsLocationId,
                    };

                    CommodityInventoryChanges.Add(cic);
                });
                return CommodityInventoryChangeBll.GetInstance().CreateCommodityInventoryChangeSeparately(CommodityInventoryChanges);
            }
            else
            {
                return new BasePostData<CommodityInventoryChange>
                {
                    code = (int)ResultCode.Parameter_Exception,
                    message = ResultCode.Parameter_Exception.ToString()
                };
            }
        }
    }
}
