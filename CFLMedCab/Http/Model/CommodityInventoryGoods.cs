using CFLMedCab.Http.Model.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Http.Model
{
    /// <summary>
    /// 商品库存货品明细
    /// </summary>
    public class CommodityInventoryGoods : BaseModel
    {
        /// <summary>
        /// 采购批次
        /// </summary>
        public string BatchId { get; set; }
        /// <summary>
        /// 生产批号
        /// </summary>
        public string BatchNumberId { get; set; }
        /// <summary>
        /// 关联商品库存
        /// </summary>
        public string CommodityInventoryDetailId { get; set; }
        /// <summary>
        /// 货品库存编号
        /// </summary>
        public string GoodsInventoryDetailId { get; set; }
        /// <summary>
        /// 货品数量 
        /// </summary>
        public string GoodsNumber { get; set; }
        /// <summary>
        /// 货品名称
        /// </summary>
        public string HospitalGoodsId { get; set; }
        /// <summary>
        /// 灭菌批号
        /// </summary>
        public string SterilizationBatchId { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }
    }
}
