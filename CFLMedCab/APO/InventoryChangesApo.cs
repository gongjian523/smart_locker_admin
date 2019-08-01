using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.APO.Inventory
{
	/// <summary>
	/// 新版库存变化的apo类
	/// </summary>
	public class InventoryChangesApo: BasePageDataApo
	{
		/// <summary>
		/// 操作类型 操作类型：0 出库；1 入库
		/// </summary>
		public int operate_type { get; set; }

		/// <summary>
		/// 起始时间
		/// </summary>
		public DateTime? startTime { get; set; }

		/// <summary>
		/// 结束时间
		/// </summary>
		public DateTime? endTime { get; set; }

		/// <summary>
		/// 商品名称
		/// </summary>
		public string name { get; set; }
	}
}
