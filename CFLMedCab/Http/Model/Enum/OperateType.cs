using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Http.Model.Enum
{
	/// <summary>
	/// 操作类型
	/// </summary>
	public enum OperateType
	{
		/// <summary>
		/// 出库
		/// </summary>
		[Description("出库")]
		出库 = 0,

		/// <summary>
		/// 入库
		/// </summary>
		[Description("入库")]
		入库 = 1

	}
}
