using CFLMedCab.Http.Model.Base;
using CFLMedCab.Http.Model.Common;
using System;
namespace CFLMedCab.Http.Model
{
	/// <summary>
	/// 商品管理
	/// </summary>
	public class Commodity: BaseModel
	{
		/// <summary>
		/// 商品码
		/// </summary>
		public string CommodityCode { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Manufacturer { get; set; }
		/// <summary>
		/// 是
		/// </summary>
		public string MixedBatch { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public Price Price { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Remark { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Size { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Specifications { get; set; }
		/// <summary>
		/// 常温;阴凉
		/// </summary>
		public string StorageAttribute { get; set; }
		/// <summary>
		/// 定数包
		/// </summary>
		public string Type { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Volume { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Weight { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public Int32 auto_id { get; set; }
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
		public Int32 is_deleted { get; set; }
		/// <summary>
		/// 棉签10*10
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

	/// <summary>
	/// 价钱
	/// </summary>
	public class Price
	{
		/// <summary>
		/// 
		/// </summary>
		public string symbol { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string value { get; set; }
	}

}
