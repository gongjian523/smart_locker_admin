using CFLMedCab.Http.ExceptionApi;
using CFLMedCab.Infrastructure.ToolHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Http.Msmq
{
	public class MsmqFactory
	{

		// 定义一个静态变量来保存类的实例
		private static MsmqFactory singleton;
		// 定义一个标识确保线程同步
		private static readonly object locker = new object();


		//定义公有方法提供一个全局访问点。
		public static MsmqFactory GetInstance()
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
						singleton = new MsmqFactory();
					}
				}
			}
			return singleton;
		}

		/// <summary>
		/// 初始化时，新建消息
		/// </summary>
		private MsmqFactory()
		{
			CreateExApiQueue();
		}

		private MessageQueue CreateExApiQueue() {

			MessageQueue mq = null;

			try
			{

				//新建消息循环队列或连接到已有的消息队列
				string path = ".\\private$\\exapiqueue";
				if (MessageQueue.Exists(path))
				{
					mq = new MessageQueue(path);
				}
				else
				{
					mq = MessageQueue.Create(path);
				}

				mq.Formatter = new XmlMessageFormatter(new Type[] { typeof(ExEntity) });

				// Add an event handler for the ReceiveCompleted event.
				mq.ReceiveCompleted += new ReceiveCompletedEventHandler(Exapi_MessageQueue_ReceiveCompleted);

				// Begin the asynchronous receive operation.
				mq.BeginReceive();

			}
			catch (MessageQueueException mess)
			{
				LogUtils.Error($"异步api创建队列异常：{mess.Message}");
			}

			
			return mq;

		}

		/// <summary>
		/// 发送异步接收消息
		/// </summary>
		/// <param name="currentExEntity"></param>
		public void SendExApi(ExEntity currentExEntity)
		{
			var mq = CreateExApiQueue();
			if (mq != null)
			{
				// Send the ExEntity to the queue.
				mq.Send(new Message(currentExEntity));
			}
			else
			{
				LogUtils.Error($"异步api消息发送失败：{currentExEntity.ToString()}");
			}

		}

		/// <summary>
		/// 异步接收消息api（不消耗队列消息的方式）
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Exapi_MessageQueue_PeekCompleted(object sender, PeekCompletedEventArgs asyncResult)
		{
			try
			{
				if (sender is MessageQueue mq)
				{
					Message me = mq.EndPeek(asyncResult.AsyncResult);

					me.Formatter = new XmlMessageFormatter(new Type[] { typeof(ExEntity) });

					if (null != me.Body && me.Body is ExEntity)
					{
						ExStepHandle.ExApiHandle((ExEntity)me.Body);
						//如果结果处理成功，则消耗该消息
						LogUtils.Debug($"异步api接收消息：{me.Body}");
					}
					else {
						LogUtils.Error($"异步api消息接收无效");
					}

					//移除该消息
					//mq.Receive();

					//开始下一次查看数据(每秒运行)
					mq.BeginReceive(new TimeSpan(0, 0, 1));


				}
			}
			catch (Exception mess)
			{
				LogUtils.Error($"异步api接收消息异常：{mess.Message}");
			}

		}

		/// <summary>
		/// 异步接收消息api（消耗队列消息的方式）
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Exapi_MessageQueue_ReceiveCompleted(object sender, ReceiveCompletedEventArgs asyncResult)
		{

			try
			{
				if (sender is MessageQueue mq)
				{
					Message me = mq.EndReceive(asyncResult.AsyncResult);
					me.Formatter = new XmlMessageFormatter(new Type[] { typeof(ExEntity) });
					if (null != me.Body && me.Body is ExEntity)
					{
						ExStepHandle.ExApiHandle((ExEntity)me.Body);
						//如果结果处理成功，则消耗该消息
						LogUtils.Debug($"异步api接收消息：{me.Body}");
					}
					else
					{
						LogUtils.Error($"异步api消息接收无效");
					}

					//开始下一次查看数据(每秒运行)
					mq.BeginReceive(new TimeSpan(0, 0, 1));


				}
			}
			catch (Exception mess)
			{
				LogUtils.Error($"异步api接收消息异常：{mess.Message}");
			}

		}


	}
}
