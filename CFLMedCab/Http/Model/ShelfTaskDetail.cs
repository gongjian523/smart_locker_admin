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
	public class ShelfTaskDetail : BaseModel
	{
		/// <summary>
		/// 
		/// </summary>
		public int Commodity { get; set; }
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
		/// 厂商id，从系统中过来
		/// </summary>
		public string ManufactorName { get; set; }

		/// <summary>
		///  厂商，通过Model查询
		/// </summary>
		[JsonIgnore]
		public string ManufactorNameStr { get; set; }

		/// <summary>
		/// 型号id，从系统中过来
		/// </summary>
		public string Model { get; set; }

		/// <summary>
		/// 型号，通过Model查询
		/// </summary>
		[JsonIgnore]
		public string ModelStr { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public int Number { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int ShelfNumber { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ShelfTaskId { get; set; }

        /// <summary>
        /// 规格id，从系统中过来
        /// </summary>
        public string Spec { get; set; }

		/// <summary>
		/// 规格，通过Spec查询
		/// </summary>
		[JsonIgnore]
		public string SpecStr { get; set; }

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
        //[JsonIgnore]
        //public int PlanShelfNumber { get; set; }

		/// <summary>
		/// 上架数量计数
		/// </summary>
		[JsonIgnore]
		public int CountShelfNumber { get; set; } = 0;

	}
}
