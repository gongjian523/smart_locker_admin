using CFLMedCab.Http.Model.Base;
using CFLMedCab.Http.Model.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Http.Model
{
	/// <summary>
	/// 拣选任务管理理
	/// </summary>
	[JsonObject(MemberSerialization.OptOut)]
	public class PickTask:BaseModel
	{
		/// <summary>
		/// 已完成
		/// </summary>
		public string BillStatus { get; set; }

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
		/// 该任务单总数
		/// </summary>
		[JsonIgnore]
		public int NeedPickTotalNumber { get; set; }

	}
}
