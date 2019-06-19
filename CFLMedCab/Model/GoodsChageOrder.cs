using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Model
{
    /// <summary>
    /// 库存变化单
    /// </summary>
    public class GoodsChageOrder
    {
        /// <summary>
        /// 库存变化单id
        /// </summary>
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int id { get; set; }

		/// <summary>
		/// 库存变化单号
		/// </summary>
		public string code { get; set; }

		/// <summary>
		/// 操作人
		/// </summary>
		[SugarColumn(IsNullable = true)]
		public int operator_id { get; set; }

        /// <summary>
        /// 生成时间
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public DateTime create_time { get; set; }

        /// <summary>
        /// 业务类型
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int business_type { get; set; }

        /// <summary>
        /// 单据状态 状态：0  待处理；1  已处理。
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int business_status { get; set; }		

	}
}
