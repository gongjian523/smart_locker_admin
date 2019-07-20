using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Http.Enum
{
	/// <summary>
	/// 返回码枚举
	/// </summary>
	public enum ResultCode
	{

		/// <summary>
		/// Result_Exception
		/// </summary>
		[Description("Result_Exception")]
		Result_Exception = -2,

		/// <summary>
		/// Request_Exception
		/// </summary>
		[Description("Request_Exception")]
		Request_Exception = -1,

		/// <summary>
		/// OK
		/// </summary>
		[Description("OK")]
		OK = 0,


	}
}
