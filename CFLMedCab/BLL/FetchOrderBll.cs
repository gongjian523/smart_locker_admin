using CFLMedCab.DAL;
using CFLMedCab.Infrastructure;
using CFLMedCab.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.BLL
{

    public class FetchOrderBll
    {
        FetchOrderDal fetchOrderDal = new FetchOrderDal();

        /// <summary>
        /// 新增领用单
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Add(FetchOrder model)
        {
            return fetchOrderDal.Insert(model);
        }

        /// <summary>
        /// 根据主键查找领用单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public FetchOrder GetById(int id)
        {
            return fetchOrderDal.CurrentDb.GetById(id);
        }

        /// <summary>
        /// 一般领用关柜确认
        /// </summary>
        /// <param name="goods"></param>
        public void UpdateGoodsStock(List<Goods> goods)
        {

        }

    }
}
