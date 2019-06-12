using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Model
{
    /// <summary>
    /// 手术单
    /// </summary>
    public class SurgeryOrder
    {
        /// <summary>
        /// 手术单编号
        /// </summary> 
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int id { get; set; }

        /// <summary>
        /// 手术时间
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public DateTime surgery_dateiime { get; set; }

    }
}
