using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.DTO
{
	public class BasePageDataDto<T>
	{
		/// <summary>
		/// 当前页
		/// </summary>
		public int PageIndex { set; get; }
		/// <summary>
		/// 当前大小
		/// </summary>
		public int PageSize { set; get; }

		/// <summary>
		/// 总数量
		/// </summary>
		public int TotalCount { set; get; }

		/// <summary>
		/// 查询的数据
		/// </summary>
		public List<T> Data { set; get; }

	}
}
