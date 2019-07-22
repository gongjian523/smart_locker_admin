using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Http.Model
{
	//用于映射商品扫描后的参数
	public class CommodityEps
	{
		/// <summary>
		/// 商品名称
		/// </summary>
       	public string CommodityName { get; set; }

		/// <summary>
		/// 商品码id
		/// </summary>
		public string CommodityCodeId { get; set; }

		/// <summary>
		/// 商品码
		/// </summary>
		public string CommodityCodeName { get; set; }

		/// <summary>
		/// 设备id
		/// </summary>
		public string EquipmentId { get; set; }

		/// <summary>
		/// 设备name
		/// </summary>
		public string EquipmentName { get; set; }

		/// <summary>
		/// 货位id
		/// </summary>
		public string GoodsLocationId { get; set; }

		/// <summary>
		/// 货位name
		/// </summary>
		public string GoodsLocationName { get; set; }

		/// <summary>
		/// 重写equal，用于hashset区分
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			CommodityEps e = obj as CommodityEps;
			return CommodityCodeName == e.CommodityCodeName;
		}

		/// <summary>
		/// 重写HashCode，用于hashset区分
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return CommodityCodeName.GetHashCode() * 100 + CommodityCodeName.GetHashCode();
		}


	}
}
