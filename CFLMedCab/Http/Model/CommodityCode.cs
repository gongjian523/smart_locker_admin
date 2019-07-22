using CFLMedCab.Http.Model.Base;
using CFLMedCab.Http.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Http.Model
{
	/// <summary>
	/// 商品码管理
	/// </summary>
	public class CommodityCode:BaseModel
	{
		/// <summary>
		/// 
		/// </summary>
		public string CommodityId { get; set; }

		/// <summary>
		/// 商品名称（一类）
		/// </summary>
		public string CommodityName { get; set; }

		/// <summary>
		/// 可使用
		/// </summary>
		public string Status { get; set; }

		/// <summary>
		/// RF码
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

		/// <summary>
		/// 操作类型 0 出库 1 入库
		/// </summary>
		public int operate_type { get; set; }

	}
}
