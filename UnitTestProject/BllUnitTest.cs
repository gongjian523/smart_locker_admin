using System;
using CFLMedCab.Http.Bll;
using CFLMedCab.Http.Model;
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
			// ShelfBll.GetInstance().GetShelfTaskCommodityDetail("ST-44");
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
