using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Model
{
    /// <summary>
    /// 手术单详情
    /// </summary>
    public class FetchOrderdtl
    {
        /// <summary>
        /// 编号
        /// </summary>
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int id { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int goods_id { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string name { get; set; }

        /// <summary>
        /// 商品编码
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string goods_code { get; set; }

        /// <summary>
        /// 单品码
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string code { get; set; }

        /// <summary>
        /// 生产批号
        /// </summary>
        public string batch_number { get; set; }

        /// <summary>
        /// 生成日期
        /// </summary>
        public DateTime birth_date { get; set; }

        /// <summary>
        /// 有效期
        /// </summary>
        public DateTime expire_date { get; set; }

        /// <summary>
        /// 有效期天数
        /// </summary>
        public int valid_period { get; set; }

        /// <summary>
        /// 货位
        /// </summary>
        public string position { get; set; }

        /// <summary>
        /// 领用属性
        /// </summary>
        public int fetch_type { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string remarks { get; set; }

        /// <summary>
        /// 耗材状态：0  待领用；1 已领用；2 已退回。
        /// </summary>
        public int status { get; set; }

        /// <summary>
        /// 关联操作单据
        /// </summary>
        public int related_order_id { get; set; }

        /// <summary>
        /// 是否临时新增 0: 否 1：是；一般领用单均为否。3。手术领用单，如标识异常为异常，且异常说明为“商品不在待领目录的”为是，其余为否。
        /// </summary>
        public int is_add { get; set; }

        /// <summary>
        /// 回退单号
        /// </summary>
        public int return_order_id { get; set; }

        /// <summary>
        /// 回退时间
        /// </summary>
        public DateTime return_time { get; set; }

        /// <summary>
        /// 回退人
        /// </summary>
        public int operator_id { get; set; }
    }
}
