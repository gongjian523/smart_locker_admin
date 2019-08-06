using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CFLMedCab.Http.Model.login
{
    /// <summary>
    /// 指纹识别返回的数据
    /// </summary>
    public class VeinMatch
    {
        /// <summary>
        /// 后续接口使用token，有效期2小时
        /// </summary>
        public string accessToken { get; set; }

        /// <summary>
        /// access_token过期后，重新获取access_token 使用的token
        /// </summary>
        public string refresh_token { get; set; }

        /// <summary>
        /// 用户信息
        /// </summary>
        public User user { get; set; }
    }


    /// <summary>
    /// 服务端的授权
    /// </summary>
    public class VeinRegister
    {
        /// <summary>
        /// 服务端返回的前面
        /// </summary>
        public string sersign { get; set; }

    }
}