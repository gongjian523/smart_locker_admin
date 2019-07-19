using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Http.Model
{
	/// <summary>
	/// 上架任务商品明细
	/// </summary>
	public class ShelfTaskCommodityDetail
	{
		/// <summary>
		/// id
		/// </summary>
		public string id { get; set; }

		/// <summary>
		/// 上架任务单id
		/// </summary>
		public string ShelfTaskId { get; set; }

		/// <summary>
		/// 商品
		/// </summary>
		public string Commodity { get; set; }

		/// <summary>
		/// 已上架数量
		/// </summary>
		public string AlreadyShelfNumber { get; set; }

		/// <summary>
		/// 待上架数量
		/// </summary>
		public string NeedShelfNumber { get; set; }

	}
}
