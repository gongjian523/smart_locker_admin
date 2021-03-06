﻿using CFLMedCab.APO;
using CFLMedCab.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.DTO.Replenish
{
	public class ReplenishSubOrderdtlApo : BasePageDataApo
	{
		/// <summary>
		/// 上架单id
		/// </summary>
		public int replenish_sub_orderid { get; set; }

		/// <summary>
		/// 上架单单号
		/// </summary>
		public string replenish_order_code { get; set; }

	}
}
