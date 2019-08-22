using CFLMedCab.Http.Helper;
using CFLMedCab.Http.Model;
using CFLMedCab.Http.Model.Base;
using CFLMedCab.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CFLMedCab.Infrastructure.ToolHelper;

namespace CFLMedCab.Http.Bll
{
    public class LocalGoodsChangeBll
    {
        /// <summary>
        /// 获取本地商品快照列表（包含详情）
        /// </summary>
        /// <returns></returns>
        public static List<Commodity> GetCommodity()
        {
            var commodityEps = ApplicationState.GetGoodsInfo();
            var baseCommodityCodes = CommodityCodeBll.GetInstance().GetCommodityCode(commodityEps);

            baseCommodityCodes = HttpHelper.GetInstance().ResultCheck(baseCommodityCodes, out bool isSuccess);
            List<Commodity> commodityLists = null;
            if (isSuccess)
            {
                //获取有效期和生产商
                CommodityCodeBll.GetInstance().GetExpirationAndManufactor(baseCommodityCodes, out bool isSuccess2);
                var tempList = baseCommodityCodes.body.objects;
                //商品列表集合
                commodityLists = tempList
                    .GroupBy(code => new { code.CommodityId, code.GoodsLocationId })
                    .Select(g => (new Commodity()
                    {
                        id = g.Key.CommodityId,//CommodityId
                        GoodsLocationId = g.Key.GoodsLocationId,
                        GoodsLocationName = g.ElementAt(0).GoodsLocationName,
                        name = g.ElementAt(0).CommodityName,//name
                        Count = g.Count(),//商品数量
                        codes = tempList.Where(it=>it.CommodityId == g.Key.CommodityId && it.GoodsLocationId == g.Key.GoodsLocationId).ToList()
                    })).ToList();
            }
            else
            {
                LogUtils.Error($"LocalGoodsChangeBll:GetCommodity {baseCommodityCodes.message}");
            }
            return commodityLists;
        }
        /// <summary>
        /// 根据commodityId和goodsLocationId查询CommodityCode列表
        /// </summary>
        /// <param name="commodityId"></param>
        /// <param name="goodsLocationId"></param>
        /// <returns></returns>
        public List<CommodityCode> GetCommodityCodes(string commodityId,string goodsLocationId)
        {
            var commodityCodes = GetCommodityCodes();
            if (null != commodityCodes)
            {
                return commodityCodes.Where(it => it.CommodityId == commodityId && it.GoodsLocationId == goodsLocationId).ToList();
            }
            return null;
        }
        /// <summary>
        /// 获取本地CommodityCode列表
        /// </summary>
        /// <returns></returns>
        private List<CommodityCode> GetCommodityCodes()
        {
            var commodityEps = ApplicationState.GetGoodsInfo();
            var baseCommodityCodes =  CommodityCodeBll.GetInstance().GetCommodityCode(commodityEps);
            baseCommodityCodes = HttpHelper.GetInstance().ResultCheck(baseCommodityCodes, out bool isSuccess);
            if (isSuccess)
            {
                return baseCommodityCodes.body.objects;
            }
            return null;
        }
    }
}
