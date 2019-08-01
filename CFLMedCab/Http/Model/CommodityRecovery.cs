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
    /// 商品回收任务单
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class CommodityRecovery : BaseModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string FinishDate { get; set; }
        /// <summary>
        /// 操作人
        /// </summary>
        public string Operator { get; set; }
        /// <summary>
        /// 单据状态：待回收，已完成
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 回收库房
        /// </summary>
        public string StoreHouse { get; set; }
        /// <summary>
        /// 回收库房名称
        /// </summary>
        [JsonIgnore]
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
}
