using CFLMedCab.APO.Inventory;
using CFLMedCab.DAL;
using CFLMedCab.DTO.Goodss;
using CFLMedCab.Infrastructure.ToolHelper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.BLL
{
	public class GoodsBll
	{

		private readonly GoodsDal GoodsDal;

		public GoodsBll()
		{
			GoodsDal = GoodsDal.GetInstance();
		}

		/// <summary>
		/// 获取商品库存变化
		/// </summary>
		/// <param name="preGoodsEpsCollect">之前商品集合</param>
		/// <param name="afterGoodsEpsCollect">之后商品集合</param>
		/// <returns></returns>
		public List<GoodsDto> GetCompareSimpleGoodsDto(HashSet<string> preGoodsEpsCollect, HashSet<string> afterGoodsEpsCollect)
		{

			var goodsDtos = new List<GoodsDto>();

			foreach (string currentEps in afterGoodsEpsCollect)
			{
				if (!preGoodsEpsCollect.Contains(currentEps))
				{
					goodsDtos.Add(new GoodsDto
					{
						code = currentEps,
						operate_type = 1,
					});
				}
			}

			foreach (string currentEps in preGoodsEpsCollect)
			{
				if (!afterGoodsEpsCollect.Contains(currentEps))
				{
					goodsDtos.Add(new GoodsDto
					{
						code = currentEps,
						operate_type = 0,
					});
				}
			}

			return goodsDtos;
		}

		/// <summary>
		/// 获取商品库存变化
		/// </summary>
		/// <param name="preGoodsEpsCollect">之前商品集合</param>
		/// <param name="afterGoodsEpsCollect">之后商品集合</param>
		/// <returns></returns>
		public List<GoodsDto> GetCompareSimpleGoodsDto(Hashtable preGoodsEpsCollect, Hashtable afterGoodsEpsCollect)
		{

            HashSet<string> preGoodsEpsHashSet = new HashSet<string>();

            HashSet<string> afterGoodsEpsHashSet = new HashSet<string>();

            foreach (HashSet<string> currentEps in preGoodsEpsCollect.Values)
            {

                preGoodsEpsHashSet.UnionWith(currentEps);

            }

            foreach (HashSet<string> currentEps in afterGoodsEpsCollect.Values)
			{
                afterGoodsEpsHashSet.UnionWith(currentEps);
            }

			return GetCompareSimpleGoodsDto(preGoodsEpsHashSet, afterGoodsEpsHashSet);
		}

		/// <summary>
		/// 获取商品库存变化
		/// </summary>
		/// <param name="preGoodsEpsCollect">之前商品集合</param>
		/// <param name="afterGoodsEpsCollect">之后商品集合</param>
		/// <returns></returns>
		public List<GoodsDto> GetCompareGoodsDto(HashSet<string> preGoodsEpsCollect, HashSet<string> afterGoodsEpsCollect)
		{
			return GoodsDal.GetGoodsDto(GetCompareSimpleGoodsDto(preGoodsEpsCollect, afterGoodsEpsCollect));
		}


		public List<GoodsDto> GetCompareGoodsDto(Hashtable preGoodsEpsCollect, Hashtable afterGoodsEpsCollect)
		{
			return GoodsDal.GetGoodsDto(GetCompareSimpleGoodsDto(preGoodsEpsCollect, afterGoodsEpsCollect));
		}

        /// <summary>
        /// 获取盘点所有数据
        /// </summary>
        /// <returns></returns>
        public List<GoodsDto> GetInvetoryGoodsDto(Hashtable goodsEpsCollect)
        {
            return GoodsDal.GetGoodsDto(goodsEpsCollect);
        }

        /// <summary>
        /// 获取库存所有数据
        /// </summary>
        /// <returns></returns>
        public List<GoodDto> GetStockGoodsDto(GetGoodApo getGoodApo,out int totalCount)
        {
            return GoodsDal.GetGoodDto(getGoodApo, out totalCount);
        }
    }
}
