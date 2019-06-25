using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Model.Enum
{
    public class UserIdEnum
    {
        /// <summary>
        /// 用户身份类型
        /// </summary>
        public enum UserIdType
        {
            /// <summary>
            /// 医生
            /// </summary>
            [Description("医生")]
            医生 = 0,

            /// <summary>
            /// 护士
            /// </summary>
            [Description("护士")]
            护士 = 1,

            /// <summary>
            /// 护士
            /// </summary>
            [Description("医院管理员")]
            医院管理员 = 2,

            /// <summary>
            /// SPD交收员
            /// </summary>
            [Description("SPD交收员")]
            SPD交收员 = 3,

            /// <summary>
            /// SPD结算员
            /// </summary>
            [Description("SPD结算员")]
            SPD结算员 = 4,

            /// <summary>
            /// SPD经理
            /// </summary>
            [Description("SPD经理")]
            SPD经理 = 5
        }
    }
}
