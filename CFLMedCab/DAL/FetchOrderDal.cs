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
    public class FetchOrderDal
    {
        /// <summary>
        /// 数据库没有表时创建
        /// </summary>
        public void CreateTable_FetchOrder()
        {
            string commandText = @"CREATE TABLE if not exists fetch_order ('id' INTEGER PRIMARY KEY AUTOINCREMENT, 
                                                                   'create_time' not null default (datetime('localtime')),
                                                                   'operator_id' INTEGER,
                                                                   'type' INTEGER, 
                                                                   'status' INTEGER,  
                                                                   'business_order_id' INTEGER);";
            SqliteHelper.Instance.ExecuteNonQuery(commandText);
            return;
        }

        /// <summary>
        /// 新增领用单
        /// </summary>
        /// <param name="fetchOrder"></param>
        /// <returns></returns>
        public int InsertNewFetchOrder(FetchOrder fetchOrder)
        {
            string commandText = string.Format(@"INSERT INTO fetch_order (create_time, operator_id, type, status, business_order_id) VALUES 
                                                ('{0}', '{1}', '{2}', '{3}', '{4}')",
                                                fetchOrder.create_time, fetchOrder.operator_id, fetchOrder.type, fetchOrder.status, fetchOrder.business_order_id);

            if (!SqliteHelper.Instance.ExecuteNonQuery(commandText))
                return 0;


            return LastInsertRowId();
        }

        /// <summary>
        /// 根据用户和领用类型查找领用单数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<FetchOrder> GetAllFetchOrder(int user,int type)
        {
            List<FetchOrder> dataList = new List<FetchOrder>();
            IDataReader data = SqliteHelper.Instance.ExecuteReader(string.Format(@"SELECT * FROM fetch_order WHERE operator_id = {0} and type = {1}", user, type));
            if (data == null)
                return dataList;
            while (data.Read())
            {
                FetchOrder entity = new FetchOrder();
                entity.id = Convert.ToInt32(data["Id"].ToString());
                entity.create_time = Convert.ToDateTime(data["create_time"]);
                entity.operator_id = Convert.ToInt32(data["operator_id"]);
                entity.type = Convert.ToInt32(data["type"]);
                entity.status = Convert.ToInt32(data["status"]);
                entity.business_order_id = Convert.ToInt32(data["business_order_id"]);
                dataList.Add(entity);
            }
            return dataList;
        }

        private int LastInsertRowId()
        {
            return Convert.ToInt16(SqliteHelper.Instance.ExecuteScalar("SELECT last_insert_rowid();"));
        }
    }
}
