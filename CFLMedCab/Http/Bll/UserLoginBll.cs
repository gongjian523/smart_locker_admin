using CFLMedCab.Http.Constant;
using CFLMedCab.Http.Helper;
using CFLMedCab.Http.Model;
using CFLMedCab.Http.Model.Base;
using CFLMedCab.Http.Model.login;
using CFLMedCab.Http.Model.param;
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
		/// 指静脉绑定
		/// </summary>
		/// <param name="param">请求参数</param>
		/// <returns></returns>
		public BaseData<string> VeinmatchBinding(VeinbindingPostParam param)
		{
			return HttpHelper.GetInstance().Post<string, VeinbindingPostParam>(param, HttpHelper.GetVeinmatchBindingUrl());
		}

		/// <summary>
		/// 指静脉识别 
		/// </summary>
		/// <param name="param">请求参数</param>
		/// <returns></returns>
		public BaseSingleData<VeinMatch> VeinmatchLogin(VeinmatchPostParam param)
		{
            System.Diagnostics.Debug.WriteLine("VeinmatchLogin: " + param.regfeature);
            return HttpHelper.GetInstance().Post<VeinMatch, VeinmatchPostParam>(HttpHelper.GetVeinmatchLoginUrl(), param, true);
		}


        /// <summary>
        /// 指静脉识别 
        /// </summary>
        /// <param name="param">请求参数</param>
        /// <returns></returns>
        public BaseData<string> VeinmatchLogin(string regfeature)
        {
            //匿名类
            return HttpHelper.GetInstance().Post<string, object>(new { regfeature }, HttpHelper.GetVeinmatchLoginUrl());
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
        public BaseSingleData<UserToken> GetUserToken(SignInParam siParam)
        {
            BaseSingleData<Token> dataSignIn = HttpHelper.GetInstance().Post<Token, SignInParam>(HttpHelper.GetSignInUrl(), siParam, true);

            //根据账户获取用户数据
            BaseSingleData<UserToken> dataUserToken = HttpHelper.GetInstance().ResultCheck((HttpHelper hh) => {

                UserSignInParam bodyPara = new UserSignInParam();
                IDictionary<string, string> headerParam = new Dictionary<string, string>();
                headerParam.Add("Authorization", dataSignIn.body.token);

                return hh.Post<UserToken, UserSignInParam>(HttpHelper.GetUserSignInUrl(), bodyPara, headerParam);

            }, dataSignIn);

            return dataUserToken;
        }

        /// <summary>
        /// 获取图形验证码
        /// </summary>
        /// <returns></returns>
        public BaseSingleData<CaptchaToken> GetCaptchaImageToken()
        {
             return HttpHelper.GetInstance().Post<CaptchaToken>(HttpHelper.GetCaptchaTokeUrl(),true);
        }
    }
}
