using CFLMedCab.Infrastructure.ToolHelper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;

namespace CFLMedCab.Http.Constant
{
	/// <summary>
	/// http相关常量类
	/// </summary>
	public class HttpConstant
	{

		/// <summary>
		/// Http请求超时时间
		/// </summary>
		public static readonly int HttpTimeOut = 5000;

		/// <summary>
		/// 通用接口域名
		/// </summary>
		private static readonly string Domain = "https://crm.chengfayun.com/";

		/// <summary>
		/// 通用接口版本号
		/// </summary>
		private static readonly string Version = "v1.0";

		/// <summary>
		/// 通用url前缀
		/// </summary>
		private static readonly string UrlPrefix = "api/" + Version + "/one/";

		/// <summary>
		/// token的url(特殊)
		/// </summary>
		private static readonly string TokenUrl = "http://implement.int.chengfayun.net/tenant-gateway/internal/api/v1.0/tenant-gateway/signin";

		/// <summary>
		/// 指静脉绑定的url(特殊)
		/// </summary>
		private static readonly string VeinmatchBindingUrlSuffix = "api/v1.0/plugin/veinmatch/binding";

		/// <summary>
		/// 指静脉识别的url(特殊)
		/// </summary>
		private static readonly string VeinmatchLoginUrlSuffix = "api/v1.0/plugin/veinmatch/login";

		

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
						|| (queryParamPropValue is List<string> && ((List<string>)queryParamPropValue).Count <= 0 ) 
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

			return Domain + UrlPrefix + tableName + "/query" + queryParamUrlStr.ToString();
		}

		/// <summary>
		/// 根据表名获取创建数据url
		/// </summary>
		/// <param name="tableName"></param>
		/// <returns></returns>
		public static string GetCreateUrl(string tableName)
		{
			return Domain + UrlPrefix + tableName;
		}

		/// <summary>
		/// 根据表名获取更新数据url
		/// </summary>
		/// <param name="tableName"></param>
		/// <returns></returns>
		public static string GetUpdateUrl(string tableName, string id)
		{
			return Domain + UrlPrefix + tableName + "/" + id;
		}

		/// <summary>
		/// 根据表名获取删除数据url
		/// </summary>
		/// <param name="tableName"></param>
		/// <returns></returns>
		public static string GetDeleteUrl(string tableName, string id, string tableVersion)
		{

			return Domain + UrlPrefix + tableName + "/" + id + "?version={" + tableVersion + "}";
		}

		/// <summary>
		/// 获取token请求的url
		/// </summary>
		/// <returns></returns>
		public static string GetTokenQueryUrl(TokenQueryParam queryParam)
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

