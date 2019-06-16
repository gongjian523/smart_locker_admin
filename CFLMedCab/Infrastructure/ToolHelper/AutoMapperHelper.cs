using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace CFLMedCab.Infrastructure.ToolHelper
{
	public static class AutoMapperHelper
	{
		/// <summary>
		///  单个对象映射
		/// </summary>
		public static T MapTo<T>(this object obj)
		{
			if (obj == null) return default(T);
			//Mapper.Initialize(x => x.CreateMap(obj.GetType(), typeof(T)));
			//return Mapper.Map<T>(obj);

			var config = new MapperConfiguration(x => x.CreateMap(obj.GetType(), typeof(T)));
			IMapper mapper = new Mapper(config);
			return mapper.Map<T>(obj);

		}

		/// <summary>
		/// 集合列表类型映射
		/// </summary>
		public static List<TDestination> MapToList<TSource, TDestination>(this IEnumerable<TSource> source)
		{
			//Mapper.Initialize(x => x.CreateMap<TSource, TDestination>());
			//return Mapper.Map<List<TDestination>>(source);
			if (source == null)
			{
				return new List<TDestination>();
			}
			var config = new MapperConfiguration(x => x.CreateMap<TSource, TDestination>());
			IMapper mapper = new Mapper(config);
			return mapper.Map<List<TDestination>>(source);
		}

		/// <summary>
		/// 集合列表类型映射
		/// </summary>
		public static List<TDestination> MapToListIgnoreId<TSource, TDestination>(this IEnumerable<TSource> source)
		{
			//Mapper.Initialize(x => x.CreateMap<TSource, TDestination>());
			//return Mapper.Map<List<TDestination>>(source);
			if (source == null)
			{
				return new List<TDestination>();
			}
			var config = new MapperConfiguration(x => x.CreateMap<TSource, TDestination>().ForMember("id", opt => opt.Ignore()));
			IMapper mapper = new Mapper(config);
			return mapper.Map<List<TDestination>>(source);
		}

	}
}
