using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Http.Model.param
{
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

}
