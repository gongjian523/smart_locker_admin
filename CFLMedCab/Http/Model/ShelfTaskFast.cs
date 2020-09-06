using CFLMedCab.Http.Model.Base;
using CFLMedCab.Http.Model.Common;
using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace CFLMedCab.Http.Model
{
	/// <summary>
	/// 上架任务（工单）
	/// </summary>
	[JsonObject(MemberSerialization.OptOut)]
	public class ShelfTaskFast : BaseModel
	{
		/// <summary>
		/// 单据状态
		/// </summary>
		public string Status { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string AbnormalCauses { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string AbnormalDescribe { get; set; }
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
		public SourceBill SourceBill { get; set; }

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
		public int NeedShelfTotalNumber { get; set; }

        /// <summary>
        /// 货柜号
        /// </summary>
        [JsonIgnore]
        public string GoodLocationName{ get; set; }

    }

	/// <summary>
	/// 便捷上货任务状态
	/// </summary>
	public enum ShelfTaskFastStatusEnum
	{
		/// <summary>
		/// 待上架
		/// </summary>
		[Description("待上架")]
		待上架 = 0,

		/// <summary>
		/// 已完成
		/// </summary>
		[Description("已完成")]
		已完成 = 1,

		///// <summary>
		///// 进行中
		///// </summary>
		//[Description("进行中")]
		//进行中 = 2,

		/// <summary>
		/// 异常
		/// </summary>
		[Description("异常")]
		异常 = 3,

		///// <summary>
		///// 已撤销
		///// </summary>
		//[Description("已撤销")]
		//已撤销 = 4,
	}
}
