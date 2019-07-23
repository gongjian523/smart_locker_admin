using CFLMedCab.Http.Model.Base;
using CFLMedCab.Http.Model.Common;
using System;

namespace CFLMedCab.Http.Model
{
	/// <summary>
	/// 设备表
	/// </summary>
	public class Equipment: BaseModel
	{
		/// <summary>
		/// 正常
		/// </summary>
		public string EquipmentStatus { get; set; }
		/// <summary>
		/// 智能柜
		/// </summary>
		public string EquipmentType { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string InventoryPlanId { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Remarks { get; set; }

		/// <summary>
		/// 启用
		/// </summary>
		public string Status { get; set; }
		/// <summary>
		/// 常温;阴凉
		/// </summary>
		public string StorageConditions { get; set; }
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
		public string place { get; set; }
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
	}
}
