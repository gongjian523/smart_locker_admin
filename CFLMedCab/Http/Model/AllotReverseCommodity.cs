using CFLMedCab.Http.Model.Base;
using CFLMedCab.Http.Model.Common;
using Newtonsoft.Json;
using System;

namespace CFLMedCab.Http.Model
{
	/// <summary>
	/// 
	/// </summary>
	[JsonObject(MemberSerialization.OptOut)]
	public class AllotReverseCommodity : BaseModel
	{
        /// <summary>
        /// 
        /// </summary>
        public string AllotReverse { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Commodity { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public string CommodityName { get; set; }

        /// <summary>
        /// 商品id
        /// </summary>
        public string CommodityId { get; set; }

        /// <summary>
        /// Manufactor id
        /// </summary>
        public string ManufactorName { get; set; }

        /// <summary>
        /// Manufactor 字符串
        /// </summary>
        [JsonIgnore]
        public string ManufactorNameStr { get; set; }


        /// <summary>
        /// Model id
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// Manufactor id
        /// </summary>
        [JsonIgnore]
        public string ModelStr { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// 本次实际拣货数量
        /// </summary>
        [JsonIgnore]
        public int PickNumber { get; set; } = 0;


        /// <summary>
        /// 拣货数量计数器
        /// </summary>
        [JsonIgnore]
        public int PickCount { get; set; } = 0;

        /// <summary>
        /// Spec id
        /// </summary>
        public string Spec { get; set; }

        /// <summary>
        /// Spec 字符串
        /// </summary>
        [JsonIgnore]
        public string SpecStr { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public object auto_id { get; set; }

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
        public object is_deleted { get; set; }

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
