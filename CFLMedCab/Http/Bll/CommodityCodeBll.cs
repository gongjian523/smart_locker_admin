using CFLMedCab.Http.Enum;
using CFLMedCab.Http.Helper;
using CFLMedCab.Http.Model;
using CFLMedCab.Http.Model.Base;
using CFLMedCab.Http.Model.Enum;
using CFLMedCab.Http.Model.param;
using CFLMedCab.Infrastructure;
using CFLMedCab.Infrastructure.ToolHelper;
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
        public List<CommodityCode> GetCompareSimpleCommodity(HashSet<CommodityEps> preCommodityEpsCollect, HashSet<CommodityEps> afterCommodityEpsCollect, List<string> locCodes)
        {
            var commodityCodes = new List<CommodityCode>();

            foreach(var code in locCodes)
            {
                HashSet<CommodityEps> pre = new HashSet<CommodityEps>(preCommodityEpsCollect.Where(item => item.GoodsLocationName == code).ToList());
                HashSet<CommodityEps> after = new HashSet<CommodityEps>(afterCommodityEpsCollect.Where(item => item.GoodsLocationName == code).ToList());
                commodityCodes.AddRange(GetCompareSimpleCommodity(pre, after));
            }
            return commodityCodes;
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
        [Obsolete]
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
                                it.Spec = commodity.Spec1;
                                it.CatalogueId = commodity.CommodityCatalogueId;
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
        /// 根据库存变化记录中商品码集合获取完整商品属性集合
        /// commodityCodes允许name有重复的情况，对应的是同一个商品在一个设备下改变货柜的场景
        /// </summary>
        /// <returns></returns>
        public BaseData<CommodityCode> GetCommodityCodeStock(List<CommodityCode> commodityCodes)
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

                    HttpHelper.GetInstance().ResultCheck(baseDataCommodity, out bool isSuccess2);

                    commodityCodes.ForEach(it =>
                    {
                        //从主系统获取的完整信息保存在列表
                        CommodityCode fullCommodityCode = baseDataCommodityCode.body.objects.Where(cit => cit.name == it.name).First();

                        it.CommodityId = fullCommodityCode.CommodityId;
                        it.ExpirationDate = fullCommodityCode.ExpirationDate;
                        it.QStatus = fullCommodityCode.QStatus;
                        it.Status = fullCommodityCode.Status;
                        it.Type = fullCommodityCode.Type;
                        it.created_at = fullCommodityCode.created_at;
                        it.created_by = fullCommodityCode.created_by;
                        it.id = fullCommodityCode.id;
                        it.owner = fullCommodityCode.owner;
                        it.permission = fullCommodityCode.permission;
                        it.record_type = fullCommodityCode.record_type;
                        it.system_mod_stamp = fullCommodityCode.system_mod_stamp;
                        it.updated_at = fullCommodityCode.updated_at;
                        it.updated_by = fullCommodityCode.updated_by;
                        it.version = fullCommodityCode.version;
                       
                        if (isSuccess1 && isSuccess2)
                        {
                            if (baseDataCommodity.body.objects.Where(cit => cit.id == fullCommodityCode.CommodityId).Count() == 0)
                            {
                                it.CommodityName = "未知商品";
                            }
                            else
                            {
                                var commodity = baseDataCommodity.body.objects.Where(cit => cit.id == fullCommodityCode.CommodityId).First();
                                it.CommodityName = commodity.name;
                                it.Spec = commodity.Spec1;
                            }
                        }
                        else
                        {
                            fullCommodityCode.CommodityName = "未知商品";
                        }
                    });
                    baseDataCommodityCode.body.objects = commodityCodes;
                }
                else
                {
					if (baseDataCommodityCode.code != (int)ResultCode.Request_Exception) {
						baseDataCommodityCode = new BaseData<CommodityCode>()
						{
							code = (int)ResultCode.Business_Exception,
							message = ResultCode.Business_Exception.ToString()
						};
					}
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

        public BaseData<CommodityCode> GetExpirationAndManufactor(BaseData<CommodityCode> bdCommodityCode, out bool isSuccess)
        {

            LogUtils.Error("GetExpirationAndManufactor: 1 " + "CCNum" + bdCommodityCode.body.objects.Count);
            isSuccess = false;
            
            //检查参数是否正确
            if (null == bdCommodityCode.body.objects || bdCommodityCode.body.objects.Count <= 0)
            {

                bdCommodityCode.code = (int)ResultCode.Parameter_Exception;
                bdCommodityCode.message = ResultCode.Parameter_Exception.ToString();

                return bdCommodityCode; 
            }

            
            //通过【商品码】从表格【商品库存管理】中查询该条库存的id（CommodityInventoryDetailId）。
            var commodityCodeIds = bdCommodityCode.body.objects.Select(it => it.id).Distinct().ToList();
            LogUtils.Error("GetExpirationAndManufactor: 2 "+ "ccidNum:" + commodityCodeIds.Count);

            var commodityInventoryDetails = HttpHelper.GetInstance().Get<CommodityInventoryDetail>(new QueryParam
            {
                @in =
                    {
                        field = "CommodityCodeId",
                        in_list = BllHelper.ParamUrlEncode(commodityCodeIds)
                    }
            });

            HttpHelper.GetInstance().ResultCheck(commodityInventoryDetails, out bool isSuccess1);
            if(!isSuccess1)
            {
                bdCommodityCode.code = (int)ResultCode.Result_Exception;
                bdCommodityCode.message = ResultCode.Result_Exception.ToString();

                return bdCommodityCode;
            }

            bdCommodityCode.body.objects.ForEach(it =>
            {
                if(commodityInventoryDetails.body.objects.Where(cid => cid.CommodityCodeId == it.id).Count() > 0)
                {
                    it.CommodityInventoryDetailId = commodityInventoryDetails.body.objects.Where(cid => cid.CommodityCodeId == it.id).First().id;
                }
            });

            //通过【关联商品】（CommodityInventoryDetailId）从表格【商品库存货品明细】中获取相关货品列表。
            var CommodityInventoryDetailIds = bdCommodityCode.body.objects.Select(it => it.CommodityInventoryDetailId).Distinct().ToList();
            LogUtils.Error("GetExpirationAndManufactor: 3"+ "cididNum:" + CommodityInventoryDetailIds.Count);

            var commodityInventoryGoods = HttpHelper.GetInstance().Get<CommodityInventoryGoods>(new QueryParam
            {
                @in =
                    {
                        field = "CommodityInventoryDetailId",
                        in_list = BllHelper.ParamUrlEncode(CommodityInventoryDetailIds)
                    }
            });

            HttpHelper.GetInstance().ResultCheck(commodityInventoryGoods, out bool isSuccess2);
            if (!isSuccess2)
            {
                bdCommodityCode.code = (int)ResultCode.Result_Exception;
                bdCommodityCode.message = ResultCode.Result_Exception.ToString();

                return bdCommodityCode;
            }

            bdCommodityCode.body.objects.ForEach(it =>
            {
                //在CommodityInventoryGoods表中，如果一个CommodityInventoryDetailId 对应有多条记录，说明一个商品对应了多个货品，那就不属于单品，智能柜不用显示
                if (commodityInventoryGoods.body.objects.Where(cig => cig.CommodityInventoryDetailId == it.CommodityInventoryDetailId).Count() == 1)
                {
                    it.BatchNumberId = commodityInventoryGoods.body.objects.Where(cid => cid.CommodityInventoryDetailId == it.CommodityInventoryDetailId).First().BatchNumberId;
                    it.HospitalGoodsId = commodityInventoryGoods.body.objects.Where(cid => cid.CommodityInventoryDetailId == it.CommodityInventoryDetailId).First().HospitalGoodsId;
                }
            });

            //通过【生产批号】（CommodityInventoryGoods.BatchNumberId）从表格【生产批号管理详情】中获得相关批号信息。
            var batchNumberIds = bdCommodityCode.body.objects.Select(it => it.BatchNumberId).Distinct().ToList();
            LogUtils.Error("GetExpirationAndManufactor: 4" + "bnidNum:" + batchNumberIds.Count);
            var batchNumber = HttpHelper.GetInstance().Get<BatchNumber>(new QueryParam
            {
                @in =
                    {
                        field = "id",
                        in_list = BllHelper.ParamUrlEncode(batchNumberIds)
                    }
            });

            HttpHelper.GetInstance().ResultCheck(batchNumber, out bool isSuccess3);
            if (!isSuccess3)
            {
                bdCommodityCode.code = (int)ResultCode.Result_Exception;
                bdCommodityCode.message = ResultCode.Result_Exception.ToString();

                return bdCommodityCode;
            }

            bdCommodityCode.body.objects.ForEach(it =>
            {
                if(it.BatchNumberId != null)
                {
                    it.ExpirationDate = Convert.ToDateTime(batchNumber.body.objects.Where(bn => bn.id == it.BatchNumberId).First().ExpirationDate);
                }
            });

            //通过【货品名称】（HospitalGoodsId）从表格【医院货品管理详情】中获得厂家名称。
            var hospitalGoodsIds = bdCommodityCode.body.objects.Select(it => it.HospitalGoodsId).Distinct().ToList();
            LogUtils.Error("GetExpirationAndManufactor: 5" + "hgidNum:" + hospitalGoodsIds.Count);
            var hospitalGoods = HttpHelper.GetInstance().Get<HospitalGoods>(new QueryParam
            {
                @in =
                    {
                        field = "id",
                        in_list = BllHelper.ParamUrlEncode(hospitalGoodsIds)
                    }
            });

            HttpHelper.GetInstance().ResultCheck(hospitalGoods, out bool isSuccess4);
            if (!isSuccess4)
            {
                bdCommodityCode.code = (int)ResultCode.Result_Exception;
                bdCommodityCode.message = ResultCode.Result_Exception.ToString();

                return bdCommodityCode;
            }

            bdCommodityCode.body.objects.ForEach(it =>
            {
                if (it.HospitalGoodsId != null)
                {
                    it.ManufactorName = hospitalGoods.body.objects.Where(hg => hg.id == it.HospitalGoodsId).First().ManufactorName;
                }
            });

            isSuccess = true;
            LogUtils.Error("GetExpirationAndManufactor: 6" + "bdccNum:" + bdCommodityCode.body.objects.Count);

            return bdCommodityCode;
        }


        public BaseData<CommodityCode> GetQualityStatus(BaseData<CommodityCode> bdCommodityCode, out bool isSuccess)
        {
            isSuccess = false;

            //检查参数是否正确
            if (null == bdCommodityCode.body.objects || bdCommodityCode.body.objects.Count <= 0)
            {

                bdCommodityCode.code = (int)ResultCode.Parameter_Exception;
                bdCommodityCode.message = ResultCode.Parameter_Exception.ToString();

                return bdCommodityCode;
            }

            //通过【商品码】从表格【商品库存管理】中查询该条库存的id（CommodityInventoryDetailId）。
            var commodityCodeIds = bdCommodityCode.body.objects.Select(it => it.id).Distinct().ToList();
            var commodityInventoryDetails = HttpHelper.GetInstance().Get<CommodityInventoryDetail>(new QueryParam
            {
                @in =
                    {
                        field = "CommodityCodeId",
                        in_list = BllHelper.ParamUrlEncode(commodityCodeIds)
                    }
            });

            HttpHelper.GetInstance().ResultCheck(commodityInventoryDetails, out bool isSuccess1);
            if (!isSuccess1)
            {
                bdCommodityCode.code = (int)ResultCode.Result_Exception;
                bdCommodityCode.message = ResultCode.Result_Exception.ToString();

                return bdCommodityCode;
            }

            bdCommodityCode.body.objects.ForEach(it =>
            {
                it.QualityStatus = commodityInventoryDetails.body.objects.Where(cid => cid.CommodityCodeId == it.id).First().QualityStatus;
                it.InventoryStatus = commodityInventoryDetails.body.objects.Where(cid => cid.CommodityCodeId == it.id).First().Status;
            });

            isSuccess = true;
            return bdCommodityCode;
        }


        public BaseData<CommodityCode> GetCatalogueName(BaseData<CommodityCode> bdCommodityCode, out bool isSuccess)
        {
            isSuccess = false;

            //检查参数是否正确
            if (null == bdCommodityCode.body.objects || bdCommodityCode.body.objects.Count <= 0)
            {

                bdCommodityCode.code = (int)ResultCode.Parameter_Exception;
                bdCommodityCode.message = ResultCode.Parameter_Exception.ToString();

                return bdCommodityCode;
            }

            //通过【目录id】从表格【商品目录】中查询该条库存的【目录名称】（name）。
            var catalogueIds = bdCommodityCode.body.objects.Select(it => it.CatalogueId).Distinct().ToList();
            var catalogueDetails = HttpHelper.GetInstance().Get<CommodityCatalogue>(new QueryParam
            {
                @in =
                    {
                        field = "id",
                        in_list = BllHelper.ParamUrlEncode(catalogueIds)
                    }
            });

            HttpHelper.GetInstance().ResultCheck(catalogueDetails, out bool isSuccess1);
            if (!isSuccess1)
            {
                bdCommodityCode.code = (int)ResultCode.Result_Exception;
                bdCommodityCode.message = ResultCode.Result_Exception.ToString();

                return bdCommodityCode;
            }

            bdCommodityCode.body.objects.ForEach(it =>
            {
                if(it.CatalogueId != null)
                {
                    it.CatalogueName = catalogueDetails.body.objects.Where(cdi => cdi.id == it.CatalogueId).First().name;
                }
            });

            isSuccess = true;
            return bdCommodityCode;
        }
    }
}
