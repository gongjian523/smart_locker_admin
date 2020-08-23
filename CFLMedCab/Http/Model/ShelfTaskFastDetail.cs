using CFLMedCab.Http.Model.Base;
using CFLMedCab.Http.Model.Common;
using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace CFLMedCab.Http.Model
{
	/// <summary>
	/// 上架任务商品明细
	/// </summary>
	[JsonObject(MemberSerialization.OptOut)]
	public class ShelfTaskFastDetail : BaseModel
	{
		/// <summary>
		/// 商品码
		/// </summary>
		public string CommodityCodeId { get; set; }

		/// <summary>
		/// 商品id
		/// </summary>
		public string CommodityId { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string EquipmentId { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string GoodsLocationId { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int GoodsNumber { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string ManufactorName { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[JsonIgnore]
		public string ManufactorName1 { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string Model { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[JsonIgnore]
		public string Model1 { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string ShelfTaskFastId { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string Spec { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[JsonIgnore]
		public string Spec1 { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string Status { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string StoreHouseId { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string created_at { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string created_by { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string owner { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public Permission permission { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string record_type { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string system_mod_stamp { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string updated_at { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string updated_by { get; set; }

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

		/// <summary>
		/// 货位名
		/// </summary>
		[JsonIgnore]
		public string GoodsLocationName { get; set; }

		/// <summary>
		/// 商品名
		/// </summary>
		[JsonIgnore]
		public string CommodityName { get; set; }


        /// <summary>
        /// 本次上架数量
        /// </summary>
        [JsonIgnore]
        public int CurShelfNumber { get; set; }

        /// <summary>
        /// 本次应上架数量
        /// </summary>
        [JsonIgnore]
        public int PlanShelfNumber { get; set; }

		/// <summary>
		/// 上架数量计数
		/// </summary>
		[JsonIgnore]
		public int CountShelfNumber { get; set; } = 0;

	}


	/// <summary>
	/// 质量状态
	/// </summary>
	public enum ShelfTaskFastDetailStatusType
	{
		/// <summary>
		/// 待上架
		/// </summary>
		[Description("待上架")]
		待上架 = 0,

		/// <summary>
		/// 已上架
		/// </summary>
		[Description("已上架")]
		已上架 = 1,
	}

}
