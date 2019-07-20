using System;
using CFLMedCab.Http.Bll;
using CFLMedCab.Http.Model;
using CFLMedCab.Http.Model.param;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
			//var data1 = ShelfBllBll.GetInstance().GetUserToken(new Account
			//{
			//	Phone = "18888888888",
			//	Password = "lidi123123"
			//});

	
		}

	}
}
