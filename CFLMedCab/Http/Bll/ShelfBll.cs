﻿using CFLMedCab.Http.Helper;
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
using System.ComponentModel;
using CFLMedCab.Http.Enum;

namespace CFLMedCab.Http.Bll
{
	/// <summary>
	/// 上架业务
	/// </summary>
	public class ShelfBll : BaseBll<ShelfBll>
	{

		/// <summary>
		/// 异常原因
		/// </summary>
		public enum AbnormalCauses
		{
			/// <summary>
			/// 待上架
			/// </summary>
			[Description("商品缺失 ")]
			商品缺失 = 1,

			/// <summary>
			/// 已完成 
			/// </summary>
			[Description("商品损坏")]
			商品损坏 = 2,

			/// <summary>
			/// 异常
			/// </summary>
			[Description("商品遗失")]
			商品遗失 = 3

		}

		/// <summary>
		/// 根据上架单号获取任务单详情
		/// </summary>
		/// <param name="shelfTaskName"></param>
		/// <returns></returns>
		public BaseData<ShelfTask> GetShelfTask(string shelfTaskName)
		{
			//获取待完成上架工单
			return HttpHelper.GetInstance().Get<ShelfTask>(new QueryParam
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
			}

		/// <summary>
		/// 根据上架单号获取商品详情
		/// </summary>
		/// <param name="shelfTaskName"></param>
		/// <returns></returns>
		public BaseData<ShelfTaskCommodityDetail> GetShelfTaskCommodityDetail(string shelfTaskName)
		{

			BaseData<ShelfTask> baseDataShelfTask = GetShelfTask(shelfTaskName);
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

		/// <summary>
		/// 根据上架单号获取商品详情
		/// </summary>
		/// <param name="shelfTaskName"></param>
		/// <returns></returns>
		public BaseData<ShelfTaskCommodityDetail> GetShelfTaskCommodityDetail(BaseData<ShelfTask> baseDataShelfTask)
		{

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

		/// <summary>
		/// 更新上架任务单
		/// </summary>
		/// <param name="commodities">操作商品集合</param>
		/// <returns></returns>
		public BaseData<ShelfTask> PutShelfTask(ShelfTask shelfTask, List<CommodityCode> commodityCodes,  List<ShelfTaskCommodityDetail> shelfTaskCommodityDetails, AbnormalCauses abnormalCauses)
		{

			var sfdCommoditys = shelfTaskCommodityDetails.Select(it => it.Commodity).ToList();

			var cccNames = commodityCodes.Select(it => it.CommodityName).ToList();

			//是否名称全部一致
			bool isAllContains = sfdCommoditys.All(cccNames.Contains) && sfdCommoditys.Count == cccNames.Count;

			if (isAllContains)
			{

				bool isAllNormal = true;

				foreach (ShelfTaskCommodityDetail stcd in shelfTaskCommodityDetails)
				{
					if (stcd.NeedShelfNumber != commodityCodes.Where(cit => cit.CommodityName == stcd.Commodity).Count())
					{
						shelfTask.Status = DocumentStatus.异常.ToString();
						shelfTask.AbnormalCauses = abnormalCauses.ToString();
						isAllNormal = false;
						break;

					}
				}

				if (isAllNormal)
				{
					shelfTask.Status = DocumentStatus.已完成.ToString();
				}

			}
			else
			{
				shelfTask.Status = DocumentStatus.异常.ToString();
				shelfTask.AbnormalCauses = abnormalCauses.ToString();
			}

			//获取待完成上架工单
			//return HttpHelper.getinstance().get<shelftask>(new queryparam
			//{
			//	view_filter =
			//	{
			//		filter =
			//		{
			//			logical_relation = "1 and 2",
			//			expressions =
			//			{
			//				new queryparam.expressions
			//				{
			//					field = "name",
			//					@operator = "==",
			//					operands =  {$"'{ httputility.urlencode(shelftaskname) }'"}
			//				},
			//				new queryparam.expressions
			//				{
			//					field = "status",
			//					@operator = "==",
			//					operands = {$"'{ httputility.urlencode(shelftaskstatus.待上架.tostring()) }'" }
			//				}
			//			}
			//		}
			//	}
			//});

			return null;
		}

	}


}
