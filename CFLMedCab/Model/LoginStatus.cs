using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Model
{
    public class LoginStatus
    {
        /// <summary>
        /// 登录状态
        /// </summary>
        public int LoginState { get; set; }

        /// <summary>
        /// 工单负责人
        /// </summary>
        public string LoginString { get; set; }

        /// <summary>
        /// 派发时间
        /// </summary>
        public string LoginString2 { get; set; }
    }
}
