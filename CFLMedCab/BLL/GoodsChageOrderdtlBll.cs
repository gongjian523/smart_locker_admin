using CFLMedCab.DAL;
using CFLMedCab.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.BLL
{
    public class GoodsChageOrderdtlBll
    {
        GoodsChageOrderdtlDal goodsChageOrderdtlDal = new GoodsChageOrderdtlDal();

        /// <summary>
        /// 新增库存变化单
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Add(GoodsChageOrderdtl model)
        {
            return goodsChageOrderdtlDal.Insert(model);
        }

        /// <summary>
        /// 批量新增库存变化详情单
        /// </summary>
        /// <param name="goodsChageOrderdtls"></param>
        /// <returns></returns>
        public bool AddGoodsChageOrderdtls(List<GoodsChageOrderdtl> goodsChageOrderdtls)
        {
            return goodsChageOrderdtlDal.CurrentDb.InsertRange(goodsChageOrderdtls);
        }
    }
}
