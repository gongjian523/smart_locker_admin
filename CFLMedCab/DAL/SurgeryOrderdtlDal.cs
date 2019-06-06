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
        /// 根据手术编号查询待领取详情
        /// </summary>
        /// <param name="id">手术编号</param>
        /// <returns></returns>
        public DataTable GetAllTakeCollect(int id)
        {
            DataTable dataTable = new DataTable("Table_New");
            dataTable.Columns.Add("id", Type.GetType("System.String"));
            dataTable.Columns.Add("goods_code", Type.GetType("System.String"));
            dataTable.Columns.Add("name", Type.GetType("System.String"));
            dataTable.Columns.Add("code", Type.GetType("System.String"));
            dataTable.Columns.Add("batch_number", Type.GetType("System.String"));
            dataTable.Columns.Add("birth_date", Type.GetType("System.String"));
            dataTable.Columns.Add("expiry_date", Type.GetType("System.String"));
            dataTable.Columns.Add("fetch_type", Type.GetType("System.String"));
            dataTable.Columns.Add("remarks", Type.GetType("System.String"));
            IDataReader data = SqliteHelper.Instance.ExecuteReader(string.Format(@"SELECT a.id,b.goods_code,b.name,b.code,b.batch_number,b.birth_date,b.expiry_date,
                                                                                 a.fetch_type,b.remarks FROM surgery_orderdtl a LEFT JOIN goods b on a.goods_id=b.id 
                                                                                 WHERE surgery_order_id = {0}", id));
            if (data == null)
                return dataTable;
            while (data.Read())
            {
                DataRow entity = dataTable.NewRow();
                entity[0] = data["Id"];
                entity[1] = data["goods_code"];
                entity[2] = data["name"];
                entity[3] = data["code"];
                entity[4] = data["batch_number"];
                entity[5] = data["birth_date"];
                entity[6] = data["expiry_date"];
                entity[5] = data["fetch_type"];
                entity[6] = data["remarks"];
                dataTable.Rows.Add(entity);
            }
            return dataTable;
        }

        /// <summary>
        /// 根据手术编号查询本次操作详情
        /// </summary>
        /// <param name="id">手术编号</param>
        /// <returns></returns>
        public DataTable GetAllSurgeryOrderdtl(int id)
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("id", Type.GetType("System.String"));
            dataTable.Columns.Add("goods_code", Type.GetType("System.String"));
            dataTable.Columns.Add("name", Type.GetType("System.String"));
            dataTable.Columns.Add("code", Type.GetType("System.String"));
            dataTable.Columns.Add("batch_number", Type.GetType("System.String"));
            dataTable.Columns.Add("birth_date", Type.GetType("System.String"));
            dataTable.Columns.Add("expiry_date", Type.GetType("System.String"));
            dataTable.Columns.Add("fetch_type", Type.GetType("System.String"));
            dataTable.Columns.Add("remarks", Type.GetType("System.String"));
            IDataReader data = SqliteHelper.Instance.ExecuteReader(string.Format(@"SELECT a.id,b.goods_code,b.name,b.code,b.batch_number,b.birth_date,b.expiry_date,
                                                                                 a.fetch_type,b.remarks FROM surgery_orderdtl a LEFT JOIN goods b on a.goods_id=b.id 
                                                                                 WHERE surgery_order_id = {0}", id));
            if (data == null)
                return dataTable;
            while (data.Read())
            {
                DataRow entity = dataTable.NewRow();
                entity[0] = data["Id"];
                entity[1] = data["goods_code"];
                entity[2] = data["name"];
                entity[3] = data["code"];
                entity[4] = data["batch_number"];
                entity[5] = data["birth_date"];
                entity[6] = data["expiry_date"];
                entity[5] = data["fetch_type"];
                entity[6] = data["remarks"];
                dataTable.Rows.Add(entity);
            }
            return dataTable;
        }

        private int LastInsertRowId()
        {
            return Convert.ToInt16(SqliteHelper.Instance.ExecuteScalar("SELECT last_insert_rowid();"));
        }
    }
}
