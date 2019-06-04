using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Model
{
    /// <summary>
    /// 手术单详情
    /// </summary>
    public class SurgeryOrderdtl
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 手术单编号
        /// </summary>
        public int surgery_order_id { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public int goods_id { get; set; }

        /// <summary>
        /// 领用属性
        /// </summary>
        public int fetch_type { get; set; }

        /// <summary>
        /// 需要领用数量
        /// </summary>
        public int number { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string remarks { get; set; }

    }
}
