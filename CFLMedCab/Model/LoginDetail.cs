using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Model
{
    public class LoginDetail
    {
		/// <summary>
		/// ID
		/// </summary>
		[SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
		public int id { get; set; }

		/// <summary>
		/// 登录ID
		/// </summary>
		public string loginId { get; set; }

		/// <summary>
		/// 登录时间
		/// </summary>
		public DateTime logintime { get; set; }

		/// <summary>
		/// 操作
		/// </summary>
		public string operate { get; set; }


		/// <summary>
		/// 目录名字
		/// </summary>
		[SugarColumn(IsNullable = true)]
		public string CatalogueName { get; set; }


		/// <summary>
		/// 规格
		/// </summary>
		[SugarColumn(IsNullable = true)]
		public string Specifications { get; set; }


		/// <summary>
		/// 货位
		/// </summary>
		[SugarColumn(IsNullable = true)]
		public string position { get; set; }


		/// <summary>
		/// 出入库
		/// </summary>
		[SugarColumn(IsNullable = true)]
		public string InOrOut { get; set; }

	}
}
