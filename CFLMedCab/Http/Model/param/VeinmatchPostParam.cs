using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Http.Model.param
{
	/// <summary>
	/// 指静脉绑定,参数
	/// </summary>
	public class VeinmatchPostParam
	{
		/// <summary>
		///  3 个指静脉特征 
		/// </summary>
		public List<string> regfeature { get; set; } = new List<string>(3);

		/// <summary>
		/// 手指名， 非必需，默认"finger_1" 
		/// </summary>
		public string finger_name { get; set; }

	}
}
