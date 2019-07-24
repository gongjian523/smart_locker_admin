using CFLMedCab.APO.Inventory;
using CFLMedCab.Http.Enum;
using CFLMedCab.Http.Helper;
using CFLMedCab.Http.Model;
using CFLMedCab.Http.Model.Base;
using CFLMedCab.Http.Model.Enum;
using CFLMedCab.Http.Model.param;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CFLMedCab.Http.Bll
{
	/// <summary>
	/// 商品比对业务
	/// </summary>
    public class CommodityCodeBll:BaseBll<CommodityCodeBll>
    {

		/// <summary>
		/// 获取商品库存变化
		/// </summary>
		/// <param name="preCommodityEpsCollect">之前商品集合</param>
		/// <param name="afterCommodityEpsCollect">之后商品集合</param>
		/// <returns></returns>
	    [Obsolete]
		private List<CommodityCode> GetCompareSimpleCommodity(HashSet<string> preCommodityEpsCollect, HashSet<string> afterCommodityEpsCollect)
        {
            var commodityCodes = new List<CommodityCode>();

			foreach (string currentEps in afterCommodityEpsCollect)
            {
                if (!preCommodityEpsCollect.Contains(currentEps))
                {

					CommodityEps currentEpsObj = JsonConvert.DeserializeObject<CommodityEps>(currentEps);

										

					commodityCodes.Add(new CommodityCode
                    {
						CommodityName = currentEpsObj.CommodityName,
						id  = currentEpsObj.CommodityCodeId,
						name = currentEpsObj.CommodityCodeName,
						operate_type = (int)OperateType.入库
                    });
                }
            }

            foreach (string currentEps in preCommodityEpsCollect)
            {
                if (!afterCommodityEpsCollect.Contains(currentEps))
                {

					CommodityEps currentEpsObj = JsonConvert.DeserializeObject<CommodityEps>(currentEps);

					commodityCodes.Add(new CommodityCode
                    {
						CommodityName = currentEpsObj.CommodityName,
						id = currentEpsObj.CommodityCodeId,
						name = currentEpsObj.CommodityCodeName,
						operate_type = (int)OperateType.出库
                    });
                }
            }

            return commodityCodes;
        }

		/// <summary>
		/// 获取商品库存变化
		/// </summary>
		/// <param name="preCommodityEpsCollect">之前商品集合</param>
		/// <param name="afterCommodityEpsCollect">之后商品集合</param>
		/// <returns></returns>

		private List<CommodityCode> GetCompareSimpleCommodity(HashSet<CommodityEps> preCommodityEpsCollect, HashSet<CommodityEps> afterCommodityEpsCollect)
		{
			var commodityCodes = new List<CommodityCode>();

			foreach (CommodityEps currentEps in afterCommodityEpsCollect)
			{
				if (!preCommodityEpsCollect.Contains(currentEps))
				{

					commodityCodes.Add(new CommodityCode
					{
						CommodityName = currentEps.CommodityName,
						id = currentEps.CommodityCodeId,
						name = currentEps.CommodityCodeName,
						EquipmentId = currentEps.EquipmentId,
						EquipmentName = currentEps.EquipmentName,
						GoodsLocationId = currentEps.GoodsLocationId,
						GoodsLocationName = currentEps.GoodsLocationName,
						operate_type = (int)OperateType.入库
					});
				}
			}

			foreach (CommodityEps currentEps in preCommodityEpsCollect)
			{
				if (!afterCommodityEpsCollect.Contains(currentEps))
				{

					commodityCodes.Add(new CommodityCode
					{
						CommodityName = currentEps.CommodityName,
						id = currentEps.CommodityCodeId,
						name = currentEps.CommodityCodeName,
						EquipmentId = currentEps.EquipmentId,
						EquipmentName = currentEps.EquipmentName,
						GoodsLocationId = currentEps.GoodsLocationId,
						GoodsLocationName = currentEps.GoodsLocationName,
						operate_type = (int)OperateType.出库
					});
				}
			}

			return commodityCodes;
		}

		/// <summary>
		/// 获取商品库存变化
		/// </summary>
		/// <param name="preCommodityEpsCollect">之前商品集合</param>
		/// <param name="afterCommodityEpsCollect">之后商品集合</param>
		/// <returns></returns>

		public BaseData<CommodityCode> GetCompareCommodity(HashSet<CommodityEps> preCommodityEpsCollect, HashSet<CommodityEps> afterCommodityEpsCollect)
		{
			return GetCommodityCode(GetCompareSimpleCommodity(preCommodityEpsCollect, afterCommodityEpsCollect));
		}


		/// <summary>
		/// 获取盘点所有数据(Hashtable)
		/// </summary>
		/// <returns></returns>
		public BaseData<CommodityCode> GetInvetoryCommodity(Hashtable goodsEpsCollect)
        {
            return GetCommodityCode(goodsEpsCollect);
        }


		/// <summary>
		/// 根据商品码集合获取完整商品属性集合
		/// </summary>
		/// <returns></returns>
		public BaseData<CommodityCode> GetCommodityCode(Hashtable commodityEpss)
		{

			HashSet<string> commodityEpsHashSetDatas = new HashSet<string>();
			foreach (HashSet<string> goodsEpsData in commodityEpss.Values)
			{
				commodityEpsHashSetDatas.UnionWith(goodsEpsData);
			}

			var commodityCodeIds = new List<string>(commodityEpsHashSetDatas.Count);

			foreach (string commodityEps in commodityEpsHashSetDatas)
			{
				CommodityEps currentEpsObj = JsonConvert.DeserializeObject<CommodityEps>(commodityEps);
				commodityCodeIds.Add(HttpUtility.UrlEncode(currentEpsObj.CommodityCodeId));
			}

			BaseData<CommodityCode> baseData = HttpHelper.GetInstance().Get<CommodityCode>(new QueryParam
			{
				@in =
				{
					field = "id",
					in_list =  commodityCodeIds
				}
			});

			return HttpHelper.GetInstance().ResultCheck(baseData);
		}


		/// <summary>
		/// 根据商品码集合获取完整商品属性集合
		/// </summary>
		/// <returns></returns>
		public BaseData<CommodityCode> GetCommodityCode(HashSet<string> commodityEpss)
		{
			var commodityCodeIds = new List<string>(commodityEpss.Count);

			foreach (string commodityEps in commodityEpss)
			{
				CommodityEps currentEpsObj = JsonConvert.DeserializeObject<CommodityEps>(commodityEps);
				commodityCodeIds.Add(HttpUtility.UrlEncode(currentEpsObj.CommodityCodeId));
			}

			BaseData<CommodityCode> baseData = HttpHelper.GetInstance().Get<CommodityCode>(new QueryParam
			{
				@in =
				{
					field = "id",
					in_list =  commodityCodeIds
				}
			});

			return HttpHelper.GetInstance().ResultCheck(baseData);
		}

		/// <summary>
		/// 根据商品码集合获取完整商品属性集合
		/// </summary>
		/// <returns></returns>
		public BaseData<CommodityCode> GetCommodityCode(List<string> commodityCodeIds)
		{

			for (int i = 0, len = commodityCodeIds.Count; i < len; i++)
			{
				commodityCodeIds[i] = HttpUtility.UrlEncode(commodityCodeIds[i]);
			}

			BaseData<CommodityCode> baseData = HttpHelper.GetInstance().Get<CommodityCode>(new QueryParam
			{
				@in =
				{
					field = "id",
					in_list =  commodityCodeIds
				}
			});

			return HttpHelper.GetInstance().ResultCheck(baseData);
		}

		/// <summary>
		/// 根据商品码集合获取完整商品属性集合
		/// </summary>
		/// <returns></returns>
		public BaseData<CommodityCode> GetCommodityCode(List<CommodityCode> commodityCodes)
		{

			List<string> commodityCodeIds = new List<string>(commodityCodes.Count);

			commodityCodes.ForEach(it => {

				commodityCodeIds.Add(HttpUtility.UrlEncode(it.id));
			});

            BaseData<CommodityCode> baseDataCommodityCode;

            if (commodityCodeIds.Count > 0)
            {
                baseDataCommodityCode = HttpHelper.GetInstance().Get<CommodityCode>(new QueryParam
                {
                    @in =
                {
                    field = "id",
                    in_list =  commodityCodeIds
                }
                });

                HttpHelper.GetInstance().ResultCheck(baseDataCommodityCode, out bool isSuccess);

                if (isSuccess)
                {
                    baseDataCommodityCode.body.objects.ForEach(it =>
                    {
                        CommodityCode simpleCommodityCode = commodityCodes.Where(cit => cit.id == it.id).First();
                        it.operate_type = simpleCommodityCode.operate_type;
                        it.CommodityName = simpleCommodityCode.CommodityName;
                        it.name = simpleCommodityCode.name;
                        it.EquipmentId = simpleCommodityCode.EquipmentId;
                        it.EquipmentName = simpleCommodityCode.EquipmentName;
                        it.GoodsLocationId = simpleCommodityCode.GoodsLocationId;
                        it.GoodsLocationName = simpleCommodityCode.GoodsLocationName;
                        it.operate_type = simpleCommodityCode.operate_type;
                    });
                }
            }
            else
            {
                baseDataCommodityCode = new BaseData<CommodityCode>()
                {
                    code = (int)ResultCode.Parameter_Exception,
                    message = ResultCode.Parameter_Exception.ToString()
                };
            }

			

			return baseDataCommodityCode;

		}

		/// <summary>
		/// 根据商品码获取完整商品属性集合
		/// </summary>
		/// <returns></returns>
		public BaseData<CommodityCode> GetCommodityCode(string commodityCodeId)
        {
			BaseData<CommodityCode> baseData = HttpHelper.GetInstance().Get<CommodityCode>(new QueryParam
			{
				@in =
				{
					field = "id",
					in_list =  { HttpUtility.UrlEncode(commodityCodeId) }
				}
			});

			return HttpHelper.GetInstance().ResultCheck(baseData);
		}

        /// <summary>
        /// 通过单品码查询商品信息是否存在
        /// </summary>
        /// <returns></returns>
        public bool IsCommodityInfoExsits(string commodityCodeId)
        {
            
			BaseData<CommodityCode> baseData = HttpHelper.GetInstance().Get<CommodityCode>(new QueryParam
			{
				@in =
				{
					field = "id",
					in_list =  { HttpUtility.UrlEncode(commodityCodeId) }
				}
			});

			HttpHelper.GetInstance().ResultCheck(baseData, out bool isSuccess);
			return isSuccess;

		}


        /// <summary>
        /// 获取所有完整商品属性集合
        /// </summary>
        /// <returns></returns>
        public BaseData<CommodityCode> GetAllCommodity()
        {
			//获取待完成上架工单
			BaseData<CommodityCode> baseData = HttpHelper.GetInstance().Get<CommodityCode>();
			return HttpHelper.GetInstance().ResultCheck(baseData);

        }

    }
}
