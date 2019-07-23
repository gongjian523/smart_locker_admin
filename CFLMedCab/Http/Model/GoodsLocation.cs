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
    /// 货位管理实体类
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class GoodsLocation : BaseModel
    {
        /// <summary>
        /// 设备存储属性：常温;阴凉;冷藏;冷冻
        /// </summary>
        public string EquipmentAttribute { get; set; }
        /// <summary>
        /// 所属设备
        /// </summary>
        public string EquipmentId { get; set; }
        /// <summary>
        /// 所属设备名称
        /// </summary>
        [JsonIgnore]
        public string EquipmentName { get; set; }
        /// <summary>
        /// 承重量
        /// </summary>
        public string Kgs { get; set; }
        /// <summary>
        /// 长（单位:cm）
        /// </summary>
        public string Length { get; set; }
        /// <summary>
        /// 特殊药品类：精二类 蛋⽩白同类 肽类 麻黄碱类
        /// </summary>
        public string SpecialDrugs { get; set; }
        /// <summary>
        /// 启用状态：启用 未启用
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 存储属性：常温 阴凉 冷藏 冷冻
        /// </summary>
        public string StorageAttribute { get; set; }
        /// <summary>
        /// 所属库房
        /// </summary>
        public string StoreHouseId { get; set; }
        /// <summary>
        /// 所属库房名称
        /// </summary>
        public string StoreHouseName { get; set; }
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
        /// 通道号
        /// </summary>
        public string enterclose { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string height { get; set; }
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
        /// <summary>
        /// 
        /// </summary>
        public string volume { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string width { get; set; }
    }

    /// <summary>
    /// 货位管理特殊药品类枚举类
    /// </summary>
    public enum GoodsLocationSpecialDrugs
    {
        /// <summary>
        /// 精二类
        /// </summary>
        [Description("精二类")]
        精二类 = 0,

        /// <summary>
        /// 蛋白同类
        /// </summary>
        [Description("蛋白同类")]
        蛋白同类 = 1,
        /// <summary>
        /// 肽类
        /// </summary>
        [Description("肽类")]
        肽类 = 2,

        /// <summary>
        /// 麻黄碱类
        /// </summary>
        [Description("麻黄碱类")]
        麻黄碱类 = 3
    }
    /// <summary>
    /// 货位管理状态枚举类
    /// </summary>
    public enum GoodsLocationStatus
    {
        /// <summary>
        /// 启用
        /// </summary>
        [Description("启用")]
        启用 = 0,

        /// <summary>
        /// 未启用
        /// </summary>
        [Description("未启用")]
        未启用 = 1,
    }
}
