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
using CFLMedCab.Model;

namespace CFLMedCab.Http.Bll
{
    public class LocalGoodsChangeBll
    {
        /// <summary>
        /// 获取本地商品快照列表（包含详情）
        /// </summary>
        /// <returns></returns>
        public static List<CommodityCode> GetCommodity()
        {
            var commodityEps = ApplicationState.GetGoodsInfo();
            var baseCommodityCodes = CommodityCodeBll.GetInstance().GetCommodityCode(commodityEps);
            baseCommodityCodes = HttpHelper.GetInstance().ResultCheck(baseCommodityCodes, out bool isSuccess);

            if (isSuccess)
            {
                //获取有效期和生产商
                CommodityCodeBll.GetInstance().GetExpirationAndManufactor(baseCommodityCodes, out bool isSuccess2);
                CommodityCodeBll.GetInstance().GetCatalogueName(baseCommodityCodes, out bool isSuccess3);

                return baseCommodityCodes.body.objects;
            }
            else
            {
                return null;
            }
        }

        /// 获取本地商品快照列表（包含详情）
        /// </summary>
        /// <returns></returns>
        public static List<CatalogueCommodity> GetCatalogueCommodity(List<CommodityCode> commodolityList)
        {
            List<CatalogueCommodity> catalogueList = new List<CatalogueCommodity>();

            foreach (var item in commodolityList)
            {
                //没有包含相同CatalogueId的数据
                if (catalogueList.Where(ci => ci.CatalogueId == item.CatalogueId).Count() == 0)
                {
                    List<CommodityCode> listDtl = commodolityList.Where(di => di.CatalogueId == item.CatalogueId).ToList();

                    List<SpecCommodity> listSpec = new List<SpecCommodity>();

                    foreach (var spec in listDtl)
                    {
                        if (listSpec.Where(si => si.Spec == spec.Spec).Count() == 0)
                        {
                            listSpec.Add(new SpecCommodity
                            {
                                CatalogueName = spec.CatalogueName,
                                Spec = spec.Spec,
                                SpecNum = listDtl.Where(id => id.Spec == spec.Spec).Count()
                            });
                        }
                    }

                    catalogueList.Add(new CatalogueCommodity
                    {
                        CatalogueId = item.CatalogueId,
                        CatalogueName = item.CatalogueName,
                        Num = listDtl.Count,
                        SpecNum = listSpec.Count,
                        SpecList = listSpec
                    });
                }
            }

            return catalogueList;
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
