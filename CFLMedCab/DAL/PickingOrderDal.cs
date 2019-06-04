using CFLMedCab.Infrastructure.SqliteHelper;
using CFLMedCab.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.DAL
{
    public class PickingOrderDal
    {
        /// <summary>
        /// 数据库没有表时创建
        /// </summary>
        public void CreateTable_PickingOrder()
        {
            string commandText = @"CREATE TABLE if not exists picking_order ('id' INTEGER PRIMARY KEY AUTOINCREMENT, 
                                                                   'principal_id' INTEGER,
                                                                   'create_time' not null default (datetime('localtime')),
                                                                   'status' INTEGER,
                                                                   'end_time' not null default (datetime('localtime')));";
            SqliteHelper.Instance.ExecuteNonQuery(commandText);
            return;
        }

        /// <summary>
        /// 新增拣货工单
        /// </summary>
        /// <param name="pickingOrder"></param>
        /// <returns></returns>
        public int InsertNewPickingOrder(PickingOrder pickingOrder)
        {
            string commandText = string.Format(@"INSERT INTO picking_order (create_time, operator_id, end_time, status) VALUES 
                                                ('{0}', '{1}', '{2}', '{3}')",
                                                pickingOrder.create_time, pickingOrder.principal_id, pickingOrder.end_time, pickingOrder.status);

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
