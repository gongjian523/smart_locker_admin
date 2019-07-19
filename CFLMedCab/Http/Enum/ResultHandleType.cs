using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Http.Enum
{
	/// <summary>
	/// 结果操作类型枚举
	/// </summary>
	public enum ResultHandleType
	{
		/// <summary>
		/// 正常
		/// </summary>
		[Description("请求正常")]
		请求正常 = 0,

		/// <summary>
		/// 异常
		/// </summary>
		[Description("请求异常")]
		请求异常 = 1,

		/// <summary>
		/// 超时
		/// </summary>
		[Description("请求超时")]
		请求超时 = 2

	}
}
