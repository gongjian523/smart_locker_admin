using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Model.Enum
{
    enum CollarType
    {

        /// <summary>
        /// 一般领用
        /// </summary>
        commonly=0,

        /// <summary>
        /// 手术领用
        /// </summary>
        surgery=1,

        /// <summary>
        /// 领用回退
        /// </summary>
        regression=2,

        /// <summary>
        /// 退货出库
        /// </summary>
        returngoods=3,

        /// <summary>
        /// 补货入库
        /// </summary>
        replenishment=4
    }
}
