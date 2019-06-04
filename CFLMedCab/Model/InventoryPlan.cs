using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Model
{
    public class InventoryPlan
    {
        /// <summary>
        /// 计划编号
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 盘点时间
        /// </summary>
        public DateTime inventory_time { get; set; }

        /// <summary>
        /// 状态：0  启用；1 未启用。默认为启用，到时智能柜自动全柜盘点生成盘点单。未启用时不进行盘点，可手动编辑为未启用
        /// </summary>
        public int status { get; set; }
    }
}
