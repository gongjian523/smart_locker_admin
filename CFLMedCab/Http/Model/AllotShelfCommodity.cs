using CFLMedCab.Http.Model.Base;
using CFLMedCab.Http.Model.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Http.Model
{
    /// <summary>
    /// 调拨上架任务商品明细资料
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class AllotShelfCommodity : BaseModel
    {
     
        /// <summary>
        /// 商品编码
        /// </summary>
        public string CommodityCode { get; set; }

        /// <summary>
        /// 商品码
        /// </summary>
        public string CommodityCodeId { get; set; }

        /// <summary>
        /// 商品码名称
        /// </summary>
        [JsonIgnore]
        public string CommodityCodeName { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public string CommodityId { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        [JsonIgnore]
        public string CommodityName { get; set; }

        /// <summary>
        /// 设备编号
        /// </summary>
        public string EquipmentId { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        [JsonIgnore]
        public string EquipmentName { get; set; }
        /// <summary>
        /// 上架货位编号
        /// </summary>
        public string GoodsLocationId { get; set; }

        /// <summary>
        /// 上架货位名称
        /// </summary>
        [JsonIgnore]
        public string GoodsLocationName { get; set; }

        /// <summary>
        /// 商品状态（未上架，已上架）
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 上架库房编号
        /// </summary>
        public string StoreHouseId { get; set; }

        /// <summary>
        /// 上架库房名称
        /// </summary>
        [JsonIgnore]
        public string StoreHouseName { get; set; }

        /// <summary>
        /// 商品类型（定数包，单品）
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 调拨上架任务编号
        /// </summary>
        public string AllotShelfId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int auto_id { get; set; }
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
        public int is_deleted { get; set; }
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
        public string updated_at { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string updated_by { get; set; }


    }
    /// <summary>
    /// 调拨上架任务商品明细资料
    /// </summary>
    public enum AllotShelfCommodityStatus
    {
        /// <summary>
        /// 未上架
        /// </summary>
        [Description("未上架")]
        未上架 = 0,

        /// <summary>
        /// 已上架
        /// </summary>
        [Description("已上架")]
        已上架 = 1,
    }
}
