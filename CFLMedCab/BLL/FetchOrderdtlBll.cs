using CFLMedCab.DAL;
using CFLMedCab.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.BLL
{
    public class FetchOrderdtlBll
    {
        FetchOrderdtlDal fetchOrderdtlDal = new FetchOrderdtlDal();

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
    }
}
