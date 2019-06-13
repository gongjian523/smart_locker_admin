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
    public class GoodsChageOrderDal : SqlSugarContext<GoodsChageOrder>
    {
        ///// <summary>
        ///// 数据库没有表时创建
        ///// </summary>
        //public void CreateTable_GoodsChageOrder()
        //{
        //    string commandText = @"CREATE TABLE if not exists goods_chage_order ('id' INTEGER PRIMARY KEY AUTOINCREMENT, 
        //                                                           'operator_id' INTEGER,
        //                                                           'create_time' not null default (datetime('localtime')),
        //                                                           'type' INTEGER, 
        //                                                           'status' INTEGER);";
        //    SqliteHelper.Instance.ExecuteNonQuery(commandText);
        //    return;
        //}

        ///// <summary>
        ///// 新增库存变化单
        ///// </summary>
        ///// <param name="goodsChageOrder"></param>
        ///// <returns></returns>
        //public int InsertNewGoodsChageOrder(GoodsChageOrder goodsChageOrder)
        //{
        //    string commandText = string.Format(@"INSERT INTO goods_chage_order (create_time, operator_id, type, status) VALUES 
        //                                        ('{0}', '{1}', '{2}', '{3}')",
        //                                        goodsChageOrder.create_time, goodsChageOrder.operator_id, goodsChageOrder.type, goodsChageOrder.status);

        //    if (!SqliteHelper.Instance.ExecuteNonQuery(commandText))
        //        return 0;


        //    return LastInsertRowId();
        //}
        
        //private int LastInsertRowId()
        //{
        //    return Convert.ToInt16(SqliteHelper.Instance.ExecuteScalar("SELECT last_insert_rowid();"));
        //}
    }
}
