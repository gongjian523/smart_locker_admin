using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Model
{
    public class InventoryPlanLDB
    {
        /// <summary>
        /// 计划编号
        /// </summary>
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int id { get; set; }

        /// <summary>
        /// 盘点时间
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string code { get; set; }

        /// <summary>
        /// 盘点时间
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string inventorytime_str { get; set; }

        /// <summary>
        /// 状态：0  启用；1 未启用。默认为启用，到时智能柜自动全柜盘点生成盘点单。未启用时不进行盘点，可手动编辑为未启用
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int status { get; set; }
    }
}
