using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Http.Enum
{
	/// <summary>
	/// 异常原因
	/// </summary>
	public enum AbnormalCauses
	{
		/// <summary>
		/// 待上架
		/// </summary>
		[Description("商品缺失 ")]
		商品缺失 = 1,

		/// <summary>
		/// 已完成 
		/// </summary>
		[Description("商品损坏")]
		商品损坏 = 2,

		/// <summary>
		/// 异常
		/// </summary>
		[Description("商品遗失")]
		商品遗失 = 3,

        /// <summary>
        /// 其他
        /// </summary>
        [Description("其他")]
        其他 = 4,

		/// <summary>
		/// 商品超出
		/// </summary>
		[Description("商品超出")]
		商品超出 = 5,

		/// <summary>
		/// 未选
		/// </summary>
		[Description("未选")]
        未选 = 6

    }
}
