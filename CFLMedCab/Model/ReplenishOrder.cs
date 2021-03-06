﻿using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Model
{
    /// <summary>
    /// 上架工单
    /// </summary>
    public class ReplenishOrder
    {
        /// <summary>
        /// 上架工单编号
        /// </summary>
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int id { get; set; }

        /// <summary>
        /// 工单负责人
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int principal_id { get; set; }

		/// <summary>
		/// 派发时间
		/// </summary>
		[SugarColumn(IsNullable = true)]
		public DateTime create_time { get; set; }

        /// <summary>
        /// 工单状态 ：0  待完成；1 已完成。
        /// </summary>
        public int status { get; set; }

        /// <summary>
        /// 完成时间
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public DateTime end_time { get; set; }
		/// <summary>
		/// 上架工单号
		/// </summary>

		public string code { get; set; }
    }

}
