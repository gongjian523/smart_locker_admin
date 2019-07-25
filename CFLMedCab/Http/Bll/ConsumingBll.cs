﻿using CFLMedCab.Http.Enum;
using CFLMedCab.Http.Helper;
using CFLMedCab.Http.Model;
using CFLMedCab.Http.Model.Base;
using CFLMedCab.Http.Model.Common;
using CFLMedCab.Http.Model.Enum;
using CFLMedCab.Http.Model.param;
using CFLMedCab.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CFLMedCab.Http.Bll
{
    /// <summary>
    /// 领用模块相关接口
    /// </summary>
    public class ConsumingBll : BaseBll<ConsumingBll>
    {
        /// <summary>
        /// 根据领用单码查询领用单信息
        /// </summary>
        /// <param name="consumingOrderName"></param>
        /// <returns></returns>
        public BaseData<ConsumingOrder> GetConsumingOrder(string consumingOrderName)
        {
            if (null == consumingOrderName)
            {
                return new BaseData<ConsumingOrder>()
                {
                    code = (int)ResultCode.Parameter_Exception,
                    message = ResultCode.Parameter_Exception.ToString()
                };
            }
            //获取待完成上架工单
            var detail =  HttpHelper.GetInstance().Get<ConsumingOrder>(new QueryParam
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
                                operands =  {$"'{ HttpUtility.UrlEncode(consumingOrderName) }'"}
                            },
                            new QueryParam.Expressions
                            {
                                field = "Status",
                                @operator = "==",
                                operands = {$"'{ HttpUtility.UrlEncode(ConsumingOrderStatus.未领用.ToString()) }'" }
                            }
                        }
                    }
                }
            });
            //校验是否含有数据，如果含有数据，拼接具体字段
            HttpHelper.GetInstance().ResultCheck(detail, out bool isSuccess);

            if (isSuccess)
            {
                detail.body.objects.ForEach(it =>
                {
                    //拼接库房名称
                    if (!string.IsNullOrEmpty(it.StoreHouseId))
                    {
                        it.StoreHouseName = GetNameById<StoreHouse>(it.StoreHouseId);
                    }
                });
            }
            //如果领⽤单作废标识为【是】则弹窗提醒手术单作废，跳转回前⻚
            if ("是".Equals(detail.body.objects[0].markId))
            {
                detail.code = (int)ResultCode.Result_Exception;
                detail.message = ResultCode.Result_Exception.ToString();
            }
            return detail;
        }
        /// <summary>
        /// 来源单据解析为【⼿手术单管理理】(ConsumingOrder.SourceBill.object_name=‘OperationOrder’ )：
        /// • 通过【手术单】(ConsumingOrder.SourceBill.object_id=OperationOrderGoodsDetail.OperationOrderId）从表格 【手术单商品明细】中查询获取领⽤用商品的列列表信息。
        /// </summary>
        /// <param name="baseDataConsumingOrder"></param>
        /// <returns></returns>
        public BaseData<OperationOrderGoodsDetail> GetOperationOrderGoodsDetail(BaseData<ConsumingOrder> baseDataConsumingOrder)
        {
            //校验是否含有数据，如果含有数据，拼接具体字段
            HttpHelper.GetInstance().ResultCheck(baseDataConsumingOrder, out bool isSuccess);
            if (!isSuccess)
            {
                return new BaseData<OperationOrderGoodsDetail>()
                {
                    code = (int)ResultCode.Parameter_Exception,
                    message = ResultCode.Parameter_Exception.ToString()
                };
            }
            if (!"OperationOrder".Equals(baseDataConsumingOrder.body.objects[0].SourceBill.object_name))
            {
                return new BaseData<OperationOrderGoodsDetail>()
                {
                    code = (Int32)ResultCode.Business_Exception,
                    message = ResultCode.Business_Exception.ToString()
                };
            }

            //根据领用单ID获取领用上列表信息
            BaseData<OperationOrderGoodsDetail> baseOperationOrderGoodsDetail = HttpHelper.GetInstance().ResultCheck((HttpHelper hh) =>
            {

                return hh.Get<OperationOrderGoodsDetail>(new QueryParam
                {
                    @in =
                    {
                        field = "OperationOrderId",
                        in_list =  { HttpUtility.UrlEncode(baseDataConsumingOrder.body.objects[0].SourceBill.object_id) }
                    }
                });

            }, baseDataConsumingOrder);

            //校验是否含有数据，如果含有数据，拼接具体字段
            HttpHelper.GetInstance().ResultCheck(baseOperationOrderGoodsDetail, out bool isSuccess2);

            if (isSuccess2)
            {
                baseOperationOrderGoodsDetail.body.objects.ForEach(it =>
                {
                    //拼接商品码名称
                    if (!string.IsNullOrEmpty(it.CommodityId))
                    {
                        it.CommodityName = GetNameById<Commodity>(it.CommodityId);
                    };
                    
                });
            }

            return baseOperationOrderGoodsDetail;

        }
        /// <summary>
        /// 来源单据解析为【医嘱处⽅方单管理理】(ConsumingOrder.SourceBill.object_name=‘PrescriptionBill’ )：
        /// • 通过【医嘱/处⽅方单】(ConsumingOrder.SourceBill.object_id=PrescriptionOrderGoodsDetail.PrescriptionBillId）从表格 【医嘱/处⽅单商品明细】中查询获取医嘱/处⽅方商品的列列表信息。
        /// </summary>
        /// <param name="commodityCode"></param>
        /// <returns></returns>
        public BaseData<PrescriptionOrderGoodsDetail> GetPrescriptionOrderGoodsDetail(BaseData<ConsumingOrder> baseDataConsumingOrder)
        {
            //校验是否含有数据，如果含有数据，拼接具体字段
            HttpHelper.GetInstance().ResultCheck(baseDataConsumingOrder, out bool isSuccess);
            if (!isSuccess)
            {
                return new BaseData<PrescriptionOrderGoodsDetail>()
                {
                    code = (int)ResultCode.Parameter_Exception,
                    message = ResultCode.Parameter_Exception.ToString()
                };
            }
            if (!"PrescriptionBill".Equals(baseDataConsumingOrder.body.objects[0].SourceBill.object_name))
            {
                return new BaseData<PrescriptionOrderGoodsDetail>()
                {
                    code = (Int32)ResultCode.Business_Exception,
                    message = ResultCode.Business_Exception.ToString()
                };
            }
            BaseData<PrescriptionOrderGoodsDetail> baseCommodityInventoryDetail = HttpHelper.GetInstance().Get<PrescriptionOrderGoodsDetail>(new QueryParam
            {
                @in =
                    {
                        field = "PrescriptionBillId",
                        in_list =  { HttpUtility.UrlEncode(baseDataConsumingOrder.body.objects[0].SourceBill.object_id) }
                    }
            });

            //校验是否含有数据，如果含有数据，拼接具体字段
            HttpHelper.GetInstance().ResultCheck(baseCommodityInventoryDetail, out bool isSuccess2);

            if (isSuccess2)
            {
                baseCommodityInventoryDetail.body.objects.ForEach(it =>
                {
                    //拼接商品编码名称
                    if (!string.IsNullOrEmpty(it.CommodityId))
                    {
                        it.CommodityName = GetNameById<Commodity>(it.CommodityId);
                    }

                });
            }
            return baseCommodityInventoryDetail;

        }
        /// <summary>
        /// 移动端 创建【领⽤用单】，且领⽤用状态为 ‘已完成’。
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public BasePostData<ConsumingOrder> CreateConsumingOrder(ConsumingOrder order)
        {
            if (null == order)
            {
                return new BasePostData<ConsumingOrder>()
                {
                    code = (int)ResultCode.Parameter_Exception,
                    message = ResultCode.Parameter_Exception.ToString()
                };
            }
            return HttpHelper.GetInstance().Post<ConsumingOrder>(new PostParam<ConsumingOrder>()
            {
                objects = new List<ConsumingOrder>() { order }
            });
        }
        /// <summary>
        /// 移动端 通过【领⽤用单编号】 查找更更新【领⽤用单】的领⽤用状态为 ‘已完成’
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public BasePutData<ConsumingOrder> UpdateConsumingOrderStatus(ConsumingOrder order)
        {

            if (null == order ||null == order.id || null == order.Status || null == order.version)
            {
                return new BasePutData<ConsumingOrder>()
                {
                    code = (int)ResultCode.Parameter_Exception,
                    message = ResultCode.Parameter_Exception.ToString()
                };
            }
            return HttpHelper.GetInstance().Put<ConsumingOrder>(new ConsumingOrder()
            {
                id = order.id,//ID
                Status = order.Status,//状态
                version = order.version//版本
            });
        }
        /// <summary>
        /// 无单领用提交接口
        /// </summary>
        /// <param name="baseDataCommodityCode"></param>
        /// <returns></returns>
        public BasePostData<CommodityInventoryChange> SubmitConsumingChangeWithoutOrder(BaseData<CommodityCode> baseDataCommodityCode)
        {
            //创建领用单
            var order = CreateConsumingOrder(new ConsumingOrder()
            {
                FinishDate = GetDateTimeNow(),//完成时间
                Status = ConsumingOrderStatus.已完成.ToString(),//领用状态
                StoreHouseId = ApplicationState.GetValue<String>((int)ApplicationKey.HouseId),//领用库房
                Type = ConsumingOrderType.一般领用.ToString()//领用类型
            });
            //校验数据是否正常
            HttpHelper.GetInstance().ResultCheck(order, out bool isSuccess);

            if (!isSuccess)
            {
                return new BasePostData<CommodityInventoryChange>()
                {
                    code = (int)ResultCode.Result_Exception,
                    message = ResultCode.Result_Exception.ToString()
                };
            }


            var normalList = new List<CommodityCode>();//回退商品列表
            var lossList = new List<CommodityCode>();//领用商品列表
            var changeList = new List<CommodityInventoryChange>();//商品库存变更记录列表

            baseDataCommodityCode.body.objects.ForEach(commodityCode => 
            {
                //为0标识为出库
                if (commodityCode.operate_type == 0) { lossList.Add(commodityCode); } else { normalList.Add(commodityCode); };

            });
            //当正常数量等于0说明未从智能柜领用商品
            if (lossList.Count <= 0)
            {
                //当正常数量大于0说明向智能柜中存放商品，需要创建商品变更记录
                if (normalList.Count > 0)
                {

                    normalList.ForEach(normal =>
                    {
                        changeList.Add(new CommodityInventoryChange()
                        {
                            CommodityCodeId = normal.id,
                            SourceBill = new SourceBill()
                            {
                                object_name = "ConsumingReturnOrder",
                            },
                            ChangeStatus = CommodityInventoryChangeStatus.正常.ToString()
                        });
                    });
                }
            }
            else
            {
                lossList.ForEach(loss =>
                {
                    changeList.Add(new CommodityInventoryChange()
                    {
                        CommodityCodeId = loss.id,
                        SourceBill = new SourceBill()
                        {
                            object_name = "ConsumingOrder",
                            object_id = order.body[0].id
                        },
                        ChangeStatus = CommodityInventoryChangeStatus.已消耗.ToString()
                    });
                });
                if (normalList.Count > 0)
                {
                    normalList.ForEach(normal =>
                    {
                        changeList.Add(new CommodityInventoryChange()
                        {
                            CommodityCodeId = normal.id,
                            SourceBill = new SourceBill()
                            {
                                object_name = "ConsumingReturnOrder",
                            },
                            ChangeStatus = CommodityInventoryChangeStatus.正常.ToString()
                        });
                    });
                }
            }

            var changes = CommodityInventoryChangeBll.GetInstance().CreateCommodityInventoryChange(changeList);

            //校验数据是否正常
            HttpHelper.GetInstance().ResultCheck(order, out bool isSuccess2);

            return changes;
        }
        /// <summary>
        /// 有单领用数据提交
        /// </summary>
        /// <param name="baseDataCommodityCode"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public BasePostData<CommodityInventoryChange> SubmitConsumingChangeWithoutOrder(BaseData<CommodityCode> baseDataCommodityCode,ConsumingOrder order)
        {
            //更新领用单状态信息
            var orderResult = UpdateConsumingOrderStatus(order);
            //校验数据是否正常
            HttpHelper.GetInstance().ResultCheck(orderResult, out bool isSuccess);
            if (!isSuccess)
            {
                return new BasePostData<CommodityInventoryChange>()
                {
                    code = orderResult.code,
                    message = orderResult.message
                };
            }
            //领用类来源单据
            var sourceBill = new SourceBill()
            {
                object_name = "ConsumingOrder",
                object_id = order.id
            };

            var tempChange = CommodityInventoryChangeBll.GetInstance().CreateCommodityInventoryChange(baseDataCommodityCode, sourceBill);
            //校验数据是否正常
            HttpHelper.GetInstance().ResultCheck(orderResult, out bool isSuccess2);

            return tempChange;
        }
        /// <summary>
        /// 提交并获取手术类领用领用状态，和商品变动状态明细
        /// </summary>
        /// <param name="baseDataCommodityCode"></param>
        /// <param name="order"></param>
        /// <param name="operationDetail"></param>
        /// <returns></returns>
        public void GetOperationOrderChangeWithOrder(BaseData<CommodityCode> baseDataCommodityCode, ConsumingOrder order, BaseData<OperationOrderGoodsDetail> operationDetail)
        {
            HttpHelper.GetInstance().ResultCheck(operationDetail, out bool isSuccess);

            HttpHelper.GetInstance().ResultCheck(baseDataCommodityCode, out bool isSuccess1);

            if(isSuccess && isSuccess1)
            {
                //手术待领用商品明细
                var operationDetails = operationDetail.body.objects;
                //获取待领用商品CommodityId列表（去重后）
                var detailCommodityIds = operationDetails.Select(it => it.CommodityId).Distinct().ToList();

                var commodityCodes = baseDataCommodityCode.body.objects;

                commodityCodes.ForEach(it => {
                    if (it.operate_type == (int)OperateType.出库)
                    {
                        it.AbnormalDisplay = AbnormalDisplay.异常.ToString();
                    }
                    else
                    {
                        if (detailCommodityIds.Contains(it.CommodityId))
                        {
                            it.AbnormalDisplay = AbnormalDisplay.正常.ToString();
                        }
                        else
                        {
                            it.AbnormalDisplay = AbnormalDisplay.异常.ToString();
                        }
                    }
                });

                //变动商品明细CommodityId列表（去重后）
                var baseDataCommodityIds = commodityCodes.Select(it => it.CommodityId).Distinct().ToList();

                //是否名称全部一致
                bool isAllContains = detailCommodityIds.All(baseDataCommodityIds.Contains) && baseDataCommodityIds.Count >= detailCommodityIds.Count;

                if (isAllContains)
                {

                    bool isAllNormal = true;

                    foreach (OperationOrderGoodsDetail oogd in operationDetails)
                    {
                        var tempCount = commodityCodes.Where(cit => cit.CommodityId == oogd.CommodityId).Count();
                        //当领用数量小于需要领用单上的数量时，状态变更为领用中
                        if (oogd.Number > tempCount)
                        {
                            order.Status = ConsumingOrderStatus.领用中.ToString();
                            isAllNormal = false;
                            break;

                        }
                    }

                    if (isAllNormal)
                    {
                        order.Status = ConsumingOrderStatus.已完成.ToString();
                    }
                }
                if(detailCommodityIds.Count > baseDataCommodityIds.Count)
                {
                    order.Status = ConsumingOrderStatus.领用中.ToString();

                }
            }
        }
        /// <summary>
        /// 提交并获取医嘱处方单领用状态，和商品变动状态明细
        /// </summary>
        /// <param name="baseDataCommodityCode"></param>
        /// <param name="order"></param>
        /// <param name="operationDetail"></param>
        /// <returns></returns>
        public void GetPrescriptionBillChangeWithOrder(BaseData<CommodityCode> baseDataCommodityCode, ConsumingOrder order, BaseData<PrescriptionOrderGoodsDetail> prescriptionDetail)
        {
            HttpHelper.GetInstance().ResultCheck(prescriptionDetail, out bool isSuccess);

            HttpHelper.GetInstance().ResultCheck(baseDataCommodityCode, out bool isSuccess1);

            if (isSuccess && isSuccess1)
            {
                //手术待领用商品明细
                var operationDetails = prescriptionDetail.body.objects;
                //获取待领用商品CommodityId列表（去重后）
                var detailCommodityIds = operationDetails.Select(it => it.CommodityId).Distinct().ToList();

                var commodityCodes = baseDataCommodityCode.body.objects;

                commodityCodes.ForEach(it => {
                    if (it.operate_type == (int)OperateType.出库)
                    {
                        it.AbnormalDisplay = AbnormalDisplay.异常.ToString();
                    }
                    else
                    {
                        if (detailCommodityIds.Contains(it.CommodityId))
                        {
                            it.AbnormalDisplay = AbnormalDisplay.正常.ToString();
                        }
                        else
                        {
                            it.AbnormalDisplay = AbnormalDisplay.异常.ToString();
                        }
                    }
                });

                //变动商品明细CommodityId列表（去重后）
                var baseDataCommodityIds = commodityCodes.Select(it => it.CommodityId).Distinct().ToList();

                //是否名称全部一致
                bool isAllContains = detailCommodityIds.All(baseDataCommodityIds.Contains) && baseDataCommodityIds.Count >= detailCommodityIds.Count;

                if (isAllContains)
                {

                    bool isAllNormal = true;

                    foreach (PrescriptionOrderGoodsDetail oogd in operationDetails)
                    {
                        var tempCount = commodityCodes.Where(cit => cit.CommodityId == oogd.CommodityId).Count();
                        int.TryParse(oogd.Number, out int number);
                        //当领用数量小于需要领用单上的数量时，状态变更为领用中
                        if ( number > tempCount)
                        {
                            order.Status = ConsumingOrderStatus.领用中.ToString();
                            isAllNormal = false;
                            break;

                        }
                    }

                    if (isAllNormal)
                    {
                        order.Status = ConsumingOrderStatus.已完成.ToString();
                    }
                }
                if (detailCommodityIds.Count > baseDataCommodityIds.Count)
                {
                    order.Status = ConsumingOrderStatus.领用中.ToString();

                }
            }
        }
    }

}
