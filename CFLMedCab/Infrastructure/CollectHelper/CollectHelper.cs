using System.Collections.Generic;

namespace CFLMedCab.Infrastructure.CollectHelper
{
	public class CollectHelper
	{
		/// <summary>
		/// 
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
	}
}
