using System;

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
		/// 商品码CommodityCodeName对应id
		/// </summary>
		public string CommodityCodeId { get; set; }

		/// <summary>
		/// 商品码(就是RFid)
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
		/// 库房id
		/// </summary>
		public string StoreHouseId { get; set; }

		/// <summary>
		/// 库房Name
		/// </summary>
		public string StoreHouseName { get; set; }

		/// <summary>
		/// 重写equal，用于hashset区分
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
           if (obj == null)
           {
                return false;
           }

           if ((obj.GetType().Equals(this.GetType())) == false)
           {
               return false;
           }

           CommodityEps temp = (CommodityEps)obj;

           return CommodityCodeName.Equals(temp.CommodityCodeName);
           
		}

		/// <summary>
		/// 重写HashCode，用于hashset区分
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return CommodityCodeName.GetHashCode();
		}


	}
}