			return TokenUrl + queryParamUrlStr.ToString();

		}


		/// <summary>
		/// 获取指静脉绑定请求的url
		/// </summary>
		/// <returns></returns>
		public static string GetVeinmatchLoginUrl()
		{
			return Domain + VeinmatchLoginUrlSuffix;
		}

		/// <summary>
		/// 获取指静脉绑定请求的url
		/// </summary>
		/// <returns></returns>
		public static string GetVeinmatchBindingUrl()
		{
			return Domain + VeinmatchBindingUrlSuffix;
		}

	}

	/// <summary>
	/// 结果操作类型枚举
	/// </summary>
	public enum ResultHandleType
	{
		/// <summary>
		/// 正常
		/// </summary>
		[Description("请求正常")]
		请求正常 = 0,

		/// <summary>
		/// 异常
		/// </summary>
		[Description("请求异常")]
		请求异常 = 1,

		/// <summary>
		/// 超时
		/// </summary>
		[Description("请求超时")]
		请求超时 = 2

	}

	/// <summary>
	/// 返回码枚举
	/// </summary>
	public enum ResultCode
	{
		/// <summary>
		/// Result_Exception
		/// </summary>
		[Description("Result_Exception")]
		Result_Exception = -2,

		/// <summary>
		/// Request_Exception
		/// </summary>
		[Description("Request_Exception")]
		Request_Exception = -1,

		/// <summary>
		/// OK
		/// </summary>
		[Description("OK")]
		OK = 0,


	}

	/// <summary>
	/// 通用用于查询的参数
	/// </summary>
	public class QueryParam
	{

		/// <summary>
		/// 支持多级排序	order_by=updated_at&order_by=created_at
		/// </summary>
		public List<string> order_by { get; set; } = new List<string>();

		/// <summary>
		/// 可选值：DESC/ASC，支持多级排序，请确保 order_flag 和 order_by 参数个数相同	order_flag=DESC&order_flag=ASC
		/// </summary>
		public List<string> order_flag { get; set; } = new List<string>();

		/// <summary>
		/// 查询个数
		/// </summary>
		public int limit { get; set; } = -1;

		/// <summary>
		/// 偏移量
		/// </summary>
		public int offset { get; set; } = -1;


		/// <summary>
		/// 有 2 个字段，field 表示要筛选的字段， in_list 表示字段的值的列表
		/// </summary>
		public In @in { get; set; } = new In();

		/// <summary>
		/// 表达式字典
		/// </summary>
		public ViewFilter view_filter { get; set; } = new ViewFilter();

		/// <summary>
		/// in对象
		/// </summary>
		public class In
		{
			public string field { get; set; }

			public List<string> in_list { get; set; } = new List<string>();
		}

		/// <summary>
		/// ViewFilter对象
		/// </summary>
		public class ViewFilter
		{
			/// <summary>
			/// 可选值为 "all" 或 "public"，代表从全部数据还是只从该用户市局中筛选
			/// </summary>
			public string filter_from { get; set; } = "all";

			/// <summary>
			/// 筛选条件
			/// </summary>
			public Filter filter { get; set; } = new Filter();

		}

		/// <summary>
		/// 筛选条件对象
		/// </summary>
		public class Filter
		{

			/// <summary>
			/// 数字代表参数 expressions 中第几个表达式，从 1 开始计数，支持 AND/OR/括号，将数字替换为对应表达式即为最终表达式。如果只有一个表达式，填写 "1" 即可
			/// </summary>
			public string logical_relation { get; set; }

			/// <summary>
			/// 表达式列表，每个元素由display_name, field, operator, operands 4 个字段组成
			/// </summary>
			public List<Expressions> expressions { get; set; } = new List<Expressions>();
		}

		/// <summary>
		/// 表达式列表对象，每个元素由display_name, field, operator, operands 4 个字段组成
		/// </summary>
		public class Expressions
		{
			
			/// <summary>
			/// 该表达式的显示名
			/// </summary>
			public string display_name { get; set; }

			/// <summary>
			/// 所要筛选的字段
			/// </summary>
			public string field { get; set; }

			/// <summary>
			/// 操作符，支持的值见下表
			/// 操作符 值
			/// 大于，小于，大于等于，小于等于，等于，不等于   	">", "<", ">=", "<=", "==", "!="
			/// 起始字符 	"STARTSWITH"
			/// 包含	 "CONTAINS"
			/// 等于（范围） 	"INRANGE"
			/// </summary>
			public string @operator { get; set; }

			/// <summary>
			/// 操作数，支持的值见下表
			/// 类型	值
			/// 整型 	"42"
			/// 浮点型 	"3.14"
			/// 字符串 	"'hello world'"(注意：双引号之内需要加单引号表示是字符串)
			/// 日期时间 	"2017-05-10T10:26:58Z"
			/// </summary>
			public List<string> operands { get; set; } = new List<string>();

		}
	}

	/// <summary>
	/// 获取token数据的参数
	/// </summary>
	public class TokenQueryParam
	{
		/// <summary>
		/// 为固定字段 【AQACQqweMIgBAAAAF8jlWJSoBWPpxUA】，在正式上线时会变化
		/// </summary>
		public string tenant_id { get; set; } = "AQACQqweMIgBAAAAF8jlWJSoBWPpxUA";

		/// <summary>
		/// 用户id
		/// </summary>
		public string user_id { get; set; }

		/// <summary>
		/// 客户端类型
		/// </summary>
		public string device { get; set; } = "web";

		/// <summary>
		/// 
		/// </summary>
		public string app { get; set; } = "crm";
	}

	public class VeinmatchPostParam
	{
		public List<string> regfeature { get; set; } = new List<string>(3);

	}


}
