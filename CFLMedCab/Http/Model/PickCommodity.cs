﻿using CFLMedCab.Http.Model.Base;
using CFLMedCab.Http.Model.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Http.Model
{
	/// <summary>
	/// 拣选商品管理理资料
	/// </summary>
	[JsonObject(MemberSerialization.OptOut)]
	public class PickCommodity:BaseModel
	{
		/// <summary>
		/// 
		/// </summary>
		public string AllotId { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string CommodityId { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string EquipmentId { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string GoodsLocationId { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int Number { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int PickNumber { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string PickTaskId { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string StoreHouseId { get; set; }
		/// <summary>
		/// 定数包
		/// </summary>
		public string Type { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string created_at { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string created_by { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string owner { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public Permission permission { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string record_type { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string system_mod_stamp { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string updated_at { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string updated_by { get; set; }

		/// <summary>
		/// 设备名
		/// </summary>
		[JsonIgnore]
		public string EquipmentName { get; set; }

		/// <summary>
		/// 库房名
		/// </summary>
		[JsonIgnore]
		public string StoreHouseName { get; set; }

		/// <summary>
		/// 货位名
		/// </summary>
		[JsonIgnore]
		public string GoodsLocationName { get; set; }

		/// <summary>
		/// 商品名
		/// </summary>
		[JsonIgnore]
		public string CommodityName { get; set; }

        /// <summary>
        /// 计划拣货数
        /// </summary>
        [JsonIgnore]
        public int PlanPickNumber { get; set; }


        /// <summary>
        /// 当前拣货数
        /// </summary>
        [JsonIgnore]
        public int CurPickNumber { get; set; }

        /// <summary>
        /// 拣货计数器
        /// </summary>
        [JsonIgnore]
        public int CountPickNumber { get; set; } = 0;
    }
}
