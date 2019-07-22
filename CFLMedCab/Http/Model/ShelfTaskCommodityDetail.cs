using CFLMedCab.Http.Model.Base;

namespace CFLMedCab.Http.Model
{
	/// <summary>
	/// 上架任务商品明细
	/// </summary>
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
		public int AlreadyShelfNumber { get; set; }

		/// <summary>
		/// 待上架数量
		/// </summary>
		public int NeedShelfNumber { get; set; }

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




	}
}
