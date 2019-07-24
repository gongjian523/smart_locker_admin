using CFLMedCab.Http.Enum;
using CFLMedCab.Http.Helper;
using CFLMedCab.Http.Model;
using CFLMedCab.Http.Model.Base;
using CFLMedCab.Http.Model.param;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CFLMedCab.Http.Bll
{
    public class InventoryTaskBll : BaseBll<InventoryTaskBll>
    {
        /// <summary>
        /// 根据盘点任务单号查询盘点任务信息
        /// </summary>
        /// <param name="taskName"></param>
        /// <returns></returns>
        public BaseData<InventoryTask> GetInventoryTaskByInventoryTaskName(string taskName)
        {
            if (null == taskName)
            {
                return new BaseData<InventoryTask>()
                {
                    code = (int)ResultCode.Parameter_Exception,
                    message = ResultCode.Parameter_Exception.ToString()
                };
            }
            //获取待盘点列表
            var task = HttpHelper.GetInstance().Get<InventoryTask>(new QueryParam
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
                                operands =  {$"'{ HttpUtility.UrlEncode(taskName) }'"}
                            },
                            new QueryParam.Expressions
                            {
                                field = "Status",
                                @operator = "==",
                                operands = {$"'{ HttpUtility.UrlEncode(InventoryTaskStatus.待盘点.ToString()) }'" }
                            }
                        }
                    }
                }
            });
            //校验是否含有数据，如果含有数据，拼接具体字段
            HttpHelper.GetInstance().ResultCheck(task, out bool isSuccess);

            return task;
        }
        /// <summary>
        /// 【手动盘点】获取盘点任务相关信息
        /// 通过【盘点任务名称】从表格InvoentoryTask【盘点任务】中查询获取盘点任务id。
        /// 通过【盘点任务单】（InventoryTask.id =InventoryOrder.InventoryTaskId）从表格【盘点单】中查询获得盘点单列列表
        /// taskName 盘点任务名称
        /// </summary>
        /// <param name="taskName"></param>
        /// <returns></returns>
        public BaseData<InventoryOrder> GetInventoryOrdersByInventoryTaskName(string taskName)
        {
            var task = GetInventoryTaskByInventoryTaskName(taskName);

            //校验是否含有数据，如果含有数据，拼接具体字段
            HttpHelper.GetInstance().ResultCheck(task, out bool isSuccess);

            var orders = new BaseData<InventoryOrder>();

            if (isSuccess)
            {

                //确认数据只有一条
                if (!string.IsNullOrEmpty(task.body.objects[0].id))//id = AQACQqweJ4wBAAAAXRA4vCD_sxWaDwQA
                {
                    orders = HttpHelper.GetInstance().Get<InventoryOrder>(new QueryParam
                    {
                        @in =
                        {
                            field = "InventoryTaskId",
                            in_list =  { HttpUtility.UrlEncode(task.body.objects[0].id) }
                        }
                    });
                    //校验是否含有数据，如果含有数据，拼接具体字段
                    HttpHelper.GetInstance().ResultCheck(orders, out bool isSuccess2);
                    if (isSuccess2)
                    {
                        
                        orders.body.objects.ForEach(order =>
                        {
                            //根据所在盘点货位获取所在货位名称
                            if (string.IsNullOrEmpty(order.GoodsLocationId))
                            {
                                order.GoodsLocationName = GetNameById<GoodsLocation>(order.GoodsLocationId);
                            }
                            //根据所在设备编号查询设备名称
                            if (string.IsNullOrEmpty(order.EquipmentId))
                            {
                                order.EquipmentName = GetNameById<Equipment>(order.EquipmentId);
                            }
                            //根据盘点库房编号查询盘点库房名称
                            if (string.IsNullOrEmpty(order.StoreHouseId))
                            {
                                order.StoreHouseName = GetNameById<StoreHouse>(order.StoreHouseId);
                            }
                        });
                    }
                }
            }
            else
            {
                orders.code = task.code;
                orders.message = task.message;
            }
            return orders;
        }

        /// <summary>
        /// 【手动盘点】 更新【盘点单管理理】的盘点状态为 ‘已确认’
        /// InventoryOrder关键字段：
        /// id 主键
        /// Status 盘点状态
        /// version 数据版本号【必须和当前数据保持一致】
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public BasePutData<InventoryOrder> updateInventoryOrderStatus(InventoryOrder order)
        {

            if (null == order || null == order.id || null == order.Status || null == order.version)
            {
                return new BasePutData<InventoryOrder>()
                {
                    code = (int)ResultCode.Parameter_Exception,
                    message = ResultCode.Parameter_Exception.ToString()
                };
            }
            var inventoryOrder = HttpHelper.GetInstance().Put<InventoryOrder>(new InventoryOrder()
            {
                id = order.id,
                Status = order.Status,
                version = order.version
            }) ;

            return inventoryOrder;
        }

        /// <summary>
        /// 【手动盘点】 逐一创建【盘点商品明细】。
        /// InventoryDetail所需字段：
        /// CommodityInventoryId 商品编码
        /// InventoryOrderId 关联盘点单
        /// Statis 质量状态 【正常 损坏】
        /// Type 类型 【账面存在 盘点缺失 盘点新增】
        /// </summary>
        /// <param name="details"></param>
        /// <returns></returns>
        public BasePostData<InventoryDetail> CreateInventoryDetail(List<InventoryDetail> details)
        {
            if(null == details || details.Count <=0)
            {
                return new BasePostData<InventoryDetail>()
                {
                    code = (int)ResultCode.Parameter_Exception,
                    message = ResultCode.Parameter_Exception.ToString()
                };
            }
            var inventoryDetails = HttpHelper.GetInstance().Post<InventoryDetail>(new PostParam<InventoryDetail>()
            {
                objects = details
            });

            return inventoryDetails;
        }
        public BaseData<Equipment> GetEquipmentByEquipmentNameOrId(string equipmentNameOrId)
        {
            if (null == equipmentNameOrId)
            {
                return new BaseData<Equipment>()
                {
                    code = (int)ResultCode.Parameter_Exception,
                    message = ResultCode.Parameter_Exception.ToString()
                };
            }
            //根据设备号或设备ID获取设备信息
            var equipment = HttpHelper.GetInstance().Get<Equipment>(new QueryParam
            {
                view_filter =
                {
                    filter =
                    {
                        logical_relation = "1 OR 2",
                        expressions =
                        {
                            new QueryParam.Expressions
                            {
                                field = "name",
                                @operator = "==",
                                operands =  {$"'{ HttpUtility.UrlEncode(equipmentNameOrId) }'"}
                            },
                            new QueryParam.Expressions
                            {
                                field = "id",
                                @operator = "==",
                                operands = {$"'{ HttpUtility.UrlEncode(equipmentNameOrId) }'" }
                            }
                        }
                    }
                }
            });
            //校验是否含有数据，如果含有数据，拼接具体字段
            HttpHelper.GetInstance().ResultCheck(equipment, out bool isSuccess);

            return equipment;
        }
        /// <summary>
        /// ⾃自动盘点：
        /// • 通过当前【设备编码/id】从表格 【设备管理】（Equipment）中查询获取设备的’⾃自动盘点计划’（InventoryPlanId）。
        /// • 通过当前【id】（InventoryPlan.id=Equipment.InventoryPlanId）从表格【盘点计划管理理】（InventoryPlan）中查询获取相关盘点信息。
        /// </summary>
        /// <param name="equipmentNameOrId"></param>
        /// <returns></returns>
        public BaseData<InventoryPlan> GetInventoryPlanByEquipmnetNameOrId(string equipmentNameOrId)
        {
            //根据设备编码或ID获取设备信息
            var equipment = GetEquipmentByEquipmentNameOrId(equipmentNameOrId);
            //校验是否含有数据，如果含有数据，进行后续操作
            HttpHelper.GetInstance().ResultCheck(equipment, out bool isSuccess);
            var plans = new BaseData<InventoryPlan>();
            if (isSuccess)
            {
                //根据自动盘点计划Id查询盘点计划相关信息
                if (!string.IsNullOrEmpty(equipment.body.objects[0].InventoryPlanId))
                {
                    plans = HttpHelper.GetInstance().Get<InventoryPlan>(new QueryParam
                    {
                        @in =
                        {
                            field = "InventoryTaskId",
                            in_list =  { HttpUtility.UrlEncode(equipment.body.objects[0].InventoryPlanId) }
                        }
                    });
                    //校验是否含有数据，如果含有数据，拼接具体字段
                    HttpHelper.GetInstance().ResultCheck(plans, out bool isSuccess2);
                }
            }
            else {
                plans.code = equipment.code;
                plans.message = equipment.message;
            }

            return plans;
        }
        /// <summary>
        /// 【智能柜】创建【盘点单管理理】返回获得id
        /// </summary>
        /// <param name="orders"></param>
        /// <returns></returns>
        public BasePostData<InventoryOrder> CreateInventoryOrder(List<InventoryOrder> orders)
        {
            if (null == orders || orders.Count <= 0)
            {
                return new BasePostData<InventoryOrder>()
                {
                    code = (int)ResultCode.Parameter_Exception,
                    message = ResultCode.Parameter_Exception.ToString()
                };
            }
            var inventoryOrders = HttpHelper.GetInstance().Post<InventoryOrder>(new PostParam<InventoryOrder>()
            {
                objects = orders
            });

            return inventoryOrders;
        }


    }
}
