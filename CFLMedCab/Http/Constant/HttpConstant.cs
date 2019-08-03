using CFLMedCab.Http.Model.param;
using CFLMedCab.Infrastructure.ToolHelper;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace CFLMedCab.Http.Constant
{
	/// <summary>
	/// http相关常量类
	/// </summary>
	public class HttpConstant
	{

		/// <summary>
		/// Http请求超时时间
		/// </summary>
		public static readonly int HttpTimeOut = 5000;

		/// <summary>
		/// 通用接口域名
		/// </summary>
		public static readonly string Domain = "https://crm.chengfayun.com/";

		/// <summary>
		/// 通用接口版本号
		/// </summary>
		public static readonly string Version = "v1.0";

		/// <summary>
		/// 通用url前缀
		/// </summary>
		public static readonly string UrlPrefix = "api/" + Version + "/one/";

		/// <summary>
		/// token的url(特殊)
		/// </summary>
		public static readonly string TokenUrl = "http://implement.int.chengfayun.net/tenant-gateway/internal/api/v1.0/tenant-gateway/signin";

        /// <summary>
        /// 指静脉绑定的url(特殊)
        /// </summary>
        //public static readonly string VeinmatchBindingUrlSuffix = "api/v1.0/plugin/veinmatch/binding";
        public static readonly string VeinmatchBindingUrlSuffix = "api/v1.0/veinmatch/binding";

        /// <summary>
        /// 指静脉识别的url(特殊)
        /// </summary>
        //public static readonly string VeinmatchLoginUrlSuffix = "api/v1.0/plugin/veinmatch/login";
        public static readonly string VeinmatchLoginUrlSuffix = "api/v1.0/veinmatch/login";

        /// <summary>
        /// 获取图形验证码token的url(特殊)
        /// </summary>
        public static readonly string CaptchaTokenUrlSuffix = "api/v1.0/account/get_captcha_token";

        /// <summary>
        /// 获取图形验证码的url(特殊)
        /// </summary>
        public static readonly string CaptchaImageUrlSuffix = "api/v1.0/account/get_captcha_image";

        /// <summary>
        /// 登录的url(特殊)
        /// </summary>
        public static readonly string SignInUrlSuffix = "api/v1.0/account/sign_in";

        /// <summary>
        /// 用户登录的url(特殊)
        /// </summary>
        public static readonly string UserSignInUrlSuffix = "api/v1.0/tenant-gateway/account/signin";


    }

}
