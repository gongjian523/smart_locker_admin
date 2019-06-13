using CFLMedCab.DAL;
using CFLMedCab.Model;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.BLL
{
    public class GoodsChangeOrderBll
    {
        GoodsChageOrderDal GoodsChageOrderDal = new GoodsChageOrderDal();
        /// <summary>
        /// 添加库存变化单并返回主键
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public int AddGoodsChangeOrder(int userId,int type)
        {
            GoodsChageOrder goodsChageOrder = new GoodsChageOrder
            {
                operator_id = userId,
                type=type
            };
           return GoodsChageOrderDal.Insert(goodsChageOrder);
        }


    }
}
