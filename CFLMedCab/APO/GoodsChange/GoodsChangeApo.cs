using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.APO.GoodsChange
{
    public class GoodsChangeApo:BasePageDataApo
    {
        /// <summary>
        /// 操作类型 操作类型：0 出库；1 入库
        /// </summary>
        public int operate_type { get; set; }

        /// <summary>
        /// 起始时间
        /// </summary>
        public DateTime startTtime  { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime endTtime { get; set; }

    }
}
