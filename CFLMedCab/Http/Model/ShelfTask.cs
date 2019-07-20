using CFLMedCab.Http.Model.Base;

namespace CFLMedCab.Http.Model
{
	/// <summary>
	/// 上架任务（工单）
	/// </summary>
	public class ShelfTask : BaseModel
	{
		
		/// <summary>
		/// 单据状态
		/// </summary>
		public string Status { get; set; }

	}
}
