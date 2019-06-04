using CFLMedCab.Infrastructure.SqliteHelper;
using CFLMedCab.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.DAL
{
    public class PickingSubOrderDal
    {
        /// <summary>
        /// 数据库没有表时创建
        /// </summary>
        public void CreateTable_PickingSubOrder()
        {
            string commandText = @"CREATE TABLE if not exists picking_order ('id' INTEGER PRIMARY KEY AUTOINCREMENT, 
                                                                   'picking_order_id' INTEGER,
                                                                   'create_time' not null default (datetime('localtime')),
                                                                   'end_time' not null default (datetime('localtime')),
                                                                   'position' VARCHAR(50),
                                                                   'status' INTEGER,
                                                                   'Inspection_order_id' BIGINT);";
            SqliteHelper.Instance.ExecuteNonQuery(commandText);
            return;
        }

        /// <summary>
        /// 新增拣货单
        /// </summary>
        /// <param name="pickingOrder"></param>
        /// <returns></returns>
        public int InsertNewPickingSubOrder(PickingSubOrder pickingSubOrder)
        {
            string commandText = string.Format(@"INSERT INTO picking_order (picking_order_id, create_time, end_time, position,status,Inspection_order_id) VALUES 
                                                ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}')",
                                                pickingSubOrder.picking_order_id, pickingSubOrder.create_time, pickingSubOrder.end_time, pickingSubOrder.position, pickingSubOrder.status, pickingSubOrder.Inspection_order_id);

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
