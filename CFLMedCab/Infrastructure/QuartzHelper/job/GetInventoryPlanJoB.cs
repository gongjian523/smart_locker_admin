using CFLMedCab.Http.Bll;
using CFLMedCab.Http.Helper;
using CFLMedCab.Infrastructure.QuartzHelper.scheduler;
using Quartz;
using System;
using System.Linq;
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
				//cronStr = "0 43 11 * * ?";
				CustomizeScheduler.GetInstance().CreateUpdateTriggerAsync(cronStr).Wait();

				//System.Threading.Thread.Sleep(10000);

				//cronStr = "0 44 11 * * ?";
				//CustomizeScheduler.GetInstance().CreateUpdateTriggerAsync(cronStr);


			}

			return null;
		}

	}

}
