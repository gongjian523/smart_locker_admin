using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Http.Model.Common
{
    /// <summary>
    /// 公共来源信息部分
    /// </summary>
    public class SourceBill
    {
        /// <summary>
        /// 来源表主键
        /// </summary>
        public string object_id { get; set; }
        /// <summary>
        /// 来源表名称
        /// </summary>
        public string object_name { get; set; }
    }
}
