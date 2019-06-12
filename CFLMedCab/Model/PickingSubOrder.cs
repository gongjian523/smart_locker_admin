using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Model
{
    /// <summary>
    /// 拣货单
    /// </summary>
    public class PickingSubOrder
    {
        /// <summary>
        /// 拣货单号
        /// </summary>
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int id { get; set; }

        /// <summary>
        /// 拣货工单号
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int picking_order_id { get; set; }

        /// <summary>
        /// 生成时间
        /// </summary>
        public DateTime create_time { get; set; }

        /// <summary>
        /// 完成时间
        /// </summary>
        public DateTime end_time { get; set; }

        /// <summary>
        /// 货位
        /// </summary>
        public string position { get; set; }

        /// <summary>
        /// 状态 状态：0 待拣货；2 部分拣货；1 已拣货。
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int status { get; set; }

        /// <summary>
        /// 验货单
        /// </summary>
        public int Inspection_order_id { get; set; }
    }
}
