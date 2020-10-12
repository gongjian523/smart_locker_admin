using System.Collections.Generic;

namespace CFLMedCab.Http.Model.param
{
	/// <summary>
	/// 通用用于put的参数
	/// </summary>
	public class PutParam<T>
	{
		/// <summary>
		/// 所要更新的记录列表，元素类型为字典，每个元素对应一条记录
		/// </summary>
		public List<T> objects { get; set; } = new List<T>();
	}
}
