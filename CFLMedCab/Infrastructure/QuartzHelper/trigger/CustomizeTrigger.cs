using CFLMedCab.Infrastructure.QuartzHelper.quartzEnum;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Infrastructure.QuartzHelper.trigger
{
	public class CustomizeTrigger
	{
		/// <summary>
		/// 获取盘点计划的触发器
		/// </summary>
		/// <returns></returns>
		public static ITrigger GetInventoryPlanTrigger()
		{
			return	TriggerBuilder.Create()
								.WithIdentity($"{GroupName.GetInventoryPlan.ToString()}Trigger", GroupName.GetInventoryPlan.ToString())
								//.EndAt(DateBuilder.DateOf(24, 0, 0))
								.StartNow()
								.Build();
		}

		/// <summary>
		/// 获取盘点计划的触发器
		/// </summary>
		/// <returns></returns>
		public static ITrigger GetExecuteInventoryPlanTrigger(string cron)
		{
			return TriggerBuilder.Create()
								.WithIdentity($"{GroupName.ExecuteInventoryPlan.ToString()}Trigger", GroupName.ExecuteInventoryPlan.ToString())
								.WithCronSchedule(cron)
								.Build();
		}


		/// <summary>
		/// 获取盘点计划的触发器
		/// </summary>
		/// <returns></returns>
		public static ITrigger GetExecuteInventoryPlanTrigger(string id, string cron)
		{
			return TriggerBuilder.Create()
								.WithIdentity(id, GroupName.ExecuteInventoryPlan.ToString())
								.WithCronSchedule(cron)
								.Build();
		}

	}
}
