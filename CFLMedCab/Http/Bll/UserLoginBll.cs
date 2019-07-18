using CFLMedCab.Http.Constant;
using CFLMedCab.Http.Helper;
using CFLMedCab.Http.Model;
using CFLMedCab.Http.Model.Base;
using System.Collections.Generic;


namespace CFLMedCab.Http.Bll
{
	/// <summary>
	/// 用户登录业务
	/// </summary>
	public class UserLoginBll
	{

		// 定义一个静态变量来保存类的实例
		private static UserLoginBll singleton;
		// 定义一个标识确保线程同步
		private static readonly object locker = new object();


		//定义公有方法提供一个全局访问点。
		public static UserLoginBll GetInstance()
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
						singleton = new UserLoginBll();
					}
				}
			}
			return singleton;
		}

		public string FingerVein;

		/// <summary>
		/// 获取token，根据用户参数
		/// </summary>
		/// <param name="account"></param>
		/// <returns></returns>
		public string GetUserToken(Account account)
		{
			BaseData<Account> baseDataAccount = HttpHelper.GetInstance().Get<Account>(new QueryParam
			{
				view_filter = new QueryParam.ViewFilter
				{
					filter = new QueryParam.Filter
					{
						logical_relation = "1 AND 2",
						expressions = new List<QueryParam.Expressions>
						{
							new QueryParam.Expressions
							{
								field = "Phone",
								@operator = "CONTAINS",
								operands =  BllHelper.OperandsProcess(new List<string>{ account.Phone })
							},
							new QueryParam.Expressions
							{
								field = "Password",
								@operator = "==",
								operands = BllHelper.OperandsProcess(new List<string>{ BllHelper.EncodeBase64Str(account.Password) })
							}
						},
					}
				}
			});

			BaseData<User> baseDataUser = null;

			if (baseDataAccount.code == (int)ResultCode.OK)
			{
				
				baseDataUser =  HttpHelper.GetInstance().Get<User>(new QueryParam
				{
					@in = new QueryParam.In
					{
						field = "MobilePhone",
						in_list = new List<string> { baseDataAccount.body.objects[0].Phone }
					}
				});

				

			}
			return baseDataUser.ToString();
		}
	}
}
