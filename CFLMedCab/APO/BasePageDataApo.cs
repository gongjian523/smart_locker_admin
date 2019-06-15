using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.APO
{
	public class BasePageDataApo
	{
		/// <summary>
		/// 当前页
		/// </summary>
		public int PageIndex { set; get; }
		/// <summary>
		/// 当前大小
		/// </summary>
		public int PageSize { set; get; }
	}
}
