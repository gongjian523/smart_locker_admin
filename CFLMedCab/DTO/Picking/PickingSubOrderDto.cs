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
	}
}
