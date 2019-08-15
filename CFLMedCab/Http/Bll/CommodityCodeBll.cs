using CFLMedCab.Http.Enum;
using CFLMedCab.Http.Helper;
using CFLMedCab.Http.Model;
using CFLMedCab.Http.Model.Base;
using CFLMedCab.Http.Model.Enum;
using CFLMedCab.Http.Model.param;
using CFLMedCab.Infrastructure;
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
	public class CommodityCodeBll : BaseBll<CommodityCodeBll>
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
					commodityCodes.Add(new CommodityCode
					{
						name = currentEps,
						//TODO: 需要关联当前本地的设备，货位，库房id
						EquipmentId = ApplicationState.GetEquipId(),
						EquipmentName = ApplicationState.GetEquipName(),
						operate_type = (int)OperateType.入库
					});
				}
			}

			foreach (string currentEps in preCommodityEpsCollect)
			{
				if (!afterCommodityEpsCollect.Contains(currentEps))
				{
					commodityCodes.Add(new CommodityCode
					{
						name = currentEps,
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
		[Obsolete]
		private List<CommodityCode> GetCompareSimpleCommodity(Hashtable preCommodityEpsCollect, Hashtable afterCommodityEpsCollect)
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
		[Obsolete]
		public BaseData<CommodityCode> GetCompareCommodity(Hashtable preCommodityEpsCollect, Hashtable afterCommodityEpsCollect)
		{
			return GetCommodityCode(GetCompareSimpleCommodity(preCommodityEpsCollect, afterCommodityEpsCollect));
		}

		/// <summary>
		/// 获取商品库存变化
		/// </summary>
		/// <param name="preCommodityEpsCollect">之前商品集合</param>
		/// <param name="afterCommodityEpsCollect">之后商品集合</param>
		/// <returns></returns>
		[Obsolete]
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
		public List<CommodityCode> GetCompareSimpleCommodity(HashSet<CommodityEps> preCommodityEpsCollect, HashSet<CommodityEps> afterCommodityEpsCollect)
		{
			var commodityCodes = new List<CommodityCode>();

			foreach (CommodityEps currentEps in afterCommodityEpsCollect)
			{
				if (!preCommodityEpsCollect.Contains(currentEps))
				{

					commodityCodes.Add(new CommodityCode
					{
						name = currentEps.CommodityCodeName,
						EquipmentId = currentEps.EquipmentId,
						EquipmentName = currentEps.EquipmentName,
						GoodsLocationId = currentEps.GoodsLocationId,
						GoodsLocationName = currentEps.GoodsLocationName,
						StoreHouseId = currentEps.StoreHouseId,
						StoreHouseName = currentEps.StoreHouseName,
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
						name = currentEps.CommodityCodeName,
						EquipmentId = currentEps.EquipmentId,
						EquipmentName = currentEps.EquipmentName,
						GoodsLocationId = currentEps.GoodsLocationId,
						GoodsLocationName = currentEps.GoodsLocationName,
						StoreHouseId = currentEps.StoreHouseId,
						StoreHouseName = currentEps.StoreHouseName,
						operate_type = (int)OperateType.出库
					});
				}
			}

			return commodityCodes;
		}

        /// <summary>
        /// 获取当前eps集合对应的CommodityCode集合
        /// </summary>
        /// <param name="CommodityEpss">hashset的eps集合</param>
        /// <returns></returns>
        public List<CommodityCode> GetSimpleCommodity(HashSet<CommodityEps> CommodityEpss)
		{
			var commodityCodes = new List<CommodityCode>(CommodityEpss.Count);

			foreach (CommodityEps currentEps in CommodityEpss)
			{
				commodityCodes.Add(new CommodityCode
				{
					name = currentEps.CommodityCodeName,
					EquipmentId = currentEps.EquipmentId,
					EquipmentName = currentEps.EquipmentName,
					GoodsLocationId = currentEps.GoodsLocationId,
					GoodsLocationName = currentEps.GoodsLocationName,
					StoreHouseId = currentEps.StoreHouseId,
					StoreHouseName = currentEps.StoreHouseName,
					operate_type = (int)OperateType.入库
				});
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
		[Obsolete]
		public BaseData<CommodityCode> GetInvetoryCommodity(Hashtable goodsEpsCollect)
		{
			return GetCommodityCode(goodsEpsCollect);
		}


		/// <summary>
		/// 根据商品码集合获取完整商品属性集合
		/// </summary>
		/// <returns></returns>
		[Obsolete]
		public BaseData<CommodityCode> GetCommodityCode(Hashtable commodityEpcs)
		{

			HashSet<string> commodityEpsHashSetDatas = new HashSet<string>();
			foreach (HashSet<string> goodsEpsData in commodityEpcs.Values)
			{
				commodityEpsHashSetDatas.UnionWith(goodsEpsData);
			}

			var commodityCodeNames = new List<string>(commodityEpsHashSetDatas.Count);

			foreach (string commodityEps in commodityEpsHashSetDatas)
			{
				commodityCodeNames.Add(HttpUtility.UrlEncode(commodityEps));
			}

			BaseData<CommodityCode> baseData = HttpHelper.GetInstance().Get<CommodityCode>(new QueryParam
			{
				@in =
				{
					field = "name",
					in_list =  commodityCodeNames
				}
			});

			return HttpHelper.GetInstance().ResultCheck(baseData);
		}


		/// <summary>
		/// 根据商品码集合获取完整商品属性集合
		/// </summary>
		/// <returns></returns>
		public BaseData<CommodityCode> GetCommodityCode(HashSet<CommodityEps> commodityEpss)
		{
			if (commodityEpss == null && commodityEpss.Count <= 0)
			{
				return new BaseData<CommodityCode>()
				{
					code = (int)ResultCode.Parameter_Exception,
					message = ResultCode.Parameter_Exception.ToString()
				};
			}
			return GetCommodityCode(GetSimpleCommodity(commodityEpss));
		}

		/// <summary>
		/// 根据商品码集合获取完整商品属性集合
		/// </summary>
		/// <returns></returns>
		public BaseData<CommodityCode> GetCommodityCode(List<CommodityEps> commodityEpss)
		{

			if (commodityEpss == null && commodityEpss.Count <= 0)
			{
				return new BaseData<CommodityCode>()
				{
					code = (int)ResultCode.Parameter_Exception,
					message = ResultCode.Parameter_Exception.ToString()
				};
			}
			return GetCommodityCode(new HashSet<CommodityEps>(commodityEpss));
		}

		/// <summary>
		/// 根据商品码集合获取完整商品属性集合
		/// </summary>
		/// <returns></returns>
		public BaseData<CommodityCode> GetCommodityCode(List<string> commodityCodeNames)
		{

			for (int i = 0, len = commodityCodeNames.Count; i < len; i++)
			{
				commodityCodeNames[i] = HttpUtility.UrlEncode(commodityCodeNames[i]);
			}

			BaseData<CommodityCode> baseData = HttpHelper.GetInstance().Get<CommodityCode>(new QueryParam
			{
				@in =
				{
					field = "name",
					in_list =  commodityCodeNames
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

			List<string> commodityCodeNames = new List<string>(commodityCodes.Count);
			List<string> commodityCodeCommodityIds = new List<string>(commodityCodes.Count);

			commodityCodes.ForEach(it =>
			{

				commodityCodeNames.Add(HttpUtility.UrlEncode(it.name));
			});

			BaseData<CommodityCode> baseDataCommodityCode;

			if (commodityCodes.Count > 0)
			{
				baseDataCommodityCode = HttpHelper.GetInstance().Get<CommodityCode>(new QueryParam
				{
					@in =
					{
						field = "name",
						in_list =  commodityCodeNames
					}
				});

				HttpHelper.GetInstance().ResultCheck(baseDataCommodityCode, out bool isSuccess);

				if (isSuccess)
				{

					baseDataCommodityCode.body.objects.ForEach(it =>
					{
						commodityCodeCommodityIds.Add(HttpUtility.UrlEncode(it.CommodityId));
					});

					var baseDataCommodity = HttpHelper.GetInstance().ResultCheck((HttpHelper hh) =>
					{
						return hh.Get<Commodity>(new QueryParam
						{
							@in =
							{
								field = "id",
								in_list =  commodityCodeCommodityIds
							}
						});

					}, baseDataCommodityCode, out bool isSuccess1);

                    HttpHelper.GetInstance().ResultCheck(baseDataCommodity,  out bool isSuccess2);

                    baseDataCommodityCode.body.objects.ForEach(it =>
					{
						if (isSuccess1 && isSuccess2)
						{
                            if (baseDataCommodity.body.objects.Where(cit => cit.id == it.CommodityId).Count() == 0)
                            {
                                it.CommodityName = "未知商品";
                            }
                            else
                            {
                                var commodity = baseDataCommodity.body.objects.Where(cit => cit.id == it.CommodityId).First();
                                it.CommodityName = commodity.name;
                            }
                        }
                        else
                        {
                            it.CommodityName = "未知商品";
                        }

						CommodityCode simpleCommodityCode = commodityCodes.Where(cit => cit.name == it.name).First();
						it.operate_type = simpleCommodityCode.operate_type;
						it.EquipmentId = simpleCommodityCode.EquipmentId;
						it.EquipmentName = simpleCommodityCode.EquipmentName;
						it.GoodsLocationId = simpleCommodityCode.GoodsLocationId;
						it.GoodsLocationName = simpleCommodityCode.GoodsLocationName;
						it.StoreHouseId = simpleCommodityCode.StoreHouseId;
						it.StoreHouseName = simpleCommodityCode.StoreHouseName;
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
		/// 根据商品码获取完整商品属性集合
		/// </summary>
		/// <returns></returns>
		public BaseData<CommodityCode> GetCommodityCodeByName(string commodityCodeName)
		{
			BaseData<CommodityCode> baseData = HttpHelper.GetInstance().Get<CommodityCode>(new QueryParam
			{
				@in =
				{
					field = "name",
					in_list =  { HttpUtility.UrlEncode(commodityCodeName) }
				}
			});

			var baseDataCommodity = HttpHelper.GetInstance().ResultCheck((HttpHelper hh) =>
			{
				return hh.Get<Commodity>(new QueryParam
				{
					@in =
					{
						field = "id",
						in_list =  { HttpUtility.UrlEncode(baseData.body.objects[0].CommodityId) }
					}
				});

			}, baseData, out bool isSuccess);

			if (isSuccess)
			{
				baseData.body.objects[0].CommodityName = baseDataCommodity.body.objects[0].name;
			}

			return baseData;
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
