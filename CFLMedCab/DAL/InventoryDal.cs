using CFLMedCab.APO;
using CFLMedCab.APO.Inventory;
using CFLMedCab.DTO.Goodss;
using CFLMedCab.DTO.Inventory;
using CFLMedCab.DTO.Replenish;
using CFLMedCab.Infrastructure;
using CFLMedCab.Infrastructure.DbHelper;
using CFLMedCab.Model;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.DAL
{
    /// <summary>
    /// 盘点库存dao层
    /// </summary>
    public class InventoryDal
    {
        //Db
        public SqlSugarClient Db = SqlSugarHelper.GetInstance().Db;

        // 定义一个静态变量来保存类的实例
        private static InventoryDal singleton;
        // 定义一个标识确保线程同步
        private static readonly object locker = new object();


        //定义公有方法提供一个全局访问点。
        public static InventoryDal GetInstance()
        {
            //这里的lock其实使用的原理可以用一个词语来概括“互斥”这个概念也是操作系统的精髓
            //其实就是当一个进程进来访问的时候，其他进程便先挂起状态
            if (singleton == null)
            {
                lock (locker)
                {
                    // 如果类的实例不存在则创建，否则直接返回
                    if (singleton == null)
                    {
                        singleton = new InventoryDal();
                    }
                }
            }
            return singleton;
        }

        /// <summary>
        /// 生成盘点记录
        /// </summary>
        /// <returns></returns>
        public int NewInventory(InventoryOrder inventoryOrder) {
            return Db.Insertable<InventoryOrder>(inventoryOrder).ExecuteReturnEntity().id;
        }

        /// <summary>
        /// 确认盘点记录
        /// </summary>
        public void ConfirmInventory(InventoryOrder inventoryOrder) {
            Db.Updateable<InventoryOrder>(inventoryOrder).ExecuteCommand();
        }


        /// <summary>
        /// 获取盘点记录
        /// </summary>
        /// <returns></returns>
        public List<InventoryOrderDto> GetInventoryOrder(InventoryOrderApo pageDataApo, out int totalCount)
        {
            totalCount = 0;
            List<InventoryOrderDto> data;

            //查询语句
            var queryable = Db.Queryable<InventoryOrder>()
                .Where(it => it.status == pageDataApo.status)
                .OrderBy(it => it.create_time, OrderByType.Desc)
                .Select<InventoryOrderDto>();


            //如果小于0，默认查全部
            if (pageDataApo.PageSize > 0)
            {
                data = queryable.ToPageList(pageDataApo.PageIndex, pageDataApo.PageSize, ref totalCount);
            }
            else
            {
                data = queryable.ToList();
                totalCount = data.Count();
            }
            return data;
        }

        /// <summary>
        /// 插入盘点记录详情（列表）
        /// </summary>
        /// <returns></returns>
        public void InsertInventoryDetails(List<InventoryOrderdtl> list)
        {
            Db.Insertable<InventoryOrderdtl>(list).ExecuteCommand();
        }
        /// <summary>
        /// 插入盘点记录详情（单个）
        /// </summary>
        /// <returns></returns>
        public void InsertInventoryDetails(InventoryOrderdtl item)
        {
            Db.Insertable<InventoryOrderdtl>(item).ExecuteCommand();
        }

        /// <summary>
        /// 更新盘点记录详情(列表)
        /// </summary>
        /// <returns></returns>
        public void UpdateInventoryDetails(List<InventoryOrderdtl> list)
        {
            Db.Updateable<InventoryOrderdtl>(list).ExecuteCommand();
        }
        /// <summary>
        /// 更新盘点记录详情(单个)
        /// </summary>
        /// <returns></returns>
        public void UpdateInventoryDetails(InventoryOrderdtl item)
        {
            Db.Updateable<InventoryOrderdtl>(item).ExecuteCommand();
        }


        public List<InventoryOrderdtl> GetInventoryDetailsByInventoryId(int invertoryOrderId)
        {
            var list =  Db.Queryable<InventoryOrderdtl>()
                .Where(it => it.inventory_order_id == invertoryOrderId)
                .OrderBy(it => it.name, OrderByType.Desc).ToList();
            return list;
        }

        /// <summary>
        /// 插入盘点计划
        /// </summary>
        /// <returns></returns>
        public void InsertInventoryPlan(List<InventoryPlanLDB> list)
        {
            Db.Insertable<InventoryPlanLDB>(list).ExecuteCommand();
        }

        /// <summary>
        /// 更新盘点计划(列表)
        /// </summary>
        /// <returns></returns>
        public void UpdateInventoryPlan(List<InventoryPlanLDB> list)
        {
            Db.Updateable<InventoryPlanLDB>(list).ExecuteCommand();
        }

        /// <summary>
        /// 更新盘点计划(单个）
        /// </summary>
        /// <returns></returns>
        public void UpdateInventoryPlan(InventoryPlanLDB item)
        {
            Db.Updateable<InventoryPlanLDB>(item).ExecuteCommand();
        }

        /// <summary>
        /// 获取盘点计划
        /// </summary>
        /// <returns></returns>
        public List<InventoryPlanLDB> GetInventoryPlan()
        {
            return Db.Queryable<InventoryPlanLDB>().OrderBy(it => it.inventorytime_str, OrderByType.Desc).ToList();
        }

        ///// <summary>
        ///// 获取盘点数据
        ///// </summary>
        //public List<GoodsDto> GetGoods(BasePageDataApo pageDataApo, out int totalCount)
        //{

        //    totalCount = 0;
        //    List<GoodsDto> data;
        //    //查询语句
        //    var queryable = Db.Queryable<GoodsDto>();

        //    data = queryable.ToPageList(pageDataApo.PageIndex, pageDataApo.PageSize, ref totalCount);

        //    return data;
        //}
    }
}
