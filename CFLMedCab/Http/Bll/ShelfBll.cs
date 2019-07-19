using CFLMedCab.Http.Helper;
using CFLMedCab.Http.Model;
using CFLMedCab.Http.Model.Base;
using CFLMedCab.Http.Model.param;
using CFLMedCab.Http.Model.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CFLMedCab.Http.Bll
{
	/// <summary>
	/// 上架业务
	/// </summary>
	class ShelfBll : BaseBll<ShelfBll>
	{
		public void GetShelfTaskList()
		{
			//获取待完成账户数据
			BaseData<ShelfTask> baseDataShelfTask = HttpHelper.GetInstance().Get<ShelfTask>(new QueryParam
			{
				@in =
					{
						field = "Status",
						in_list =  { HttpUtility.UrlEncode(ShelfTaskStatus.待上架.ToString()) }
					}
			});



		}
	}
}
