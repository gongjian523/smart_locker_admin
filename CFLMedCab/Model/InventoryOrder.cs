using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Model
{
    public class InventoryOrder
    {
        /// <summary>
        /// 盘点单号
        /// </summary>
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int id { get; set; }

        /// <summary>
        /// 盘点类型:：0  自动盘点；1 手动盘点
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int type { get; set; }

        /// <summary>
        /// 盘点时间
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public DateTime create_time { get; set; }

        /// <summary>
        /// 确认时间
        /// </summary>
        public DateTime confirm_time { get; set; }

        /// <summary>
        /// 盘点人
        /// </summary>
        public int operator_id { get; set; }

        /// <summary>
        /// 确认人
        /// </summary>
        public int inspector_id { get; set; }

        /// <summary>
        /// 盘点单状态：0  待确认；1  已确认。
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int status { get; set; }
    }
}
