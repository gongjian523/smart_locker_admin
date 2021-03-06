﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Http.Model.param
{
    public class SignInParam
    {
        /// <summary>
        /// 图形验证码Token
        /// </summary>
        public string captcha_token { get; set; } 

        /// <summary>
        /// 图形验证码
        /// </summary>
        public string captcha_value { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string password { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string phone { get; set; }

        /// <summary>
        /// 登录来源 web app
        /// </summary>
        public string source { get; set; } = "app";
    }


    public class UserSignInParam
    {
        /// <summary>
        /// 租户id, 为固定字段 【AQACQqweMIgBAAAAF8jlWJSoBWPpxUA】，在正式上线时会变化
        /// </summary>
        public string tenant_id { get; set; } = "AQACQqweMIgBAAAAF8-jlWJSoBWPpxUA";

        /// <summary>
        /// 登录来源 web app
        /// </summary>
        public string source { get; set; } = "app";
    }
}
