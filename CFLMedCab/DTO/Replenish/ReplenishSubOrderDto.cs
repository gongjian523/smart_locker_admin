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

		/// <summary>
		/// 派发时间，来源总工单创建时间
		/// </summary>
		public DateTime distribute_time { get; set; }

	}
}
