using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Model.Enum
{
	public class GoodsChageAttribute
	{
		/// <summary>
		/// 领用类型:领用单关联的库存变化单的的业务类型
		/// </summary>
		public RequisitionType RequisitionType { get; set; }

		/// <summary>
		/// 领用状态 1。状态：待认领；已认领。2。一般领用，有单手术领用为已认领；无单手术领用为待认领，后台管理员在将其变为已认领状态时效录入业务单号。
		/// </summary>
		public RequisitionStatus RequisitionStatus { get; set; }

		/// <summary>
		/// 耗材状态 1。状态：已领用；已回退。2。领用单生成后默认为已领用，关联的回退单生成后变成已回退。
		/// </summary>
		public ConsumablesStatus ConsumablesStatus { get; set; }
	}
}
