﻿using CFLMedCab.Http.Model.Base;
using CFLMedCab.Http.Model.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Http.Model
{
    /// <summary>
    /// 商品库存管理理资料表
    /// </summary>
    public class CommodityInventory : BaseModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string CommodityCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string CommodityCodeId { get; set; }
        /// <summary>
        /// 商品名称编号
        /// </summary>
        public string CommodityId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string DepartmentId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string EquipmentId { get; set; }
        
        public DateTime ExpirationDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string GoodsLocationId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string GroupId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ManufactorName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Model { get; set; }
        /// <summary>
        /// 质量状态
        /// </summary>
        public string QualityStatus { get; set; }

        /// <summary>
        /// 未领用
        /// </summary>
        public string Spec { get; set; }

        /// <summary>
        /// 未领用
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 所在库房编号
        /// </summary>
        public string StoreHouseId { get; set; }

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
        public object unusedAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string updated_at { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string updated_by { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public object usedAmount { get; set; }
    }



}
