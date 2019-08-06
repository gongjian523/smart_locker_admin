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

			var baseDataInventoryPlan = InventoryTaskBll.GetInstance().GetInventoryPlanByEquipmnetNameOrId(ApplicationState.GetEquipId());

			//校验是否含有数据，如果含有数据，进行后续操作
			HttpHelper.GetInstance().ResultCheck(baseDataInventoryPlan, out bool isSuccess);

			if (isSuccess)
			{
				var inventoryPlans = baseDataInventoryPlan.body.objects;
				//cronStr = "0 43 11 * * ?";
				CustomizeScheduler.GetInstance().SchedulerStartOrUpdateOrDeleteByPlans(inventoryPlans).Wait();
				//cronStr = "0 44 11 * * ?";
			}
			else
			{
				LogUtils.Error($"拉取盘点定时计划失败！");
			}

			return null;

		}

	}

}
