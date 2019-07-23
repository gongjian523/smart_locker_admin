using System.Collections.Generic;
using Newtonsoft.Json;
using System.Threading;
using CFLMedCab.Http.Constant;
using CFLMedCab.Http.Model.Base;
using System.Net;
using CFLMedCab.Infrastructure.ToolHelper;
using System;
using CFLMedCab.Http.Model.param;
using System.Text;
using CFLMedCab.Http.Enum;
using CFLMedCab.Http.Model.login;

namespace CFLMedCab.Http.Helper
{
	public class HttpHelper
	{

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
        /// 请求头
        /// </summary>
        private IDictionary<string, string> Headers = new Dictionary<string, string>();

        /// <summary>
        /// 根据表名获取查询数据url
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static string GetQueryUrl(string tableName, QueryParam queryParam)
		{

			StringBuilder queryParamUrlStr = new StringBuilder();
			if (queryParam != null)
			{
				queryParamUrlStr.Append("?");

				var queryParamProps = queryParam.GetType().GetProperties();

				foreach (var queryParamProp in queryParamProps)
				{
					//从原对象中获取值
					var queryParamPropValue = queryParamProp.GetValue(queryParam, null);

					//根据参数赋值情况过滤
					if (queryParamPropValue == null
						|| (queryParamPropValue is List<string> && ((List<string>)queryParamPropValue).Count <= 0)
						|| (queryParamPropValue is QueryParam.In && ((QueryParam.In)queryParamPropValue).in_list.Count <= 0)
						|| (queryParamPropValue is QueryParam.ViewFilter && ((QueryParam.ViewFilter)queryParamPropValue).filter.expressions.Count <= 0)
						|| (queryParamPropValue is int && (int)queryParamPropValue == -1))
						continue;

					switch (queryParamProp.Name)
					{
						//拼接排序相关字段
						case "order_by":
						case "order_flag":
							List<string> orderValues = (List<string>)queryParamPropValue;
							orderValues.ForEach(value => {
								queryParamUrlStr.Append(queryParamProp.Name);
								queryParamUrlStr.Append("=");
								queryParamUrlStr.Append(value);
								queryParamUrlStr.Append("&");
							});

							break;
						//拼接分页相关字段
						case "limit":
						case "offset":
							string pageValue = queryParamPropValue.ToString();
							queryParamUrlStr.Append(queryParamProp.Name);
							queryParamUrlStr.Append("=");
							queryParamUrlStr.Append(pageValue);
							queryParamUrlStr.Append("&");
							break;

						//拼接json 过滤查询参数
						case "in":
						case "view_filter":
							string jsonValue = JsonConvert.SerializeObject(queryParamPropValue);
							queryParamUrlStr.Append(queryParamProp.Name);
							queryParamUrlStr.Append("=");
							queryParamUrlStr.Append(jsonValue);
							queryParamUrlStr.Append("&");
							break;
						default:
							break;
					}

				}

				if (queryParamUrlStr.Length > 0)
					//去掉末尾的&
					queryParamUrlStr.Remove(queryParamUrlStr.Length - 1, 1);

			}

			LogUtils.Debug($"url参数为：{queryParamUrlStr.ToString()}");

			return HttpConstant.Domain + HttpConstant.UrlPrefix + tableName + "/query" + queryParamUrlStr.ToString();
		}

		/// <summary>
		/// 根据表名获取创建数据url
		/// </summary>
		/// <param name="tableName"></param>
		/// <returns></returns>
		public static string GetCreateUrl(string tableName)
		{
			return HttpConstant.Domain + HttpConstant.UrlPrefix + tableName;
		}

		/// <summary>
		/// 根据表名获取更新数据url
		/// </summary>
		/// <param name="tableName"></param>
		/// <returns></returns>
		public static string GetUpdateUrl(string tableName, string id)
		{
			return HttpConstant.Domain + HttpConstant.UrlPrefix + tableName + "/" + id;
		}

		/// <summary>
		/// 根据表名获取删除数据url
		/// </summary>
		/// <param name="tableName"></param>
		/// <returns></returns>
		public static string GetDeleteUrl(string tableName, string id, string tableVersion)
		{

			return HttpConstant.Domain + HttpConstant.UrlPrefix + tableName + "/" + id + "?version={" + tableVersion + "}";
		}

