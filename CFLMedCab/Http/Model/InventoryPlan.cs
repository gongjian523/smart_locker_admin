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
    /// 盘点计划管理
    /// @λ智能柜/货架 盘点任务单：IT20190723000015，ID：AQACQqweJ4wBAAAAXRA4vCD_sxWaDwQA
    /// </summary>
    public class InventoryPlan : BaseModel
    {
        /// <summary>
        /// 每日
        /// </summary>
        public string CheckPeriod { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool Enabled { get; set; }
        /// <summary>
        /// 第一天
        /// </summary>
        public string InventoryDay { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string InventoryTime { get; set; }
        /// <summary>
        /// 周一
        /// </summary>
        public string InventoryWeekday { get; set; }
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
