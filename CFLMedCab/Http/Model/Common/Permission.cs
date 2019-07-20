using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Http.Model.Common
{
	/// <summary>
	/// 公共部分权限
	/// </summary>
	public class Permission
	{
		/// <summary>
		/// 
		/// </summary>
		public string deletable { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string transferable { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string updatable { get; set; }
	}
}
