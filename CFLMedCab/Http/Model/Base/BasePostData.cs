using System;
using System.Collections.Generic;

namespace CFLMedCab.Http.Model.Base
{
    public class BasePostData<T>
    {
        /// <summary>
        /// 响应状态码, 0 代表成功，其他值含义参考错误码文档
        /// </summary>
        public int code { get; set; }

        /// <summary>
        /// 返回提示消息
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// body
        /// </summary>
        public List<T> body { get; set; }
    }
}
