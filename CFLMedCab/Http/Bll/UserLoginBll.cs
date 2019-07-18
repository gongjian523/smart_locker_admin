using CFLMedCab.Http.Constant;
using CFLMedCab.Http.Helper;
using CFLMedCab.Http.Model;
using CFLMedCab.Http.Model.Base;
using System.Collections.Generic;
using System.Web;

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

		/// <summary>
		/// 指静脉绑定
		/// </summary>
		/// <param name="param">请求参数</param>
		/// <returns></returns>
		public BaseData<string> VeinmatchBinding(VeinmatchPostParam param)
		{
			return HttpHelper.GetInstance().Post<string, VeinmatchPostParam>(param, HttpConstant.GetVeinmatchBindingUrl());
		}

		/// <summary>
		/// 指静脉识别 
		/// </summary>
		/// <param name="param">请求参数</param>
		/// <returns></returns>
		public BaseData<string> VeinmatchLogin(string regfeature)
		{
			//匿名类
			return HttpHelper.GetInstance().Post<string, object>(new { regfeature }, HttpConstant.GetVeinmatchLoginUrl());
		}

		/// <summary>
		/// 获取token，根据用户参数
		/// </summary>
		/// <param name="account"></param>
		/// <returns></returns>
		public BaseData<string> GetUserToken(Account account)
		{
			//获取账户数据
			BaseData<Account> baseDataAccount = HttpHelper.GetInstance().Get<Account>(new QueryParam
			{
				view_filter = 
				{
					filter = 
					{
						logical_relation = "1 AND 2",
						expressions =
						{
							new QueryParam.Expressions
							{
								field = "Phone",
								@operator = "CONTAINS",
								operands =  {$"'{ HttpUtility.UrlEncode(account.Phone) }'"}
							},
							new QueryParam.Expressions
							{
								field = "Password",
								@operator = "==",
								operands = {$"'{ BllHelper.EncodeBase64Str(account.Password) }'" }
							}
						}
					}
				}
			});

			//根据账户获取用户数据
			BaseData<User> baseDataUser = HttpHelper.GetInstance().ResultCheck((HttpHelper hh) => {

				return hh.Get<User>(new QueryParam
				{
					@in =
					{
						field = "MobilePhone",
						in_list =  { HttpUtility.UrlEncode(baseDataAccount.body.objects[0].Phone) }
					}
				});

			}, baseDataAccount);

			//根据用户获取token
			return HttpHelper.GetInstance().ResultCheck((HttpHelper hh) =>
			{

				return hh.Get<string>(HttpConstant.GetTokenQueryUrl(new TokenQueryParam
				{
					user_id = baseDataUser.body.objects[0].id
				}));

			}, baseDataUser);

		}
	}
}
