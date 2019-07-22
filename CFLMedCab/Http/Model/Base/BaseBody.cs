using System;
using System.Collections.Generic;

namespace CFLMedCab.Http.Model.Base
{
	/// <summary>
	/// HTTP查询基础对象的body
	/// </summary>
	/// <typeparam name="T">查询记录的对象类型</typeparam>
	public class BaseBody<T>
	{
		/// <summary>
		/// 查询到的记录列表
		/// </summary>
		public List<T> objects { get; set; }

		/// <summary>
		/// 全局偏移量，当前查询到的最后一条记录的偏移量+1，想要查询后面的记录，该值即为下次查询的 offset 参数
		/// </summary>
		public Int32 global_offset { get; set; }

	}
}
