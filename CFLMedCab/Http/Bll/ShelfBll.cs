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
	public class ShelfBll : BaseBll<ShelfBll>
	{
		/// <summary>
		/// 根据上架单号获取商品详情
		/// </summary>
		/// <param name="shelfTaskName"></param>
		/// <returns></returns>
		public BaseData<ShelfTaskCommodityDetail> GetShelfTaskCommodityDetail(string shelfTaskName)
		{
			//获取待完成上架工单
			BaseData<ShelfTask> baseDataShelfTask = HttpHelper.GetInstance().Get<ShelfTask>(new QueryParam
			{
				view_filter =
				{
					filter =
					{
						logical_relation = "1 AND 2",
						expressions =
						{
							new QueryParam.Expressions
							{
								field = "name",
								@operator = "==",
								operands =  {$"'{ HttpUtility.UrlEncode(shelfTaskName) }'"}
							},
							new QueryParam.Expressions
							{
								field = "Status",
								@operator = "==",
								operands = {$"'{ HttpUtility.UrlEncode(ShelfTaskStatus.待上架.ToString()) }'" }
							}
						}
					}
				}
			});

			//根据上架工单获取用户数据获取shel
			BaseData<ShelfTaskCommodityDetail> baseDataShelfTaskCommodityDetail = HttpHelper.GetInstance().ResultCheck((HttpHelper hh) => {

				return hh.Get<ShelfTaskCommodityDetail>(new QueryParam
				{
					@in =
					{
						field = "ShelfTaskId",
						in_list =  { HttpUtility.UrlEncode(baseDataShelfTask.body.objects[0].id) }
					}
				});

			}, baseDataShelfTask);

			//校验是否含有数据，如果含有数据，拼接具体字段
			HttpHelper.GetInstance().ResultCheck(baseDataShelfTaskCommodityDetail, out bool isSuccess);

			if (isSuccess)
			{
				baseDataShelfTaskCommodityDetail.body.objects.ForEach(it => 
				{
					//拼接设备名字
					if (!string.IsNullOrEmpty(it.EquipmentId))
					{
						it.EquipmentId = GetNameById<Equipment>(it.EquipmentId);
					}

					//拼接库房名字
					if (!string.IsNullOrEmpty(it.EquipmentId))
					{
						it.StoreHouseId = GetNameById<StoreHouse>(it.StoreHouseId);
					}

				});
			}

			return baseDataShelfTaskCommodityDetail;

		}

	}
}
