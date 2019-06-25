using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Model.Enum
{
    public class NavBtnEnum
    {
        /// <summary>
        /// 用户身份类型
        /// </summary>
        public enum NavBtnType
        {
            /// <summary>
            /// 一般领用
            /// </summary>
            [Description("一般领用")]
            一般领用 = 0,

            /// <summary>
            /// 手术领用
            /// </summary>
            [Description("手术领用")]
            手术领用 = 1,

            /// <summary>
            /// 领用退回
            /// </summary>
            [Description("领用退回")]
            领用退回 = 2,

            /// <summary>
            /// 补货入库
            /// </summary>
            [Description("补货入库")]
            补货入库 = 3,

            /// <summary>
            /// 退货出库
            /// </summary>
            [Description("退货出库")]
            退货出库 = 4,

            /// <summary>
            /// 库存盘点
            /// </summary>
            [Description("库存盘点")]
            库存盘点 = 5,

            /// <summary>
            /// 库存查询
            /// </summary>
            [Description("库存查询")]
            库存查询 = 6
        }
    }
}
