using CFLMedCab.APO;
using CFLMedCab.DTO.Replenish;
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
        /// 手动盘点时生成盘点记录
        /// </summary>
        /// <returns></returns>
        public void addInventory(InventoryOrder inventory) {
            
             
            //获取当前时间
            inventory.create_time = System.DateTime.Now;
            //获取当前用户
            inventory.operator_id = 1;
            //设置状态为待确认
            inventory.status = 0;
            //生成记录
            Db.Insertable<InventoryOrder>(inventory).ExecuteReturnEntity();
            
        }
        /// <summary>
        /// 确认盘点记录
        /// </summary>
        public void confirmInventory(InventoryOrder inventoryOrder) {
            //设置状态为已确认
            inventoryOrder.status = 1;
            Db.Updateable<InventoryOrder>(inventoryOrder).ExecuteCommand();

        }

        /// <summary>
        /// 获取盘点数据
        /// </summary>
        public List<GoodsDto> GetGoods(BasePageDataApo pageDataApo, out int totalCount)
        {

            totalCount = 0;
            List<GoodsDto> data;
            //查询语句
            var queryable = Db.Queryable<GoodsDto>();

            data = queryable.ToPageList(pageDataApo.PageIndex, pageDataApo.PageSize, ref totalCount);

            return data;
        }
    }
}
