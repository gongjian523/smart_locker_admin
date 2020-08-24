using CFLMedCab.Http.Model.Base;
using CFLMedCab.Http.Model.Common;
using Newtonsoft.Json;
using System;

namespace CFLMedCab.Http.Model
{
	/// <summary>
	/// 调拨
	/// </summary>
	[JsonObject(MemberSerialization.OptOut)]
	public class AllotAcceptance : BaseModel
	{

		/// <summary>
		/// 
		/// </summary>
		public string AllotId { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string DistributionTaskId { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string InDepartment { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string OutStoreHouseId { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string StoreHouseId { get; set; }

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
