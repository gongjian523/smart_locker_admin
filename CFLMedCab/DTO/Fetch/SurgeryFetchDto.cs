using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.DTO.Fetch
{
    public class SurgeryFetchDto
    {
        /// <summary>
        /// 领用单id
        /// </summary>
        public int fetch_order_id { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string goods_name { get; set; }

        /// <summary>
        /// 商品码
        /// </summary>
        public string goods_code { get; set; }

        /// <summary>
        /// 领用属性
        /// </summary>
        public int fetch_type { get; set; }

        /// <summary>
        /// 待领用数量
        /// </summary>
        public int wait_num { get; set; }
        
        /// <summary>
        /// 已领用数量
        /// </summary>
        public int fetch_num { get; set; }

        /// <summary>
        /// 本柜库存数量
        /// </summary>
        public int stock_num { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string remarks { get; set; }
    }
}
