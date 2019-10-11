using CFLMedCab.Http.Model;
using CFLMedCab.Http.Model.Base;
using CFLMedCab.Http.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CFLMedCab.Http.ExceptionApi.ExStepHandle;

namespace CFLMedCab.Http.ExceptionApi
{
	/// <summary>
	/// 消息实体
	/// </summary>
	[Serializable]
	public class ExEntity
	{
		/// <summary>
		/// 未请求初始化数据的商品或已过滤的商品
		/// </summary>
		public List<CommodityCode> commodityCodeList { get; set; }

		/// <summary>
		/// 已经创建的订单
		/// </summary>
		public ConsumingOrder consumingOrder { get; set; }


		/// <summary>
		/// 已初始化的数据
		/// </summary>
		public BaseData<CommodityCode> bdcommodityCode { get; set; }

		/// <summary>
		/// 库存变化需要提供的数据
		/// </summary>
		public List<CommodityInventoryChange> commodityInventoryChangeList { get; set; }

		/// <summary>
		/// 领用类型
		/// </summary>
		public ConsumingOrderType consumingOrderType { get; set; }

		/// <summary>
		/// 业务类型
		/// </summary>
		public SourceBill sourceBill { get; set; }


		/// <summary>
		/// api异步提交业务类型
		/// </summary>
		public BusinessType businessType { get; set; }


		/// <summary>
		/// api异步提交业务步骤
		/// </summary>
		public BusinessStep businessStep { get; set; }


	}
}
