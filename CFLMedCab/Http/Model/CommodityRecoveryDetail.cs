using CFLMedCab.Http.Model.Base;
using CFLMedCab.Http.Model.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Http.Model
{
	/// <summary>
	/// 拣选商品管理理资料
	/// </summary>
	[JsonObject(MemberSerialization.OptOut)]
	public class CommodityRecoveryDetail : BaseModel
	{
		/// <summary>
		/// 
		/// </summary>
		[JsonIgnore]
		public string CommodityCodeName { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string CommodityCodeId { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string CommodityInventoryId { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string CommodityRecoveryId { get; set; }
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
		public string ManufactorName { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Model { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Specifications { get; set; }
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
    }
}
