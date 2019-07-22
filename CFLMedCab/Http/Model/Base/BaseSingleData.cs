using System;

namespace CFLMedCab.Http.Model.Base
{
    public class BaseSingleData<T>
    {
        /// <summary>
        /// 响应状态码, 0 代表成功，其他值含义参考错误码文档
        /// </summary>
        public Int32 code { get; set; }

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
        public T body { get; set; }
    }
}
