using CFLMedCab.Http.Model.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CFLMedCab.Http.Model
{

    /// <summary>
    /// 手术有单领用和医嘱处方领用页面转换的参数
    /// </summary>
    public class FetchParam
    {
        /// <summary>
        /// 领用单
        /// </summary>
        public BaseData<ConsumingOrder> bdConsumingOrder { get; set; }

        /// <summary>
        /// 手术领用单
        /// </summary>
        public BaseData<OperationOrderGoodsDetail> bdOperationOrderGoodsDetail { get; set; }

        /// <summary>
        /// 医嘱处方领单
        /// </summary>
        //public BaseData<PrescriptionOrderGoodsDetail> bdPrescriptionOrderGoodsDetail { get; set; }
    }
}