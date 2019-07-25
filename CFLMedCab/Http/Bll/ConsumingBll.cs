using CFLMedCab.Http.Enum;
using CFLMedCab.Http.Helper;
using CFLMedCab.Http.Model;
using CFLMedCab.Http.Model.Base;
using CFLMedCab.Http.Model.Enum;
using CFLMedCab.Http.Model.param;
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
/*
        public BaseData<ConsumingOrder> SubmitConsumingChangeWithOrder(BaseData<CommodityCode> baseDataCommodityCode,)
        {
            var order = CreateConsumingOrder(new ConsumingOrder()
            {
                FinishDate = "2019-07-"
            });
        }*/
    }

}
