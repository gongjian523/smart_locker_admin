using System.Collections.Generic;
using System.Collections;

namespace CFLMedCab.Infrastructure.ToolHelper
{
	public class CollectHelper
	{
		/// <summary>
		/// 对比集合新增和删除
		/// </summary>
		/// <param name="originalCollect">原始集合</param>
		/// <param name="targetCollect">用来比较的目标集合</param>
		/// <param name="addCollect">比较后新增的集合，可能为空</param>
		/// <param name="deleteCollect">比较后删除的集合，可能为空</param>
		public static void CompareCollect(HashSet<string> originalCollect, HashSet<string> targetCollect, out HashSet<string> addCollect, out HashSet<string> deleteCollect)
		{
			addCollect = new HashSet<string>();
			deleteCollect = new HashSet<string>();
			foreach (string currentEps in targetCollect)
			{
				if (!originalCollect.Contains(currentEps))
				{
					addCollect.Add(currentEps);
				}
			}

			foreach (string currentEps in originalCollect)
			{
				if (!targetCollect.Contains(currentEps))
				{
					deleteCollect.Add(currentEps);
				}
			}

		}

		/// <summary>
		/// 对比集合新增和删除
		/// </summary>
		/// <param name="originalCollect">原始集合</param>
		/// <param name="targetCollect">用来比较的目标集合</param>
		/// <param name="addCollect">比较后新增的集合，可能为空</param>
		/// <param name="deleteCollect">比较后删除的集合，可能为空</param>
		public static void CompareCollect(Hashtable originalCollect, Hashtable targetCollect, out HashSet<string> addCollect, out HashSet<string> deleteCollect)
		{

			addCollect = new HashSet<string>();
			deleteCollect = new HashSet<string>();
			foreach (string currentEps in targetCollect.Values)
			{

				if (!originalCollect.ContainsValue(currentEps))
				{
					addCollect.Add(currentEps);
				}
			}

			foreach (string currentEps in originalCollect.Values)
			{
				if (!targetCollect.ContainsValue(currentEps))
				{
					deleteCollect.Add(currentEps);
				}
			}

		}

	}
}
