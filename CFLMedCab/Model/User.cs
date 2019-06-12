using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Model
{
    /// <summary>
    /// 医疗柜使用者人员信息
    /// </summary>
    public class User
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int id { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDataType = "Nvarchar(255)")]
        public string name { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int role { get; set; }

        /// <summary>
        /// 指静脉数值图像
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDataType = "Varchar(255)")]
        public string vein_id { get; set; }
    }
}
