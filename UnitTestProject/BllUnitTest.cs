using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CFLMedCab.Http.Bll;
using CFLMedCab.Http.Enum;
using CFLMedCab.Http.Helper;
using CFLMedCab.Http.Model;
using CFLMedCab.Http.Model.Base;
using CFLMedCab.Http.Model.Common;
using CFLMedCab.Http.Model.param;
using CFLMedCab.Http.Msmq;
using CFLMedCab.Infrastructure;
using CFLMedCab.Infrastructure.QuartzHelper;
using CFLMedCab.Infrastructure.QuartzHelper.job;
using CFLMedCab.Infrastructure.QuartzHelper.quartzEnum;
using CFLMedCab.Infrastructure.QuartzHelper.scheduler;
using CFLMedCab.Infrastructure.QuartzHelper.trigger;
using CFLMedCab.Infrastructure.ToolHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using static CFLMedCab.Http.Bll.ConsumingBll;

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

        public BaseData<CommodityCode> GetBaseData()
        {
            return CommodityCodeBll.GetInstance().GetCompareCommodity(
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
        }
		[TestMethod]
		public void ShelfBllTestMethod()
		{

            BaseData<CommodityCode> baseDataCommodityCode = GetBaseData();


			BaseData<ShelfTask> baseDataShelfTask = ShelfBll.GetInstance().GetShelfTask("OS20190721000052");

			BaseData<ShelfTaskCommodityDetail> baseDataShelfTaskCommodityDetail = ShelfBll.GetInstance().GetShelfTaskCommodityDetail(baseDataShelfTask);

			ShelfBll.GetInstance().GetShelfTaskChange(baseDataCommodityCode, baseDataShelfTask.body.objects[0], baseDataShelfTaskCommodityDetail);

			BasePutData<ShelfTask> putData = ShelfBll.GetInstance().PutShelfTask(baseDataShelfTask.body.objects[0]);
			BasePostData<CommodityInventoryChange> basePostData  = ShelfBll.GetInstance().CreateShelfTaskCommodityInventoryChange(baseDataCommodityCode, baseDataShelfTask.body.objects[0], true);

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

	    /// <summary>
		/// 拣货业务测试方法
		/// </summary>
		[TestMethod]
		public void PickBllTestMethod()
		{

            BaseData<CommodityCode> baseDataCommodityCode = GetBaseData();


			BaseData<PickTask> baseDataPickTask = PickBll.GetInstance().GetPickTask("ST20190721000031");

			BaseData<PickCommodity> baseDataPickTaskCommodityDetail = PickBll.GetInstance().GetPickTaskCommodityDetail(baseDataPickTask);

			PickBll.GetInstance().GetPickTaskChange(baseDataCommodityCode, baseDataPickTask.body.objects[0], baseDataPickTaskCommodityDetail);

			BasePutData<PickTask> putData = PickBll.GetInstance().PutPickTask(baseDataPickTask.body.objects[0]);
			BasePostData<CommodityInventoryChange> basePostData = PickBll.GetInstance().CreatePickTaskCommodityInventoryChange(baseDataCommodityCode, baseDataPickTask.body.objects[0], true);

		}

		/// <summary>
		/// 测试通过【领⽤用单码】从表格 【领⽤用单】中查询获取领⽤用单的详情【ID，markId(作废标识)】
		/// </summary>
		[TestMethod]
        public void ConsumingQueryOrderTestMethod()
        {
            //根据通过【领⽤用单码】从表格 【领⽤用单】中查询获取领⽤用单的id，以及markId（作废标识）。（如果领⽤用单作废标识为【是】则弹窗提醒⼿手术单作废，跳转回前⻚页）
            //var temp = ConsumingBll.GetInstance().GetConsumingOrder("456465412321");
            //LogUtils.Debug(temp.body.objects[0].id);
            /*            var id = "AQACQqweDg8BAAAAlQ6o8NWbsxUtQwQA";*//*
            //通过【关联领⽤用单】(ConsumingOrder.id= ConsumingGoodsDetail.ConsumingOrderId）从表格 【领⽤用商品明细】中查询获取领⽤用商品的列列表信息。
            var lists = ConsumingBll.GetInstance().GetOperationOrderGoodsDetail(temp);
            LogUtils.Debug(lists);
            //通过【商品编码】从表格【商品库存管理理】中查询商品详情获得商品名称。
            var details = ConsumingBll.GetInstance().GetPrescriptionOrderGoodsDetail(temp);
            LogUtils.Debug(details);
*/
            //根据医嘱处方名称获取医嘱处方信息
            var temp2 = ConsumingBll.GetInstance().GetPrescriptionBill("456465412321");
            LogUtils.Debug(temp2.body.objects[0]);
        }
        /// <summary>
        /// 领用模块接口模拟
        /// </summary>
        [TestMethod]
        public void ConsumingPostOrderTestMethod()
        {
            //测试移动端 创建【领用单】，且领⽤用状态为 ‘已完成’。
            var temp = ConsumingBll.GetInstance().CreateConsumingOrder(new ConsumingOrder()
            {
                FinishDate = "2019-07-23T09:31:00Z",//完成时间
                Status = ConsumingOrderStatus.已完成.ToString(),//领用状态
                StoreHouseId = null,//领用库房【暂未知数据来源】
                Type = ConsumingOrderType.一般领用.ToString()
            });
            LogUtils.Debug(temp);
            //移动端 通过【领⽤用单编号】 查找更更新【领⽤用单】的领⽤用状态为 ‘已完成’

            var puttemp = ConsumingBll.GetInstance().UpdateConsumingOrderStatus(new ConsumingOrder()
            {
                id = "AQACQqweDg8BAAAAv0_s8_fnsxXXagQA",
                Status = ConsumingOrderStatus.未领用.ToString(),
                version = 4//必须和当前数据版本保持一致
            });

            LogUtils.Debug(puttemp);
        }
        /// <summary>
        /// 测试领用部分商品库存变更记录创建
        /// </summary>
        [TestMethod]
        public void CommodityInventoryChangeTestMethod()
        {
            /*            //创建商品库存变更记录
                        var temp = CommodityInventoryChangeBll.GetInstance().CreateCommodityInventoryChange(new List<CommodityInventoryChange>()
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

                        LogUtils.Debug(temp);
            */
            //根据商品码变更列表和来源单据创建库存变更记录资料
            BaseData<CommodityCode> baseDataCommodityCode = GetBaseData();
            //货物领用
            SourceBill sourceBill = new SourceBill()
            {
                object_name = "ConsumingOrder",
                object_id = ""
            };
            var changes = CommodityInventoryChangeBll.GetInstance().CreateCommodityInventoryChange(baseDataCommodityCode, sourceBill);
            LogUtils.Debug(changes);

        }

        /// <summary>
        /// 商品回退接口测试
        /// </summary>
        [TestMethod]
        public void RollbackTestMethod()
        {
            //var name = "L00000010";
            //var temp = RollbackBll.GetInstance().GetGoodsLocation(name);
            //LogUtils.Debug(temp);

            //var commodityCode = "C00000053";
            //var temp2 = RollbackBll.GetInstance().GetCommodity(commodityCode);

            //LogUtils.Debug(temp2);

            var baseDetail = GetBaseData();
            var change = CommodityInventoryChangeBll.GetInstance().CreateCommodityInventoryChange(baseDetail);
            LogUtils.Debug(change);

        }
        [TestMethod]
        public void InventoryTestMethod()
        {


			JsonSerializerSettings jsetting = new JsonSerializerSettings
			{
				NullValueHandling = NullValueHandling.Ignore
			};

			string ret = JsonConvert.SerializeObject(new InventoryOrder
			{
				ConfirmDate = DateTime.Now.ToString("s")
			}, Formatting.Indented, jsetting);

			var taskName = "IT20190723000015";
            var temp = InventoryTaskBll.GetInstance().GetInventoryOrdersByInventoryTaskName(taskName);
            LogUtils.Debug(temp);

			

/*            //通过【盘点任务名称】从表格【盘点任务】中查询获取盘点任务id。• 通过【盘点任务单】（InventoryTask.id =InventoryOrder.InventoryTaskId）从表格【盘点单】中查询获得盘点单列列表
            var taskName = "IT20190723000015";
            var inventoryOrders = InventoryTaskBll.GetInstance().GetInventoryOrdersByInventoryTaskName(taskName);
            LogUtils.Debug(inventoryOrders);

            //移动端 更更新【盘点单管理理】的盘点状态为 ‘已确认’
            var inventoryOrder = InventoryTaskBll.GetInstance().UpdateInventoryOrderStatus(new InventoryOrder()
            {
                id = "AQACQqweDg8BAAAAc0w5SjP_sxVVfgQA",//AQACQqweJ4wBAAAAsOJ96Cr_sxWbDwQA
                Status = InventoryOrderStatus.待盘点.ToString(),
                ConfirmDate = "2019-07-24T15:19:00Z",
                version = 0
            });
            LogUtils.Debug(inventoryOrder);*/


            //• 通过当前【设备编码/id】从表格 【设备管理理】（Equipment）中查询获取设备的’⾃自动盘点计划’（InventoryPlanId）。
            //• 通过当前【id】（InventoryPlan.id = Equipment.InventoryPlanId）从表格【盘点计划管理理】（InventoryPlan）中查询获取相关盘点信息。
            var equipmentName = "E00000008";//设备名称
            //var equipmentId = "AQACQqweDg8BAAAAFUD8WDEPsxV_FwQA";//设备ID
            var plans = InventoryTaskBll.GetInstance().GetInventoryPlanByEquipmnetNameOrId(equipmentName);
            //var plans = InventoryTaskBll.GetInstance().GetInventoryPlanByEquipmnetNameOrId(equipmentId);
            LogUtils.Debug(plans);

            var createOrder = InventoryTaskBll.GetInstance().CreateInventoryOrder(new List<InventoryOrder>() {
                new InventoryOrder()
                {
                    ConfirmDate = "2019-07-24T15:19:00Z",//确认时间
                    EquipmentId = "",//盘点设备
                    GoodsLocationId = "",//盘点货位
                    Status = "",//盘点状态
                    StoreHouseId = "",//盘点库房
                    Type = InventoryOrderType.自动创建.ToString()//创建类型

                }
            });
		}

		/// <summary>
		/// 定时任务相关测试类
		/// </summary>
		[TestMethod]
		public void QuartzTestMethod()
		{

			CustomizeScheduler.GetInstance().SchedulerStart<GetInventoryPlanJoB>(CustomizeTrigger.GetInventoryPlanTrigger(), GroupName.GetInventoryPlan);

			System.Threading.Thread.Sleep(2000000);

		}

        [TestMethod]
        public void TestCommodityRecovery()
        {
            var recovery = CommodityRecoveryBll.GetInstance().GetCommodityRecovery("RT20190731000029");

            if(null != recovery && null != recovery.body && null != recovery.body.objects)
            {
                var temp = CommodityRecoveryBll.GetInstance().SubmitCommodityRecoveryChange(GetBaseData(), recovery.body.objects[0]);
            }

        }
        [TestMethod]
        public void TestGoodsInfo()
        {
            var codes = GetBaseData().body.objects.GroupBy(code => new { code.CommodityId, code.GoodsLocationId }).Select(g => (new Commodity()
                {
                    id = g.Key.CommodityId,//CommodityId
                    GoodsLocationId = g.Key.GoodsLocationId,
                    GoodsLocationName = g.ElementAt(0).GoodsLocationName,
                    name = g.ElementAt(0).CommodityName,//name
                    Count = g.Count(),//商品数量
                    codes = GetBaseData().body.objects.Where(it => it.CommodityId == g.Key.CommodityId && it.GoodsLocationId == g.Key.GoodsLocationId).ToList()
            })).ToList();

        }

        [TestMethod]
        public void TestUserLogin()
        {
            var token = UserLoginBll.GetInstance().GetUserToken(new SignInParam()
            {
                phone = "+86 18408252063",
                password = ""
            });
            LogUtils.Debug(token);
            var userInfo = UserLoginBll.GetInstance().GetUserInfo("+86 18408252063");
            LogUtils.Debug(userInfo);

            //var temp = UserLoginBll.GetInstance().GetCaptchaImageToken();
            //LogUtils.Debug(temp);

        }

        [TestMethod]
        public void TestBase64()
        {
            var password = "";
            var pas64 = Convert.ToBase64String(Encoding.Default.GetBytes(password));
            LogUtils.Debug(pas64);
        }

		[TestMethod]
		public void TestBase64De()
		{

			var password = BllHelper.DecodeBase64Str("YXM4NDExOTQwMDM=");

			LogUtils.Debug(password);
		}

		[TestMethod]
		public void TestByte()
		{

			ushort us = ushort.MaxValue;

			var usBytes = BitConverter.GetBytes(us);

			ushort aus =  BitConverter.ToUInt16(usBytes, 0);


		}

		[TestMethod]
		public void TestNetwork()
		{
            BaseData<User> bdUser = UserLoginBll.GetInstance().GetUserInfo("+86 18628293148");
            //BaseData<User> bdUser = UserLoginBll.GetInstance().GetUserInfo("+86 13765810065");
			HttpHelper.GetInstance().ResultCheck(bdUser, out bool bdUserIsSucess);

		}


		[TestMethod]
		public void TestQueueSend()
		{
			MsmqFactory.GetInstance();

			// Simulate doing other work on the current thread.
			System.Threading.Thread.Sleep(TimeSpan.FromSeconds(60));

		}


	}
}
