using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Model
{
    public class LoginInfo
    {
		/// <summary>
		/// 登录ID
		/// </summary>
		[SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
		public int id { get; set; }

		/// <summary>
		/// 姓名
		/// </summary>
		public string name { get; set; }

		/// <summary>
		/// 用户名,手机号码，带有+86
		/// </summary>
		[SugarColumn(IsNullable = true)]
		public string username { get; set; }

		/// <summary>
		/// 科室
		/// </summary>
		[SugarColumn(IsNullable = true)]
		public string department { get; set; }

		/// <summary>
		/// 登录时间
		/// </summary>
		public DateTime logintime { get; set; }


		/// <summary>
		/// 登出时间
		/// </summary>
		public DateTime logouttime { get; set; }

		/// <summary>
		/// 退出类型
		/// </summary>
		[SugarColumn(IsNullable = true)]
		public string logoutInfo  { get; set; }

	}
}
