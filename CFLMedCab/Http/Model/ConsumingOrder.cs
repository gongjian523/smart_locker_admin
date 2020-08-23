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
    /// 关联领用单表
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class ConsumingOrder : BaseModel
    {

        /// <summary>
        /// 
        /// </summary>
        public string DepartmentId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Remarks { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Obsolete]
        [JsonIgnore]
        public string FinishDate { get; set; }
        /// <summary>
        /// 未打印
        /// </summary>
        [Obsolete]
        [JsonIgnore]
        public string PrintStatus { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Obsolete]
        [JsonIgnore]
        public string Printer { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public SourceBill SourceBill { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 未领用
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 所在库房
        /// </summary>
        [Obsolete]
        [JsonIgnore]
        public string StoreHouseId { get; set; }
        /// <summary>
        /// 所在库房名称
        /// </summary>
        [JsonIgnore]
        public string StoreHouseName { get; set; }

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
        [Obsolete]
        [JsonIgnore]
        public string markId { get; set; }

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
    public enum ConsumingOrderStatus
    {
        /// <summary>
        /// 未领用
        /// </summary>
        [Description("未领用")]
        未领用 = 0,

        /// <summary>
        /// 领用中
        /// </summary>
        [Description("领用中")]
        领用中 = 1,

        /// <summary>
        /// 已完成
        /// </summary>
        [Description("已完成")]
        已完成 = 2,
        /// <summary>
        /// 异常
        /// </summary>
        [Description("异常")]
        异常 = 3

    }

    public enum ConsumingOrderType
    {
        /// <summary>
        /// 一般领用
        /// </summary>
        [Description("一般领用")]
        一般领用 = 0,

        /// <summary>
        /// 手术领用
        /// </summary>
        [Description("手术领用")]
        手术领用 = 1,

        /// <summary>
        /// 医嘱处方领用
        /// </summary>
        [Description("医嘱处方领用")]
        医嘱处方领用 = 2,

		/// <summary>
		/// 故障领用
		/// </summary>
		[Description("故障领用")]
		故障领用 = 3
	}
}