		/// <summary>
		/// 获取token请求的url
		/// </summary>
		/// <returns></returns>
		public static string GetCommonQueryUrl<T>(string urlPrefix, T queryParam)
		{
			StringBuilder queryParamUrlStr = new StringBuilder();
			if (queryParam != null)
			{
				queryParamUrlStr.Append("?");

				var queryParamProps = queryParam.GetType().GetProperties();

				foreach (var queryParamProp in queryParamProps)
				{
					//从原对象中获取值
					var queryParamPropValue = queryParamProp.GetValue(queryParam, null);

					if (queryParamPropValue == null)
						continue;

					string value = (string)queryParamPropValue;
					queryParamUrlStr.Append(queryParamProp.Name);
					queryParamUrlStr.Append("=");
					queryParamUrlStr.Append(value);
					queryParamUrlStr.Append("&");

				}

				if (queryParamUrlStr.Length > 0)
					//去掉末尾的&
					queryParamUrlStr.Remove(queryParamUrlStr.Length - 1, 1);


			}

			LogUtils.Debug($"url参数为：{queryParamUrlStr.ToString()}");

			return urlPrefix + queryParamUrlStr.ToString();

		}


		/// <summary>
		/// 获取指静脉绑定请求的url
		/// </summary>
		/// <returns></returns>
		public static string GetVeinmatchLoginUrl()
		{
			return HttpConstant.Domain + HttpConstant.VeinmatchLoginUrlSuffix;
		}

		/// <summary>
		/// 获取指静脉绑定请求的url
		/// </summary>
		/// <returns></returns>
		public static string GetVeinmatchBindingUrl()
		{
			return HttpConstant.Domain + HttpConstant.VeinmatchBindingUrlSuffix;
		}

        /// <summary>
        /// 获取图形验证码token的url
        /// </summary>
        /// <returns></returns>
        public static string GetCaptchaTokeUrl()
        {
            return HttpConstant.Domain + HttpConstant.CaptchaTokenUrlSuffix;
        }


        /// <summary>
        /// 获取图形验证码的url(特殊)
        /// </summary>
        /// <returns></returns>
        public static string GetCaptchaImageUrl()
        {
            return HttpConstant.Domain + HttpConstant.CaptchaImageUrlSuffix;
        }

        /// <summary>
        /// 获取登录的url
        /// </summary>
        /// <returns></returns>
        public static string GetSignInUrl()
        {
            return HttpConstant.Domain + HttpConstant.SignInUrlSuffix;
        }

