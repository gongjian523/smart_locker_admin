using CFLMedCab.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CFLMedCab.DTO.Goodss
{
	public class GoodsDto: Goods
	{


		/// <summary>
		/// 操作类型 操作类型：0 出库；1 入库 （用于判断一组商品里，是否是出库或入库，比如说对比开门和关门的后的商品）
		/// </summary>
		public int operate_type { get; set; }
	}
}