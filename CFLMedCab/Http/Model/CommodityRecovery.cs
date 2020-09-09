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
    /// 商品回收任务单
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class CommodityRecovery : BaseModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string AbnormalCauses { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string AbnormalDescribe { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ApprovalStatus { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Department { get; set; }

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

    /// <summary>
    /// 回收取货任务状态
    /// </summary>
    public enum CommodityRecoveryStatusEnum
    {
        /// <summary>
        /// 待回收
        /// </summary>
        [Description("待回收")]
        待回收 = 0,

        /// <summary>
        /// 进行中
        /// </summary>
        [Description("进行中")]
        进行中 = 1,

        /// <summary>
        /// 已完成
        /// </summary>
        [Description("已完成")]
        已完成 = 3,

        /// <summary>
        /// 已撤销
        /// </summary>
        [Description("已撤销")]
        已撤销 = 4,

        /// <summary>
        /// 异常
        /// </summary>
        [Description("异常")]
        异常 = 5,
    }
}
