﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Model
{
    public class FetchOrderdtlView
    {
        /// <summary>
        /// 领用详情单ID
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 商品码
        /// </summary>
        public string code { get; set; }

        /// <summary>
        /// 领用属性
        /// </summary>
        public int fetch_type { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string remarks { get; set; }

        /// <summary>
        /// 待领用数
        /// </summary>
        public int wait_collar { get; set; }

        /// <summary>
        /// 已领用数
        /// </summary>
        public int already_led { get; set; }
    }
}
