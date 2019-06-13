using CFLMedCab.DAL;
using CFLMedCab.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.BLL
{
    public class FetchOrderdtlBll
    {
        FetchOrderdtlDal fetchOrderdtlDal = new FetchOrderdtlDal();
        GoodsDal goodsDal = new GoodsDal();
        /// <summary>
        /// 根据领用单号获取领用商品数量
        /// </summary>
        /// <param name="id">领用单</param>
        /// <returns></returns>
        public List<FetchOrderdtlView> GetDetailsUsage(int id)
        {
            string sql = string.Format(@"SELECT  id,name,goods_code,fetch_type,status,remarks,sum(CASE status WHEN 0 THEN 1 ELSE 0 END ) wait_collar 
                                         ,sum(CASE status WHEN 1 THEN 1 ELSE 0 END ) already_led from FetchOrderdtl where related_order_id={0} group by code", id);
            return fetchOrderdtlDal.Db.Ado.SqlQuery<FetchOrderdtlView>(sql, new object()).ToList();
        }

        /// <summary>
        /// 通过商品编号查询商品
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public FetchOrderdtl GetFetchOrderdtl(string code)
        {
            return fetchOrderdtlDal.Db.Queryable<FetchOrderdtl>().Where(t => t.goods_code == code).First();
        }

        /// <summary>
        /// 根据库存变化数据添加领用详情表
        /// </summary>
        /// <param name="goodsChageOrderdtls"></param>
        /// <returns></returns>
        public bool AddFetchOrderdtls(List<GoodsChageOrderdtl> goodsChageOrderdtls,int fetchOrderId)
        {
            List<FetchOrderdtl> fetchOrderdtls = new List<FetchOrderdtl>();
            foreach (GoodsChageOrderdtl item in goodsChageOrderdtls)
            {
                FetchOrderdtl fetchOrderdtl = new FetchOrderdtl();
                fetchOrderdtl.batch_number = item.batch_number;
                fetchOrderdtl.birth_date = item.birth_date;
                fetchOrderdtl.code = item.code;
                fetchOrderdtl.expire_date = item.expire_date;
                fetchOrderdtl.fetch_type = 1;
                fetchOrderdtl.goods_code = item.goods_code;
                fetchOrderdtl.goods_id = item.id;
                fetchOrderdtl.name = item.name;
                fetchOrderdtl.position = item.position;
                fetchOrderdtl.remarks = item.remarks;
                fetchOrderdtl.valid_period = item.valid_period;
                fetchOrderdtl.status = 1;
                fetchOrderdtl.related_order_id = fetchOrderId;
                fetchOrderdtl.is_add = 0;
                fetchOrderdtls.Add(fetchOrderdtl);
            }
            return fetchOrderdtlDal.CurrentDb.InsertRange(fetchOrderdtls);
        }


        /// <summary>
        /// 领用库存变化明显表
        /// </summary>
        /// <param name="hashtable">变化数据</param>
        /// <param name="dataType">机构数据类型 出库/入库</param>
        /// <param name="pageType">页面操作类型 出库/入库</param>
        /// <returns></returns>
        public List<GoodsChageOrderdtl> newGoodsChageOrderdtls(HashSet<string> hashtable, int dataType, int pageType,string explain, ref int exceptional)
        {
            List<GoodsChageOrderdtl> goodsChageOrderdtlsList = new List<GoodsChageOrderdtl>();
            foreach (string item in hashtable)
            {
                Goods goods = goodsDal.GetGoodsById(Convert.ToInt32(item));
                if (goods != null)
                {
                    GoodsChageOrderdtl goodsChageOrderdtl = new GoodsChageOrderdtl();
                    goodsChageOrderdtl.batch_number = goods.batch_number;
                    goodsChageOrderdtl.birth_date = goods.birth_date;
                    goodsChageOrderdtl.code = goods.code;
                    goodsChageOrderdtl.expire_date = goods.expiry_date;
                    goodsChageOrderdtl.fetch_type = 1;
                    goodsChageOrderdtl.goods_code = goods.goods_code;
                    goodsChageOrderdtl.goods_id = goods.id;
                    goodsChageOrderdtl.name = goods.name;
                    goodsChageOrderdtl.operate_type = dataType;
                    goodsChageOrderdtl.position = goods.position;
                    goodsChageOrderdtl.remarks = goods.remarks;
                    goodsChageOrderdtl.valid_period = goods.valid_period;
                    if (dataType != pageType)//类型不同则为异常
                    {
                        exceptional++;
                        goodsChageOrderdtl.exceptional = 1;
                        goodsChageOrderdtl.explain = explain;
                    }
                    else
                        goodsChageOrderdtl.exceptional = 0;
                    goodsChageOrderdtlsList.Add(goodsChageOrderdtl);
                }
            }
            return goodsChageOrderdtlsList;
        }
    }
}
