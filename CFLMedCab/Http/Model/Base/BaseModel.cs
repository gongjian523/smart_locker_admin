using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Http.Model.Base
{
	/// <summary>
	/// 实体通用属性
	/// </summary>
	public class BaseModel
	{
		/// <summary>
		/// id
		/// </summary>
		public string id { get; set; }

		/// <summary>
		/// name
		/// </summary>
		public string name { get; set; }

		/// <summary>
		/// 版本
		/// </summary>
		public string version { get; set; }
	}
}
