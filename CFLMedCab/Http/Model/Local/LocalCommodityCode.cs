using CFLMedCab.Http.Model.Base;
using CFLMedCab.Http.Model.Common;
using Newtonsoft.Json;
using SqlSugar;
using System;

namespace CFLMedCab.Http.Model
{
	/// <summary>
	/// 商品码管理
	/// </summary>
	public class LocalCommodityCode
	{

		/// <summary>
		/// id
		/// </summary>
		[SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
		public int id { get; set; }

		/// <summary>
		/// name
		/// </summary>
		public string name { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string CommodityId { get; set; }

		/// <summary>
		/// 可使用
		/// </summary>
		[SugarColumn(IsNullable = true)]
		public string Status { get; set; }

		/// <summary>
		/// RF码
		/// </summary>
		[SugarColumn(IsNullable = true)]
		public string Type { get; set; }

		/// <summary>
		/// 设备id
		/// </summary>
		[SugarColumn(IsNullable = true)]
		public string EquipmentId { get; set; }

		/// <summary>
		/// 设备name
		/// </summary>
		[SugarColumn(IsNullable = true)]
		public string EquipmentName { get; set; }

		/// <summary>
		/// 货位id
		/// </summary>
		[SugarColumn(IsNullable = true)]
		public string GoodsLocationId { get; set; }

		/// <summary>
		/// 货位name
		/// </summary>
		[SugarColumn(IsNullable = true)]
		public string GoodsLocationName { get; set; }

		/// <summary>
		/// 操作类型 0 出库 1 入库
		/// </summary>
		[SugarColumn(IsNullable = true)]
		public int operate_type { get; set; }

		/// <summary>
		/// 商品名称（一类）
		/// </summary>
		[SugarColumn(IsNullable = true)]
		public string CommodityName { get; set; }

		/// <summary>
		/// 业务名称
		/// </summary>
		public string sourceBill { get; set; }

		/// <summary>
		/// 操作时间
		/// </summary>
		[SugarColumn(IsNullable = true)]
		public DateTime create_time { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string operater { get; set; }

		/// <summary>
		/// 厂家名称
		/// </summary>
		[SugarColumn(IsNullable = true)]
		public string ManufactorName { get; set; }

		/// <summary>
		/// 新系统下的型号
		/// </summary>
		[SugarColumn(IsNullable = true)]
		public string Model { get; set; }

		/// <summary>
		/// 新系统下的规格
		/// </summary>
		[SugarColumn(IsNullable = true)]
		public string Spec { get; set; }

	}
}
