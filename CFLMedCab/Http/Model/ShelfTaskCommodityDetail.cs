using CFLMedCab.Http.Model.Base;
using Newtonsoft.Json;
using System;

namespace CFLMedCab.Http.Model
{
	/// <summary>
	/// 上架任务商品明细
	/// </summary>
	[JsonObject(MemberSerialization.OptOut)]
	public class ShelfTaskCommodityDetail : BaseModel
	{
		/// <summary>
		/// 上架任务单id
		/// </summary>
		public string ShelfTaskId { get; set; }

		/// <summary>
		/// 商品
		/// </summary>
		public string Commodity { get; set; }

		/// <summary>
		/// 已上架数量
		/// </summary>
		public Int32 AlreadyShelfNumber { get; set; }

		/// <summary>
		/// 待上架数量
		/// </summary>
		public Int32 NeedShelfNumber { get; set; }

		/// <summary>
		/// 商品类型
		/// </summary>
		public string Type { get; set; }

		/// <summary>
		/// 设备
		/// </summary>
		public string EquipmentId { get; set; }

		/// <summary>
		/// 上架库房
		/// </summary>
		public string StoreHouseId { get; set; }

		/// <summary>
		/// 设备名
		/// </summary>
		[JsonIgnore]
		public string EquipmentName { get; set; }

		/// <summary>
		/// 库房名
		/// </summary>
		[JsonIgnore]
		public string StoreHouseName { get; set; }


	}
}
