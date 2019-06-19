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
        [SugarColumn(IsNullable = true)]
        public string name { get; set; }

        /// <summary>
        /// 角色  
        /// 0 医生；1 护士；2 医院管理员；3 SPD交收员；4 SPD结算员；5 SPD经理
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int role { get; set; }

        /// <summary>
        /// 指静脉数值图像
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int vein_id { get; set; }
    }
}
