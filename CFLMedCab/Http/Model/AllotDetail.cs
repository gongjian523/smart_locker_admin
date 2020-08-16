using CFLMedCab.Http.Model.Base;
using CFLMedCab.Http.Model.Common;
using Newtonsoft.Json;
using System;

namespace CFLMedCab.Http.Model
{
	/// <summary>
	/// 上架任务商品明细
	/// </summary>
	[JsonObject(MemberSerialization.OptOut)]
	public class AllotDetail : BaseModel
	{
        /// <summary>
        /// 
        /// </summary>
        public string AllotId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Commodity { get; set; }

		/// <summary>
		/// 商品id
		/// </summary>
		public string CommodityId { get; set; }

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
        public string Model { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Number { get; set; }


		/// <summary>
		/// 
		/// </summary>
		public string Spec { get; set; }

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
