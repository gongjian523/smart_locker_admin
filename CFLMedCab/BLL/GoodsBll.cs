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
		/// 获取补货操作商品情况
		/// </summary>
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


		public List<GoodsDto> GetCompareSimpleGoodsDto(Hashtable preGoodsEpsCollect, Hashtable afterGoodsEpsCollect)
		{

			var goodsDtos = new List<GoodsDto>();

			foreach (string currentEps in afterGoodsEpsCollect.Values)
			{
				if (!preGoodsEpsCollect.ContainsValue(currentEps))
				{
					goodsDtos.Add(new GoodsDto
					{
						code = currentEps,
						operate_type = 1,
					});
				}
			}

			foreach (string currentEps in preGoodsEpsCollect.Values)
			{
				if (!afterGoodsEpsCollect.ContainsValue(currentEps))
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
		/// 获取补货操作商品情况
		/// </summary>
		/// <returns></returns>
		public List<GoodsDto> GetCompareGoodsDto(HashSet<string> preGoodsEpsCollect, HashSet<string> afterGoodsEpsCollect)
		{
			return GoodsDal.GetGoodsDto(GetCompareSimpleGoodsDto(preGoodsEpsCollect, afterGoodsEpsCollect));
		}


		public List<GoodsDto> GetCompareGoodsDto(Hashtable preGoodsEpsCollect, Hashtable afterGoodsEpsCollect)
		{
			return GoodsDal.GetGoodsDto(GetCompareSimpleGoodsDto(preGoodsEpsCollect, afterGoodsEpsCollect));
		}
	}
}
