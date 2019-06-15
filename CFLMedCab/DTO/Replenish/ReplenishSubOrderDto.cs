using CFLMedCab.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.DTO.Replenish
{
	public class ReplenishSubOrderDto
	{
		/// <summary>
		/// 上架单id
		/// </summary>
		public int id { get; set; }

		/// <summary>
		/// 上架单编号
		/// </summary>
		public string code { get; set; }

		/// <summary>
		/// 上架工单号
		/// </summary>
		public string replenish_order_code { get; set; }

		/// <summary>
		/// 生成时间
		/// </summary>
		public DateTime create_time { get; set; }

		/// <summary>
		/// 完成时间
		/// </summary>
		public DateTime end_time { get; set; }

		/// <summary>
		/// 货位
		/// </summary>
		public string position { get; set; }

		/// <summary>
		/// 状态
		/// </summary>
		public int status { get; set; }

		/// <summary>
		/// 验货单
		/// </summary>
		public int Inspection_order_id { get; set; }
		/// <summary>
		/// 待上架商品数量
		/// </summary>
		public int not_picked_goods_num { get; set; }
	}
}
