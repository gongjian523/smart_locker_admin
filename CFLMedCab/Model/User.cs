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
        public int id { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        public int role { get; set; }

        /// <summary>
        /// 指静脉数值图像
        /// </summary>
        public string vein_id { get; set; }
    }
}
