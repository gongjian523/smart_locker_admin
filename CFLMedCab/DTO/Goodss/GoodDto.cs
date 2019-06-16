using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.DTO.Goodss
{
    public class GoodDto
    {
 
        /// <summary>
        /// 商品名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 商品编码
        /// </summary>
        public string goods_code { get; set; }

        /// <summary>
        /// 商品数量
        /// </summary>
        public int amount { get; set; }

        /// <summary>
        /// 最早日期
        /// </summary>
        public DateTime  expire_time  { get; set; }
    }
}
