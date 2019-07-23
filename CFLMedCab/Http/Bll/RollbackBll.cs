using CFLMedCab.Http.Helper;
using CFLMedCab.Http.Model;
using CFLMedCab.Http.Model.Base;
using CFLMedCab.Http.Model.param;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CFLMedCab.Http.Bll
{
    /// <summary>
    /// 回退模块相应接口
    /// </summary>
    public class RollbackBll :BaseBll<RollbackBll>
    {
        /// <summary>
        /// 通过【货位编码】从表格 【货位管理】中查询获取货位的【所属库房、所属设备】
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public BaseData<GoodsLocation> GetGoodsLocation(string name)
        {
            var baseGoodsLocation = HttpHelper.GetInstance().Get<GoodsLocation>(new QueryParam
            {
                @in =
                    {
                        field = "name",
                        in_list =  { HttpUtility.UrlEncode(name) }
                    }
            });

            //校验是否含有数据，如果含有数据，拼接具体字段
            HttpHelper.GetInstance().ResultCheck(baseGoodsLocation, out bool isSuccess);

            if (isSuccess)
            {
                baseGoodsLocation.body.objects.ForEach(it =>
                {
                    //拼接所属设备名称
                    if (!string.IsNullOrEmpty(it.EquipmentId))
                    {
                        it.EquipmentName = GetNameById<Equipment>(it.EquipmentId);
                    };
                    //拼接所属库房名称
                    if (!string.IsNullOrEmpty(it.StoreHouseId))
                    {
                        it.StoreHouseName = GetNameById<StoreHouse>(it.StoreHouseId);
                    }

                });
            }

            return baseGoodsLocation;
        }
        /// <summary>
        /// 通过【商品编码】从表格【商品管理理】中查询到相应商品详情
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public BaseData<Commodity> GetCommodity(string CommodityCode)
        {
            var baseCommodity = HttpHelper.GetInstance().Get<Commodity>(new QueryParam
            {
                @in =
                    {
                        field = "CommodityCode",
                        in_list =  { HttpUtility.UrlEncode(CommodityCode) }
                    }
            });
            return baseCommodity ;
        }
    }
}
