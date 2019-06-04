using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Model
{
    /// <summary>
    /// 库存变化单详情
    /// </summary>
    public class GoodsChageOrderdtl
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 库存变化单编号
        /// </summary>
        public int good_change_orderid { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public int goods_id { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 商品编码
        /// </summary>
        public string goods_code { get; set; }

        /// <summary>
        /// 单品码
        /// </summary>
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
        /// 操作类型 操作类型：0 出库；1 入库
        /// </summary>
        public int operate_type { get; set; }

        /// <summary>
        /// 异常标识 异常标识：1 异常； 0  非异常
        /// </summary>
        public int exceptional { get; set; }

        /// <summary>
        /// 异常说明
        /// </summary>
        public string explain { get; set; }

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
        /// 业务确认状态 状态： 0  待确认；1 已确认
        /// </summary>
        public int status { get; set; }

        /// <summary>
        /// 关联操作单据 1。标识非异常的商品，自动关联业务单号。
        /// </summary>
        public int related_order_id { get; set; }
    }
}
