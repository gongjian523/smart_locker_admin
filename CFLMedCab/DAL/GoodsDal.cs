
using CFLMedCab.DTO.Goodss;
using CFLMedCab.Infrastructure.DbHelper;
using CFLMedCab.Model;
using SqlSugar;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using CFLMedCab.APO.Inventory;
using System;

namespace CFLMedCab.DAL
{
    public class GoodsDal
    {
        //Db
        public SqlSugarClient Db;

        // 定义一个静态变量来保存类的实例
        private static GoodsDal singleton;

        // 定义一个标识确保线程同步
        private static readonly object locker = new object();

        //定义公有方法提供一个全局访问点。
        public static GoodsDal GetInstance()
        {
            //这里的lock其实使用的原理可以用一个词语来概括“互斥”这个概念也是操作系统的精髓
            //其实就是当一个进程进来访问的时候，其他进程便先挂起状态
            if (singleton == null)
            {
                lock (locker)
                {
                    // 如果类的实例不存在则创建，否则直接返回
                    if (singleton == null)
                    {
                        singleton = new GoodsDal();
                    }
                }
            }
            return singleton;
        }

        // 定义私有构造函数，使外界不能创建该类实例
        private GoodsDal()
        {
            Db = SqlSugarHelper.GetInstance().Db;
        }

        //
        public void InsertGoods(List<Goods> list)
        {
            Db.Insertable<Goods>(list).ExecuteCommand();
        }

        public int GetGoodsNum()
        {
            return Db.Queryable<Goods>().Select<Goods>().ToList().Count;
        }


        /// <summary>
        /// 根据集合获取完整商品属性集合
        /// </summary>
        /// <returns></returns>
        public List<GoodsDto> GetGoodsDto(HashSet<string> goodsEpsDatas)
        {
            //查询语句
            return Db.Queryable<Goods>()
                .Where(it => goodsEpsDatas.Contains(it.goods_code))
                .OrderBy(it => it.expire_date, OrderByType.Asc)
                .Select<GoodsDto>()
                .ToList();
        }

        /// <summary>
        /// 根据集合获取符合条件的商品属性集合
        /// </summary>
        /// <returns></returns>
        public List<GoodsDto> GetGoodsDto(GetGoodsApo pageDataApo, out int totalCount)
        {
            totalCount = 0;
            List<GoodsDto> data;

            //查询语句
            var queryable = Db.Queryable<Goods>()
                .Where(it => pageDataApo.goodsEpsDatas.Contains(it.code))
                .WhereIF(pageDataApo.expire_date != null, it => it.expire_date <= pageDataApo.expire_date && it.expire_date >= DateTime.Now)
                .WhereIF(!string.IsNullOrWhiteSpace(pageDataApo.name), it => it.name.Contains(pageDataApo.name))
                .WhereIF(!string.IsNullOrWhiteSpace(pageDataApo.code), it => it.goods_code.Contains(pageDataApo.code))
                .OrderBy(it => it.name, OrderByType.Asc)
                .Select<GoodsDto>();

            //如果小于0，默认查全部
            if (pageDataApo.PageSize > 0)
            {
                data = queryable.ToPageList(pageDataApo.PageIndex, pageDataApo.PageSize, ref totalCount);
            }
            else
            {
                data = queryable.ToList();
                totalCount = data.Count();
            }
            return data;
        }

        /// <summary>
        /// 根据集合获取完整商品属性集合(带有状态)
        /// </summary>
        /// <returns></returns>
        public List<GoodsDto> GetGoodsDto(List<GoodsDto> goodsDatas)
        {
            //转换为可支持的数组类型
            var codeArray = goodsDatas.Select(it => it.code).ToArray();

            //查询语句
            return Db.Queryable<Goods>()
                .Where(it => codeArray.Contains(it.code))
                .Select<GoodsDto>()
                .OrderBy(it => it.expire_date, OrderByType.Asc)
                .Mapper(it =>
                {
                    it.operate_type = goodsDatas.Where(goodsData => goodsData.code.Equals(it.code)).Single().operate_type;
                }).ToList();

        }


        /// <summary>
        /// 根据集合获取完整商品属性集合
        /// </summary>
        /// <returns></returns>
        public List<GoodsDto> GetGoodsDto(Hashtable goodsEpsDatas)
        {
            HashSet<string> goodsEpsHashSetDatas = new HashSet<string>();
            foreach (HashSet<string> goodsEpsData in goodsEpsDatas.Values)
            {
                goodsEpsHashSetDatas.UnionWith(goodsEpsData);
            }
            return GetGoodsDto(goodsEpsHashSetDatas);
        }




        /// <summary>
        /// 根据集合获取完整商品属性集合
        /// </summary>
        /// <returns></returns>
        public List<GoodDto> GetGoodDto(GetGoodApo pageDataApo, out int totalCount)
        {
            totalCount = 0;
            List<GoodDto> data;

            //查询语句
            var queryable = Db.Queryable<Goods>()
                .Where(it => pageDataApo.goodsEpsDatas.Contains(it.code))
                .WhereIF(!string.IsNullOrWhiteSpace(pageDataApo.name), it => it.name.Contains(pageDataApo.name))
                .WhereIF(!string.IsNullOrWhiteSpace(pageDataApo.code), it => it.goods_code.Contains(pageDataApo.code))
                .GroupBy(it => it.goods_code)
                .OrderBy(it => it.expire_date, OrderByType.Asc)

                .Select(it => new GoodDto
                {
                    name = it.name,
                    goods_code = it.goods_code,
                    amount = SqlFunc.AggregateCount(it.id),
                    expire_time = SqlFunc.AggregateMin(it.expire_date)
                });

            //如果小于0，默认查全部
            if (pageDataApo.PageSize > 0)
            {
                data = queryable.ToPageList(pageDataApo.PageIndex, pageDataApo.PageSize, ref totalCount);
            }
            else
            {
                data = queryable.ToList();
                totalCount = data.Count();
            }
            return data;


        }




    }
}
