using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Model
{
    public class FetchOrder
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int id { get; set; }

		/// <summary>
		/// 领用单号
		/// </summary>
		public string code { get; set; }

		/// <summary>
		/// 领用时间
		/// </summary>
		[SugarColumn(IsNullable = true)]
        public DateTime create_time { get; set; }

        /// <summary>
        /// 领用人
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int operator_id { get; set; }

        /// <summary>
        /// 领用类型
        /// </summary>
        public int type { get; set; }

        /// <summary>
        /// 认领状态
        /// </summary>
        public int status { get; set; }

        /// <summary>
        /// 业务单号
        /// </summary>
        public string business_order_code { get; set; }

        /// <summary>
        /// 清台单编号
        /// </summary>
        public int clear_platform_id { get; set; }

        /// <summary>
        /// 是否清台
        /// </summary>
        public int is_clearplatform { get; set; }
    }
}
