using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Model
{
    public class InOutRecord
    {
		/// <summary>
		/// ID
		/// </summary>
		[SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
		public int id { get; set; }

		/// <summary>
		/// 登录ID
		/// </summary>
		public int login_id { get; set; }


		/// <summary>
		/// 操作时间
		/// </summary
		public DateTime create_time { get; set; }

		/// <summary>
		/// 操作类型
		/// </summary>
		public string operate { get; set; }


		/// <summary>
		/// 操作人
		/// </summary>
		public string user_name { get; set; }


		/// <summary>
		/// 操作人
		/// </summary>
		public string department { get; set; }

	}
}
