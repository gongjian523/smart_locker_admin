using CFLMedCab.Infrastructure.SqliteHelper;
using CFLMedCab.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.DAL
{
    public class FetchOrderDal
    {
        public void CreateTable_FetchOrder()
        {
            string commandText = @"CREATE TABLE if not exists fetch_order ('id' INTEGER PRIMARY KEY AUTOINCREMENT, 
                                                                   'create_time' not null default (datetime('localtime')),
                                                                   'operator_id' INTEGER,
                                                                   'type' INTEGER, 
                                                                   'status' INTEGER,  
                                                                   'business_order_id' INTEGER;";
            SqliteHelper.Instance.ExecuteNonQuery(commandText);
            return;
        }

        public int InsertNewFetchOrder(FetchOrder fetchOrder)
        {
            string commandText = string.Format(@"INSERT INTO fetch_order (create_time, operator_id, type, status, business_order_id) VALUES 
                                                ('{0}', {1}, '{2}', '{3}', '{4}')",
                                                fetchOrder.create_time, fetchOrder.operator_id, fetchOrder.type, fetchOrder.status, fetchOrder.business_order_id);

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
