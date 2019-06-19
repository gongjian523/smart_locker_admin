using CFLMedCab.APO.Inventory;
using CFLMedCab.DAL;
using CFLMedCab.DTO.Goodss;
using CFLMedCab.Infrastructure.ToolHelper;
using CFLMedCab.Model;
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
        /// 获取盘点所有数据(Hashtable)
        /// </summary>
        /// <returns></returns>
        public List<GoodsDto> GetInvetoryGoodsDto(Hashtable goodsEpsCollect)
        {
            return GoodsDal.GetGoodsDto(goodsEpsCollect);
        }


        /// <summary>
        /// 获取盘点所有数据(HashSet)
        /// </summary>
        /// <returns></returns>
        public List<GoodsDto> GetInvetoryGoodsDto(HashSet<string> goodsEpsCollect)
        {
            return GoodsDal.GetGoodsDto(goodsEpsCollect);
        }


        /// <summary>
        /// 根据商品码集合获取完整商品属性集合
        /// </summary>
        /// <returns></returns>
        public List<GoodsDto> GetGoodsDto(string goodsCode)
        {
            //查询语句
            return GoodsDal.GetGoodsDto(goodsCode);
        }

        /// <summary>
        /// 通过单品码查询商品信息是否存在
        /// </summary>
        /// <returns></returns>
        public bool IsGoodsInfoExsits(string code)
        {
            HashSet<string> hs = new HashSet<string>();
            hs.Add(code);
            return GoodsDal.GetGoodsDto(hs).Count > 0;
        }

        /// <summary>
        /// 库存快照
        /// </summary>
        /// <param name="getGoodApo"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<GoodDto> GetStockGoodsDto(GetGoodApo getGoodApo, out int totalCount)
        {
            return GoodsDal.GetGoodDto(getGoodApo, out totalCount);
        }

        /// <summary>
        /// 效期 查询
        /// </summary>
        /// <param name="getGoodApo"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<GoodsDto> GetValidityGoodsDto(GetGoodsApo getGoodApo, out int totalCount)
        {
            return GoodsDal.GetGoodsDto(getGoodApo, out totalCount);
        }

        /// <summary>
        /// 获取当前表中所有商品的种类
        /// </summary>
        /// <returns></returns>
        public int GetGoodsTypeNum()
        {
            return GoodsDal.GetGoodsNum();
        }

        public void InsertGood(List<Goods> list)
        {
            GoodsDal.InsertGoods(list);
        }
    }
}
