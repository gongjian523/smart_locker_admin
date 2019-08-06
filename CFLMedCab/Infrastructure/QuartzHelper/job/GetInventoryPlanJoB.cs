using CFLMedCab.Http.Bll;
using CFLMedCab.Http.Helper;
using CFLMedCab.Http.Model;
using CFLMedCab.Infrastructure.QuartzHelper.scheduler;
using CFLMedCab.Infrastructure.ToolHelper;
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
				var inventoryPlans = baseDataInventoryPlan.body.objects;
				inventoryPlans.ForEach(item =>
				{
					item.InventoryTime = "17:57:00";
				});

				//cronStr = "0 43 11 * * ?";
				CustomizeScheduler.GetInstance().SchedulerStartOrUpdateOrDeleteByPlans(inventoryPlans).Wait();


				inventoryPlans.ForEach(item =>
				{
					item.InventoryTime = "17:59:00";
				});


				InventoryPlan inventoryPlan = inventoryPlans[0].MapTo<InventoryPlan>();

				inventoryPlan.id = inventoryPlan.id + "134";
				inventoryPlan.InventoryTime = "18:01:00";

				inventoryPlans.Add(inventoryPlan);

				//模拟重复任务
				CustomizeScheduler.GetInstance().SchedulerStartOrUpdateOrDeleteByPlans(inventoryPlans).Wait();

				//cronStr = "0 44 11 * * ?";
			}

			return null;

		}

	}

}
