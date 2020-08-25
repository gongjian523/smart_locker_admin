using CFLMedCab.Http.Enum;
using CFLMedCab.Http.Helper;
using CFLMedCab.Http.Model;
using CFLMedCab.Http.Model.Base;
using CFLMedCab.Http.Model.Common;
using CFLMedCab.Http.Model.Enum;
using CFLMedCab.Http.Model.param;
using CFLMedCab.Infrastructure;
using CFLMedCab.Infrastructure.ToolHelper;
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
                        logical_relation = "1 AND 2 AND",
                        expressions =
                        {
                            new QueryParam.Expressions
                            {
                                field = "name",
                                @operator = "==",
                                operands =  {$"'{ HttpUtility.UrlEncode(commodityRecoveryName) }'"}
                            },
                            new QueryParam.Expressions
                            {
                                field = "Operator",
                                @operator = "==",
                                operands = {$"'{ HttpUtility.UrlEncode(ApplicationState.GetUserInfo().id)}'"}
                            },
                            new QueryParam.Expressions
                            {
                                field = "StoreHouse",
                                @operator = "==",
                                operands = {$"'{ HttpUtility.UrlEncode(ApplicationState.GetHouseId())}'"}
                            },
                        }
                    }
                }
            });
            //校验是否含有数据，如果含有数据，拼接具体字段
            recovery = HttpHelper.GetInstance().ResultCheck(recovery, out bool isSuccess);

            if (!isSuccess)
            {
                recovery.code = (int)ResultCode.Result_Exception;
                recovery.message = ResultCode.Result_Exception.ToString();
            }
            else
            {
                //如果领⽤单作废标识为【是】则弹窗提醒手术单作废，跳转回前⻚
                if ("已完成".Equals(recovery.body.objects[0].Status) || "已撤销".Equals(recovery.body.objects[0].Status))
                {
                    recovery.code = (int)ResultCode.Task_Exception;
                    recovery.message = ResultCode.Task_Exception.ToString();
                }
            }

            return recovery;
        }

        /// <summary>
        /// 根据拣货单号获取商品详情（属于这个）
        /// </summary>
        /// <param name="commodityRecovery"></param>
        /// <returns></returns>
        public BaseData<CommodityRecoveryDetail> GetCommodityRecoveryDetail(CommodityRecovery commodityRecovery)
        {
            BaseData<CommodityRecoveryDetail> bdCommodityRecoveryDetail = HttpHelper.GetInstance().Get<CommodityRecoveryDetail>(new QueryParam
            {
                view_filter =
                {
                    filter =
                    {
                        logical_relation = "1 AND 2 AND 3",
                        expressions =
                        {
                            new QueryParam.Expressions
                            {
                                field = "CommodityRecoveryId",
                                @operator = "==",
                                operands =  {$"'{ HttpUtility.UrlEncode(commodityRecovery.id) }'"}
                            },
                            new QueryParam.Expressions
                            {
                                field = "EquipmentId",
                                @operator = "==",
                                operands = {$"'{ HttpUtility.UrlEncode(ApplicationState.GetValue<string>((int)ApplicationKey.EquipId)) }'" }
                            },
                            new QueryParam.Expressions
                            {
                                field = "StoreHouseId",
                                @operator = "==",
                                operands = {$"'{ HttpUtility.UrlEncode(ApplicationState.GetValue<string>((int)ApplicationKey.HouseId)) }'" }
                            }
                        }
                    }
                }

            });

            //校验是否含有数据，如果含有数据，拼接具体字段
            HttpHelper.GetInstance().ResultCheck(bdCommodityRecoveryDetail, out bool isSuccess);

            if (isSuccess)
            {
                bdCommodityRecoveryDetail.body.objects.ForEach(it =>
                {
                    //拼接设备名字
                    if (!string.IsNullOrEmpty(it.EquipmentId))
                    {
                        it.EquipmentName = GetNameById<Equipment>(it.EquipmentId);
                    }

                    //拼接库房名字
                    if (!string.IsNullOrEmpty(it.StoreHouseId))
                    {
                        it.StoreHouseName = GetNameById<StoreHouse>(it.StoreHouseId);
                    }

                    //拼接货位名字
                    if (!string.IsNullOrEmpty(it.GoodsLocationId))
                    {
                        it.GoodsLocationName = GetNameById<GoodsLocation>(it.GoodsLocationId);
                    }
                    
                    BaseData<CommodityCode> bdCommodityCode = GetCommodityCode(it.CommodityCodeId);
                    if(bdCommodityCode.code == (int)ResultCode.OK)
                    {
                        //拼接厂商
                        if (string.IsNullOrEmpty(it.ManufactorName))
                        {
                            it.ManufactorName = bdCommodityCode.body.objects[0].ManufactorName;
                        }
                        //拼接型号
                        if (string.IsNullOrEmpty(it.Model))
                        {
                            it.Model = bdCommodityCode.body.objects[0].Model;
                        }
                        //拼接规格
                        if (string.IsNullOrEmpty(it.Specifications))
                        {
                            it.Specifications = bdCommodityCode.body.objects[0].Spec;
                        }

                        //拼接RF码
                        it.CommodityCodeName = it.name;

                        //拼接商品名称
                        if (!string.IsNullOrEmpty(bdCommodityCode.body.objects[0].CommodityId))
                        {
                            it.CommodityName = GetNameById<Commodity>(bdCommodityCode.body.objects[0].CommodityId); 
                        }

                    }
                });
            }

            return bdCommodityRecoveryDetail;

        }



        /// <summary>
        /// 获取变化后的上架单
        /// </summary>
        /// <param name="bdCommodityCode"></param>
        /// <param name="baseDataShelfTask"></param>
        /// <param name="bdCommodityRecoveryDetail"></param>
        /// <returns></returns>
        public void GetCommodityRecoveryChange(BaseData<CommodityCode> bdCommodityCode, CommodityRecovery commodityRecovery, BaseData<CommodityRecoveryDetail> bdCommodityRecoveryDetail)
        {
            HttpHelper.GetInstance().ResultCheck(bdCommodityRecoveryDetail, out bool isSuccess);
            HttpHelper.GetInstance().ResultCheck(bdCommodityCode, out bool isSuccess1);

            if (isSuccess && isSuccess1)
            {
                List<string> locIds = bdCommodityCode.body.objects.Select(item => item.GoodsLocationId).Distinct().ToList();
                List<string> status = new List<string>();

                locIds.ForEach(id => {

                    //上架任务单商品RF码id
                    var sfdCommodityCodeIds = bdCommodityRecoveryDetail.body.objects.Where(item => item.GoodsLocationId == id).Select(it => it.CommodityCodeId).Distinct().ToList();

                    var commodityCodes = bdCommodityCode.body.objects.Where(item => item.GoodsLocationId == id).ToList();

                    //商品异常状态回显
                    commodityCodes.ForEach(it => {
                        if (it.operate_type == (int)OperateType.入库)
                        {
                            it.AbnormalDisplay = AbnormalDisplay.异常.ToString();
                        }
                        else
                        {
                            if (sfdCommodityCodeIds.Contains(it.id))
                            {
                                it.AbnormalDisplay = AbnormalDisplay.正常.ToString();
                            }
                            else
                            {
                                it.AbnormalDisplay = AbnormalDisplay.异常.ToString();
                            }
                        }
                    });
                });

                if (bdCommodityCode.body.objects.Where(item => item.operate_type == (int)OperateType.入库).Count() > 0)
                {
                    //包含了出库商品，状态单就显示异常
                    commodityRecovery.Status = DocumentStatus.异常.ToString();
                }
                else if (bdCommodityCode.body.objects.Where(item => item.operate_type == (int)OperateType.出库 && item.AbnormalDisplay == AbnormalDisplay.异常.ToString()).Count() > 0)
                {
                    //包含了出库商品，状态单就显示异常
                    commodityRecovery.Status = DocumentStatus.异常.ToString();
                }
                else
                {
                    int needPickNum = bdCommodityRecoveryDetail.body.objects.Where(item => item.Status == AllotShelfCommodityStatus.未上架.ToString()).Count();
                    int normalPickNum = bdCommodityCode.body.objects.Where(item => item.operate_type == (int)OperateType.出库 && item.AbnormalDisplay == AbnormalDisplay.正常.ToString()).Count();

                    if (normalPickNum >= needPickNum)
                    {
                        //正常拣货的商品数量超过或者等于（实际上不应该超过）需要拣货的商品数量，状态设置成已完成
                        commodityRecovery.Status = DocumentStatus.已完成.ToString();
                        //这个状态在提交前还需要最后的修改，判断这个任务单在其他设备上是否完成
                    }
                    else
                    {
                        commodityRecovery.Status = DocumentStatus.进行中.ToString();
                    }
                }
            }
        }


        /// <summary>
        /// 更新上架任务单
        /// </summary>
        /// <param name="commodityRecovery"></param>
        /// <param name="abnormalCauses"></param>
        /// <param name="bAutoSubmit">是否是主动提交</param>
        /// <returns></returns>
        public BasePutData<CommodityRecovery> PutCommodityRecovery(CommodityRecovery commodityRecovery)
        {

            CommodityRecovery task = new CommodityRecovery
            {
                id = commodityRecovery.id,
                Status = commodityRecovery.Status,
                version = commodityRecovery.version
            };

            if (commodityRecovery.Status == CommodityRecoveryStatusEnum.异常.ToString())
            {
                if(commodityRecovery.AbnormalCauses != "")
                {
                    task.AbnormalCauses = commodityRecovery.AbnormalCauses;
                }
            }
            else if(commodityRecovery.Status == CommodityRecoveryStatusEnum.已完成.ToString())
            {
                if(IsCommodityRecoveryCompleteInOtherLocation(commodityRecovery))
                {
                    task.FinishDate = GetDateTimeNow();
                }
                else
                {
                    task.Status = CommodityRecoveryStatusEnum.进行中.ToString();
                }
            }

            BasePutData<CommodityRecovery> basePutData = HttpHelper.GetInstance().Put(task);

            if (basePutData.code != 0)
            {
                LogUtils.Error("PutCommodityRecovery 失败！ " + basePutData.message);
            }
            return basePutData;
        }


        /// <summary>
        /// 根据拣货单号获取商品详情
        /// </summary>
        /// <param name="pickTaskName"></param>
        /// <returns></returns>
        public bool IsCommodityRecoveryCompleteInOtherLocation(CommodityRecovery commodityRecovery)
        {
            BaseData<CommodityRecoveryDetail> bdCommodityRecoveryDetail = HttpHelper.GetInstance().Get<CommodityRecoveryDetail>(new QueryParam
            {
                view_filter =
                {
                    filter =
                    {
                        logical_relation = "1",
                        expressions =
                        {
                            new QueryParam.Expressions
                            {
                                field = "CommodityRecoveryId",
                                @operator = "==",
                                operands =  {$"'{ HttpUtility.UrlEncode(commodityRecovery.id) }'"}
                            },
                        }
                    }
                }

            });

            //校验是否含有数据，如果含有数据，拼接具体字段
            HttpHelper.GetInstance().ResultCheck(bdCommodityRecoveryDetail, out bool isSuccess);

            if (isSuccess)
            {
                //在其他货柜中的还有待回收的物品
                if(bdCommodityRecoveryDetail.body.objects.Where(item => (item.StoreHouseId !=  ApplicationState.GetHouseId() || item.EquipmentId != ApplicationState.GetEquipId()) && item.Status == "待回收").Count() > 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="recovery"></param>
        /// <returns></returns>

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
                        operate_type = loss.operate_type,
                        StoreHouseId = recovery.StoreHouse,//变更库房
                        EquipmentId = null,//变更后设备
                        GoodsLocationId = null//变更后货位
                    });
                });
            }

            return  CommodityInventoryChangeBll.GetInstance().CreateCommodityInventoryChangeSeparately(changeList);
        }
    }
}
