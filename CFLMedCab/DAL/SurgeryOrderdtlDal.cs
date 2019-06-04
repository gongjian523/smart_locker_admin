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
    public class SurgeryOrderdtlDal
    {
        /// <summary>
        /// 数据库没有表时创建
        /// </summary>
        public void CreateTable_SurgeryOrderdtl()
        {
            string commandText = @"CREATE TABLE if not exists surgery_orderdtl ('id' INTEGER PRIMARY KEY AUTOINCREMENT, 
                                                                   'surgery_order_id'  BIGINT,
                                                                   'name' VARCHAR(50),
                                                                   'goods_id' INTEGER,
                                                                   'fetch_type' INTEGER,
                                                                   'number' INTEGER,
                                                                   'remarks' VARCHAR(200));";
            SqliteHelper.Instance.ExecuteNonQuery(commandText);
            return;
        }

        /// <summary>
        /// 删表
        /// </summary>
        public void dropTable()
        {
            string sql = "DROP TABLE surgery_orderdtl";
            SqliteHelper.Instance.ExecuteNonQuery(sql);
            return;
        }

        /// <summary>
        /// 新增手术单详情数据
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public int InsertNewSurgeryOrderdtl(SurgeryOrderdtl surgeryOrderDtl)
        {
            string commandText = string.Format(@"INSERT INTO surgery_orderdtl (surgery_order_id,name,goods_id,fetch_type,number,remarks) VALUES 
                                                ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}')",
                                                 surgeryOrderDtl.surgery_order_id, surgeryOrderDtl.name,surgeryOrderDtl.goods_id, surgeryOrderDtl.fetch_type, surgeryOrderDtl.number, surgeryOrderDtl.remarks);

            if (!SqliteHelper.Instance.ExecuteNonQuery(commandText))
                return 0;


            return LastInsertRowId();
        }

        /// <summary>
        /// 根据手术编号查询手术单详情
        /// </summary>
        /// <param name="id">手术编号</param>
        /// <returns></returns>
        public List<SurgeryOrderdtl> GetAllSurgeryOrderdtl(int id)
        {
            string commandText1= string.Format(@"SELECT * FROM surgery_orderdtl");
            IDataReader data1 = SqliteHelper.Instance.ExecuteReader(commandText1);
            
            List<SurgeryOrderdtl> dataList = new List<SurgeryOrderdtl>();
            IDataReader data = SqliteHelper.Instance.ExecuteReader(string.Format(@"SELECT * FROM surgery_orderdtl WHERE surgery_order_id = {0}", id));
            if (data == null)
                return dataList;
            while (data.Read())
            {
                SurgeryOrderdtl entity = new SurgeryOrderdtl();
                entity.id = Convert.ToInt32(data["Id"].ToString());
                entity.fetch_type = Convert.ToInt32(data["fetch_type"]);
                entity.name = data["name"].ToString();
                entity.number = Convert.ToInt32(data["number"]);
                entity.remarks = data["remarks"].ToString();
                entity.surgery_order_id = Convert.ToInt32(data["surgery_order_id"]);
                entity.goods_id = Convert.ToInt32(data["goods_id"]);
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
