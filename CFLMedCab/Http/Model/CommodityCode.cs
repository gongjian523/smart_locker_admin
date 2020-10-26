﻿using CFLMedCab.Http.Model.Base;
using CFLMedCab.Http.Model.Common;
using Newtonsoft.Json;
using System;

namespace CFLMedCab.Http.Model
{
	/// <summary>
	/// 商品码管理
	/// </summary>
	[JsonObject(MemberSerialization.OptOut)]
	public class CommodityCode:BaseModel
	{
		/// <summary>
		/// 
		/// </summary>
		public string CommodityId { get; set; }

		/// <summary>
		/// 厂家名称 在新系统中可以直接获取到
		/// </summary>
		public string ManufactorName { get; set; }

		/// <summary>
		/// 新系统下的型号 可以直接获取
		/// </summary>
		public string Model { get; set; }

		/// <summary>
		/// 新系统下的规格 原有Specifications的准备停止使用
		/// </summary>
		public string Spec { get; set; }

		/// <summary>
		/// 可使用
		/// </summary>
		public string Status { get; set; }

		/// <summary>
		/// RF码
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
		/// 库房id
		/// </summary>
		[JsonIgnore]
		public string StoreHouseId { get; set; }

		/// <summary>
		/// 库房Name
		/// </summary>
		[JsonIgnore]
		public string StoreHouseName { get; set; }

		/// <summary>
		/// 设备id
		/// </summary>
		[JsonIgnore]
		public string EquipmentId { get; set; }

		/// <summary>
		/// 设备name
		/// </summary>
		[JsonIgnore]
		public string EquipmentName { get; set; }

		/// <summary>
		/// 货位id
		/// </summary>
		[JsonIgnore]
		public string GoodsLocationId { get; set; }

		/// <summary>
		/// 货位name
		/// </summary>
		[JsonIgnore]
		public string GoodsLocationName { get; set; }

		/// <summary>
		/// 操作类型 0 出库 1 入库
		/// </summary>
		[JsonIgnore]
		public int operate_type { get; set; }

		/// <summary>
		/// 商品名称（一类）
		/// </summary>
		[JsonIgnore]
		public string CommodityName { get; set; }

		/// <summary>
		/// 异常显示逻辑
		/// </summary>
		[JsonIgnore]
		public string AbnormalDisplay { get; set; }

		/// <summary>
		/// 商品库存编码id
		/// </summary>
		public string CommodityInventoryId { get; set; }

		/// <summary>
		/// 是否损坏，在手动盘点两种设置
		/// </summary>
		[JsonIgnore]
		public string QStatus { get; set; } = "正常";

        /// <summary>
        /// 商品库存明细
        /// </summary>
        [JsonIgnore]
        public string CommodityInventoryDetailId { get; set; }

        /// <summary>
        /// 失效日期
        /// </summary>
        [JsonIgnore]
        public DateTime ExpirationDate { get; set; }

        /// <summary>
        /// 生产批号
        /// </summary>
        [JsonIgnore]
        public string BatchNumberId { get; set; }


        /// <summary>
        /// 医院货品ID
        /// </summary>
        [JsonIgnore]
        public string HospitalGoodsId { get; set; }

        /// <summary>
        /// 规格
        /// </summary>
		[Obsolete]
        [JsonIgnore]
        public string Specifications { get; set; }

        /// <summary>
        /// 目录id
        /// </summary>
        [JsonIgnore]
		[Obsolete]
		public string CatalogueId { get; set; }

        /// <summary>
        /// 目录名字
        /// </summary>
        [JsonIgnore]
        public string CatalogueName { get; set; }

        /// <summary>
        /// 质量状态，从CommodityInventoryDetail而来
        /// </summary>
        [JsonIgnore]
        public string QualityStatus { get; set; }

        /// <summary>
        /// 库存状态，从CommodityInventoryDetail而来
        /// </summary>
        [JsonIgnore]
        public string InventoryStatus { get; set; }

		/// <summary>
		/// 科室id，从CommodityInventoryDetail而来
		/// </summary>
		[JsonIgnore]
		public string DepartmentId { get; set; }

		/// <summary>
		/// 科室名称，从Department而来
		/// </summary>
		[JsonIgnore]
		public string Department { get; set; }
	}
}
