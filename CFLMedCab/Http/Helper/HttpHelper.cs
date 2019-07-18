using System.Collections.Generic;
using Newtonsoft.Json;
using System.Threading;
using CFLMedCab.Http.Constant;
using CFLMedCab.Http.Model.Base;
using System.Net;
using CFLMedCab.Infrastructure.ToolHelper;

namespace CFLMedCab.Http.Helper
{
	public class HttpHelper
	{

		private string token = null;

		// 定义一个静态变量来保存类的实例
		private static HttpHelper singleton;

		// 定义一个标识确保线程同步
		private static readonly object locker = new object();

		//定义公有方法提供一个全局访问点。
		public static HttpHelper GetInstance()
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
						singleton = new HttpHelper();
					}
				}
			}
			return singleton;
		}

		public HttpHelper()
		{
			//处理HttpWebRequest访问https有安全证书的问题（ 请求被中止: 未能创建 SSL/TLS 安全通道。）
			ServicePointManager.ServerCertificateValidationCallback += (s, cert, chain, sslPolicyErrors) => true;
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
		}


		/// <summary>
		/// 通用返回结果处理类
		/// </summary>
		/// <typeparam name="T">结果消息体类型</typeparam>
		/// <param name="resultHandleType">结果处理类型</param>
		/// <param name="handleEventWait">处理http线程类</param>
		/// <param name="result">字符串结果</param>
		/// <param name="ret">返回处理后结果</param>
		private void ResultHand<T>(ResultHandleType resultHandleType, HandleEventWait handleEventWait, string result, out BaseData<T> ret)
		{
			ret = null;

			switch (resultHandleType)
			{
				case ResultHandleType.Normal:

					ret = JsonConvert.DeserializeObject<BaseData<T>>(result);
					handleEventWait.Set();

					break;
				case ResultHandleType.Abnormal:

					ret = new BaseData<T>
					{
						code = (int)ResultCode.Request_Exception,
						message = result
					};
					handleEventWait.Set();

					break;
				case ResultHandleType.TimeOut:

					if (handleEventWait.WaitOne(HttpConstant.HttpTimeOut))
					{
						ret = new BaseData<T>
						{
							code = (int)ResultCode.Request_Exception,
							message = result
						};
					}

					break;
				default:
					break;
			}

		}

		/// <summary>
		/// http线程阻塞操作类
		/// </summary>
		private class HandleEventWait {

			private readonly EventWaitHandle eventWaitHandle;

			public HandleEventWait()
			{
				//手动   非终止状态
				eventWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
			}

			/// <summary>
			/// 告知释放信号量
			/// </summary>
			public void Set()
			{
				if (!eventWaitHandle.SafeWaitHandle.IsClosed)
					eventWaitHandle.Set();
			}

			/// <summary>
			/// 关闭资源
			/// </summary>
			public void Close()
			{
				if(!eventWaitHandle.SafeWaitHandle.IsClosed)
					eventWaitHandle.Close();
			}

			/// <summary>
			/// 阻塞当前线程
			/// </summary>
			/// <param name="timeout">阻塞超时时间</param>
			/// <returns></returns>
			public bool WaitOne(int timeout)
			{

				bool waitRet = !eventWaitHandle.WaitOne(timeout);
				if (waitRet)
				{
					Close();
				}

				return waitRet;
			}

		}


		/// <summary>
		/// 同步获取get请求结果
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public BaseData<T> Get<T>(QueryParam queryParam) where T : class
		{

			var handleEventWait = new HandleEventWait();
			BaseData<T> ret = null;

			JumpKick.HttpLib.Http.Get(HttpConstant.GetQueryUrl(typeof(T).Name, queryParam)).Headers(GetHeaders()).OnSuccess(result =>
			{
				ResultHand(ResultHandleType.Normal, handleEventWait, result, out ret);

			}).OnFail(webexception =>
			{
				ResultHand(ResultHandleType.Abnormal, handleEventWait, webexception.Message, out ret);

			}).Go();

			ResultHand(ResultHandleType.TimeOut, handleEventWait, "请求超时", out ret);

			return ret;

		}

		/// <summary>
		/// 同步获取post请求结果
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public BaseData<T> Post<T>(T postParam) where T : class
		{

			var handleEventWait = new HandleEventWait();
			BaseData<T> ret = null;

			JumpKick.HttpLib.Http.Post(HttpConstant.GetCreateUrl(typeof(T).Name)).Headers(GetHeaders()).Body(JsonConvert.SerializeObject(postParam)).OnSuccess(result =>
			{
				ResultHand(ResultHandleType.Normal, handleEventWait, result, out ret);

			}).OnFail(webexception =>
			{
				ResultHand(ResultHandleType.Abnormal, handleEventWait, webexception.Message, out ret);

			}).Go();

			ResultHand(ResultHandleType.TimeOut, handleEventWait, "请求超时", out ret);

			return ret;

		}

		/// <summary>
		/// 同步获取post请求结果
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public BaseData<T> Post<T>(T postParam, string url) where T : class
		{

			var handleEventWait = new HandleEventWait();
			BaseData<T> ret = null;

			JumpKick.HttpLib.Http.Post(url).Headers(GetHeaders()).Body(JsonConvert.SerializeObject(postParam)).OnSuccess(result =>
			{
				ResultHand(ResultHandleType.Normal, handleEventWait, result, out ret);

			}).OnFail(webexception =>
			{
				ResultHand(ResultHandleType.Abnormal, handleEventWait, webexception.Message, out ret);

			}).Go();

			ResultHand(ResultHandleType.TimeOut, handleEventWait, "请求超时", out ret);

			return ret;

		}

		/// <summary>
		/// 同步获取get请求结果
		/// </summary>
		/// <param name="queryParam">token参数</param>
		/// <returns></returns>
		public BaseData<T> Get<T>(string url) where T : class
		{
			var handleEventWait = new HandleEventWait();
			BaseData<T> ret = null;

			JumpKick.HttpLib.Http.Get(url).Headers(GetHeaders()).OnSuccess(result =>
			{
				ResultHand(ResultHandleType.Normal, handleEventWait, result, out ret);

			}).OnFail(webexception =>
			{
				ResultHand(ResultHandleType.Abnormal, handleEventWait, webexception.Message, out ret);

			}).Go();

			ResultHand(ResultHandleType.TimeOut, handleEventWait, "请求超时", out ret);

			return ret;

		}

		/// <summary>
		/// 获取
		/// </summary>
		/// <returns></returns>
		public IDictionary<string, string> GetHeaders()
		{
			if (string.IsNullOrEmpty(token))
			{
				token = "Ae0kAFOHHF0AAEFRQUNRcXdlSjVjQkFBQUF1cExjbFdKU29CVUZjUlFBQVFBQ1Fxd2VNSWdCQUFBQUY4LWpsV0pTb0JXUHB4VUH0y7iG-0fJJYsEhQeKyCbno1iv5jjVq-EN2xf0RG1Fvnd_PrvSGFxXg2CjMhq5isDjtI4ez0GbyxsWmzmgZa1t";
			}

			return new Dictionary<string, string>
			{
				{ "x-token", token }
			};
		}

		/// <summary>
		/// 设置token值
		/// </summary>
		/// <returns></returns>
		public void SetHeaders(string token)
		{
			this.token = token;
		}

	}
}
