using CFLMedCab.Http.Model.Base;
using CFLMedCab.Http.Model.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Http.Model
{
	public class CommodityInventoryChange: BaseModel
	{
        public string CommodityCodeId { get; set; }
        /// <summary>
        /// 商品码
        /// </summary>
        public string CommodityCode { get; set; }

		/// <summary>
		/// 来源单据
		/// </summary>
		public SourceBill SourceBill { get; set; }

        /// <summary>
        /// 变更后设备
        /// </summary>
		public string EquipmentId { get; set; }

        /// <summary>
        /// 变更后货位
        /// </summary>
        public string GoodsLocationId { get; set; }
        
        /// <summary>
        /// 变更库房
        /// </summary>
        public string StoreHouseId { get; set; }
        /// <summary>
        /// 变更后状态
        /// </summary>
        public string ChangeStatus { get; set; }

    }

    /// <summary>
    /// 变更后状态[正常 未上架 待回收 配送在途 拣货作业 已消耗 已回收 损耗]
    /// </summary>
    public enum CommodityInventoryChangeStatus
    {
        /// <summary>
        /// 正常
        /// </summary>
        [Description("正常")]
        正常 = 0,

        /// <summary>
        /// 未上架
        /// </summary>
        [Description("未上架")]
        未上架 = 1,

        /// <summary>
        /// 待回收
        /// </summary>
        [Description("待回收")]
        待回收 = 2,

        /// <summary>
        /// 配送在途
        /// </summary>
        [Description("配送在途")]
        配送在途 = 3,

        /// <summary>
        /// 拣货作业
        /// </summary>
        [Description("拣货作业")]
        拣货作业 = 4,

        /// <summary>
        /// 已消耗
        /// </summary>
        [Description("已消耗")]
        已消耗 = 5,

        /// <summary>
        /// 已回收
        /// </summary>
        [Description("已回收")]
        已回收 = 6,

        /// <summary>
        /// 损耗
        /// </summary>
        [Description("损耗")]
        损耗 = 7
    }

}
