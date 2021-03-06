﻿using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Model
{
    /// <summary>
    /// 上架单详情
    /// </summary>
    public class ReplenishSubOrderdtl
    {
        /// <summary>
        /// 编号
        /// </summary>
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int id { get; set; }

        /// <summary>
        /// 上架单id
        /// </summary>
        public int replenish_sub_orderid { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public int goods_id { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 商品编码
        /// </summary>
        public string goods_code { get; set; }

        /// <summary>
        /// 单品码
        /// </summary>
        public string code { get; set; }

        /// <summary>
        /// 生产批号
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string batch_number { get; set; }

        /// <summary>
        /// 生成日期
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public DateTime birth_date { get; set; }

        /// <summary>
        /// 有效期
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public DateTime expire_date { get; set; }

        /// <summary>
        /// 有效期天数
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int valid_period { get; set; }

        /// <summary>
        /// 货位
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string position { get; set; }

        /// <summary>
        /// 领用属性
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int fetch_type { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string remarks { get; set; }

        /// <summary>
        /// 拣货状态 状态：0  待拣货；1  已拣货。
        /// </summary>
        public int status { get; set; }
    }
}
