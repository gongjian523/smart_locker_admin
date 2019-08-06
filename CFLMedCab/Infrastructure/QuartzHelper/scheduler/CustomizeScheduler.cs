using CFLMedCab.Http.Model;
using CFLMedCab.Infrastructure.QuartzHelper.job;
using CFLMedCab.Infrastructure.QuartzHelper.quartzEnum;
using CFLMedCab.Infrastructure.QuartzHelper.trigger;
using CFLMedCab.Infrastructure.ToolHelper;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Infrastructure.QuartzHelper.scheduler
{
	public class CustomizeScheduler
	{

		// 定义一个静态变量来保存类的实例
		private static CustomizeScheduler singleton;

		// 定义一个标识确保线程同步
		private static readonly object locker = new object();

		//定义公有方法提供一个全局访问点。
		public static CustomizeScheduler GetInstance()
		{
			//这里的lock其实使用的原理可以用一个词语来概括“互斥”这个概念也是操作系统的精髓
			//其实就是当一个进程进来访问的时候，其他进程便先挂起状态
			if (singleton == null)
			{
				lock (locker)
				{
					// 如果类的实例不存在则创建，否则直接返回
					if (singleton == null)
					{
						singleton = new CustomizeScheduler();
					}
				}
			}
			return singleton;
		}

		/// <summary>
		/// 调度器
		/// </summary>
		private IScheduler Scheduler;

		/// <summary>
		/// 调度器工厂
		/// </summary>
		private readonly ISchedulerFactory Factory;

		public CustomizeScheduler()
		{
			//创建一个调度器
			Factory = new StdSchedulerFactory();
			CreateScheduler();
		}

		private void CreateScheduler(){

			if (Factory != null)
			{
				Scheduler = Factory.GetScheduler().Result;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="trigger"></param>
		/// <param name="groupName"></param>
		public void SchedulerStart<T>(ITrigger trigger, GroupName groupName) where T : IJob
		{
			if (Scheduler == null)
			{
				CreateScheduler();
			}

			IJobDetail jobDetail = JobBuilder.Create<T>().WithIdentity(typeof(T).Name, groupName.ToString()).Build();
			//将任务与触发器添加到调度器中
			Scheduler.ScheduleJob(jobDetail, trigger);
				//开始执行
			Scheduler.Start();
			
			
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="trigger"></param>
		/// <param name="groupName"></param>
		/// <param name="jobDetailName"></param>
		[Obsolete]
		public async Task SchedulerStartAsync<T>(ITrigger trigger, GroupName groupName, string jobDetailName) where T : IJob
		{
			if (Scheduler == null)
			{
				CreateScheduler();
			}

			IJobDetail jobDetail = JobBuilder.Create<T>().WithIdentity(jobDetailName, groupName.ToString()).Build();
			//将任务与触发器添加到调度器中
			await Scheduler.ScheduleJob(jobDetail, trigger);
			//开始执行
			await Scheduler.Start();

		}

		/// <summary>
		/// 获取TriggerKey通过
		/// </summary>
		/// <param name="inventoryPlan"></param>
		/// <returns></returns>
		public TriggerKey GetTriggerKeyByPlan(InventoryPlan inventoryPlan)
		{
			return new TriggerKey(inventoryPlan.id, GroupName.ExecuteInventoryPlan.ToString());
		}

		/// <summary>
		/// 获取JobKey通过
		/// </summary>
		/// <param name="inventoryPlan"></param>
		/// <returns></returns>
		public JobKey GetJobKeyByPlan(InventoryPlan inventoryPlan)
		{
			return new JobKey(inventoryPlan.id, GroupName.ExecuteInventoryPlan.ToString());
		}

		/// <summary>
		/// 获取JobKey通过
		/// </summary>
		/// <param name="triggerKey"></param>
		/// <returns></returns>
		public JobKey GetJobKeyByPlan(TriggerKey triggerKey)
		{
			return new JobKey(triggerKey.Name, GroupName.ExecuteInventoryPlan.ToString());
		}


		/// <summary>
		/// 更新或者新建任务通过inventoryPlan
		/// </summary>
		/// <param name="inventoryPlan"></param>
		/// <returns></returns>
		public async Task SchedulerStartOrUpdateOrDeleteByPlans(List<InventoryPlan> inventoryPlans)
		{

			var currentTriggerKeys = await Scheduler.GetTriggerKeys(GroupMatcher<TriggerKey>.GroupEquals(GroupName.ExecuteInventoryPlan.ToString()));

			var startAndUpdateTriggerKeys = new List<TriggerKey>(inventoryPlans.Count);
			inventoryPlans.ForEach(item => {
				startAndUpdateTriggerKeys.Add(GetTriggerKeyByPlan(item));
			});

			var DeleteTriggerKeys = currentTriggerKeys.Except(startAndUpdateTriggerKeys).ToList();

			//新增或修改任务
			inventoryPlans.ForEach(async item => 
			{
				await SchedulerStartOrUpdateByPlan(item);
			});


			//删除任务
			DeleteTriggerKeys.ForEach(async item => 
			{
				await SchedulerDeleteByPlan(item);
			});

		}

		/// <summary>
		/// 更新或者新建任务通过inventoryPlan
		/// </summary>
		/// <param name="inventoryPlan"></param>
		/// <returns></returns>
		public async Task SchedulerStartOrUpdateByPlan(InventoryPlan inventoryPlan)
		{
			if (IsExistScheduleJob(inventoryPlan))
			{
				await SchedulerUpdateByPlan(inventoryPlan);
			}
			else
			{
				await SchedulerStartByPlan(inventoryPlan);
			}
		}

		/// <summary>
		/// 删除任务通过inventoryPlan
		/// </summary>
		/// <param name="inventoryPlan"></param>
		/// <returns></returns>
		public async Task SchedulerDeleteByPlan(InventoryPlan inventoryPlan)
		{
			if (Scheduler == null)
			{
				CreateScheduler();
			}

			if (IsExistScheduleJob(inventoryPlan))
			{
				await Scheduler.UnscheduleJob(GetTriggerKeyByPlan(inventoryPlan));
				await Scheduler.DeleteJob(GetJobKeyByPlan(inventoryPlan));
			}
			
		}

		/// <summary>
		/// 删除任务通过inventoryPlan
		/// </summary>
		/// <param name="inventoryPlan"></param>
		/// <returns></returns>
		public async Task SchedulerDeleteByPlan(TriggerKey triggerKey)
		{
			if (Scheduler == null)
			{
				CreateScheduler();
			}
			else
			{
				await Scheduler.UnscheduleJob(triggerKey);
				await Scheduler.DeleteJob(GetJobKeyByPlan(triggerKey));
			}
		}

		/// <summary>
		/// 开始任务通过盘点计划
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="trigger"></param>
		/// <param name="groupName"></param>
		/// <param name="jobDetailName"></param>
		public async Task SchedulerStartByPlan(InventoryPlan inventoryPlan)
		{
			if (Scheduler == null)
			{
				CreateScheduler();
			}

			var currentTrigger = CustomizeTrigger.GetExecuteInventoryPlanTrigger(inventoryPlan.id, QuartzUtils.GetQuartzCron(inventoryPlan));

			IJobDetail jobDetail = JobBuilder.Create<ExecuteInventoryPlanJoB>().WithIdentity(inventoryPlan.id, GroupName.ExecuteInventoryPlan.ToString()).Build();
			//将任务与触发器添加到调度器中
			await Scheduler.ScheduleJob(jobDetail, currentTrigger);
			//开始执行
			await Scheduler.Start();

		}

		/// <summary>
		/// 更新任务通过盘点计划（已有同样的任务的时候）
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="trigger"></param>
		/// <param name="groupName"></param>
		/// <param name="jobDetailName"></param>
		public async Task SchedulerUpdateByPlan(InventoryPlan inventoryPlan)
		{
			if (Scheduler == null)
			{
				CreateScheduler();
			}

			var triggerKey = GetTriggerKeyByPlan(inventoryPlan);

			var trigger = (ICronTrigger)await Scheduler.GetTrigger(triggerKey);

			var currentCron = QuartzUtils.GetQuartzCron(inventoryPlan);

			//处理是否更新任务的定时时间
			if (trigger.CronExpressionString != currentCron)
			{
				var triggerNew = TriggerBuilder.Create()
				.WithIdentity(triggerKey.Name, triggerKey.Group)
				.ForJob(trigger.JobKey)
				.WithCronSchedule(currentCron)
				.Build();
				await Scheduler.RescheduleJob(trigger.Key, triggerNew);

			}


			//处理是否更新任务的执行状态
			TriggerState triggerState = await Scheduler.GetTriggerState(triggerKey);
			switch (triggerState)
			{
				case TriggerState.Normal:

					if (!inventoryPlan.Enabled)
					{
						await Scheduler.PauseTrigger(triggerKey);
					}

					break;
				case TriggerState.Paused:

					if (inventoryPlan.Enabled)
					{
						
						await Scheduler.ResumeTrigger(triggerKey);
					}

					break;
                //如果处于其他状态，暂不进行任何处理
				default:
					LogUtils.Error($"{triggerKey.Name}定时任务更新时，原定时任务出现其他状态{triggerState.ToString()}");
					break; 
			}

		}

		/// <summary>
		/// 根据cron创建或更新任务
		/// </summary>
		/// <param name="cron"></param>
		/// <returns></returns>
		[Obsolete]
		public async Task CreateUpdateTriggerAsync(string cron)
		{

			var triggerName = $"{cron}Trigger";
			var triggerGroupName = GroupName.ExecuteInventoryPlan.ToString();
			var trigger = await Scheduler.GetTrigger(new TriggerKey(triggerName, triggerGroupName));

			var ret = IsExistScheduleJob(triggerName, triggerGroupName);

			if (!ret.Result)
			{
				await SchedulerStartAsync<ExecuteInventoryPlanJoB>(CustomizeTrigger.GetExecuteInventoryPlanTrigger(cron), GroupName.ExecuteInventoryPlan, cron);
			}
			else
			{
				var triggerNew = TriggerBuilder.Create().WithIdentity(triggerName, triggerGroupName).ForJob(trigger.JobKey).WithCronSchedule(cron).Build();
			    await Scheduler.RescheduleJob(trigger.Key, triggerNew);
			}
			
			
		}


		/// <summary>
		/// 判断任务是否存在
		/// </summary>
		/// <param name="triggerkey"></param>
		/// <returns></returns>
		public async Task<bool> IsExistScheduleJob(TriggerKey triggerkey)
		{
			var trigger = await Scheduler.GetTrigger(triggerkey);
			return trigger != null;
		}

		/// <summary>
		/// 判断任务是否存在
		/// </summary>
		/// <param name="triggerkey"></param>
		/// <returns></returns>
		public bool IsExistScheduleJob(InventoryPlan inventoryPlan)
		{
			return IsExistScheduleJob(GetTriggerKeyByPlan(inventoryPlan)).Result;
		}

		/// <summary>
		/// 判断任务是否存在
		/// </summary>
		/// <param name="triggerName"></param>
		/// <param name="triggerGroupName"></param>
		/// <returns></returns>
		public async Task<bool> IsExistScheduleJob(string triggerName, string triggerGroupName)
		{
			return await IsExistScheduleJob(new TriggerKey(triggerName, triggerGroupName));
		}


		/// <summary>
		/// 获取当前调度器
		/// </summary>
		public IScheduler GetCurrentScheduler()
		{
			return Scheduler;
		}

		/// <summary>
		/// 在应用程序关闭调度
		/// </summary>
		public void Close()
		{
			//在应用程序关闭时运行的代码
			if (Scheduler != null)
			{
				Scheduler.Shutdown(true);
			}
		}


	}
}
