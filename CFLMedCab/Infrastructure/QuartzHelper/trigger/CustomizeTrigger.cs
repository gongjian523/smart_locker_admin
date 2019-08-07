using CFLMedCab.Infrastructure.QuartzHelper.quartzEnum;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

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
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load($"{ApplicationState.GetProjectRootPath()}/MyProject.xml");
			XmlNode root = xmlDoc.SelectSingleNode("config");//指向根节点
			XmlNode quartz = root.SelectSingleNode("quartz");//指向quartz节点
			
			return	TriggerBuilder.Create()
								.WithIdentity($"{GroupName.GetInventoryPlan.ToString()}Trigger", GroupName.GetInventoryPlan.ToString())
								.WithCronSchedule(quartz.SelectSingleNode("inventory_task_cron").InnerText)
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
