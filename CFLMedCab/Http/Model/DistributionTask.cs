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
	public class DistributionTask : BaseModel
	{

		/// <summary>
		/// 
		/// </summary>
		public string AllotId { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string EnterStoreHouse { get; set; }

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
        public string Operator { get; set; }

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
        public string PickTaskId { get; set; }

        /// <summary>
        /// 单据状态
        /// </summary>
        public string Status { get; set; }

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
