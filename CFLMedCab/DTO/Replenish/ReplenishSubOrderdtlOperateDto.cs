using CFLMedCab.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.DTO.Replenish
{
	public class ReplenishSubOrderdtlOperateDto : ReplenishSubOrderdtl
	{
		/// <summary>
		/// 操作类型 0 出库；1 入库 
		/// </summary>
		public int operate_type { get; set; }

		/// <summary>
		/// 操作类型说明
		/// </summary>
		public string operate_type_description { get; set; }

		/// <summary>
		/// 异常标识
		/// </summary>
		public string exception_flag { get; set; }

		/// <summary>
		/// 异常说明
		/// </summary>
		public string exception_description { get; set; }




	}

	/// <summary>
	/// 操作类型
	/// </summary>
	public enum OperateType
	{
		/// <summary>
		/// 出库
		/// </summary>
		[Description("出库")]
		出库 = 0,

		/// <summary>
		/// 入库
		/// </summary>
		[Description("入库")]
		入库 = 1

	}

	/// <summary>
	/// 异常标识
	/// </summary>
	public enum ExceptionFlag
	{
		/// <summary>
		/// 正常标识，不显示，所以为空字符串
		/// </summary>
		[Description("")]
		正常 = 1,

		/// <summary>
		/// 异常标识
		/// </summary>
		[Description("异常")]
		异常 = 2

	}

	/// <summary>
	/// 异常说明
	/// </summary>
	public enum ExceptionDescription
	{
		/// <summary>
		/// 操作与业务类型冲突
		/// </summary>
		[Description("操作与业务类型冲突")]
		操作与业务类型冲突 = 1,

		/// <summary>
		/// 上架商品不在工单目录
		/// </summary>
		[Description("上架商品不在工单目录")]
		上架商品不在工单目录 = 2

	}

}
