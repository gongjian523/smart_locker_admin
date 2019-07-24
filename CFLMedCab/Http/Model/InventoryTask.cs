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
    /// 盘点任务单
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class InventoryTask : BaseModel
    {
        /// <summary>
        /// 完成时间
        /// </summary>
        public string FinishDate { get; set; }
        /// <summary>
        /// 操作人
        /// </summary>
        public string @Operator { get; set; }
        /// <summary>
        /// 单据状态：待盘点 已完成
        /// </summary>
        public string Status { get; set; }
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

    public enum InventoryTaskStatus
    {
        /// <summary>
        /// 待盘点
        /// </summary>
        [Description("待盘点")]
        待盘点 = 0,

        /// <summary>
        /// 已完成
        /// </summary>
        [Description("已完成")]
        已完成 = 1

    }
}
