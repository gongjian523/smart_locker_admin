using CFLMedCab.DTO.Goodss;
using CFLMedCab.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.APO.Surgery
{
	public class SurgeryOrderApo: BasePageDataApo
	{
		/// <summary>
		/// 手术单号
		/// </summary>
		public string SurgeryOrderCode { set; get; }

		/// <summary>
		/// 柜子里库存商品集
		/// </summary>
		public List<GoodsDto> GoodsDtos { set; get; }

		/// <summary>
		/// 一次操作变化的商品集
		/// </summary>
		public List<GoodsDto> OperateGoodsDtos { set; get; }

	}
}
