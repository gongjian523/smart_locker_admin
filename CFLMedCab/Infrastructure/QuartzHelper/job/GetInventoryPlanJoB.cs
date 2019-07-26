using CFLMedCab.Http.Bll;
using CFLMedCab.Http.Helper;
using CFLMedCab.Infrastructure.QuartzHelper.quartzEnum;
using CFLMedCab.Infrastructure.QuartzHelper.scheduler;
using CFLMedCab.Infrastructure.QuartzHelper.trigger;
using CFLMedCab.Infrastructure.ToolHelper;
using Newtonsoft.Json;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Infrastructure.QuartzHelper.job
{
	/// <summary>
	/// 获取自动盘点计划的任务
	/// </summary>
	public class GetInventoryPlanJoB : IJob
	{
		public Task Execute(IJobExecutionContext context)
		{
			Console.WriteLine("GetInventoryPlanJoB进入");

			var baseDataInventoryPlan = InventoryTaskBll.GetInstance().GetInventoryPlanByEquipmnetNameOrId("AQACQqweDg8BAAAAFUD8WDEPsxV_FwQA");

			//校验是否含有数据，如果含有数据，进行后续操作
			HttpHelper.GetInstance().ResultCheck(baseDataInventoryPlan, out bool isSuccess);

			if (isSuccess)
			{
				var inventoryPlan = baseDataInventoryPlan.body.objects.First();
				string cronStr = QuartzUtils.GetQuartzCron(inventoryPlan);
				if (!CustomizeScheduler.GetInstance().JobIsExist(cronStr, GroupName.ExecuteInventoryPlan.ToString()))
				{
					LogUtils.Debug($"已执行自动盘点的计划任务：{JsonConvert.SerializeObject(baseDataInventoryPlan)}");
					CustomizeScheduler.GetInstance().SchedulerStart<ExecuteInventoryPlanJoB>(CustomizeTrigger.GetExecuteInventoryPlanTrigger(cronStr), GroupName.ExecuteInventoryPlan, cronStr);
				}

				System.Threading.Thread.Sleep(7000);

				if (!CustomizeScheduler.GetInstance().JobIsExist(cronStr, GroupName.ExecuteInventoryPlan.ToString()))
				{
					LogUtils.Debug($"已执行自动盘点的计划任务：{JsonConvert.SerializeObject(baseDataInventoryPlan)}");
					CustomizeScheduler.GetInstance().SchedulerStart<ExecuteInventoryPlanJoB>(CustomizeTrigger.GetExecuteInventoryPlanTrigger(cronStr), GroupName.ExecuteInventoryPlan, cronStr);
				}

			}

			return null;
		}
	}
}
