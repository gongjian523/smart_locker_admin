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
    public class ReplenishSubOrderdtlDal
    {
        /// <summary>
        /// 数据库没有表时创建
        /// </summary>
        public void CreateTable_ReplenishSubOrderdtl()
        {
            string commandText = @"CREATE TABLE if not exists replenish_sub_orderdtl ('id' INTEGER PRIMARY KEY AUTOINCREMENT, 
                                                                   'goods_id' BIGINT,
                                                                   'name' VARCHAR(50),
                                                                   'goods_code' VARCHAR(50), 
                                                                   'code' VARCHAR(50),
                                                                   'batch_number' VARCHAR(50),
                                                                   'birth_date' not null default (datetime('localtime')),
                                                                   'expire_date' not null default (datetime('localtime')),
                                                                   'valid_period' INTEGER,
                                                                   'position' VARCHAR(50),
                                                                   'fetch_type' INTEGER,
                                                                   'remarks' VARCHAR(200),
                                                                   'status' INTEGER,
                                                                   'replenish_sub_orderid' BIGINT);";
            SqliteHelper.Instance.ExecuteNonQuery(commandText);
            return;
        }

        /// <summary>
        /// 新增上货单
        /// </summary>
        /// <param name="pickingOrder"></param>
        /// <returns></returns>
        public int InsertNewReplenishSubOrderdtl(ReplenishSubOrderdtl pickingSubOrderdtl)
        {
            string commandText = string.Format(@"INSERT INTO replenish_sub_orderdtl (goods_id, name, goods_code, code,batch_number,birth_date,expire_date,valid_period,position,fetch_type,remarks,status,replenish_sub_orderid) VALUES 
                                                ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}')",
                                               pickingSubOrderdtl.goods_id, pickingSubOrderdtl.name, pickingSubOrderdtl.goods_code, pickingSubOrderdtl.code, pickingSubOrderdtl.batch_number, pickingSubOrderdtl.birth_date, pickingSubOrderdtl.expire_date,
                                               pickingSubOrderdtl.valid_period, pickingSubOrderdtl.position, pickingSubOrderdtl.fetch_type, pickingSubOrderdtl.remarks, pickingSubOrderdtl.status, pickingSubOrderdtl.replenish_sub_orderid);

            if (!SqliteHelper.Instance.ExecuteNonQuery(commandText))
                return 0;


            return LastInsertRowId();
        }

        /// <summary>
        /// 查询待拣货
        /// </summary>
        /// <returns></returns>
        public List<ReplenishSubOrderdtl> GetPickingSubOrderdtl(int id)
        {
            List<ReplenishSubOrderdtl> replenishSubOrderdtls = new List<ReplenishSubOrderdtl>();
            IDataReader data = SqliteHelper.Instance.ExecuteReader(string.Format(@"SELECT * FROM replenish_sub_orderdtl where replenish_sub_orderid={0} and status=0",id));
            if (data == null)
                return replenishSubOrderdtls;
            while (data.Read())
            {
                ReplenishSubOrderdtl replenishSubOrderdtl = new ReplenishSubOrderdtl();
                replenishSubOrderdtl.id = Convert.ToInt32(data["id"]);
                replenishSubOrderdtl.goods_code = data["goods_code"].ToString();
                replenishSubOrderdtl.name = data["name"].ToString();
                replenishSubOrderdtl.code = data["code"].ToString();
                replenishSubOrderdtl.position = data["position"].ToString();
                replenishSubOrderdtl.expire_date = Convert.ToDateTime(data["expire_date"]);
                replenishSubOrderdtl.fetch_type = Convert.ToInt32(data["fetch_type"]);
                replenishSubOrderdtl.remarks = data["remarks"].ToString();
                replenishSubOrderdtls.Add(replenishSubOrderdtl);
            }
            return replenishSubOrderdtls;
        }

        private int LastInsertRowId()
        {
            return Convert.ToInt16(SqliteHelper.Instance.ExecuteScalar("SELECT last_insert_rowid();"));
        }
    }
}
