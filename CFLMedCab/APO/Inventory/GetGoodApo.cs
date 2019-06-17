using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.APO.Inventory
{
    public class GetGoodApo:BasePageDataApo
    {
        /// <summary>
        /// 库存快照
        /// </summary>
        public HashSet<string> goodsEpsDatas { get; set; }

        /// <summary>
        /// 商品编码
        /// </summary>
        public string code { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string name { get; set; }
    }
}
