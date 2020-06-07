using System;
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
        ///【AQACQqweDg8BAAAAYqCQtRIJuxU4iQgA】 测试环境 
        /// </summary>
        //public string tenant_id { get; set; } =   "AQACQqweMIgBAAAAF8-jlWJSoBWPpxUA";

        //贵阳二医正式租户id
        public string tenant_id { get; set; } = "AQAWb9HOnJ4BAAAAXx1TE61Y2hUc-AQA";
        //贵阳二医正式租户id - END

        //贵阳二医测试租户id
        //public string tenant_id { get; set; } = "AQDKljDmSz4BAAAAvhxdQMihDBbpfwEA";
        //贵阳二医测试租户id - END

        /// <summary>
        /// 登录来源 web app
        /// </summary>
        public string source { get; set; } = "app";
    }
}
