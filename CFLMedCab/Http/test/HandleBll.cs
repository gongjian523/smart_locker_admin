using System.ComponentModel;


namespace CFLMedCab.Http.Bll
{
	/// <summary>
	/// 业务层处理类，其他需要调用业务的功能和代码块，需订阅该委托，该类通过方法和参数区分不同业务，并通过委托通知到具体的功能和代码块
	/// </summary>
	public abstract class HandleBll : BaseBll
	{

		//事件处理方法（用于派生类实现）
		protected abstract void GetEventHandle(object sender, ResultEventArgs e);

		//事件处理方法（用于派生类实现）
		protected abstract void PostEventHandle(object sender, ResultEventArgs e);

		//事件处理方法（用于派生类实现）
		protected abstract void PutEventHandle(object sender, ResultEventArgs e);

		//事件处理方法（用于派生类实现）
		protected abstract void DeleteEventHandle(object sender, ResultEventArgs e);

		//注册事件处理程序
		public HandleBll()
		{
			getHttpEventHandler += new GetHttpEventHandler(GetEventHandle);
			postHttpEventHandler += new PostHttpEventHandler(PostEventHandle);
			putHttpEventHandler += new PutHttpEventHandler(PutEventHandle);
			deleteHttpEventHandler += new DeleteHttpEventHandler(DeleteEventHandle);

		}

		/// <summary>
		/// 业务操作类型
		/// </summary>
		public enum BllHandleType
		{
			/// <summary>
			/// Get业务操作
			/// </summary>
			[Description("Get业务操作")]
			Get = 0,

			/// <summary>
			/// Post业务操作
			/// </summary>
			[Description("Post业务操作")]
			Post = 1,

			/// <summary>
			/// Put业务操作
			/// </summary>
			[Description("Put业务操作")]
			Put = 2,


			/// <summary>
			/// Delete业务操作
			/// </summary>
			[Description("Delete业务操作")]
			Delete = 3

		}

		/// <summary>
		/// 业务操作类型
		/// </summary>
		public enum BllType
		{
			/// <summary>
			/// 用户登录模块
			/// </summary>
			[Description("用户登录模块")]
			UserLogin = 0,

	

		}


		public string BeforeBllHandle<T>(BllHandleType bllHandleType) where T : class
		{

			return null;
		}


		public void AfterBllHandle()
		{
			
		}

	}
}
