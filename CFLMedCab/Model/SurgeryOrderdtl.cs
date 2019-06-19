using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Model
{
    /// <summary>
    /// 手术单耗材详情
    /// </summary>
    public class SurgeryOrderdtl
	{
        /// <summary>
        /// 收单耗材耗材id
        /// </summary> 
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int id { get; set; }

		/// <summary>
		/// 关联手术单号
		/// </summary>
		public string surgery_order_code { get; set; }

		/// <summary>
		/// 商品编号
		/// </summary>
		public string goods_code { get; set; }

		/// <summary>
		/// 商品名称
		/// </summary>
		[SugarColumn(IsNullable = true)]
		public string name { get; set; }

		/// <summary>
		/// 领用属性
		/// </summary>
		public int fetch_type { get; set; }

        /// <summary>
        /// 需要领用数量
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int fetch_num { get; set; }

        /// <summary>
        /// 已经领用数量
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int already_fetch_num { get; set; }

        /// <summary>
        /// 待领用数量
        /// </summary>		
        [SugarColumn(IsNullable = true)]
        public int not_fetch_num { get; set; }

        /// <summary>
        /// 异常标识
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string exception_flag_description { get; set; }

        /// <summary>
        /// 异常说明
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string exception_description { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string remarks { get; set; }


	}
}
