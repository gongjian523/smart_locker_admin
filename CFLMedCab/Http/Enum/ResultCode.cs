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
		/// Task_Exception
		/// </summary>
		[Description("Task_Exception")]
		Task_Exception = -3,

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
        /// Business_Exception 业务异常
        /// </summary>
        [Description("Business_Exception")]
        Business_Exception = -3,

        /// <summary>
        /// 参数异常
        /// </summary>
        [Description("Parameter_Exception")]
        Parameter_Exception = -4,

        /// <summary>
        /// 
        /// </summary>
        [Description("Discard_Exception")]
        Discard_Exception = -5,

		/// <summary>
		/// OK
		/// </summary>
		[Description("OK")]
		OK = 0,


	}
}
