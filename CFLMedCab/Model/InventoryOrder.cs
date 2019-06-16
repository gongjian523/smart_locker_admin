using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Model
{
    /// <summary>
    /// 盘点类型:：0  自动盘点；1 手动盘点
    /// </summary>
    public enum InventoryType{
        Auto,
        Manual
    }

    /// <summary>
    /// 盘点单状态：0  待确认；1  已确认。
    /// </summary>
    public enum InventoryStatus
    {
        Unconfirm,
        Confirm
    }

    /// <summary>
    /// 账面库存：0 手工新增；1  账面存在
    /// </summary>
    public enum GoodsInventoryStatus
    {
        Manual,
        Auto
    }


    public class InventoryOrder
    {
        /// <summary>
        /// 盘点编号
        /// </summary>
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int id { get; set; }
        /// <summary>
        /// 盘点单号
        /// </summary>
        public string code { get; set; }

        /// <summary>
        /// 盘点类型:：0  自动盘点；1 手动盘点
        /// </summary>
        public int type { get; set; }

        /// <summary>
        /// 盘点时间
        /// </summary>
        public DateTime create_time { get; set; }

        /// <summary>
        /// 确认时间
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public DateTime confirm_time { get; set; }

        /// <summary>
        /// 盘点人
        /// </summary>
        public int operator_id { get; set; }

        /// <summary>
        /// 盘点人姓名
        /// </summary>
        public string operator_name { get; set; }

        /// <summary>
        /// 确认人
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int inspector_id { get; set; }

        /// <summary>
        /// 确认人姓名
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string inspector_name { get; set; }

        /// <summary>
        /// 盘点单状态：0  待确认；1  已确认。
        /// </summary>
        public int status { get; set; }
    }

    public class InventoryDetailPara : InventoryOrder
    {
        /// <summary>
        /// 按钮的类型
        /// </summary>
        /// 0 确认， 1查询详情
        public int btnType { get; set; }
    }

}
