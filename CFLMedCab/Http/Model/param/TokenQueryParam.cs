﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Http.Model.param
{
	/// <summary>
	/// 获取token数据的参数
	/// </summary>
	public class TokenQueryParam
	{
        /// <summary>
        /// 为固定字段 【AQACQqweMIgBAAAAF8jlWJSoBWPpxUA】，在正式上线时会变化 遵义测试环境使用
        /// 【AQACQqweDg8BAAAAYqCQtRIJuxU4iQgA】 测试环境
        /// </summary>
        //public string tenant_id { get; set; } = "AQACQqweMIgBAAAAF8-jlWJSoBWPpxUA";
        //public string tenant_id { get; set; } = "AQAWb9HOnJ4BAAAAXx1TE61Y2hUc-AQA";
        //用于调试厂商无法显示的版本
        public string tenant_id { get; set; } = "AQA-WeZOAk0BAAAAvC3DOH1V6hWO3Q4A";

        /// <summary>
        /// 用户id
        /// </summary>
        public string user_id { get; set; }

		/// <summary>
		/// 客户端类型
		/// </summary>
		public string device { get; set; } = "app";

		/// <summary>
		/// 
		/// </summary>
		public string app { get; set; } = "crm";
	}
}
