using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Http.Enum
{
	/// <summary>
	/// 异常展示
	/// </summary>
	public enum AbnormalDisplay
	{
		/// <summary>
		/// 正常
		/// </summary>
		[Description("正常")]
		正常 = 1,

		/// <summary>
		/// 异常 
		/// </summary>
		[Description("异常")]
		异常 = 2

	}
}
