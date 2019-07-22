using CFLMedCab.Http.Model.Base;

namespace CFLMedCab.Http.Model
{
	public class CommodityInventoryChange: BaseModel
	{
		/// <summary>
		/// 商品码
		/// </summary>
		public string CommodityCode { get; set; }

		/// <summary>
		/// 来源单据
		/// </summary>
		public string SourceBill { get; set; }


		//public string 
	}
}
