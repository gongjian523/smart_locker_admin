using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Model
{
    public class FetchOrder
    {
        /// <summary>
        /// 领用单号（主键ID）
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 领用时间
        /// </summary>
        public DateTime create_time { get; set; }

        /// <summary>
        /// 领用人
        /// </summary>
        public int operator_id { get; set; }

        /// <summary>
        /// 领用类型
        /// </summary>
        public int type { get; set; }

        /// <summary>
        /// 认领状态
        /// </summary>
        public int status { get; set; }

        /// <summary>
        /// 业务单号
        /// </summary>
        public int business_order_id { get; set; }
    }
}
