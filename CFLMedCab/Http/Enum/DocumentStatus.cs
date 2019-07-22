using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Http.Enum
{
	/// <summary>
	/// 单据状态枚举
	/// </summary>
	public enum DocumentStatus
	{

		/// <summary>
		/// 待上架
		/// </summary>
		[Description("待上架")]
		待上架 = 1,

		/// <summary>
		/// 已完成 
		/// </summary>
		[Description("已完成")]
		已完成 = 2,

		/// <summary>
		/// 异常
		/// </summary>
		[Description("异常")]
		异常 = 3

	}
}
