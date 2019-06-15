
using CFLMedCab.DTO.Goodss;
using CFLMedCab.Infrastructure.DbHelper;
using CFLMedCab.Model;
using SqlSugar;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

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


		/// <summary>
		/// 根据集合获取完整商品属性集合
		/// </summary>
		/// <returns></returns>
		public List<GoodsDto> GetGoodsDto(HashSet<string> goodsEpsDatas)
		{
			//查询语句
			return Db.Queryable<Goods>()
				.Where(it => goodsEpsDatas.Contains(it.code))
				.Select<GoodsDto>()
				.OrderBy(it => it.expiry_date, OrderByType.Asc)
				.ToList();
		}

		/// <summary>
		/// 根据集合获取完整商品属性集合(带有状态)
		/// </summary>
		/// <returns></returns>
		public List<GoodsDto> GetGoodsDto(List<GoodsDto> goodsDatas)
		{
			//查询语句
			return Db.Queryable<Goods>()
				.Where(it => goodsDatas.Exists(goodsData => goodsData.code.Equals(it.code)))
				.Select<GoodsDto>()
				.OrderBy(it=>it.expiry_date, OrderByType.Asc)
				.Mapper(it=> {
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
			foreach (var goodsEpsData in goodsEpsDatas.Values)
			{
				goodsEpsHashSetDatas.Add((string)goodsEpsData);
			}

			return GetGoodsDto(goodsEpsHashSetDatas);


		}

	}
}
