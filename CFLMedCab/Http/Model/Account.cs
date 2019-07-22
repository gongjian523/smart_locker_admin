

using CFLMedCab.Http.Model.Base;

namespace CFLMedCab.Http.Model
{
	/// <summary>
	/// 账户
	/// </summary>
	public class Account:BaseModel
	{

		/// <summary>
		/// ⽤户名
		/// </summary>
		public string AccounName { get; set; }

		/// <summary>
		/// ⽤户账号
		/// </summary>
		public string Phone { get; set; }

		/// <summary>
		/// ⽤户密码
		/// </summary>
		public string Password { get; set; }

		/// <summary>
		/// 智能柜⾝身份标识（指静脉做标识）
		/// </summary>
		public string SmartDevice { get; set; }


        /// <summary>
        /// 图形验证码token
        /// </summary>
        public string CaptchaToken { get; set; }

        /// <summary>
        /// 图形验证码
        /// </summary>
        public string CaptchaValue { get; set; }
    }
}
