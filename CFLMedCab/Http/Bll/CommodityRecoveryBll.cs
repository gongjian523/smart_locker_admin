using CFLMedCab.Http.Enum;
using CFLMedCab.Http.Helper;
using CFLMedCab.Http.Model;
using CFLMedCab.Http.Model.Base;
using CFLMedCab.Http.Model.Common;
using CFLMedCab.Http.Model.param;
using CFLMedCab.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CFLMedCab.Http.Bll
{
    /// <summary>
    /// 回收取货-智能柜端
    /// </summary>
    public class CommodityRecoveryBll : BaseBll<CommodityRecoveryBll>
    {
        /// <summary>
        /// 通过【任务单号 and 操作⼈（Operator）】从表格 【商品回收任务单】中查询获取商品回收任务单相关信息。
		/// </summary>
		/// <param name="commodityRecoveryName"></param>
		/// <returns></returns>
		public BaseData<CommodityRecovery> GetCommodityRecovery(string commodityRecoveryName)
        {
            if (null == commodityRecoveryName)
            {
                return new BaseData<CommodityRecovery>()
                {
                    code = (int)ResultCode.Parameter_Exception,
                    message = ResultCode.Parameter_Exception.ToString()
                };
            }
            //获取待完成上架工单
            var recovery = HttpHelper.GetInstance().Get<CommodityRecovery>(new QueryParam
            {
                view_filter =
                {
                    filter =
                    {
                        //logical_relation = "1 AND 2",
                        logical_relation = "1",
                        expressions =
                        {
                            new QueryParam.Expressions
                            {
                                field = "name",
                                @operator = "==",
                                operands =  {$"'{ HttpUtility.UrlEncode(commodityRecoveryName) }'"}
                            }
                            //,
                            //new QueryParam.Expressions
                            //{
                            //    field = "Operator",
                            //    @operator = "==",
                            //    operands = {$"'{ HttpUtility.UrlEncode(ApplicationState.GetUserInfo().id)}'"}
                            //}
                        }
                    }
                }
            });
            //校验是否含有数据，如果含有数据，拼接具体字段
            recovery = HttpHelper.GetInstance().ResultCheck(recovery, out bool isSuccess);

            if (isSuccess)
            {
                recovery.body.objects.ForEach(it =>
                {
                    //拼接库房名称
                    if (!string.IsNullOrEmpty(it.StoreHouse))
                    {
                        it.StoreHouseName = GetNameById<StoreHouse>(it.StoreHouse);
                    }
                });
            }
            return recovery;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="recovery"></param>
        /// <returns></returns>
        public BasePutData<CommodityRecovery> UpdateCommodityRecoveryStatus(CommodityRecovery recovery)
        {

            if (null == recovery || null == recovery.id || null == recovery.Status || null == recovery.version)
            {
                return new BasePutData<CommodityRecovery>()
                {
                    code = (int)ResultCode.Parameter_Exception,
                    message = ResultCode.Parameter_Exception.ToString()
                };
            }
            return HttpHelper.GetInstance().Put<CommodityRecovery>(new CommodityRecovery()
            {
                id = recovery.id,//ID
                Status = recovery.Status,//状态
                version = recovery.version//版本
            });
        }
        public BasePostData<CommodityInventoryChange> SubmitCommodityRecoveryChange(BaseData<CommodityCode> baseDataCommodityCode, CommodityRecovery recovery)
        {

            //商品回收类来源单据
            var sourceBill = new SourceBill()
            {
                object_name = "CommodityRecovery",
                object_id = recovery.id
            };

            var lossList = new List<CommodityCode>();//领用商品列表
            var changeList = new List<CommodityInventoryChange>();//商品库存变更记录列表

            baseDataCommodityCode.body.objects.ForEach(commodityCode =>
            {
                //为0标识为出库，即对应回收下架
                if (commodityCode.operate_type == 0) { lossList.Add(commodityCode); }
            });
            //当正常数量等于0说明未从智能柜领用商品
            if (lossList.Count > 0)
            {
                lossList.ForEach(loss =>
                {
                    changeList.Add(new CommodityInventoryChange()
                    {
                        CommodityCodeId = loss.id,//商品码
                        SourceBill = sourceBill,//来源单据
                        ChangeStatus = CommodityInventoryChangeStatus.待回收.ToString(),//变更后状态
                        StoreHouseId = recovery.StoreHouse,//变更库房
                        EquipmentId = null,//变更后设备
                        GoodsLocationId = null//变更后货位
                    });
                });
            }

            var changes = CommodityInventoryChangeBll.GetInstance().CreateCommodityInventoryChange(changeList);

            //校验数据是否正常
            HttpHelper.GetInstance().ResultCheck(changes, out bool isSuccess2);
            if (isSuccess2)
            {
                return new BasePostData<CommodityInventoryChange>()
                {
                    code = changes.code,
                    message = changes.message
                };
            }
            return changes;


        }
    }
}
