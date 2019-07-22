using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Http.Model
{
	//用于映射商品扫描后的参数
	public class CommodityEps
	{
		/// <summary>
		/// 商品名称
		/// </summary>
       	public string CommodityName { get; set; }

		/// <summary>
		/// 商品码id
		/// </summary>
		public string CommodityCodeId { get; set; }

		/// <summary>
		/// 商品码
		/// </summary>
		public string CommodityCodeName { get; set; }

	}
}
