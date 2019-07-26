using CFLMedCab.Infrastructure.QuartzHelper.quartzEnum;
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
		public void SchedulerStart<T>(ITrigger trigger, GroupName groupName, string jobDetailName) where T : IJob
		{
			if (Scheduler == null)
			{
				CreateScheduler();
			}

			IJobDetail jobDetail = JobBuilder.Create<T>().WithIdentity(jobDetailName, groupName.ToString()).Build();
			//将任务与触发器添加到调度器中
			Scheduler.ScheduleJob(jobDetail, trigger);
			//开始执行
			Scheduler.Start();

		}

		/// <summary>
		/// 判断任务名是否正在运行
		/// </summary>
		/// <param name="jobName"></param>
		/// <returns></returns>
		public bool JobIsExist(string jobName, string groupName) 
		{

			bool ret = false;

			if (Scheduler != null)
			{
				//Scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(groupName));

				var jobExecutionContexts = Scheduler.GetCurrentlyExecutingJobs();
				if (jobExecutionContexts.Result != null && jobExecutionContexts.Result.Count > 0)
				{
				
					var jobExecutionContext = jobExecutionContexts.Result.Where(it => it.Trigger.Key.Name == groupName).FirstOrDefault();

					if (jobExecutionContext != null)
					{
						ret = jobName.Equals(jobExecutionContext.JobDetail.Key.Name);
					}

					
				}
				
			}

			return ret;

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
