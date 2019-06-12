using CFLMedCab.DAL.DbHelper;
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
    public class PickingSubOrderdtlDal : SqlSugarContext<PickingSubOrderdtl>
    {
        ///// <summary>
        ///// 数据库没有表时创建
        ///// </summary>
        //public void CreateTable_PickingSubOrderdtl()
        //{
        //    string commandText = @"CREATE TABLE if not exists picking_sub_orderdtl ('id' INTEGER PRIMARY KEY AUTOINCREMENT, 
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
        ///// 新增拣货单
        ///// </summary>
        ///// <param name="pickingOrder"></param>
        ///// <returns></returns>
        //public int InsertNewPickingSubOrderdtl(PickingSubOrderdtl pickingSubOrderdtl)
        //{
        //    string commandText = string.Format(@"INSERT INTO picking_sub_orderdtl (goods_id, name, goods_code, code,batch_number,birth_date,expire_date,valid_period,position,fetch_type,remarks,status,related_order_id) VALUES 
        //                                        ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}')",
        //                                       pickingSubOrderdtl.goods_id, pickingSubOrderdtl.name, pickingSubOrderdtl.goods_code, pickingSubOrderdtl.code, pickingSubOrderdtl.batch_number, pickingSubOrderdtl.birth_date, pickingSubOrderdtl.expire_date,
        //                                       pickingSubOrderdtl.valid_period, pickingSubOrderdtl.position, pickingSubOrderdtl.fetch_type, pickingSubOrderdtl.remarks, pickingSubOrderdtl.status, pickingSubOrderdtl.related_order_id);

        //    if (!SqliteHelper.Instance.ExecuteNonQuery(commandText))
        //        return 0;


        //    return LastInsertRowId();
        //}

        ///// <summary>
        ///// 查询待拣货
        ///// </summary>
        ///// <returns></returns>
        //public List<PickingSubOrderdtl> GetPickingSubOrderdtl(int id)
        //{
        //    List<PickingSubOrderdtl> pickingSubOrderdtls = new List<PickingSubOrderdtl>();
        //    IDataReader data = SqliteHelper.Instance.ExecuteReader(string.Format(@"SELECT a.* FROM picking_sub_orderdtl a left join picking_sub_order b on a.related_order_id=b.id where  b.picking_order_id ={0} and a.status=0", id));
        //    if (data == null)
        //        return pickingSubOrderdtls;
        //    while (data.Read())
        //    {
        //        PickingSubOrderdtl pickingSubOrderdtl = new PickingSubOrderdtl();
        //        pickingSubOrderdtl.id = Convert.ToInt32(data["id"]);
        //        pickingSubOrderdtl.goods_code = data["goods_code"].ToString();
        //        pickingSubOrderdtl.name = data["name"].ToString();
        //        pickingSubOrderdtl.code = data["code"].ToString();
        //        pickingSubOrderdtl.position = data["position"].ToString();
        //        pickingSubOrderdtl.expire_date = Convert.ToDateTime(data["expire_date"]);
        //        pickingSubOrderdtl.fetch_type = Convert.ToInt32(data["fetch_type"]);
        //        pickingSubOrderdtl.remarks = data["remarks"].ToString();
        //        pickingSubOrderdtls.Add(pickingSubOrderdtl);
        //    }
        //    return pickingSubOrderdtls;
        //}

        //private int LastInsertRowId()
        //{
        //    return Convert.ToInt16(SqliteHelper.Instance.ExecuteScalar("SELECT last_insert_rowid();"));
        //}
    }
}
