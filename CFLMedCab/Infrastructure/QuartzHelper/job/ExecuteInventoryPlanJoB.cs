using CFLMedCab.Http.Bll;
using CFLMedCab.Http.Helper;
using CFLMedCab.Infrastructure.DeviceHelper;
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
	/// 执行自动盘点计划的任务
	/// </summary>
	public class ExecuteInventoryPlanJoB : IJob
	{
		public Task Execute(IJobExecutionContext context)
		{
			Console.WriteLine("ExecuteInventoryPlanJoB进入");

			var epcs = RfidHelper.GetEpcDataJson(out bool isGetSucess);

			if (isGetSucess)
			{
				var baseDataCommodityCodes = CommodityCodeBll.GetInstance().GetCommodityCode(epcs);

				HttpHelper.GetInstance().ResultCheck(baseDataCommodityCodes, out bool isSuccess);

				if (isSuccess)
				{
					var basePostInventoryDetail = InventoryTaskBll.GetInstance().CreateInventoryOrderAndDetail(baseDataCommodityCodes.body.objects);
					LogUtils.Debug($"已执行自动盘点任务：{JsonConvert.SerializeObject(basePostInventoryDetail)}");
				}
			}

			return null;
			
		}
	}
}
