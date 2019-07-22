using CFLMedCab.Http.Model.Base;
using CFLMedCab.Http.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Http.Model
{
    /// <summary>
    /// 关联领用单表
    /// </summary>
    public class ConsumingOrder : BaseModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string FinishDate { get; set; }
        /// <summary>
        /// 未打印
        /// </summary>
        public string PrintStatus { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Printer { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public SourceBill SourceBill { get; set; }
        /// <summary>
        /// 未领用
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string StoreHouseId { get; set; }
        /// <summary>
        /// 手术领用
        /// </summary>
        public string Type { get; set; }
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
}
