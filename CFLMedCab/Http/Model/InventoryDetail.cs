using CFLMedCab.Http.Model.Base;
using CFLMedCab.Http.Model.Common;
using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace CFLMedCab.Http.Model
{
    /// <summary>
    /// 盘点商品明细资料
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    [Obsolete]
    public class InventoryDetail : BaseModel
    {
        /// <summary>
        /// 商品码
        /// </summary>
        public string CommodityCodeId { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        [JsonIgnore]
        public string CommodityCodeName { get; set; }
        /// <summary>
        /// 商品编码
        /// </summary>
        public string CommodityInventoryId { get; set; }
        /// <summary>
        /// 商品类型
        /// </summary>
        public string CommodityType { get; set; }
        /// <summary>
        /// 盘点关联单
        /// </summary>
        public string InventoryOrderId { get; set; }
        /// <summary>
        /// 质量状态：正常 损坏
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 类型：账面存在 盘点缺失 盘点新增
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public object auto_id { get; set; }
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
        public object is_deleted { get; set; }
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
    /// 盘点商品明细状态
    /// </summary>
    public enum InventoryDetailStatus
    {
        /// <summary>
        /// 正常
        /// </summary>
        [Description("正常")]
        正常 = 0,

        /// <summary>
        /// 损坏
        /// </summary>
        [Description("损坏")]
        损坏 = 1
    }
    /// <summary>
    /// 盘点商品明细类型
    /// </summary>
    public enum InventoryDetailType
    {
        /// <summary>
        /// 账面存在
        /// </summary>
        [Description("账面存在")]
        账面存在 = 0,

        /// <summary>
        /// 盘点缺失
        /// </summary>
        [Description("盘点缺失")]
        盘点缺失 = 1,

        /// <summary>
        /// 盘点新增
        /// </summary>
        [Description("盘点新增")]
        盘点新增 = 2

    }
}
