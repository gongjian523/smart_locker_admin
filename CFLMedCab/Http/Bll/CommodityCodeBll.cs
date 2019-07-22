using CFLMedCab.APO.Inventory;
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
    public class CommodityCodeBll
    {

        /// <summary>
        /// 获取商品库存变化
        /// </summary>
        /// <param name="preCommodityEpsCollect">之前商品集合</param>
        /// <param name="afterCommodityEpsCollect">之后商品集合</param>
        /// <returns></returns>
        public List<CommodityCode> GetCompareSimpleCommodity(HashSet<string> preCommodityEpsCollect, HashSet<string> afterCommodityEpsCollect)
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
        public List<CommodityCode> GetCompareSimpleCommodity(Hashtable preCommodityEpsCollect, Hashtable afterCommodityEpsCollect)
        {

            HashSet<string> preCommodityEpsHashSet = new HashSet<string>();

            HashSet<string> afterCommodityEpsHashSet = new HashSet<string>();

            foreach (HashSet<string> currentEps in preCommodityEpsCollect.Values)
            {

                preCommodityEpsHashSet.UnionWith(currentEps);

            }

            foreach (HashSet<string> currentEps in afterCommodityEpsCollect.Values)
            {
                afterCommodityEpsHashSet.UnionWith(currentEps);
            }

            return GetCompareSimpleCommodity(preCommodityEpsHashSet, afterCommodityEpsHashSet);
        }

        /// <summary>
        /// 获取商品库存变化
        /// </summary>
        /// <param name="preCommodityEpsCollect">之前商品集合</param>
        /// <param name="afterCommodityEpsCollect">之后商品集合</param>
        /// <returns></returns>
        public BaseData<CommodityCode> GetCompareCommodity(HashSet<string> preCommodityEpsCollect, HashSet<string> afterCommodityEpsCollect)
        {
            return GetCommodityCode(GetCompareSimpleCommodity(preCommodityEpsCollect, afterCommodityEpsCollect));
        }


		/// <summary>
		/// 获取商品库存变化
		/// </summary>
		/// <param name="preCommodityEpsCollect">之前商品集合</param>
		/// <param name="afterCommodityEpsCollect">之后商品集合</param>
		/// <returns></returns>
		public BaseData<CommodityCode> GetCompareCommodity(Hashtable preCommodityEpsCollect, Hashtable afterCommodityEpsCollect)
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
			List<string> commodityIds = new List<string>(commodityCodes.Count);

			commodityCodes.ForEach(it => {

				commodityCodeIds.Add(HttpUtility.UrlEncode(it.id));
				commodityIds.Add(HttpUtility.UrlEncode(it.CommodityId));
			});

			BaseData<CommodityCode> baseDataCommodityCode = HttpHelper.GetInstance().Get<CommodityCode>(new QueryParam
			{
				@in =
				{
					field = "id",
					in_list =  commodityCodeIds
				}
			});


			BaseData<Commodity> baseDataCommodity = HttpHelper.GetInstance().Get<Commodity>(new QueryParam
			{
				@in =
				{
					field = "id",
					in_list =  commodityIds
				}
			});

			HttpHelper.GetInstance().ResultCheck(baseDataCommodityCode, out bool isSuccess1);

			HttpHelper.GetInstance().ResultCheck(baseDataCommodity, out bool isSuccess2);

			if (isSuccess1 && isSuccess2)
			{
				baseDataCommodityCode.body.objects.ForEach(it =>
				{
					it.operate_type = commodityCodes.Where(cit => cit.id == it.id).Select(cit => cit.operate_type).Single();
					it.CommodityName = baseDataCommodity.body.objects.Where(bit => bit.id == it.CommodityId).Select(bit => bit.name).Single();
				});
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
