using CFLMedCab.Http.Model.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Http.Model
{
    /// <summary>
    /// 医院货品管理详情
    /// </summary>
    public class HospitalGoods: BaseModel
    {
        /// <summary>
        /// 别名
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// 货品编码
        /// </summary>
        public string GoodsCode { get; set; }

        /// <summary>
        /// 证照状态
        /// </summary>
        public string LicenceStatus { get; set; }

        /// <summary>
        /// 厂家名称
        /// </summary>
        public string ManufactorName { get; set; }

        /// <summary>
        /// 有效期预警天数
        /// </summary>
        public string ValidWarningDays { get; set; }
    }
}
