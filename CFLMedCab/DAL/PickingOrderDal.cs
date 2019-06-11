using CFLMedCab.Infrastructure.SqliteHelper;
using CFLMedCab.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.DAL
{
    public class PickingOrderDal
    {
        PickingSubOrderdtlDal pickingSubOrderdtlDal = new PickingSubOrderdtlDal();
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

        /// <summary>
        /// 获取所有拣货工单
        /// </summary>
        /// <returns></returns>
        public List<PickingOrder> GetAllPickingOrder()
        {
            List<PickingOrder> pickingOrders = new List<PickingOrder>();
            IDataReader data = SqliteHelper.Instance.ExecuteReader(string.Format(@"SELECT id,principal_id,create_time FROM picking_order where status=0"));
            if (data == null)
                return pickingOrders;
            while (data.Read())
            {
                PickingOrder pickingOrder = new PickingOrder();
                pickingOrder.id = Convert.ToInt32(data["id"]);
                pickingOrder.principal_id = Convert.ToInt32(data["principal_id"]);
                pickingOrder.create_time = Convert.ToDateTime(data["create_time"]);
                pickingOrders.Add(pickingOrder);
            }
            
            return pickingOrders;
        }

        private int LastInsertRowId()
        {
            return Convert.ToInt16(SqliteHelper.Instance.ExecuteScalar("SELECT last_insert_rowid();"));
        }
    }
}
