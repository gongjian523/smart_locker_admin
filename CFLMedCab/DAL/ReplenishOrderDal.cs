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
    public class ReplenishOrderDal
    {
        ReplenishSubOrderdtlDal replenishSubOrderdtlDal = new ReplenishSubOrderdtlDal();
        /// <summary>
        /// 数据库没有表时创建
        /// </summary>
        public void CreateTable_ReplenishOrder()
        {
            string commandText = @"CREATE TABLE if not exists replenish_order ('id' INTEGER PRIMARY KEY AUTOINCREMENT, 
                                                                   'principal_id' INTEGER,
                                                                   'create_time' not null default (datetime('localtime')),
                                                                   'status' INTEGER,
                                                                   'end_time' not null default (datetime('localtime')));";
            SqliteHelper.Instance.ExecuteNonQuery(commandText);
            return;
        }

        /// <summary>
        /// 新增上架工单
        /// </summary>
        /// <param name="replenishOrder"></param>
        /// <returns></returns>
        public int InsertNewReplenishOrder(ReplenishOrder replenishOrder)
        {
            string commandText = string.Format(@"INSERT INTO replenish_order (create_time, operator_id, end_time, status) VALUES 
                                                ('{0}', '{1}', '{2}', '{3}')",
                                                replenishOrder.create_time, replenishOrder.principal_id, replenishOrder.end_time, replenishOrder.status);

            if (!SqliteHelper.Instance.ExecuteNonQuery(commandText))
                return 0;


            return LastInsertRowId();
        }

        /// <summary>
        /// 获取所有上架单
        /// </summary>
        /// <returns></returns>
        public List<ReplenishOrderView> GetAllReplenishOrder()
        {
            List<ReplenishOrderView> replenishOrderView = new List<ReplenishOrderView>();
            IDataReader data = SqliteHelper.Instance.ExecuteReader(string.Format(@"SELECT a.id,a.principal_id,a.create_time,b.id bid FROM replenish_order a left join replenish_sub_order b on a.id=b.replenish_order_id"));
            if (data == null)
                return replenishOrderView;
            while (data.Read())
            {
                ReplenishOrderView replenishSubOrderdtl = new ReplenishOrderView();
                replenishSubOrderdtl.id = Convert.ToInt32(data["id"]);
                replenishSubOrderdtl.principal_id = Convert.ToInt32(data["principal_id"]);
                replenishSubOrderdtl.create_time = Convert.ToDateTime(data["create_time"]);
                object hh = data["bid"].ToString();
                if (data["bid"].ToString()!=null&& data["bid"].ToString() != "")
                    replenishSubOrderdtl.replenishSubOrderdtls = replenishSubOrderdtlDal.GetPickingSubOrderdtl(Convert.ToInt32(data["bid"]));
                replenishOrderView.Add(replenishSubOrderdtl);
            }

            return replenishOrderView;
        }

        private int LastInsertRowId()
        {
            return Convert.ToInt16(SqliteHelper.Instance.ExecuteScalar("SELECT last_insert_rowid();"));
        }
    }
}
