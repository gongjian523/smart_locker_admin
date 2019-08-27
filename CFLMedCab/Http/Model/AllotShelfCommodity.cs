using CFLMedCab.Http.Model.Base;
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
        public string CommodityCode;

        /// <summary>
        /// 商品码
        /// </summary>
        public string CommodityCodeId;

        /// <summary>
        /// 商品码名称
        /// </summary>
        [JsonIgnore]
        public string CommodityCodeName;

        /// <summary>
        /// 商品编号
        /// </summary>
        public string CommodityId;

        /// <summary>
        /// 商品名称
        /// </summary>
        [JsonIgnore]
        public string CommodityName;

        /// <summary>
        /// 设备编号
        /// </summary>
        public string EquipmentId;

        /// <summary>
        /// 设备名称
        /// </summary>
        [JsonIgnore]
        public string EquipmentName;
        /// <summary>
        /// 上架货位编号
        /// </summary>
        public string GoodsLocationId;

        /// <summary>
        /// 上架货位名称
        /// </summary>
        [JsonIgnore]
        public string GoodsLocationName;

        /// <summary>
        /// 商品状态（未上架，已上架）
        /// </summary>
        public string Status;

        /// <summary>
        /// 上架库房编号
        /// </summary>
        public string StoreHouseId;

        /// <summary>
        /// 上架库房名称
        /// </summary>
        [JsonIgnore]
        public string StoreHouseName;

        /// <summary>
        /// 商品类型（定数包，单品）
        /// </summary>
        public string Type;

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
