using CFLMedCab.Http.Enum;
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
    public class CommodityInventoryChangeBll : BaseBll<CommodityInventoryChangeBll>
    {
        /// <summary>
        /// 创建商品库存变更记录
        /// </summary>
        /// <param name="changes"></param>
        /// <returns></returns>
        public BasePostData<CommodityInventoryChange> createCommodityInventoryChange(List<CommodityInventoryChange> changes)
        {
            if(null == changes || changes.Count <= 0)
            {
                return new BasePostData<CommodityInventoryChange>()
                {
                    code = (int)ResultCode.Parameter_Exception,
                    message = ResultCode.Parameter_Exception.ToString()
                };
            }
            return HttpHelper.GetInstance().Post<CommodityInventoryChange>(new PostParam<CommodityInventoryChange>()
            {
                objects = changes
            });
        }
    }
}
