using System;

namespace CFLMedCab.Http.Model.Base
{
	/// <summary>
	/// HTTP查询基础对象
	/// </summary>
	public class BaseData<T>
	{
		/// <summary>
		/// 响应状态码, 0 代表成功，其他值含义参考错误码文档
		/// </summary>
		public int code { get; set; }

		/// <summary>
		/// 返回提示消息
		/// </summary>
		public string message { get; set; }

		/// <summary>
		/// 描述
		/// </summary>
		public string description { get; set; }

		/// <summary>
		/// body下有objects 和 global_offset 2 个字段
		/// </summary>
		public BaseBody<T> body { get; set; }

	}
}
