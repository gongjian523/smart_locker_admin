using CFLMedCab.Http.Model.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Http.Model
{
    /// <summary>
    /// 生产批号管理详情
    /// </summary>
    public class BatchNumber : BaseModel
    {
        /// <summary>
        /// 失效日期
        /// </summary>
        public string ExpirationDate { get; set; }
        /// <summary>
        /// 关联医院货品
        /// </summary>
        public string HospitalGoodsId { get; set; }
        /// <summary>
        /// 生产日期
        /// </summary>
        public string ProductionDate { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }
        /// <summary>
        /// 状态 
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 定时任务更新
        /// </summary>
        public string updatedByCrontab { get; set; }
    }
}
