using CFLMedCab.APO.GoodsChange;
using CFLMedCab.DAL;
using CFLMedCab.DTO.Goodss;
using CFLMedCab.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.BLL
{
    public class GoodsChangeOrderBll
    {
        private readonly GoodsChangeOrderDal goodsChageOrderDal;

        public GoodsChangeOrderBll()
        {
            goodsChageOrderDal = GoodsChangeOrderDal.GetInstance();
        }

        /// <summary>
        /// 库存记录查询
        /// </summary>
        /// <param name="pageDataApo"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<GoodsChangeDto> GetGoodsChange(GoodsChangeApo pageDataApo, out int totalCount)
        {
            return goodsChageOrderDal.GetGoodsChange(pageDataApo,out totalCount);
        }
    }
}
