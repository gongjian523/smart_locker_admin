using CFLMedCab.Infrastructure.DbHelper;
using CFLMedCab.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.DAL
{
    public class GoodsChageOrderdtlDal:SqlSugarContext<GoodsChageOrderdtl>
    {
        /// <summary>
        /// 批量新增库存变化详情
        /// </summary>
        /// <param name="goodsChageOrderdtls"></param>
        /// <returns></returns>
        public bool AddGoodsChageOrderdtl(List<GoodsChageOrderdtl> goodsChageOrderdtls)
        {
            return CurrentDb.InsertRange(goodsChageOrderdtls);
        }
        ///// <summary>
        ///// 数据库没有表时创建
        ///// </summary>
        //public void CreateTable_GoodsChageOrderdtl()
        //{
        //    string commandText = @"CREATE TABLE if not exists goods_chage_orderdtl ('id' INTEGER PRIMARY KEY AUTOINCREMENT, 
        //                                                           'good_change_orderid' BIGINT,
        //                                                           'goods_id' BIGINT,
        //                                                           'name' VARCHAR(50),
        //                                                           'goods_code' VARCHAR(50), 
        //                                                           'code' VARCHAR(50),
        //                                                           'batch_number' VARCHAR(50),
        //                                                           'birth_date' not null default (datetime('localtime')), 
        //                                                           'expire_date' not null default (datetime('localtime')), 
        //                                                           'valid_period' INTEGER,
        //                                                           'operate_type' INTEGER,
        //                                                           'exception_flag' INTEGER,
        //                                                           'exception_description' VARCHAR(50),
        //                                                           'position' INTEGER, 
        //                                                           'fetch_type' VARCHAR(50),
        //                                                           'remarks' VARCHAR(200), 
        //                                                           'status' INTEGER, 
        //                                                           'related_order_id' BIGINT);";
        //    SqliteHelper.Instance.ExecuteNonQuery(commandText);
        //    return;
        //}

        ///// <summary>
        ///// 新增库存变化详情单
        ///// </summary>
        ///// <param name="goodsChageOrderdtl"></param>
        ///// <returns></returns>
        //public int InsertNewGoodsChageOrderdtl(GoodsChageOrderdtl goodsChageOrderdtl)
        //{
        //    string commandText = string.Format(@"INSERT INTO goods_chage_orderdtl (good_change_orderid,goods_id, name, goods_code, code,batch_number,
        //                                        birth_date,expire_date,valid_period,operate_type,exception_flag,exception_description,position,fetch_type,remarks,status,related_order_id) VALUES 
        //                                        ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}')",
        //                                        goodsChageOrderdtl.good_change_orderid, goodsChageOrderdtl.goods_id, goodsChageOrderdtl.name, goodsChageOrderdtl.goods_code, 
        //                                        goodsChageOrderdtl.code,goodsChageOrderdtl.batch_number, goodsChageOrderdtl.birth_date, goodsChageOrderdtl.expire_date, 
        //                                        goodsChageOrderdtl.valid_period,goodsChageOrderdtl.operate_type, goodsChageOrderdtl.exception_flag, goodsChageOrderdtl.exception_description,
        //                                        goodsChageOrderdtl.position, goodsChageOrderdtl.fetch_type, goodsChageOrderdtl.remarks, goodsChageOrderdtl.status, goodsChageOrderdtl.related_order_id);

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
