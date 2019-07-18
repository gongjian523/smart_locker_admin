
using CFLMedCab.Http.Constant;
using CFLMedCab.Http.Helper;
using JumpKick.HttpLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Http.Bll
{
	/// <summary>
	/// 业务基础类，包含跟http工具类交互逻辑，提供
	/// </summary>
	public class BaseBll
	{
		/// <summary>
		/// 自定义http请求处理参数
		/// </summary>
		public class ResultEventArgs : EventArgs
		{
			public ResultEventArgs(Type result)
			{
				Result = result;
			}
			public Type Result { get; }
		}

		//Get请求声明关于事件的委托(通用委托)
		protected delegate void GetHttpEventHandler(object sender, ResultEventArgs e);

		//Get请求声明事件
		protected event GetHttpEventHandler getHttpEventHandler;

		//Post请求声明关于事件的委托(通用委托)
		protected delegate void PostHttpEventHandler(object sender, ResultEventArgs e);

		//Post请求声明事件
		protected event PostHttpEventHandler postHttpEventHandler;

		//put请求声明关于事件的委托(通用委托)
		protected delegate void PutHttpEventHandler(object sender, ResultEventArgs e);

		//put请求声明事件
		protected event PutHttpEventHandler putHttpEventHandler;

		//Delete请求声明关于事件的委托(通用委托)
		protected delegate void DeleteHttpEventHandler(object sender, ResultEventArgs e);

		//声明事件
		protected event DeleteHttpEventHandler deleteHttpEventHandler;

		//get请求
		public void Get<T>(QueryParam queryParam) where T : class
		{
			if (getHttpEventHandler != null)
			{
				getHttpEventHandler?.Invoke(HttpHelper.GetInstance().Get<T>(queryParam), new ResultEventArgs(typeof(T)));
			}
		}

		/// <summary>
		/// 获取
		/// </summary>
		/// <returns></returns>
		public IDictionary<string, string> GetHeaders()
		{
			return new Dictionary<string, string>
			{
				{ "x-token", "Ae0kAFOHHF0AAEFRQUNRcXdlSjVjQkFBQUF1cExjbFdKU29CVUZjUlFBQVFBQ1Fxd2VNSWdCQUFBQUY4LWpsV0pTb0JXUHB4VUH0y7iG-0fJJYsEhQeKyCbno1iv5jjVq-EN2xf0RG1Fvnd_PrvSGFxXg2CjMhq5isDjtI4ez0GbyxsWmzmgZa1t" }
			};
		}

	}
}
