using CFLMedCab.Infrastructure.DbHelper;
using CFLMedCab.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.DAL
{
    public class SurgeryOrderdtlDal : SqlSugarContext<FetchOrderdtl>
    {
        ///// <summary>
        ///// 数据库没有表时创建
        ///// </summary>
        //public void CreateTable_SurgeryOrderdtl()
        //{
        //    string commandText = @"CREATE TABLE if not exists surgery_orderdtl ('id' INTEGER PRIMARY KEY AUTOINCREMENT, 
        //                                                           'goods_id' BIGINT,
        //                                                           'name' VARCHAR(50),
        //                                                           'goods_code' VARCHAR(50), 
        //                                                           'code' VARCHAR(50),
        //                                                           'batch_number' VARCHAR(50),
        //                                                           'birth_date' not null default (datetime('localtime')),
        //                                                           'expire_date' not null default (datetime('localtime')),
        //                                                           'valid_period' INTEGER,
        //                                                           'position' VARCHAR(50),
        //                                                           'fetch_type' INTEGER,
        //                                                           'remarks' VARCHAR(200),
        //                                                           'status' INTEGER,
        //                                                           'related_order_id' BIGINT);";
        //    SqliteHelper.Instance.ExecuteNonQuery(commandText);
        //    return;
        //}

        ///// <summary>
        ///// 删表
        ///// </summary>
        //public void dropTable()
        //{
        //    string sql = "DROP TABLE surgery_orderdtl";
        //    SqliteHelper.Instance.ExecuteNonQuery(sql);
        //    return;
        //}

        ///// <summary>
        ///// 新增手术单详情数据
        ///// </summary>
        ///// <param name="user"></param>
        ///// <returns></returns>
        //public int InsertNewSurgeryOrderdtl(FetchOrderdtl surgeryOrderDtl)
        //{
        //    string commandText = string.Format(@"INSERT INTO surgery_orderdtl ((goods_id, name, goods_code, code,batch_number,birth_date,expire_date,valid_period,position,fetch_type,remarks,status,related_order_id) VALUES 
        //                                        ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}')",
        //                                       surgeryOrderDtl.goods_id, surgeryOrderDtl.name, surgeryOrderDtl.goods_code, surgeryOrderDtl.code, surgeryOrderDtl.batch_number, surgeryOrderDtl.birth_date, surgeryOrderDtl.expire_date,
        //                                       surgeryOrderDtl.valid_period, surgeryOrderDtl.position, surgeryOrderDtl.fetch_type, surgeryOrderDtl.remarks, surgeryOrderDtl.status, surgeryOrderDtl.related_order_id);

        //    if (!SqliteHelper.Instance.ExecuteNonQuery(commandText))
        //        return 0;


        //    return LastInsertRowId();
        //}


        ///// <summary>
        ///// 根据手术编号查询待领取详情
        ///// </summary>
        ///// <param name="id">手术编号</param>
        ///// <returns></returns>
        //public List<FetchOrderdtl> GetAllTakeCollect(int id)
        //{
        //    List<FetchOrderdtl> surgeryOrderdtls = new List<FetchOrderdtl>();
        //    IDataReader data = SqliteHelper.Instance.ExecuteReader(string.Format(@"SELECT * FROM surgery_orderdtl WHERE related_order_id = {0}", id));
        //    if (data == null)
        //        return surgeryOrderdtls;
        //    while (data.Read())
        //    {
        //        FetchOrderdtl surgeryOrderdtl = new FetchOrderdtl();
        //        surgeryOrderdtl.id = Convert.ToInt32(data["id"]);
        //        surgeryOrderdtl.goods_code = data["goods_code"].ToString();
        //        surgeryOrderdtl.name = data["name"].ToString();
        //        surgeryOrderdtl.code = data["code"].ToString();
        //        surgeryOrderdtl.position = data["position"].ToString();
        //        surgeryOrderdtl.expire_date = Convert.ToDateTime(data["expire_date"]);
        //        surgeryOrderdtl.fetch_type = Convert.ToInt32(data["fetch_type"]);
        //        surgeryOrderdtl.remarks = data["remarks"].ToString();
        //        surgeryOrderdtls.Add(surgeryOrderdtl);
        //    }
        //    return surgeryOrderdtls;
        //}

        ///// <summary>
        ///// 根据手术编号查询本次操作详情
        ///// </summary>
        ///// <param name="id">手术编号</param>
        ///// <returns></returns>
        //public DataTable GetAllSurgeryOrderdtl(int id)
        //{
        //    DataTable dataTable = new DataTable();
        //    dataTable.Columns.Add("id", Type.GetType("System.String"));
        //    dataTable.Columns.Add("goods_code", Type.GetType("System.String"));
        //    dataTable.Columns.Add("name", Type.GetType("System.String"));
        //    dataTable.Columns.Add("code", Type.GetType("System.String"));
        //    dataTable.Columns.Add("batch_number", Type.GetType("System.String"));
        //    dataTable.Columns.Add("birth_date", Type.GetType("System.String"));
        //    dataTable.Columns.Add("expire_date", Type.GetType("System.String"));
        //    dataTable.Columns.Add("fetch_type", Type.GetType("System.String"));
        //    dataTable.Columns.Add("remarks", Type.GetType("System.String"));
        //    IDataReader data = SqliteHelper.Instance.ExecuteReader(string.Format(@"SELECT a.id,b.goods_code,b.name,b.code,b.batch_number,b.birth_date,b.expire_date,
        //                                                                         a.fetch_type,b.remarks FROM surgery_orderdtl a LEFT JOIN goods b on a.goods_id=b.id 
        //                                                                         WHERE surgery_order_id = {0}", id));
        //    if (data == null)
        //        return dataTable;
        //    while (data.Read())
        //    {
        //        DataRow entity = dataTable.NewRow();
        //        entity[0] = data["Id"];
        //        entity[1] = data["goods_code"];
        //        entity[2] = data["name"];
        //        entity[3] = data["code"];
        //        entity[4] = data["batch_number"];
        //        entity[5] = data["birth_date"];
        //        entity[6] = data["expire_date"];
        //        entity[5] = data["fetch_type"];
        //        entity[6] = data["remarks"];
        //        dataTable.Rows.Add(entity);
        //    }
        //    return dataTable;
        //}

        //private int LastInsertRowId()
        //{
        //    return Convert.ToInt16(SqliteHelper.Instance.ExecuteScalar("SELECT last_insert_rowid();"));
        //}
    }
}
