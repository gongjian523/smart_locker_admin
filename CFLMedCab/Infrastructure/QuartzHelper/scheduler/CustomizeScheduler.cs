using CFLMedCab.Infrastructure.QuartzHelper.job;
using CFLMedCab.Infrastructure.QuartzHelper.quartzEnum;
using CFLMedCab.Infrastructure.QuartzHelper.trigger;
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
		/// 根据cron创建或更新任务
		/// </summary>
		/// <param name="cron"></param>
		/// <returns></returns>
		public async Task CreateUpdateTriggerAsync(string cron)
		{

			var triggerName = $"{GroupName.ExecuteInventoryPlan.ToString()}Trigger";
			var triggerGroupName = GroupName.ExecuteInventoryPlan.ToString();
			var trigger = await Scheduler.GetTrigger(new TriggerKey(triggerName, triggerGroupName));
			if (trigger == null)
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
