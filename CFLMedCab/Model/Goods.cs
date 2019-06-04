using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Model
{
    public class Goods
    {
        /// <summary>
        /// 商品编号
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 单品码
        /// </summary>
        public string code { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 商品编码
        /// </summary>
        public string goods_code { get; set; }

        /// <summary>
        /// 生产批次
        /// </summary>
        public string batch_number { get; set; }

        /// <summary>
        /// 生成日期
        /// </summary>
        public DateTime birth_date { get; set; }

        /// <summary>
        /// 有效期天数
        /// </summary>
        public int valid_period { get; set; }

        /// <summary>
        /// 有效期
        /// </summary>
        public DateTime expiry_date { get; set; }

        /// <summary>
        /// 领用属性
        /// </summary>
        public int fetch_type { get; set; }

        /// <summary>
        /// 货位
        /// </summary>
        public string position { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string remarks { get; set; }
    }
}
