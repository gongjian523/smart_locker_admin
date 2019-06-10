using CFLMedCab.Infrastructure.SqliteHelper;
using CFLMedCab.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.DAL
{
    public class ReplenishSubOrderDal
    {
        /// <summary>
        /// 数据库没有表时创建
        /// </summary>
        public void CreateTable_ReplenishSubOrder()
        {
            string commandText = @"CREATE TABLE if not exists replenish_sub_order ('id' INTEGER PRIMARY KEY AUTOINCREMENT, 
                                                                   'replenish_order_id' INTEGER,
                                                                   'create_time' not null default (datetime('localtime')),
                                                                   'end_time' not null default (datetime('localtime')),
                                                                   'position' VARCHAR(50),
                                                                   'status' INTEGER,
                                                                   'Inspection_order_id' BIGINT);";
            SqliteHelper.Instance.ExecuteNonQuery(commandText);
            return;
        }

        /// <summary>
        /// 新增上架单
        /// </summary>
        /// <param name="pickingOrder"></param>
        /// <returns></returns>
        public int InsertNewPickingSubOrder(ReplenishSubOrder replenishSubOrder)
        {
            string commandText = string.Format(@"INSERT INTO replenish_sub_order (replenish_order_id, create_time, end_time, position,status,Inspection_order_id) VALUES 
                                                ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}')",
                                                replenishSubOrder.replenish_order_id, replenishSubOrder.create_time, replenishSubOrder.end_time, replenishSubOrder.position, replenishSubOrder.status, replenishSubOrder.Inspection_order_id);

            if (!SqliteHelper.Instance.ExecuteNonQuery(commandText))
                return 0;


            return LastInsertRowId();
        }

        private int LastInsertRowId()
        {
            return Convert.ToInt16(SqliteHelper.Instance.ExecuteScalar("SELECT last_insert_rowid();"));
        }
    }
}
