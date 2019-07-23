using CFLMedCab.Http.Model.Base;
using CFLMedCab.Http.Model.Common;
using System;

namespace CFLMedCab.Http.Model
{
	/// <summary>
	/// 库房
	/// </summary>
	public class StoreHouse: BaseModel
	{
		/// <summary>
		/// 
		/// </summary>
		public Address Address { get; set; }
		/// <summary>
		/// 医疗临床
		/// </summary>
		public string Department { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Remarks { get; set; }
		/// <summary>
		/// 启用
		/// </summary>
		public string Status { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string StoreHouseCode { get; set; }
		/// <summary>
		/// 科室库
		/// </summary>
		public string Type { get; set; }

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

	public class Address
	{
		/// <summary>
		/// 东城区
		/// </summary>
		public string city { get; set; }
		/// <summary>
		/// 中国
		/// </summary>
		public string country { get; set; }
		/// <summary>
		/// 北京市
		/// </summary>
		public string state { get; set; }
		/// <summary>
		/// 水电费水电费
		/// </summary>
		public string street { get; set; }
	}

}
