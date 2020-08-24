using CFLMedCab.Http.Bll;
using CFLMedCab.Http.Enum;
using CFLMedCab.Http.Helper;
using CFLMedCab.Http.Model;
using CFLMedCab.Http.Model.Base;
using CFLMedCab.Http.Model.Common;
using CFLMedCab.Http.Msmq;
using CFLMedCab.Infrastructure;
using CFLMedCab.Infrastructure.ToolHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace CFLMedCab.Http.ExceptionApi
{
	/// <summary>
	/// 异常api处理类（抽象）
	/// </summary>
	public class ExStepHandle
	{

		/// <summary>
		/// 获取商品完整属性
		/// </summary>
		/// <param name="commodityCodeList"></param>
		/// <returns></returns>
		public static BaseData<CommodityCode> GoodsInitData(List<CommodityCode> commodityCodeList) {

			BaseData<CommodityCode> bdCommodityCode = CommodityCodeBll.GetInstance().GetCommodityCodeStock(commodityCodeList);
			HttpHelper.GetInstance().ResultCheck(bdCommodityCode, out bool isSuccess);
			if (isSuccess)
			{
				bdCommodityCode = CommodityCodeBll.GetInstance().GetQualityStatus(bdCommodityCode, out isSuccess);
			}
			return bdCommodityCode;
		}

		/// <summary>
		/// 所有领用商品初始化
		/// </summary>
		/// <param name="commodityCodeList"></param>
		/// <returns></returns>
		public static BaseData<CommodityCode> FetchGoodsInitData(List<CommodityCode> commodityCodeList)
		{

			var bdCommodityCode = GoodsInitData(commodityCodeList);


			//校验是否含有数据
			HttpHelper.GetInstance().ResultCheck(bdCommodityCode, out bool isSuccess);

			if (isSuccess) {

				bdCommodityCode.body.objects.ToList().ForEach(it =>
				{
					if (it.operate_type == 1 || it.operate_type == 0 && it.QualityStatus == QualityStatusType.过期.ToString())
					{
						it.AbnormalDisplay = AbnormalDisplay.异常.ToString();
					}
				});

			}

			return bdCommodityCode;

		}

		/// <summary>
		/// 回退领用初始化
		/// </summary>
		/// <param name="bdCommodityCode"></param>
		/// <returns></returns>
		public static BaseData<CommodityCode> ReturnGoodsInitData(List<CommodityCode> commodityCodeList)
		{

			var bdCommodityCode = GoodsInitData(commodityCodeList);

			//校验是否含有数据
			HttpHelper.GetInstance().ResultCheck(bdCommodityCode, out bool isSuccess);

			if (isSuccess)
			{
				bdCommodityCode.body.objects.Where(item => item.operate_type == 0).ToList().ForEach(it =>
				{
					it.AbnormalDisplay = AbnormalDisplay.异常.ToString();
				});
			}
			return bdCommodityCode;

		}

		/// <summary>
		/// 创建领用订单
		/// </summary>
		/// <param name="bdCommodityCode"></param>
		/// <param name="type"></param>
		/// <param name="sourceBill"></param>
		public static List<CommodityInventoryChange> FetchCreateOrder(ConsumingOrder consumingOrder, List<CommodityCode> lossList, out bool isExNetWork)
		{

			isExNetWork = false;

			var changeList = new List<CommodityInventoryChange>();//商品库存变更记录列表

			//创建领用单
			var order = ConsumingBll.GetInstance().CreateConsumingOrder(consumingOrder);

			if (order.code == (int)ResultCode.Request_Exception)
			{
				isExNetWork = true;
			}

			//校验数据是否正常
			HttpHelper.GetInstance().ResultCheck(order, out bool isSuccess);

			if (isSuccess)
			{
				lossList.ForEach(loss =>
				{
					changeList.Add(new CommodityInventoryChange()
					{
						CommodityCodeId = loss.id,
						SourceBill = new SourceBill()
						{
							object_name = "ConsumingOrder",
							object_id = order.body[0].id
						},
						ChangeStatus = CommodityInventoryChangeStatus.已消耗.ToString(),
						operate_type = loss.operate_type
					});
				});
			}

			return changeList;

		}

		/// <summary>
		/// 根据商品码变更列表和来源单据创建库存变更记录资料（回退）
		/// </summary>
		/// <param name="baseDataCommodityCode"></param>
		/// <returns></returns>
		public static List<CommodityInventoryChange> ReturnCreateOrder(BaseData<CommodityCode> baseDataCommodityCode, out bool isExNetWork)
		{

			isExNetWork = false;

			var changeList = new List<CommodityInventoryChange>();//商品库存变更记录列表

			var consumingOrder = ConsumingBll.GetInstance().CreateConsumingOrder(new ConsumingOrder()
			{
				//Status = ConsumingOrderStatus.领用中.ToString(),
				//StoreHouseId = ApplicationState.GetValue<String>((int)ApplicationKey.HouseId),
				Type = ConsumingOrderType.一般领用.ToString()
			});

			if (consumingOrder.code == (int)ResultCode.Request_Exception)
			{
				isExNetWork = true;
			}

			//校验数据是否正常
			HttpHelper.GetInstance().ResultCheck(consumingOrder, out bool isSuccess);
			if (isSuccess)
			{
				//创建商品库存变更记录资料【出库::领用】
				baseDataCommodityCode.body.objects.Where(it => it.operate_type == 0).ToList().ForEach(commodityCode =>
				{
					var temp = new CommodityInventoryChange()
					{
						CommodityCodeId = commodityCode.id,
						//出库变更更后库房、变更更后设备、变更更后货位 value 值都为null。
						SourceBill = new SourceBill()
						{
							object_name = "ConsumingOrder",
							object_id = consumingOrder.body[0].id
						},
						ChangeStatus = CommodityInventoryChangeStatus.已消耗.ToString(),
						operate_type = commodityCode.operate_type
					};
					changeList.Add(temp);
				});
				
			}
			return changeList;
		}


		public static void ExApiSendQueueHandle(ExEntity currentExEnity)
		{
			MsmqFactory.GetInstance().SendExApi(currentExEnity);
		}

		/// <summary>
		/// 如果库存变化api请求结果联网异常，则发送消息
		/// </summary>
		/// <param name="baseData"></param>
		/// <param name="changeList"></param>
		public static void ExApiSendQueueFetchGoodsInitDataHandle(BaseData<CommodityCode> baseData, List<CommodityCode> commodityCodeList)
		{
			if (baseData.code == (int)ResultCode.Request_Exception)
			{
				ExApiSendQueueHandle(new ExEntity
				{
					businessType = BusinessType.领用,
					businessStep = BusinessStep.商品未初始化,
					commodityCodeList = commodityCodeList,
				});
			}
		}

		/// <summary>
		/// 如果库存变化api请求结果联网异常，则发送消息
		/// </summary>
		/// <param name="baseData"></param>
		/// <param name="changeList"></param>
		public static void ExApiSendQueueReturnGoodsInitDataHandle(BaseData<CommodityCode> baseData, List<CommodityCode> commodityCodeList)
		{
			if (baseData.code == (int)ResultCode.Request_Exception)
			{
				ExApiSendQueueHandle(new ExEntity
				{
					businessType = BusinessType.回退,
					businessStep = BusinessStep.商品未初始化,
					commodityCodeList = commodityCodeList,
				});
			}
		}


		/// <summary>
		/// 如果创建单号api请求结果联网异常，则发送消息
		/// </summary>
		/// <param name="baseData"></param>
		/// <param name="changeList"></param>
		public static void ExApiSendQueueFetchCreateOrderHandle(BasePostData<ConsumingOrder> baseData, ConsumingOrder consumingOrder, List<CommodityCode> lossList)
		{
			if (baseData.code == (int)ResultCode.Request_Exception)
			{
				ExApiSendQueueHandle(new ExEntity
				{
					businessType = BusinessType.领用,
					businessStep = BusinessStep.商品初始化,
					consumingOrder = consumingOrder,
					commodityCodeList = lossList
				});
			}
		}

		/// <summary>
		/// 如果创建单号api请求结果联网异常，则发送消息
		/// </summary>
		/// <param name="baseData"></param>
		/// <param name="changeList"></param>
		public static void ExApiSendQueueReturnCreateOrderHandle(BasePostData<ConsumingOrder> baseData, BaseData<CommodityCode> baseDataCommodityCode)
		{
			if (baseData.code == (int)ResultCode.Request_Exception)
			{
				ExApiSendQueueHandle(new ExEntity
				{
					businessType = BusinessType.回退,
					businessStep = BusinessStep.商品初始化,
					bdcommodityCode = baseDataCommodityCode
				});
			}
		}

		/// <summary>
		/// 如果库存变化api请求结果联网异常，则发送消息
		/// </summary>
		/// <param name="baseData"></param>
		/// <param name="changeList"></param>
		public static void ExApiSendQueueGoodsChangesHandle(BasePostData<CommodityInventoryChange> baseData, List<CommodityInventoryChange> changeList)
		{
			if (baseData.code == (int)ResultCode.Request_Exception)
			{
				ExApiSendQueueHandle(new ExEntity
				{
					businessType = BusinessType.库存变化,
					businessStep = BusinessStep.创建领用单,
					commodityInventoryChangeList = changeList
				});
			}
		}

		/// <summary>
		/// 对已经接收的消息分步处理
		/// </summary>
		/// <param name="currentExEnity"></param>
		public static void ExApiHandle(ExEntity currentExEnity)
		{

			if (currentExEnity.businessType == BusinessType.领用)
			{
				switch (currentExEnity.businessStep)
				{
					case BusinessStep.商品未初始化:

						BaseData<CommodityCode> bdCommodityCode = FetchGoodsInitData(currentExEnity.commodityCodeList);
						if (bdCommodityCode.code == (int)ResultCode.Request_Exception)
						{
							ExApiSendQueueHandle(currentExEnity);
						}
						else
						{

							ConsumingBll.GetInstance().SubmitConsumingChangeWithoutOrder(bdCommodityCode, currentExEnity.consumingOrderType);

						} 

						break;
					case BusinessStep.商品初始化:

						List<CommodityInventoryChange> changesList = FetchCreateOrder(currentExEnity.consumingOrder, currentExEnity.commodityCodeList, out bool isExNetWork);

						if (isExNetWork)
						{
							ExApiSendQueueHandle(currentExEnity);
						}
						else
						{
							CommodityInventoryChangeBll.GetInstance().CreateCommodityInventoryChangeSeparately(changesList);
						}
						break;
					default:
						LogUtils.Error($"异步api消息领用业务步骤无效：{currentExEnity.ToString()}");
						break;

				}
			}
			else if (currentExEnity.businessType == BusinessType.回退)
			{
				switch (currentExEnity.businessStep)
				{
					case BusinessStep.商品未初始化:

						BaseData<CommodityCode> bdCommodityCode = ReturnGoodsInitData(currentExEnity.commodityCodeList);
						if (bdCommodityCode.code == (int)ResultCode.Request_Exception)
						{
							ExApiSendQueueHandle(currentExEnity);
						}
						else
						{
							CommodityInventoryChangeBll.GetInstance().CreateCommodityInventoryChange(bdCommodityCode);
						}

						break;
					case BusinessStep.商品初始化:

						List<CommodityInventoryChange> changesList = ReturnCreateOrder(currentExEnity.bdcommodityCode, out bool isExNetWork);

						if (isExNetWork)
						{
							ExApiSendQueueHandle(currentExEnity);
						}
						else
						{
							CommodityInventoryChangeBll.GetInstance().CreateCommodityInventoryChangeSeparately(changesList);
						}
						break;
					default:
						LogUtils.Error($"异步api消息回退业务步骤无效：{currentExEnity.ToString()}");
						break;
				}
			}
			else if (currentExEnity.businessType == BusinessType.库存变化)
			{
				if (currentExEnity.businessStep == BusinessStep.创建领用单)
				{
					CommodityInventoryChangeBll.GetInstance().CreateCommodityInventoryChangeSeparately(currentExEnity.commodityInventoryChangeList);
				}
				else
				{
					LogUtils.Error($"异步api消息库存变化步骤无效：{currentExEnity.ToString()}");
				}
			}
			else
			{
				LogUtils.Error($"异步api消息业务类型无效：{currentExEnity.ToString()}");
			}
		
		
		}

		/// <summary>
		/// 业务类型
		/// </summary>
		public enum BusinessType
		{
			/// <summary>
			/// 领用（手术领用、医嘱领用、一般领用）
			/// </summary>
			[Description("领用")]
			领用 = 1,

			/// <summary>
			/// 回退
			/// </summary>
			[Description("回退")]
			回退 = 2,

			/// <summary>
			/// 库存变化
			/// </summary>
			[Description("库存变化")]
			库存变化 = 3

		}

		/// <summary>
		/// 业务已执行到的步骤
		/// </summary>
		public enum BusinessStep
		{

			/// <summary>
			/// 商品初始化
			/// </summary>
			[Description("商品未初始化")]
			商品未初始化 = 0,

			/// <summary>
			/// 商品初始化
			/// </summary>
			[Description("商品初始化")]
			商品初始化 = 1,

			/// <summary>
			/// 创建领用单
			/// </summary>
			[Description("创建领用单")]
			创建领用单 = 2,

		}




	}
}
