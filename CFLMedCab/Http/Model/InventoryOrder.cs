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
    /// 盘点单管理
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class InventoryOrder: BaseModel
    {
        /// <summary>
        /// 确认时间
        /// </summary>
        public string ConfirmDate { get; set; }
        /// <summary>
        /// 盘点设备
        /// </summary>
        public string EquipmentId { get; set; }
        /// <summary>
        /// 盘点设备名称
        /// </summary>
        [JsonIgnore]
        public string EquipmentName { get; set; }
        /// <summary>
        /// 盘点货位
        /// </summary>
        public string GoodsLocationId { get; set; }
        /// <summary>
        /// 盘点货位名称
        /// </summary>
        [JsonIgnore]
        public string GoodsLocationName { get; set; }
        /// <summary>
        /// 盘点任务单
        /// </summary>
        public string InventoryTaskId { get; set; }
        /// <summary>
        /// 盘点状态：待盘点 盘点中 待确认 已确认
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 盘点库房
        /// </summary>
        public string StoreHouseId { get; set; }
        /// <summary>
        /// 盘点库房名称
        /// </summary>
        [JsonIgnore]
        public string StoreHouseName { get; set; }
        /// <summary>
        /// T创建类型：手动创建 自动创建
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
    /// 盘点单状态
    /// </summary>
    public enum InventoryOrderStatus
    {
        ///待盘点 盘点中 待确认 已确认
        /// <summary>
        /// 待盘点
        /// </summary>
        [Description("待盘点")]
        待盘点 = 0,

        /// <summary>
        /// 盘点中
        /// </summary>
        [Description("盘点中")]
        盘点中 = 1,

        /// <summary>
        /// 待确认
        /// </summary>
        [Description("待确认")]
        待确认 = 2,

        /// <summary>
        /// 已确认
        /// </summary>
        [Description("已确认")]
        已确认 = 3,
        /// <summary>
        /// 已完成
        /// </summary>
        [Description("已完成")]
        已完成 = 4

    }
    /// <summary>
    /// 盘点单类型
    /// </summary>
    public enum InventoryOrderType
    {

        /// <summary>
        /// 手动创建
        /// </summary>
        [Description("手动创建")]
        手动创建 = 0,

        /// <summary>
        /// 自动创建
        /// </summary>
        [Description("自动创建")]
        自动创建 = 1

    }
}
