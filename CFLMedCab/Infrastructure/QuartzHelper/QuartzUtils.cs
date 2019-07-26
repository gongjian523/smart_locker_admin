using CFLMedCab.Http.Model;
using CFLMedCab.Infrastructure.QuartzHelper.quartzEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Infrastructure.QuartzHelper
{
	/// <summary>
	/// QuartzUtils工具
	/// </summary>
	public class QuartzUtils
	{

		
		/// <summary>
		/// 根据计划获取cron表达式
		/// </summary>
		/// <param name="inventoryPlan"></param>
		/// <returns></returns>
		public static string GetQuartzCron(InventoryPlan inventoryPlan)
		{
			string ret = null;


			switch (inventoryPlan.CheckPeriod)
			{
				case "每日":
					ret = $"{timeSplitAndCompose(inventoryPlan.InventoryTime)} * * ?";
					break;
				case "每周":
					string weekNumStr = ConvertEnumToString<Week>(inventoryPlan.InventoryWeekday);
					ret = $"{timeSplitAndCompose(inventoryPlan.InventoryTime)} ? * {weekNumStr} *";
					break;
				case "每月":
					switch (inventoryPlan.InventoryDay)
					{
						case "第一天":
							ret = $"{timeSplitAndCompose(inventoryPlan.InventoryTime)} 1 * ? *";
							break;
						case "最后一天":
							ret = $"{timeSplitAndCompose(inventoryPlan.InventoryTime)} L * ? *";
							break;
						default:
							break;
					}
					break;
				default:
					break;
			}

			return ret;

		}

		/// <summary>
		/// 如果有多零的字符串，转换成单个0
		/// </summary>
		/// <param name="time"></param>
		/// <returns></returns>
		private static string timeSplitAndCompose(string time)
		{

			if (time.Contains(":"))
			{
				string[] times = time.Split(new char[] { ':' }, 3);
				time = $"{timeTrimAllZero(times[2])} {timeTrimAllZero(times[1])} {timeTrimAllZero(times[0])}";
			}
			return time;

		}

		/// <summary>
		/// 如果有多零的字符串，转换成单个0
		/// </summary>
		/// <param name="time"></param>
		/// <returns></returns>
		private static string timeTrimAllZero(string time)
		{
			
			if (time.Length == 2 && ParseInt(time) == 0)
			{
				time = "0";
			}

			return time;
		}

		/// <summary>
		/// 字符串转int
		/// </summary>
		/// <param name="intStr">要进行转换的字符串</param>
		/// <param name="defaultValue">默认值，默认：0</param>
		/// <returns></returns>
		public static int ParseInt(string intStr, int defaultValue = 0)
		{
			int parseInt;
			if (int.TryParse(intStr, out parseInt))
				return parseInt;
			return defaultValue;
		}

		/// <summary>
		/// 枚举 int 转 枚举名称
		/// </summary>
		/// <typeparam name="T">枚举</typeparam>
		/// <param name="itemValue">int值</param>
		/// <returns></returns>
		public static string ConvertEnumToString<T>(string weekName)
		{
			return ((int)(Enum.Parse(typeof(T), weekName.ToString()))).ToString();
		}
	}
}
