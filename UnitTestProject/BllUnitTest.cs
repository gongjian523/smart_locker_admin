using System;
using System.Collections.Generic;
using CFLMedCab.Http.Bll;
using CFLMedCab.Http.Model;
using CFLMedCab.Http.Model.Base;
using CFLMedCab.Http.Model.param;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace UnitTestProject
{
	[TestClass]
	public class BllUnitTest
	{
		[TestMethod]
		public void UserLoginBllTestMethod()
		{
			var data1 = UserLoginBll.GetInstance().GetUserToken(new Account
			{
				Phone = "18888888888",
				Password = "lidi123123"
			});

			var data2 = UserLoginBll.GetInstance().VeinmatchBinding(new VeinmatchPostParam
			{
                regfeature = "544a5",
                finger_name = "test"
            });

			var data3 = UserLoginBll.GetInstance().VeinmatchLogin("544a5");
		}

		[TestMethod]
		public void ShelfBllTestMethod()
		{

			BaseData<CommodityCode> baseDataCommodityCode = CommodityCodeBll.GetInstance().GetCompareCommodity(
				new HashSet<CommodityEps>()
				{
					new CommodityEps
					{
						CommodityCodeId = "AQACQqweBhEBAAAAwXCOmiFcsxUmKAIA",
						CommodityCodeName = "QR00000038",
						CommodityName = "止血包",
						EquipmentId = "AQACQqweDg8BAAAAFUD8WDEPsxV_FwQA",
						EquipmentName = "E00000008",
						GoodsLocationId = "AQACQqweJ4wBAAAAjYv6XmUPsxWWowMA",
						GoodsLocationName = "L00000013"

					}
				},

				new HashSet<CommodityEps>()
				{
					new CommodityEps
					{
						CommodityCodeId = "AQACQqweBhEBAAAAVF0JmCFcsxUkKAIA",
						CommodityCodeName = "QR00000035",
						CommodityName = "止血包",
						EquipmentId = "AQACQqweDg8BAAAAFUD8WDEPsxV_FwQA",
						EquipmentName = "E00000008",
						GoodsLocationId = "AQACQqweJ4wBAAAAjYv6XmUPsxWWowMA",
						GoodsLocationName = "L00000013"

					}
				}
			);


			BaseData<ShelfTask> baseDataShelfTask = ShelfBll.GetInstance().GetShelfTask("ST-44");

			BaseData<ShelfTaskCommodityDetail> baseDataShelfTaskCommodityDetail = ShelfBll.GetInstance().GetShelfTaskCommodityDetail(baseDataShelfTask);


			ShelfBll.GetInstance().GetShelfTaskChange(baseDataCommodityCode, baseDataShelfTask, baseDataShelfTaskCommodityDetail);

			JsonSerializerSettings jsetting = new JsonSerializerSettings
			{
				NullValueHandling = NullValueHandling.Ignore
			};
		
			string ret = JsonConvert.SerializeObject(new ShelfTask
			{
				Status = "上架",
				AbnormalCauses = "商品缺失",
				AbnormalDescribe = "异常描述"
			}, Formatting.Indented, jsetting);
		}

	}
}