        /// <summary>
        /// 获取用户登录url
        /// </summary>
        /// <returns></returns>
        public static string GetUserSignInUrl()
        {
            return HttpConstant.Domain + HttpConstant.UserSignInUrlSuffix;
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
				case ResultHandleType.请求正常:

					ret = JsonConvert.DeserializeObject<BaseData<T>>(result);
					handleEventWait.Set();

					break;
				case ResultHandleType.请求异常:

					ret = new BaseData<T>
					{
						code = (int)ResultCode.Request_Exception,
						message = result
					};
					handleEventWait.Set();

					break;
				case ResultHandleType.请求超时:

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
        /// 通用返回结果处理类
        /// </summary>
        /// <typeparam name="T">结果消息体类型</typeparam>
        /// <param name="resultHandleType">结果处理类型</param>
        /// <param name="handleEventWait">处理http线程类</param>
        /// <param name="result">字符串结果</param>
        /// <param name="ret">返回处理后结果</param>
        private void ResultHand<T>(ResultHandleType resultHandleType, HandleEventWait handleEventWait, string result, out BasePostData<T> ret)
        {
            ret = null;

            switch (resultHandleType)
            {
                case ResultHandleType.请求正常:

                    ret = JsonConvert.DeserializeObject<BasePostData<T>>(result);
                    handleEventWait.Set();

                    break;
                case ResultHandleType.请求异常:

                    ret = new BasePostData<T>
                    {
                        code = (int)ResultCode.Request_Exception,
                        message = result
                    };
                    handleEventWait.Set();

                    break;
                case ResultHandleType.请求超时:

                    if (handleEventWait.WaitOne(HttpConstant.HttpTimeOut))
                    {
                        ret = new BasePostData<T>
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
		/// 通用返回结果处理类
		/// </summary>
		/// <typeparam name="T">结果消息体类型</typeparam>
		/// <param name="resultHandleType">结果处理类型</param>
		/// <param name="handleEventWait">处理http线程类</param>
		/// <param name="result">字符串结果</param>
		/// <param name="ret">返回处理后结果</param>
		private void ResultHand<T>(ResultHandleType resultHandleType, HandleEventWait handleEventWait, string result, out BaseSinglePostData<T> ret)
		{
			ret = null;

			switch (resultHandleType)
			{
				case ResultHandleType.请求正常:

					ret = JsonConvert.DeserializeObject<BaseSinglePostData<T>>(result);
					handleEventWait.Set();

					break;
				case ResultHandleType.请求异常:

					ret = new BaseSinglePostData<T>
					{
						code = (int)ResultCode.Request_Exception,
						message = result
					};
					handleEventWait.Set();

					break;
				case ResultHandleType.请求超时:

					if (handleEventWait.WaitOne(HttpConstant.HttpTimeOut))
					{
						ret = new BaseSinglePostData<T>
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
		/// 通用返回结果处理类
		/// </summary>
		/// <typeparam name="T">结果消息体类型</typeparam>
		/// <param name="resultHandleType">结果处理类型</param>
		/// <param name="handleEventWait">处理http线程类</param>
		/// <param name="result">字符串结果</param>
		/// <param name="ret">返回处理后结果</param>
		private void ResultHand<T>(ResultHandleType resultHandleType, HandleEventWait handleEventWait, string result, out BasePutData<T> ret)
		{
			ret = null;

			switch (resultHandleType)
			{
				case ResultHandleType.请求正常:

					ret = JsonConvert.DeserializeObject<BasePutData<T>>(result);
					handleEventWait.Set();

					break;
				case ResultHandleType.请求异常:

					ret = new BasePutData<T>
					{
						code = (int)ResultCode.Request_Exception,
						message = result
					};
					handleEventWait.Set();

					break;
				case ResultHandleType.请求超时:

					if (handleEventWait.WaitOne(HttpConstant.HttpTimeOut))
					{
						ret = new BasePutData<T>
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

			JumpKick.HttpLib.Http.Get(GetQueryUrl(typeof(T).Name, queryParam)).Headers(GetHeaders()).OnSuccess(result =>
			{
				ResultHand(ResultHandleType.请求正常, handleEventWait, result, out ret);

			}).OnFail(webexception =>
			{
				ResultHand(ResultHandleType.请求异常, handleEventWait, webexception.Message, out ret);

			}).Go();

			ResultHand(ResultHandleType.请求超时, handleEventWait, ResultHandleType.请求超时.ToString(), out ret);

			return ret;

		}

		/// <summary>
		/// 同步获取get请求结果
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public BaseData<T> Get<T>() where T : class
		{

			var handleEventWait = new HandleEventWait();
			BaseData<T> ret = null;

			JumpKick.HttpLib.Http.Get(GetQueryUrl(typeof(T).Name, null)).Headers(GetHeaders()).OnSuccess(result =>
			{
				ResultHand(ResultHandleType.请求正常, handleEventWait, result, out ret);

			}).OnFail(webexception =>
			{
				ResultHand(ResultHandleType.请求异常, handleEventWait, webexception.Message, out ret);

			}).Go();

			ResultHand(ResultHandleType.请求超时, handleEventWait, ResultHandleType.请求超时.ToString(), out ret);

			return ret;

		}

        /// <summary>
        /// 同步获取get请求结果
        /// </summary>
        /// <param name="url">已经拼接好的url</param>
        /// <returns></returns>
        public BaseData<T> Get<T>(string url) where T : class
        {
            var handleEventWait = new HandleEventWait();
            BaseData<T> ret = null;

            JumpKick.HttpLib.Http.Get(url).Headers(GetHeaders()).OnSuccess(result =>
            {
                ResultHand(ResultHandleType.请求正常, handleEventWait, result, out ret);

            }).OnFail(webexception =>
            {
                ResultHand(ResultHandleType.请求异常, handleEventWait, webexception.Message, out ret);

            }).Go();

            ResultHand(ResultHandleType.请求超时, handleEventWait, ResultHandleType.请求超时.ToString(), out ret);

            return ret;

        }

        /// <summary>
        /// 同步获取get请求结果
        /// </summary>
        /// <param name="url">已经拼接好的url</param>
        /// <returns></returns>
        public string Get<T>(string urlPrefix, T queryParam) where T : class
        {
            var handleEventWait = new HandleEventWait();
            string ret = null;

            JumpKick.HttpLib.Http.Get(GetCommonQueryUrl(urlPrefix, queryParam)).Headers(GetHeaders()).OnSuccess(result =>
            {
                ret = result;
                handleEventWait.Set();
            }).OnFail(webexception =>
            {
                ret = webexception.Message;
                handleEventWait.Set();

            }).Go();

            if (!handleEventWait.WaitOne(HttpConstant.HttpTimeOut))
            {
                ret = ResultHandleType.请求超时.ToString();
            }

            return ret;
        }

        /// <summary>
        /// 同步获取get请求结果
        /// </summary>
        /// <param name="url">已经拼接好的url</param>
        /// <returns></returns>
        public BasePostData<T> Get<T,K>(string urlPrefix, K queryParam) where T: class
        {
            var handleEventWait = new HandleEventWait();
			BasePostData<T> ret = null;

            string url = GetCommonQueryUrl(urlPrefix, queryParam);

            JumpKick.HttpLib.Http.Get(url).OnSuccess(result =>
            {
                ResultHand(ResultHandleType.请求正常, handleEventWait, result, out ret);

            }).OnFail(webexception =>
            {
                ResultHand(ResultHandleType.请求异常, handleEventWait, webexception.Message, out ret);

            }).Go();

            ResultHand(ResultHandleType.请求超时, handleEventWait, ResultHandleType.请求超时.ToString(), out ret);

            return ret;
        }



        /// <summary>
        /// 同步获取post请求结果
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public BasePostData<T> Post<T>(T postParam) where T : class
		{

			var handleEventWait = new HandleEventWait();
			BasePostData<T> ret = null;
			LogUtils.Debug($"post请求参数为{JsonConvert.SerializeObject(postParam)}");

			JsonSerializerSettings jsetting = new JsonSerializerSettings
			{
				NullValueHandling = NullValueHandling.Ignore
			};


			JumpKick.HttpLib.Http.Post(GetCreateUrl(typeof(T).Name)).Headers(GetHeaders()).Body(JsonConvert.SerializeObject(postParam, Formatting.Indented, jsetting)).OnSuccess(result =>
			{
				ResultHand(ResultHandleType.请求正常, handleEventWait, result, out ret);

			}).OnFail(webexception =>
			{
				ResultHand(ResultHandleType.请求异常, handleEventWait, webexception.Message, out ret);

			}).Go();

			ResultHand(ResultHandleType.请求超时, handleEventWait, ResultHandleType.请求超时.ToString(), out ret);

			return ret;

		}

		/// <summary>
		/// 同步获取post请求结果
		/// </summary>
		/// <param name="postParam">通用post参数</param>
		/// <returns></returns>
		public BasePostData<T> Post<T>(PostParam<T> postParam) where T : class
		{

			var handleEventWait = new HandleEventWait();
			BasePostData<T> ret = null;


            LogUtils.Debug($"post请求参数为{JsonConvert.SerializeObject(postParam)}");

			JsonSerializerSettings jsetting = new JsonSerializerSettings
			{
				NullValueHandling = NullValueHandling.Ignore
			};

            var temp = JsonConvert.SerializeObject(postParam, Formatting.Indented, jsetting);

            JumpKick.HttpLib.Http.Post(GetCreateUrl(typeof(T).Name)).Headers(GetHeaders()).Body(JsonConvert.SerializeObject(postParam, Formatting.Indented, jsetting)).OnSuccess(result =>
			{
				ResultHand(ResultHandleType.请求正常, handleEventWait, result, out ret);

			}).OnFail(webexception =>
			{
				ResultHand(ResultHandleType.请求异常, handleEventWait, webexception.Message, out ret);

			}).Go();

			ResultHand(ResultHandleType.请求超时, handleEventWait, ResultHandleType.请求超时.ToString(), out ret);

			return ret;

		}

		/// <summary>
		/// 同步获取post请求结果
		/// </summary>
		/// <param name="url"></param>
		/// <param name="postParam">post参数</param>
		/// <returns></returns>
		public BasePostData<T> Post<T, K>(K postParam, string url) where T : class
		{

			var handleEventWait = new HandleEventWait();
			BasePostData<T> ret = null;

			LogUtils.Debug($"post的url为：{url} ; post请求参数为{JsonConvert.SerializeObject(postParam)}");

			JsonSerializerSettings jsetting = new JsonSerializerSettings
			{
				NullValueHandling = NullValueHandling.Ignore
			};


			JumpKick.HttpLib.Http.Post(url).Headers(GetHeaders()).Body(JsonConvert.SerializeObject(postParam, Formatting.Indented, jsetting)).OnSuccess(result =>
			{
				ResultHand(ResultHandleType.请求正常, handleEventWait, result, out ret);

			}).OnFail(webexception =>
			{
				ResultHand(ResultHandleType.请求异常, handleEventWait, webexception.Message, out ret);

			}).Go();

			ResultHand(ResultHandleType.请求超时, handleEventWait, ResultHandleType.请求超时.ToString(), out ret);

			return ret;

		}

        /// <summary>
        /// 同步获取post请求结果
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public BasePostData<T> Post<T>(string url) where T : class
        {
            var handleEventWait = new HandleEventWait();
			BasePostData<T> ret = null;

            JumpKick.HttpLib.Http.Post(url).Headers(GetHeaders()).OnSuccess(result =>
            {
                ResultHand(ResultHandleType.请求正常, handleEventWait, result, out ret);

            }).OnFail(webexception =>
            {
                ResultHand(ResultHandleType.请求异常, handleEventWait, webexception.Message, out ret);

            }).Go();

            ResultHand(ResultHandleType.请求超时, handleEventWait, ResultHandleType.请求超时.ToString(), out ret);

            return ret;
        }

        // <summary>
        /// 同步获取post请求结果
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public BaseSinglePostData<T> Post<T>(string url, bool single) where T : class
        {
            var handleEventWait = new HandleEventWait();
			BaseSinglePostData<T> ret = null;

            JumpKick.HttpLib.Http.Post(url).Headers(GetHeaders()).OnSuccess(result =>
            {
                ResultHand(ResultHandleType.请求正常, handleEventWait, result, out ret);

            }).OnFail(webexception =>
            {
                ResultHand(ResultHandleType.请求异常, handleEventWait, webexception.Message, out ret);

            }).Go();

            ResultHand(ResultHandleType.请求超时, handleEventWait, ResultHandleType.请求超时.ToString(), out ret);

            return ret;
        }

		// <summary>
		/// 同步获取post请求结果
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public BaseSinglePostData<T> Post<T, K>(string url, K postParam, bool isSingle) where T : class
		{
			var handleEventWait = new HandleEventWait();
			BaseSinglePostData<T> ret = null;

			JsonSerializerSettings jsetting = new JsonSerializerSettings
			{
				NullValueHandling = NullValueHandling.Ignore
			};

			JumpKick.HttpLib.Http.Post(url).Headers(GetHeaders()).Body(JsonConvert.SerializeObject(postParam, Formatting.Indented, jsetting)).OnSuccess(result =>
			{
				ResultHand(ResultHandleType.请求正常, handleEventWait, result, out ret);

			}).OnFail(webexception =>
			{
				ResultHand(ResultHandleType.请求异常, handleEventWait, webexception.Message, out ret);

			}).Go();

			ResultHand(ResultHandleType.请求超时, handleEventWait, ResultHandleType.请求超时.ToString(), out ret);

			return ret;
		}

		// <summary>
		/// 同步获取post请求结果
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public BasePostData<T> Post<T,K>(string url, K postParam) where T : class
        {
            var handleEventWait = new HandleEventWait();
			BasePostData<T> ret = null;

			JsonSerializerSettings jsetting = new JsonSerializerSettings
			{
				NullValueHandling = NullValueHandling.Ignore
			};
			
			JumpKick.HttpLib.Http.Post(url).Headers(GetHeaders()).Body(JsonConvert.SerializeObject(postParam, Formatting.Indented, jsetting)).OnSuccess(result =>
            {
                ResultHand(ResultHandleType.请求正常, handleEventWait, result, out ret);

            }).OnFail(webexception =>
            {
                ResultHand(ResultHandleType.请求异常, handleEventWait, webexception.Message, out ret);

            }).Go();

            ResultHand(ResultHandleType.请求超时, handleEventWait, ResultHandleType.请求超时.ToString(), out ret);

            return ret;
        }


        // <summary>
        /// 同步获取post请求结果
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public BasePostData<T> Post<T, K>(string url, K postParam,  IDictionary<string, string> headerParam) where T : class
        {
            var handleEventWait = new HandleEventWait();
			BasePostData<T> ret = null;

            JsonSerializerSettings jsetting = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            JumpKick.HttpLib.Http.Post(url).Headers(headerParam).Body(JsonConvert.SerializeObject(postParam, Formatting.Indented, jsetting)).OnSuccess(result =>
            {
                ResultHand(ResultHandleType.请求正常, handleEventWait, result, out ret);

            }).OnFail(webexception =>
            {
                ResultHand(ResultHandleType.请求异常, handleEventWait, webexception.Message, out ret);

            }).Go();

            ResultHand(ResultHandleType.请求超时, handleEventWait, ResultHandleType.请求超时.ToString(), out ret);

            return ret;
        }

		// <summary>
		/// 同步获取post请求结果
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public BaseSinglePostData<T> Post<T, K>(string url, K postParam, IDictionary<string, string> headerParam, bool isS) where T : class
		{
			var handleEventWait = new HandleEventWait();
			BaseSinglePostData<T> ret = null;

			JsonSerializerSettings jsetting = new JsonSerializerSettings
			{
				NullValueHandling = NullValueHandling.Ignore
			};

			JumpKick.HttpLib.Http.Post(url).Headers(headerParam).Body(JsonConvert.SerializeObject(postParam, Formatting.Indented, jsetting)).OnSuccess(result =>
			{
				ResultHand(ResultHandleType.请求正常, handleEventWait, result, out ret);

			}).OnFail(webexception =>
			{
				ResultHand(ResultHandleType.请求异常, handleEventWait, webexception.Message, out ret);

			}).Go();

			ResultHand(ResultHandleType.请求超时, handleEventWait, ResultHandleType.请求超时.ToString(), out ret);

			return ret;
		}

		/// <summary>
		/// 同步获取post请求结果
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public BasePutData<T> Put<T>(T putParam) where T : BaseModel
		{

			var handleEventWait = new HandleEventWait();
			BasePutData<T> ret = null;
			LogUtils.Debug($"put请求参数为{JsonConvert.SerializeObject(putParam)}");

			JsonSerializerSettings jsetting = new JsonSerializerSettings
			{
				NullValueHandling = NullValueHandling.Ignore
			};

			JumpKick.HttpLib.Http.Post(GetUpdateUrl(typeof(T).Name, putParam.id)).Headers(GetHeaders()).Body(JsonConvert.SerializeObject(putParam, Formatting.Indented, jsetting)).OnSuccess(result =>
			{
				ResultHand(ResultHandleType.请求正常, handleEventWait, result, out ret);

			}).OnFail(webexception =>
			{
				ResultHand(ResultHandleType.请求异常, handleEventWait, webexception.Message, out ret);

			}).Go();

			ResultHand(ResultHandleType.请求超时, handleEventWait, ResultHandleType.请求超时.ToString(), out ret);

			return ret;

		}


		/// <summary>
		/// 获取token
		/// </summary>
		/// <returns></returns>
		public IDictionary<string, string> GetHeaders()
		{
            if (Headers.Count <= 0)
            {
                SetHeaders("Ae0kAFOHHF0AAEFRQUNRcXdlSjVjQkFBQUF1cExjbFdKU29CVUZjUlFBQVFBQ1Fxd2VNSWdCQUFBQUY4LWpsV0pTb0JXUHB4VUH0y7iG-0fJJYsEhQeKyCbno1iv5jjVq-EN2xf0RG1Fvnd_PrvSGFxXg2CjMhq5isDjtI4ez0GbyxsWmzmgZa1t");
            }

            return Headers;
		}

        /// <summary>
        /// 设置token值
        /// </summary>
        /// <returns></returns>
        public void SetHeaders(string token)
		{
            Headers.Clear();
            Headers.Add("x-token", token);
		}

        /// <summary>
        /// 检查结果是否正确
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="baseData"></param>
        /// <param name="isSuccess"></param>
        /// <returns></returns>
        public BaseData<T> ResultCheck<T>(BaseData<T> baseData, out bool isSuccess)
		{
			isSuccess = false;

			if (baseData.code == (int)ResultCode.OK)
			{
				if (baseData.body != null && baseData.body.global_offset > 0)
				{
					isSuccess = true;
				}
				//结果集正常，但为空
				else
				{
					baseData.code = (int)ResultCode.Result_Exception;
					baseData.message = ResultCode.Result_Exception.ToString();
				}
			}

			return baseData;

		}

		/// <summary>
		/// 检查结果是否正确
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="K"></typeparam>
		/// <param name="baseData"></param>
		/// <param name="isSuccess"></param>
		/// <returns></returns>
		public BasePutData<T> ResultCheckPutByBase<T>(BaseData<T> baseData, out bool isSuccess)
		{
			isSuccess = false;

			BasePutData<T> basePutData = null;

			if (baseData.code == (int)ResultCode.OK)
			{
				if (baseData.body != null && baseData.body.global_offset > 0)
				{
					isSuccess = true;
				}
				//结果集正常，但为空
				else
				{
					basePutData = new BasePutData<T>
					{
						code = (int)ResultCode.Result_Exception,
						message = ResultCode.Result_Exception.ToString()
					};
				}
			}
			else
			{
				basePutData = new BasePutData<T>
				{
					code = baseData.code,
					message = baseData.message
				};
			}

			return basePutData;

		}



		/// <summary>
		/// 检查结果是否正确
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="K"></typeparam>
		/// <param name="baseData"></param>
		/// <param name="isSuccess"></param>
		/// <returns></returns>
		public BasePostData<T> ResultCheck<T>(BasePostData<T> baseData, out bool isSuccess)
		{
			isSuccess = false;

			if (baseData.code == (int)ResultCode.OK)
			{
				if (baseData.body != null && baseData.body.Count > 0)
				{
					isSuccess = true;
				}
				//结果集正常，但为空
				else
				{
					baseData.code = (int)ResultCode.Result_Exception;
					baseData.message = ResultCode.Result_Exception.ToString();
				}
			}

			return baseData;

		}


		/// <summary>
		/// 检查结果是否正确
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="K"></typeparam>
		/// <param name="baseData"></param>
		/// <param name="isSuccess"></param>
		/// <returns></returns>
		public BasePutData<T> ResultCheck<T>(BasePutData<T> baseData, out bool isSuccess)
		{
			isSuccess = false;

			if (baseData.code == (int)ResultCode.OK)
			{
				if (baseData.body != null)
				{
					isSuccess = true;
				}
				//结果集正常，但为空
				else
				{
					baseData.code = (int)ResultCode.Result_Exception;
					baseData.message = ResultCode.Result_Exception.ToString();
				}
			}

			return baseData;

		}

		/// <summary>
		/// 检查结果是否正确
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="K"></typeparam>
		/// <param name="baseData"></param>
		/// <returns></returns>
		public BaseData<T> ResultCheck<T>(BaseData<T> baseData)
		{

			if (baseData.code == (int)ResultCode.OK)
			{
				if (baseData.body == null || baseData.body.global_offset <= 0)
				{
					baseData.code = (int)ResultCode.Result_Exception;
					baseData.message = ResultCode.Result_Exception.ToString();
				}
			}

			return baseData;

		}

		/// <summary>
		/// 检查结果是否正确,用于多次表关联的校验
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="K"></typeparam>
		/// <param name="baseData"></param>
		/// <returns></returns>
		public BaseData<T> ResultCheck<T,K>(Func<HttpHelper, BaseData<T>> func, BaseData<K> baseData)
		{

			BaseData<T> ret;

			//结果集正常
			if (baseData.code == (int)ResultCode.OK)
			{
				if (baseData.body != null && baseData.body.global_offset > 0)
				{
					ret = func(this);
				}
				//结果集正常，但为空
				else
				{
					ret = new BaseData<T>
					{
						code = (int)ResultCode.Result_Exception,
						message = ResultCode.Result_Exception.ToString()
					};
				}
			}
			//结果集异常
			else
			{
				ret = new BaseData<T>()
				{
					code = baseData.code,
					description = baseData.description,
					message = baseData.message
				};
			}

			return ret;
		}


		/// <summary>
		/// 检查结果是否正确,用于多次表关联的校验
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="K"></typeparam>
		/// <param name="baseData"></param>
		/// <returns></returns>
		public BaseSinglePostData<T> ResultCheck<T, K>(Func<HttpHelper, BaseSinglePostData<T>> func, BaseSinglePostData<K> baseData)
		{

			BaseSinglePostData<T> ret;

			//结果集正常
			if (baseData.code == (int)ResultCode.OK)
			{
				if (baseData.body != null)
				{
					ret = func(this);
				}
				//结果集正常，但为空
				else
				{
					ret = new BaseSinglePostData<T>
					{
						code = (int)ResultCode.Result_Exception,
						message = ResultCode.Result_Exception.ToString()
					};
				}
			}
			//结果集异常
			else
			{
				ret = new BaseSinglePostData<T>()
				{
					code = baseData.code,
					description = baseData.description,
					message = baseData.message
				};
			}

			return ret;
		}

		/// <summary>
		/// 检查结果是否正确,用于多次表关联的校验
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="K"></typeparam>
		/// <param name="baseData"></param>
		/// <returns></returns>
		public BasePostData<T> ResultCheck<T, K>(Func<HttpHelper, BasePostData<T>> func, BasePostData<K> baseData)
        {

			BasePostData<T> ret;

            //结果集正常
            if (baseData.code == (int)ResultCode.OK)
            {
                if (baseData.body != null)
                {
                    ret = func(this);
                }
                //结果集正常，但为空
                else
                {
                    ret = new BasePostData<T>
                    {
                        code = (int)ResultCode.Result_Exception,
                        message = ResultCode.Result_Exception.ToString()
                    };
                }
            }
            //结果集异常
            else
            {
                ret = new BasePostData<T>()
                {
                    code = baseData.code,
                    description = baseData.description,
                    message = baseData.message
                };
            }

            return ret;
        }

		/// <summary>
		/// 检查结果是否正确,用于多次表关联的校验
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="K"></typeparam>
		/// <param name="baseData"></param>
		/// <returns></returns>
		public BasePostData<T> ResultCheck<T, K>(Func<HttpHelper, BasePostData<T>> func, BaseSinglePostData<K> baseData)
		{

			BasePostData<T> ret;

			//结果集正常
			if (baseData.code == (int)ResultCode.OK)
			{
				if (baseData.body != null)
				{
					ret = func(this);
				}
				//结果集正常，但为空
				else
				{
					ret = new BasePostData<T>
					{
						code = (int)ResultCode.Result_Exception,
						message = ResultCode.Result_Exception.ToString()
					};
				}
			}
			//结果集异常
			else
			{
				ret = new BasePostData<T>()
				{
					code = baseData.code,
					description = baseData.description,
					message = baseData.message
				};
			}

			return ret;
		}

		/// <summary>
		/// 检查结果是否正确,用于多次表关联的校验
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="K"></typeparam>
		/// <param name="baseData"></param>
		/// <returns></returns>
		public string ResultCheck<T>(Func<HttpHelper, string> func, BasePostData<T> data)
        {

            string ret;

            //结果集正常
            if (data.code == (int)ResultCode.OK)
            {
                if (data.body != null)
                {
                    ret = func(this);
                }
                //结果集正常，但为空
                else
                {
                    ret = ResultCode.Result_Exception.ToString();
                }
            }
            //结果集异常
            else
            {
                ret = data.message;
            }

            return ret;
        }

    }
}
