using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Model
{
    public class InventoryOrderdal
    {
        /// <summary>
        /// 盘点详情单号
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 盘点单编号
        /// </summary>
        public int inventory_order_id { get; set; }

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
        /// 1。类型：账面存在；盘点新增。2。如该商品RFID读卡器感应到，则为账面存在。3。如为手动新增则为盘点新增默认为待上架，对所属工单进行操作的库存变化单中有该商品，且操作非异常，确认后状态变为已上架
        /// </summary>
        public int goods_type { get; set; }

        /// <summary>
        /// 账面库存
        /// </summary>
        public int book_inventory { get; set; }

        /// <summary>
        /// 实际库存
        /// </summary>
        public int actual_inventory { get; set; }

        /// <summary>
        /// 差异数量
        /// </summary>
        public int num_differences { get; set; }
    }
}
