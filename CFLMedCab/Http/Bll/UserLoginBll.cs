using CFLMedCab.Http.Constant;
using CFLMedCab.Http.Helper;
using CFLMedCab.Http.Model;
using CFLMedCab.Http.Model.Base;
using CFLMedCab.Http.Model.login;
using CFLMedCab.Http.Model.param;
using CFLMedCab.Infrastructure.ToolHelper;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web;

namespace CFLMedCab.Http.Bll
{
	/// <summary>
	/// 用户登录业务
	/// </summary>
	public class UserLoginBll : BaseBll<UserLoginBll>
	{

        /// <summary>
        /// 指静脉识别 
        /// </summary>
        /// <param name="param">请求参数</param>
        /// <returns></returns>
        public BaseSinglePostData<VeinRegister> VeinmatchRegister(VeinregisterPostParam param)
        {
            System.Diagnostics.Debug.WriteLine("VeinmatchLogin: " + param.devsign);
            //HttpHelper.GetInstance().SetHeaders("Ae0kAFOHHF0AAEFRQUNRcXdlSjVjQkFBQUF1cExjbFdKU29CVUZjUlFBQVFBQ1Fxd2VNSWdCQUFBQUY4LWpsV0pTb0JXUHB4VUH0y7iG-0fJJYsEhQeKyCbno1iv5jjVq-EN2xf0RG1Fvnd_PrvSGFxXg2CjMhq5isDjtI4ez0GbyxsWmzmgZa1t");
            return HttpHelper.GetInstance().PostByAdminToken<VeinRegister, VeinregisterPostParam>(HttpHelper.GetVeinmatchRegisterUrl(), param);
        }

        /// <summary>
        /// 指静脉绑定
        /// </summary>
        /// <param name="param">请求参数</param>
        /// <returns></returns>
        public BasePostData<string> VeinmatchBinding(VeinbindingPostParam param)
		{
			return HttpHelper.GetInstance().Post<string, VeinbindingPostParam>(param, HttpHelper.GetVeinmatchBindingUrl());
		}

		/// <summary>
		/// 指静脉识别 
		/// </summary>
		/// <param name="param">请求参数</param>
		/// <returns></returns>
		public BaseSinglePostData<VeinMatch> VeinmatchLogin(VeinmatchPostParam param)
		{
            System.Diagnostics.Debug.WriteLine("VeinmatchLogin: " + param.regfeature);

            //HttpHelper.GetInstance().SetHeaders("Ae0kAFOHHF0AAEFRQUNRcXdlSjVjQkFBQUF1cExjbFdKU29CVUZjUlFBQVFBQ1Fxd2VNSWdCQUFBQUY4LWpsV0pTb0JXUHB4VUH0y7iG-0fJJYsEhQeKyCbno1iv5jjVq-EN2xf0RG1Fvnd_PrvSGFxXg2CjMhq5isDjtI4ez0GbyxsWmzmgZa1t");
            return HttpHelper.GetInstance().PostByAdminToken<VeinMatch, VeinmatchPostParam>(HttpHelper.GetVeinmatchLoginUrl(), param);
		}


        ///// <summary>
        ///// 指静脉识别 
        ///// </summary>
        ///// <param name="param">请求参数</param>
        ///// <returns></returns>
        //public BasePostData<string> VeinmatchLogin(string regfeature)
        //{
        //    //匿名类
        //    System.Diagnostics.Debug.WriteLine("VeinmatchLogin: " + regfeature);
        //    return HttpHelper.GetInstance().Post<string, object>(new { regfeature }, HttpHelper.GetVeinmatchLoginUrl());
        //}

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

				return hh.Get<string>(HttpHelper.GetCommonQueryUrl(HttpConstant.TokenUrl, new TokenQueryParam
				{
					user_id = baseDataUser.body.objects[0].id
				}));

			}, baseDataUser);

		}
   

        /// <summary>
        /// 获取token，根据用户参数
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public BaseSinglePostData<UserToken> GetUserToken(SignInParam siParam)
        {
			BaseSinglePostData<Token> dataSignIn = HttpHelper.GetInstance().Post<Token, SignInParam>(HttpHelper.GetSignInUrl(), siParam, true);

			//根据账户获取用户数据
			BaseSinglePostData<UserToken> dataUserToken = HttpHelper.GetInstance().ResultCheck((HttpHelper hh) => {

                UserSignInParam bodyPara = new UserSignInParam();
                IDictionary<string, string> headerParam = new Dictionary<string, string>();
                headerParam.Add("Authorization", dataSignIn.body.token);

                return hh.Post<UserToken, UserSignInParam>(HttpHelper.GetUserSignInUrl(), bodyPara, headerParam, true);

            }, dataSignIn);

            return dataUserToken;
        }
        /// <summary>
        /// 获取图形验证码
        /// </summary>
        /// <returns></returns>
        public BaseSinglePostData<CaptchaToken> GetCaptchaImageToken()
        {
             return HttpHelper.GetInstance().Post<CaptchaToken>(HttpHelper.GetCaptchaTokeUrl(),true);
        }


        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns></returns>
        public BaseData<User> GetUserInfo(string mobilePhone)
        {

            //根据账户获取用户数据
            BaseData<User> baseDataUser = HttpHelper.GetInstance().Get<User>(new QueryParam
            {
                @in =
                    {
                        field = "MobilePhone",
                        in_list =  { HttpUtility.UrlEncode(mobilePhone) }
                    }
            });
            baseDataUser = HttpHelper.GetInstance().ResultCheck(baseDataUser, out bool isSuccess);

            return baseDataUser;
        }
    }
}
