using CFLMedCab.Http.Model.Base;
using CFLMedCab.Http.Model.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Http.Model
{
    /// <summary>
    /// 领用商品明细表
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class ConsumingGoodsDetail : BaseModel
    {
        /// <summary>
        /// 商品码
        /// </summary>
        public string CommodityCodeId { get; set; }
        /// <summary>
        /// 商品码名称
        /// </summary>
        public string CommodityCodeName { get; set; }
        /// <summary>
        /// 商品编码
        /// </summary>
        public string CommodityId { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string CommodityName { get; set; }
        /// <summary>
        /// 已领用商品ID
        /// </summary>
        public string CommodityInventoryId { get; set; }
        /// <summary>
        /// 已领用商品名称
        /// </summary>
        public string CommodityInventoryName { get; set; }
        /// <summary>
        /// 关联领用单
        /// </summary>
        public string ConsumingOrderId { get; set; }
        /// <summary>
        /// 设备ID
        /// </summary>
        public string EquipmentId { get; set; }
        /// <summary>
        /// 设备名称
        /// </summary>
        public string EquipmentName { get; set; }
        /// <summary>
        /// 所在货位ID
        /// </summary>
        public string GoodsLocationId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ReceiveDate { get; set; }
        /// <summary>
        /// 未领用
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 所在库房ID
        /// </summary>
        public string StoreHouseId { get; set; }
        /// <summary>
        /// 所在库房名称
        /// </summary>
        public string StoreHouseName { get; set; }
        /// <summary>
        /// 定数包
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string created_at { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string created_by { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string owner { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Permission permission { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string record_type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string system_mod_stamp { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public object unusedAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string updated_at { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string updated_by { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public object usedAmount { get; set; }

        /// <summary>
        /// 在机柜的库存数量
        /// </summary>
        [JsonIgnore]
        public int stockNum { get; set; }

        /// <summary>
        /// 还需要领用的数量（unusedAmount - usedAmount）
        /// </summary>
        //[JsonIgnore]
        //public int needNum { get; set; }
    }
}
