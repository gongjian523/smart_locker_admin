using CFLMedCab.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.DTO.Goodss
{
    public class GoodsChangeDto: GoodsChageOrderdtl
    {
        /// <summary>
        /// 创造日期
        /// </summary>
        public DateTime create_time { get; set; }

        /// <summary>
        /// 业务类型
        /// </summary>
        public int business_type { get; set; }
    }
}
