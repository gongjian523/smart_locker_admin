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
    public class SurgeryOrderDal
    {
        /// <summary>
        /// 数据库没有表时创建
        /// </summary>
        public void CreateTable_SurgeryOrder()
        {
            string commandText = @"CREATE TABLE if not exists surgery_order('id' INTEGER PRIMARY KEY AUTOINCREMENT, 
                                                                            'surgery_dateiime'  not null default (datetime('localtime')),
                                                                             'number' INTEGER);";
            SqliteHelper.Instance.ExecuteNonQuery(commandText);
            return;
        }

        /// <summary>
        /// 删表
        /// </summary>
        public void dropTable()
        {
            string sql = "DROP TABLE surgery_order";
            SqliteHelper.Instance.ExecuteNonQuery(sql);
            return;
        }

        /// <summary>
        /// 新增手术单数据
        /// </summary>
        /// <param name="surgeryOrder"></param>
        /// <returns></returns>
        public int InsertNewSurgeryOrder(SurgeryOrder surgeryOrder)
        {
            string commandText = string.Format(@"INSERT INTO surgery_order (surgery_dateiime,number) VALUES 
                                                ('{0}', '{1}')",
                                                surgeryOrder.id, surgeryOrder.surgery_dateiime);

            if (!SqliteHelper.Instance.ExecuteNonQuery(commandText))
                return 0;


            return LastInsertRowId();
        }

        /// <summary>
        /// 根据手术编号查询数据是否存在，返回手术编号
        /// </summary>
        /// <param name="id">手术编号</param>
        /// <returns></returns>
        public SurgeryOrder GetSurgeryOrderById(int id)
        {
            string commandText = string.Format(@"SELECT * FROM surgery_order WHERE id = {0}", id);
            List<SurgeryOrder> dataList = new List<SurgeryOrder>();
            IDataReader data = SqliteHelper.Instance.ExecuteReader(commandText);
            while (data.Read())
            {
                SurgeryOrder entity = new SurgeryOrder();
                entity.id= Convert.ToInt32(data["Id"].ToString());
                entity.surgery_dateiime= Convert.ToDateTime(data["surgery_dateiime"].ToString());
                dataList.Add(entity);
            }
            if(dataList.Count>0)
                return dataList.First();
            return null;
        }

        private int LastInsertRowId()
        {
            return Convert.ToInt16(SqliteHelper.Instance.ExecuteScalar("SELECT last_insert_rowid();"));
        }
    }
}
