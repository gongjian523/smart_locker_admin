using CFLMedCab.Http.Helper;
using CFLMedCab.Http.Model;
using CFLMedCab.Http.Model.Base;
using CFLMedCab.Http.Model.param;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Http.Bll
{
    /// <summary>
    /// 商品库存变更记录信资料
    /// </summary>
    class CommodityInventoryChangeBll : BaseBll<CommodityInventoryChangeBll>
    {
        /// <summary>
        /// 创建商品库存变更记录
        /// </summary>
        /// <param name="changes"></param>
        /// <returns></returns>
        public BaseData<CommodityInventoryChange> createCommodityInventoryChange(List<CommodityInventoryChange> changes)
        {
            return HttpHelper.GetInstance().Post<CommodityInventoryChange, PostParam<CommodityInventoryChange>>(new PostParam<CommodityInventoryChange>()
            {
                objects = changes
            }, HttpHelper.GetCreateUrl(typeof(CommodityInventoryChange).Name));
        }
    }
}
