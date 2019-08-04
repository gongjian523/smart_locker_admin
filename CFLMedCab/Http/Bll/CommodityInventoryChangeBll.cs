using CFLMedCab.Http.Enum;
using CFLMedCab.Http.Helper;
using CFLMedCab.Http.Model;
using CFLMedCab.Http.Model.Base;
using CFLMedCab.Http.Model.Common;
using CFLMedCab.Http.Model.param;
using CFLMedCab.Infrastructure;
using CFLMedCab.Infrastructure.ToolHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Http.Bll
{
    /// <summary>
    /// 商品库存变更记录信资料
    /// </summary>
    public class CommodityInventoryChangeBll : BaseBll<CommodityInventoryChangeBll>
    {
        /// <summary>
        /// 创建商品库存变更记录
        /// </summary>
        /// <param name="changes"></param>
        /// <returns></returns>
        public BasePostData<CommodityInventoryChange> CreateCommodityInventoryChange(List<CommodityInventoryChange> changes)
        {
            if(null == changes || changes.Count <= 0)
            {
                LogUtils.Warn("CreateCommodityInventoryChange" + ResultCode.Result_Exception.ToString());
                return new BasePostData<CommodityInventoryChange>()
                {
                    code = (int)ResultCode.Parameter_Exception,
                    message = ResultCode.Parameter_Exception.ToString()
                };
            }
            return HttpHelper.GetInstance().Post<CommodityInventoryChange>(new PostParam<CommodityInventoryChange>()
            {
                objects = changes
            });
        }
        /// <summary>
        /// 根据商品码变更列表和来源单据创建库存变更记录资料(有单领用)
        /// </summary>
        /// <param name="baseDataCommodityCode"></param>
        /// <param name="sourceBill"></param>
        /// <returns></returns>
        public BasePostData<CommodityInventoryChange> CreateCommodityInventoryChange(BaseData<CommodityCode> baseDataCommodityCode, SourceBill sourceBill)
        {
            if (null == baseDataCommodityCode || null == sourceBill || null == sourceBill.object_name)
            {
                return new BasePostData<CommodityInventoryChange>()
                {
                    code = (int)ResultCode.Parameter_Exception,
                    message = ResultCode.Parameter_Exception.ToString()
                };
            }
            List<CommodityInventoryChange> changes = new List<CommodityInventoryChange>();
            baseDataCommodityCode.body.objects.ForEach(commodityCode =>
            {
                var temp = new CommodityInventoryChange()
                {
                    CommodityCodeId = commodityCode.id,
                };
                switch (commodityCode.operate_type)
                {
                    case 0:
                        //出库变更更后库房、变更更后设备、变更更后货位 value 值都为null。
                        temp.SourceBill = sourceBill;
                        temp.ChangeStatus = CommodityInventoryChangeStatus.已消耗.ToString();
                        break;
                    case 1:
                        temp.SourceBill = new SourceBill()
                        {
                            object_name = "ConsumingReturnOrder",
                        };
                        temp.ChangeStatus = CommodityInventoryChangeStatus.正常.ToString();
                        temp.EquipmentId = commodityCode.EquipmentId;
                        temp.GoodsLocationId = commodityCode.GoodsLocationId;
                        temp.StoreHouseId = commodityCode.StoreHouseId;
                        break;
                }
                changes.Add(temp);
            });
            return CreateCommodityInventoryChange(changes);
        }   
        /// <summary>
        /// 根据商品码变更列表和来源单据创建库存变更记录资料（回退）
        /// </summary>
        /// <param name="baseDataCommodityCode"></param>
        /// <returns></returns>
        public BasePostData<CommodityInventoryChange> CreateCommodityInventoryChange(BaseData<CommodityCode> baseDataCommodityCode)
        {
            if (null == baseDataCommodityCode)
            {
                return new BasePostData<CommodityInventoryChange>()
                {
                    code = (int)ResultCode.Parameter_Exception,
                    message = ResultCode.Parameter_Exception.ToString()
                };
            }
            List<CommodityInventoryChange> changes = new List<CommodityInventoryChange>();
            var count = baseDataCommodityCode.body.objects.Select(it => it.operate_type == 0).Count();
            if(count > 0)//当出库数量大于0说明在领用需要创建领用单
            {
                var consumingOrder = ConsumingBll.GetInstance().CreateConsumingOrder(new ConsumingOrder()
                {
                    Status = ConsumingOrderStatus.已完成.ToString(),
                    StoreHouseId = ApplicationState.GetValue<String>((int)ApplicationKey.HouseId),
                    Type = ConsumingOrderType.一般领用.ToString()

                });
                //校验数据是否正常
                HttpHelper.GetInstance().ResultCheck(consumingOrder, out bool isSuccess);
                if (isSuccess)
                {
                    //创建商品库存变更记录资料【出库::领用】
                    baseDataCommodityCode.body.objects.Where(it=>it.operate_type == 0).ToList().ForEach(commodityCode =>
                    {
                        var temp = new CommodityInventoryChange()
                        {
                            CommodityCodeId = commodityCode.id,
                            //出库变更更后库房、变更更后设备、变更更后货位 value 值都为null。
                            SourceBill = new SourceBill()
                            {
                                object_name = "ConsumingOrder",
                                object_id = consumingOrder.body[0].id
                            },
                            ChangeStatus = CommodityInventoryChangeStatus.已消耗.ToString()
                        };
                        changes.Add(temp);
                    });
                }
            }
            ////创建商品库存变更记录资料【入库::回退】
            baseDataCommodityCode.body.objects.Where(it => it.operate_type == 1).ToList().ForEach(commodityCode =>
            {
                var temp = new CommodityInventoryChange()
                {
                    CommodityCodeId = commodityCode.id,
                    SourceBill = new SourceBill()
                    {
                        object_name = "ConsumingReturnOrder",
                    },
                    ChangeStatus = CommodityInventoryChangeStatus.正常.ToString(),
                    EquipmentId = commodityCode.EquipmentId,
                    GoodsLocationId = commodityCode.GoodsLocationId,
                    StoreHouseId = commodityCode.StoreHouseId,
                };
                changes.Add(temp);
            });

            return CreateCommodityInventoryChange(changes);
        }
    }
}
