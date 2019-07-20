

using CFLMedCab.Http.Model.Base;

namespace CFLMedCab.Http.Model
{
	/// <summary>
	/// 用户
	/// </summary>
	public class User : BaseModel
	{

		/// <summary>
		/// 手机
		/// </summary>
		public string Phone { get; set; }

		/// <summary>
		/// 目前用的账户字段
		/// </summary>
		public string MobilePhone { get; set; }
		

	}
}
