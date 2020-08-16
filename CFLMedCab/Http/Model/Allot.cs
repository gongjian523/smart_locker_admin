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
	public class Allot : BaseModel
	{

		/// <summary>
		/// 
		/// </summary>
		public string ApprovalStatus { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string FinishDate { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string InDepartment { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string InStoreHouse { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string OutDepartment { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string OutStoreHouse { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Remarks { get; set; }

        /// <summary>
        /// 单据状态
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Type { get; set; }

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

        /// <summary>
        /// 货柜号
        /// </summary>
        [JsonIgnore]
        public string GoodLocationName{ get; set; }

    }
}
