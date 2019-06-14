﻿using CFLMedCab.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.DTO.Replenish
{
	public class ReplenishSubOrderDto: ReplenishSubOrder
	{

		/// <summary>
		/// 待上架商品数量
		/// </summary>
		public int not_picked_goods_num { get; set; }
	}
}
