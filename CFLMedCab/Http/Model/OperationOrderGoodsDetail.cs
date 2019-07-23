using CFLMedCab.Http.Model.Base;
using CFLMedCab.Http.Model.Common;
using Newtonsoft.Json;
using System;

namespace CFLMedCab.Http.Model
{
    /// <summary>
    /// 手术单商品明细资料
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class OperationOrderGoodsDetail:BaseModel
    {
        /// <summary>
        /// 商品名称
        /// </summary>
        public string CommodityId { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        [JsonIgnore]
        public string CommodityName { get; set; } 
        /// <summary>
        /// 商品数量
        /// </summary>
        public Int32 Number { get; set; }
        /// <summary>
        /// 关联手术单
        /// </summary>
        public string OperationOrderId { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }
        /// <summary>
        /// 商品类型
        /// </summary>
        public string Type { get; set; }

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
        public string updated_at { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string updated_by { get; set; }

    }
}
