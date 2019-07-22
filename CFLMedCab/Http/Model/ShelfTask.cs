﻿using CFLMedCab.Http.Model.Base;
using CFLMedCab.Http.Model.Common;

namespace CFLMedCab.Http.Model
{
	/// <summary>
	/// 上架任务（工单）
	/// </summary>
	public class ShelfTask : BaseModel
	{
		/// <summary>
		/// 单据状态
		/// </summary>
		public string Status { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string AbnormalCauses { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string AbnormalDescribe { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string FinishDate { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string @Operator { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int auto_id { get; set; }
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
		public string id { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int is_deleted { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string name { get; set; }
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

	}
}
