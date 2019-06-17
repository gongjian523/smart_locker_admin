using CFLMedCab.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.DTO.Surgery
{
	public class SurgeryOrderdtlDto: SurgeryOrderdtl
	{
		/// <summary>
		/// 库存数量
		/// </summary>
		public int stock_num { get; set; }
	}
}
