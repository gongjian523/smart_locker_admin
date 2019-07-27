using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Infrastructure.QuartzHelper.quartzEnum
{

	/// <summary>
	/// 分组组名
	/// </summary>
	public enum GroupName
	{
		/// <summary>
		/// 获取计划分组
		/// </summary>
		[Description("GetInventoryPlan")]
		GetInventoryPlan = 0,

		/// <summary>
		/// 已清台
		/// </summary>
		[Description("ExecuteInventoryPlan")]
		ExecuteInventoryPlan = 1
	}
}
