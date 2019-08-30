using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Model.Enum
{
    /// <summary>
    /// 没有指定商品明细的开门界面
    /// </summary>
    public enum OpenDoorViewType
    {
        /// <summary>
        /// 一般领用
        /// </summary>
        Fetch=0,

        /// <summary>
        /// 领用回退
        /// </summary>
        FetchReturn = 1,

        /// <summary>
        /// 库存调整
        /// </summary>
        StockSwitch = 2,
    }
}
