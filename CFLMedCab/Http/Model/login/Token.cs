using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Http.Model.login
{
    //验证Token
    public class Token
    {
        /// <summary>
        /// 验证token; 成功的时候才有
        /// </summary>
        public string token { get; set; }

        /// <summary>
        /// 账号冻结时间; 失败的时候才有
        /// </summary>
        public string freeze_time { get; set; }

        /// <summary>
        /// 剩余尝试次数
        /// </summary>
        public string left_times { get; set; }

        /// <summary>
        /// 最⼤限制次数
        /// </summary>
        public string max_times { get; set; }

    }



    //用户Token
    public class UserToken
    {
        /// <summary>
        /// 后续接口使用token，有效期2小时
        /// </summary>
        public string access_token { get; set; }

        /// <summary>
        /// access_token过期后，重新获取access_token 使用的token
        /// </summary>
        public string refresh_token { get; set; }
    }
}
