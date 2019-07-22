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
        public enum ConsumingOrderStatus
        {
            /// <summary>
            /// 出库
            /// </summary>
            [Description("未领用")]
            未领用 = 0,

            /// <summary>
            /// 入库
            /// </summary>
            [Description("领用中")]
            领用中 = 1,

            /// <summary>
            /// 入库
            /// </summary>
            [Description("已完成")]
            已完成 = 2

        }
        /// <summary>
        /// 根据领用单码查询领用单信息
        /// </summary>
        /// <param name="consumingOrderName"></param>
        /// <returns></returns>
        public BaseData<ConsumingOrder> GetConsumingOrder(string consumingOrderName)
        {
            //获取待完成上架工单
            return HttpHelper.GetInstance().Get<ConsumingOrder>(new QueryParam
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
        }
        /// <summary>
        /// 来源单据解析为【⼿手术单管理理】(ConsumingOrder.SourceBill.object_name=‘OperationOrder’ )：
        /// • 通过【手术单】(ConsumingOrder.SourceBill.object_id=OperationOrderGoodsDetail.OperationOrderId）从表格 【手术单商品明细】中查询获取领⽤用商品的列列表信息。
        /// </summary>
        /// <param name="baseDataConsumingOrder"></param>
        /// <returns></returns>
        public BaseData<OperationOrderGoodsDetail> GetOperationOrderGoodsDetail(BaseData<ConsumingOrder> baseDataConsumingOrder)
        {
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
            HttpHelper.GetInstance().ResultCheck(baseOperationOrderGoodsDetail, out bool isSuccess);

            if (isSuccess)
            {
                baseOperationOrderGoodsDetail.body.objects.ForEach(it =>
                {
                    //拼接商品码名称
                    if (!string.IsNullOrEmpty(it.CommodityId))
                    {
                        it.CommodityName = GetNameById<Commodity>(it.CommodityId);
                    }
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
            HttpHelper.GetInstance().ResultCheck(baseCommodityInventoryDetail, out bool isSuccess);

            if (isSuccess)
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
    }

}
