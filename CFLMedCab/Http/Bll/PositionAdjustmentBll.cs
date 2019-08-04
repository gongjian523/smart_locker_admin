using CFLMedCab.Http.Enum;
using CFLMedCab.Http.Helper;
using CFLMedCab.Http.Model;
using CFLMedCab.Http.Model.Base;
using CFLMedCab.Http.Model.Common;
using CFLMedCab.Http.Model.param;
using CFLMedCab.Infrastructure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Http.Bll
{
    /// <summary>
    /// 货位调整
    /// </summary>
    public class PositionAdjustmentBll : BaseBll<PositionAdjustmentBll>
    {
        /// <summary>
        /// 货位调整
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
            //创建商品库存变更记录资料【出库::下架】
            var outList = baseDataCommodityCode.body.objects.Where(it => it.operate_type == 0).ToList();
            if(null != outList || outList.Count > 0)
            {
                outList.ForEach(commodityCode =>
                {
                    var temp = new CommodityInventoryChange()
                    {
                        CommodityCodeId = commodityCode.id,
                        //出库变更更后库房、变更更后设备、变更更后货位 value 值都为null。
                        StoreHouseId = commodityCode.StoreHouseId,//变更后库房
                        ChangeStatus = CommodityInventoryChangeStatus.未上架.ToString(),
                        AdjustStatus = CommodityInventoryChangeAdjustStatus.是.ToString()
                    };
                    changes.Add(temp);
                });
            }

            //创建商品库存变更记录资料【入库::上架】
            var inList = baseDataCommodityCode.body.objects.Where(it => it.operate_type == 1).ToList();
            if(null != outList || outList.Count > 0)
            {
                inList.ForEach(commodityCode =>
                {
                    var temp = new CommodityInventoryChange()
                    {
                        CommodityCodeId = commodityCode.id,
                        ChangeStatus = CommodityInventoryChangeStatus.正常.ToString(),
                        AdjustStatus = CommodityInventoryChangeAdjustStatus.是.ToString(),
                        EquipmentId = commodityCode.EquipmentId,
                        GoodsLocationId = commodityCode.GoodsLocationId,
                        StoreHouseId = commodityCode.StoreHouseId
                    };
                    changes.Add(temp);
                });
            }
            return CommodityInventoryChangeBll.GetInstance().CreateCommodityInventoryChange(changes);
        }
    }
}
