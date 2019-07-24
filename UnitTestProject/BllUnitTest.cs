using System;
using System.Collections.Generic;
using CFLMedCab.Http.Bll;
using CFLMedCab.Http.Enum;
using CFLMedCab.Http.Model;
using CFLMedCab.Http.Model.Base;
using CFLMedCab.Http.Model.Common;
using CFLMedCab.Http.Model.param;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using static CFLMedCab.Http.Bll.ConsumingBll;
using static CFLMedCab.Http.Bll.ShelfBll;

namespace UnitTestProject
{
	[TestClass]
	public class BllUnitTest
	{
		[TestMethod]
		public void UserLoginBllTestMethod()
		{
			//var data1 = UserLoginBll.GetInstance().GetUserToken(new Account
			//{
			//	Phone = "18888888888",
			//	Password = "lidi123123"
			//});

			//var data2 = UserLoginBll.GetInstance().VeinmatchBinding(new VeinbindingPostParam
			//{
   //             regfeature = "544a5",
   //             finger_name = "test"
   //         });

			//var data3 = UserLoginBll.GetInstance().VeinmatchLogin(new VeinmatchPostParam {
   //             regfeature = "544a5"
   //         });
		}

		/// <summary>
		/// 上架业务测试方法
		/// </summary>
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

			BaseData<ShelfTask> baseDataShelfTask = ShelfBll.GetInstance().GetShelfTask("OS20190721000052");

			BaseData<ShelfTaskCommodityDetail> baseDataShelfTaskCommodityDetail = ShelfBll.GetInstance().GetShelfTaskCommodityDetail(baseDataShelfTask);

			BaseData<ShelfTask> baseDataShelfTaskChange = ShelfBll.GetInstance().GetShelfTaskChange(baseDataCommodityCode, baseDataShelfTask, baseDataShelfTaskCommodityDetail);

			//BasePutData<ShelfTask> putData = ShelfBll.GetInstance().PutShelfTask(baseDataShelfTaskChange, AbnormalCauses.商品损坏 );
			BasePostData<CommodityInventoryChange> basePostData  = ShelfBll.GetInstance().CreateShelfTaskCommodityInventoryChange(baseDataCommodityCode, baseDataShelfTask);

		}

		/// <summary>
		/// 拣货业务测试方法
		/// </summary>
		[TestMethod]
		public void PickBllTestMethod()
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


			BaseData<PickTask> baseDataPickTask = PickBll.GetInstance().GetPickTask("ST20190721000031");

			BaseData<PickCommodity> baseDataPickTaskCommodityDetail = PickBll.GetInstance().GetPickTaskCommodityDetail(baseDataPickTask);

			BaseData<PickTask> baseDataPickTaskChange = PickBll.GetInstance().GetPickTaskChange(baseDataCommodityCode, baseDataPickTask, baseDataPickTaskCommodityDetail);


			BasePutData<PickTask> putData = PickBll.GetInstance().PutPickTask(baseDataPickTaskChange, AbnormalCauses.商品损坏 );
			//BasePostData<CommodityInventoryChange> basePostData = PickBll.GetInstance().CreatePickTaskCommodityInventoryChange(baseDataCommodityCode, baseDataPickTask);

		}

		/// <summary>
		/// 测试通过【领⽤用单码】从表格 【领⽤用单】中查询获取领⽤用单的详情【ID，markId(作废标识)】
		/// </summary>
		[TestMethod]
        public void ConsumingQueryOrderTestMethod()
        {
            //根据通过【领⽤用单码】从表格 【领⽤用单】中查询获取领⽤用单的id，以及markId（作废标识）。（如果领⽤用单作废标识为【是】则弹窗提醒⼿手术单作废，跳转回前⻚页）
            var temp = ConsumingBll.GetInstance().GetConsumingOrder("U20190723000051");
            Console.WriteLine(temp.body.objects[0].id);
            /*            var id = "AQACQqweDg8BAAAAlQ6o8NWbsxUtQwQA";*/
            //通过【关联领⽤用单】(ConsumingOrder.id= ConsumingGoodsDetail.ConsumingOrderId）从表格 【领⽤用商品明细】中查询获取领⽤用商品的列列表信息。
            var lists = ConsumingBll.GetInstance().GetOperationOrderGoodsDetail(temp);
            Console.WriteLine(lists);
            //通过【商品编码】从表格【商品库存管理理】中查询商品详情获得商品名称。
            var details = ConsumingBll.GetInstance().GetPrescriptionOrderGoodsDetail(temp);
            Console.WriteLine(details);

        }
        [TestMethod]
        public void ConsumingPostOrderTestMethod()
        {
/*            //测试移动端 创建【领用单】，且领⽤用状态为 ‘已完成’。
            var temp = ConsumingBll.GetInstance().CreateConsumingOrder(new ConsumingOrder()
            {
                FinishDate = "2019-07-23T09:31:00Z",//完成时间
                Status = ConsumingOrderStatus.已完成.ToString(),//领用状态
                StoreHouseId = null,//领用库房【暂未知数据来源】
                Type = ConsumingOrderType.一般领用.ToString()
            });
            Console.WriteLine(temp);*/
            //移动端 通过【领⽤用单编号】 查找更更新【领⽤用单】的领⽤用状态为 ‘已完成’

            var puttemp = ConsumingBll.GetInstance().UpdateConsumingOrderStatus(new ConsumingOrder()
            {
                id = "AQACQqweDg8BAAAAv0_s8_fnsxXXagQA",
                Status = ConsumingOrderStatus.未领用.ToString(),
                version = 4//必须和当前数据版本保持一致
            });

            Console.WriteLine(puttemp);
        }
        /// <summary>
        /// 测试商品库存变更记录创建
        /// </summary>
        [TestMethod]
        public void CommodityInventoryChangeTestMethod()
        {

            var temp = CommodityInventoryChangeBll.GetInstance().createCommodityInventoryChange(new List<CommodityInventoryChange>()
            {
                new CommodityInventoryChange()
                {
                    CommodityCodeId = "AQACQqweDg8BAAAAq09Zts9esxXkLwQA",//商品码【扫描】
                    SourceBill = new SourceBill()//来源单据
                    {
                        object_name = "ConsumingOrder",
                        object_id = "AQACQqweDg8BAAAAv0_s8_fnsxXXagQA"
                    },
                    ChangeStatus = CommodityInventoryChangeStatus.已消耗.ToString()
                }
            });

            Console.WriteLine(temp);
        }

        [TestMethod]
        public void RollbackTestMethod()
        {
/*            var name = "L00000010";
            var temp = RollbackBll.GetInstance().GetGoodsLocation(name);
            Console.WriteLine(temp);
*/
            var commodityCode = "C00000053";
            var temp2 = RollbackBll.GetInstance().GetCommodity(commodityCode);

            Console.WriteLine(temp2);

        }

    }

}
