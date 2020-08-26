using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Model
{
    public class InOutDetail
    {
		/// <summary>
		/// ID
		/// </summary>
		[SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
		public int id { get; set; }

		/// <summary>
		/// 出入库记录id
		/// </summary>
		public int in_out_id { get; set; }

		/// <summary>
		/// 登录id
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
		/// 
		/// </summary>
		[SugarColumn(IsNullable = true)]
		public string manufactor_name { get; set; }

		/// <summary>
		/// 新系统下的型号
		/// </summary>
		[SugarColumn(IsNullable = true)]
		public string model { get; set; }

		/// <summary>
		/// 目录名字
		/// </summary>
		[SugarColumn(IsNullable = true)]
		public string ctalogue_name { get; set; }

		/// <summary>
		/// 规格
		/// </summary>
		[SugarColumn(IsNullable = true)]
		public string specifications { get; set; }


		/// <summary>
		/// 货位
		/// </summary>
		[SugarColumn(IsNullable = true)]
		public string position { get; set; }


		/// <summary>
		/// 出入库
		/// </summary>
		[SugarColumn(IsNullable = true)]
		public string in_out { get; set; }
	} 
}
