﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Model
{
    public class PickingSubOrderdtl
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int id { get; set; }

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
        public string batch_number { get; set; }

        /// <summary>
        /// 生成日期
        /// </summary>
        public DateTime birth_date { get; set; }

        /// <summary>
        /// 有效期
        /// </summary>
        public DateTime expire_date { get; set; }

        /// <summary>
        /// 有效期天数
        /// </summary>
        public int valid_period { get; set; }

        /// <summary>
        /// 货位
        /// </summary>
        public string position { get; set; }

        /// <summary>
        /// 领用属性
        /// </summary>
        public int fetch_type { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string remarks { get; set; }

        /// <summary>
        /// 上架状态 状态：0  待上架；1 已上架。
        /// </summary>
        public int status { get; set; }

        /// <summary>
        /// 关联操作单据
        /// </summary>
        public int related_order_id { get; set; }
    }
}