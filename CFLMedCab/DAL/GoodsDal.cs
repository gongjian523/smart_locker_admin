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
    public class GoodsDal : SqlSugarContext<Goods>
    {
        ///// <summary>
        ///// 数据库没有表时创建
        ///// </summary>
        //public void CreateTable_Goods()
        //{
        //    string commandText = @"CREATE TABLE if not exists goods ('id' INTEGER PRIMARY KEY AUTOINCREMENT, 
        //                                                           'code' VARCHAR(50),
        //                                                           'name' VARCHAR(50),
        //                                                           'goods_code' VARCHAR(50), 
        //                                                           'batch_number' VARCHAR(50),  
        //                                                           'birth_date' not null default (datetime('localtime')),
        //                                                            'valid_period' INTEGER,
        //                                                            'expiry_date'  not null default (datetime('localtime')),
        //                                                            'fetch_type' INTEGER,
        //                                                            'position' VARCHAR(50),
        //                                                            'remarks' VARCHAR(200));";
        //    SqliteHelper.Instance.ExecuteNonQuery(commandText);
        //    return;
        //}

        ///// <summary>
        ///// 新增商品
        ///// </summary>
        ///// <param name="fetchOrder"></param>
        ///// <returns></returns>
        //public int InsertNewGoods(Goods goods)
        //{
        //    string commandText = string.Format(@"INSERT INTO goods (code, name, goods_code, batch_number, birth_date,valid_period,expiry_date,fetch_type,position,remarks) VALUES 
        //                                        ('{0}', '{1}', '{2}', '{3}', '{4}',{5}', '{6}', '{7}', '{8}', '{9}')",
        //                                        goods.code, goods.name,goods.goods_code,goods.batch_number,goods.birth_date,goods.valid_period, goods.expiry_date,goods.fetch_type,goods.position,goods.remarks);

        //    if (!SqliteHelper.Instance.ExecuteNonQuery(commandText))
        //        return 0;


        //    return LastInsertRowId();
        //}

        ///// <summary>
        ///// 查询商品数据数据
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //public List<Goods> GetAllGoods(int user, int type)
        //{
        //    List<Goods> dataList = new List<Goods>();
        //    IDataReader data = SqliteHelper.Instance.ExecuteReader(string.Format(@"SELECT * FROM goods"));
        //    if (data == null)
        //        return dataList;
        //    while (data.Read())
        //    {
        //        Goods entity = new Goods();
        //        entity.id = Convert.ToInt32(data["Id"].ToString());
        //        entity.code = data["create_time"].ToString();
        //        entity.name = data["name"].ToString();
        //        entity.goods_code = data["goods_code"].ToString();
        //        entity.batch_number = data["batch_number"].ToString();
        //        entity.birth_date= Convert.ToDateTime(data["birth_date"]);
        //        entity.valid_period = Convert.ToInt32(data["valid_period"]);
        //        entity.expiry_date= Convert.ToDateTime(data["expiry_date"]);
        //        entity.fetch_type= Convert.ToInt32(data["fetch_type"]);
        //        entity.remarks = data["remarks"].ToString();
        //        entity.position = data["position"].ToString();
        //        dataList.Add(entity);
        //    }
        //    return dataList;
        //}

        //private int LastInsertRowId()
        //{
        //    return Convert.ToInt16(SqliteHelper.Instance.ExecuteScalar("SELECT last_insert_rowid();"));
        //}
    }
}
