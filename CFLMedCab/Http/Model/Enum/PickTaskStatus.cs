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
	public enum PickTaskStatus
	{
		/// <summary>
		/// 待上架
		/// </summary>
		[Description("待拣货")]
		待拣货 = 0,

		/// <summary>
		/// 已完成 
		/// </summary>
		[Description("已完成")]
		已完成 = 1,

		/// <summary>
		/// 异常
		/// </summary>
		[Description("异常")]
		异常 = 2,

        /// <summary>
        /// 进行中
        /// </summary>
        [Description("进行中")]
        进行中 = 3 ,

        /// <summary>
        /// 已撤销
        /// </summary>
        [Description("已撤销")]
        已撤销 = 4

    }
}
