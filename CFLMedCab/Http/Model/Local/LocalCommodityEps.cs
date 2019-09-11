using SqlSugar;
using System;
using System.Collections.Generic;

namespace CFLMedCab.Http.Model
{
	//用于映射商品扫描后的参数
	public class LocalCommodityEps
	{
		/// <summary>
		/// id
		/// </summary>
		[SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
		public int id { get; set; }

		/// <summary>
		/// 当前扫描的集合
		/// </summary>
		[SugarColumn(IsNullable = true, Length = 65535)]
		public string commodityEpsList { get; set; }

		/// <summary>
		/// 创建时间
		/// </summary>
	
		public DateTime create_time { get; set; }


	}
}
