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
    /// 调拨上架任务
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class AllotShelf : BaseModel
    {
        /// <summary>
        /// 异常原因（‘商品缺失', ‘商品损坏', ‘商品遗失',‘其他’）
        /// </summary>
        public string AbnormalCauses;

        /// <summary>
        /// 异常描述
        /// </summary>
        public string AbnormalDescribe;

        /// <summary>
        /// 操作人
        /// </summary>
        public string Operator;

        /// <summary>
        /// 完成时间
        /// </summary>
        public DateTime FinishDate;

        /// <summary>
        /// 单据状态（待上架，已完成，异常，进行中）
        /// </summary>
        public string Status;

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

        /// <summary>
        /// 该任务单总数
        /// </summary>
        [JsonIgnore]
        public int ShelfNumber { get; set; }

        /// <summary>
        /// 货柜号
        /// </summary>
        [JsonIgnore]
        public string GoodLocationName { get; set; }

    }
    /// <summary>
    /// 调拨上架任务资料
    /// </summary>
    public enum AllotShelfStatusEnum
    {
        /// <summary>
        /// 待上架
        /// </summary>
        [Description("待上架")]
        待上架 = 0,

        /// <summary>
        /// 已完成
        /// </summary>
        [Description("已完成")]
        已完成 = 1,

        /// <summary>
        /// 异常
        /// </summary>
        [Description("异常")]

        异常 = 3,

        /// <summary>
        /// 进行中
        /// </summary>
        [Description("进行中")]
        进行中 = 4
    }
}
