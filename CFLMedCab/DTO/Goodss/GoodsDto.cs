using CFLMedCab.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CFLMedCab.DTO.Goodss
{
	public class GoodsDto: Goods
	{


		/// <summary>
		/// 操作类型 操作类型：0 出库；1 入库 （用于判断一组商品里，是否是出库或入库，比如说对比开门和关门的后的商品）
		/// </summary>
		public int operate_type { get; set; }

		/// <summary>
		/// 操作类型说明
		/// </summary>
		public string operate_type_description { get; set; }

		/// <summary>
		/// 异常标识
		/// </summary>
		public int exception_flag { get; set; }

		/// <summary>
		/// 异常标识
		/// </summary>
		public string exception_flag_description { get; set; }

		/// <summary>
		/// 异常说明
		/// </summary>
		public string exception_description { get; set; }

        /// <summary>
        /// 异常说明备份
        /// </summary>
        public string exception_description_bak { get; set; }

        /// <summary>
        /// 确认按钮是否可见
        /// </summary>
        public bool  confirm_btn_visibility { get; set; }

        /// <summary>
        /// 规格
        /// </summary>
        public string Specifications { get; set; }

        /// <summary>
        /// 有效期
        /// </summary>
        public string ExpirationDate { get; set; }

        /// <summary>
        /// 厂商
        /// </summary>
        public string ManufactorName { get; set; }
    }
}