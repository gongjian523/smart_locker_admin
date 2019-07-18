using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace CFLMedCab.Http.Helper
{
	/// <summary>
	/// 业务工具处理类
	/// </summary>
	public class BllHelper
	{
		/// <summary>
		/// 通用json list参数的urlEncode编码
		/// </summary>
		/// <param name="paramList"></param>
		/// <returns></returns>
		public static List<string> ParamUrlEncode(List<string> paramList)
		{
			for (int i = 0, len = paramList.Count; i < len; i++)
			{
				paramList[i] = HttpUtility.UrlEncode(paramList[i]);
			}
	        return paramList;

		}

		/// <summary>
		/// 加单引号操作,用于查询表达式
		/// </summary>
		/// <param name="operands"></param>
		/// <returns></returns>
		public static List<string> OperandsProcess(List<string> operands)
		{

			for (int i = 0, len = operands.Count; i < len; i++)
			{
				operands[i] = $"'{ HttpUtility.UrlEncode(operands[i]) }'";
			}

			return operands;
		}

		/// <summary>
		/// 查询表达式内部的字符串转换成base64
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string EncodeBase64Str(string str)
		{
			return Convert.ToBase64String(Encoding.ASCII.GetBytes(str));
		}


	}
}
