using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Model.Enum
{
	class GoodsCommonEnum
	{
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
		正常 = 0,

		/// <summary>
		/// 异常标识
		/// </summary>
		[Description("异常")]
		异常 = 1

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
		/// 领用属性与业务类型冲突
		/// </summary>
		[Description("领用属性与业务类型冲突")]
		领用属性与业务类型冲突 = 2,

		/// <summary>
		/// 拣货商品不在工单目录
		/// </summary>
		[Description("拣货商品不在工单目录")]
		拣货商品不在工单目录 = 3,


		/// <summary>
		/// 上架商品不在工单目录
		/// </summary>
		[Description("上架商品不在工单目录")]
		上架商品不在工单目录 = 4
	}

	/// <summary>
	/// 领用类型
	/// </summary>
	public enum RequisitionType
	{
		/// <summary>
		/// 一般领用
		/// </summary>
		[Description("一般领用")]
		一般领用 = 1,


		/// <summary>
		/// 手术领用
		/// </summary>
		[Description("手术领用")]
		手术领用 = 2,

		/// <summary>
		/// 领用回退
		/// </summary>
		[Description("领用回退")]
		领用回退 = 3,
	}

	/// <summary>
	/// 领用状态
	/// </summary>
	public enum RequisitionStatus
	{
		/// <summary>
		/// 待认领
		/// </summary>
		[Description("待认领")]
		待认领 = 1,


		/// <summary>
		/// 已认领
		/// </summary>
		[Description("已认领")]
		已认领 = 2

	}

	/// <summary>
	/// 领用属性
	/// </summary>
	public enum RequisitionAttribute
	{
		/// <summary>
		/// 有单领用
		/// </summary>
		[Description("有单领用")]
		有单领用 = 1,


		/// <summary>
		/// 无单领用
		/// </summary>
		[Description("无单领用")]
		无单领用 = 2

	}

	/// <summary>
	/// 耗材状态
	/// </summary>
	public enum ConsumablesStatus
	{
		/// <summary>
		/// 已领用
		/// </summary>
		[Description("已领用")]
		已领用 = 1,


		/// <summary>
		/// 已退回
		/// </summary>
		[Description("已退回")]
		已退回 = 2

	}


}
