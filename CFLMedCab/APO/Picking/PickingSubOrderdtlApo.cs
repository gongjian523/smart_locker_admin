using CFLMedCab.APO;
using CFLMedCab.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.DTO.Picking
{
	public class PickingSubOrderdtlApo : BasePageDataApo
	{
		/// <summary>
		/// 拣货单id
		/// </summary>
		public int picking_sub_orderid { get; set; }

		/// <summary>
		/// 拣货单单号
		/// </summary>
		public string picking_order_code { get; set; }

	}
}
