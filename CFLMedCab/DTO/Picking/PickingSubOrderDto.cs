using CFLMedCab.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.DTO.Picking
{
	public class PickingSubOrderDto : PickingSubOrder
	{

		/// <summary>
		/// 拣货商品数量
		/// </summary>
		public int picked_goods_num { get; set; }

		/// <summary>
		/// 派发时间，来源总工单创建时间
		/// </summary>
		public DateTime distribute_time;

	}
}
