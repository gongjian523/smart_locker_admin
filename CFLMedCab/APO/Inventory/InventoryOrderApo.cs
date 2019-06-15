using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.APO.Inventory
{
    public class InventoryOrderApo : BasePageDataApo
    {
        /// <summary>
        /// 盘点单状态：0  待确认；1  已确认。
        /// </summary>
        public int status { get; set; }
    }
}
