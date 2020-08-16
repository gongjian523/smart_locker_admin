using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Model.Enum
{
    enum SubViewType
    {
        /// <summary>
        /// 登录页
        /// </summary>
        Login,

        /// <summary>
        /// 首页
        /// </summary>
        Home,

        /// <summary>
        /// 一般领用关门页
        /// </summary>
        GerFetchClose,

        /// <summary>
        /// 手术无单领用关门页
        /// </summary>
        SurFetchWoOrderClose,

        /// <summary>
        /// 手术有单领用关门页
        /// </summary>
        SurFetchWOrderClose,

        /// <summary>
        /// 领用回退关门页
        /// </summary>
        ReturnFetchClose,

        /// <summary>
        /// 盘点详情页
        /// </summary>
        InventoryDtl,

        /// <summary>
        /// 上架关门页
        /// </summary>
        ReplenishmentClose,

        /// <summary>
        /// 快捷上架关门页
        /// </summary>
        ShelfFastClose,

        /// <summary>
        /// 调拨上架关门页
        /// </summary>
        AllotShelfClose,

        /// <summary>
        /// 回收取货和库存调整关门页
        /// </summary>
        ReturnClose,

        /// <summary>
        /// 拣货的关门页
        /// </summary>
        ReturnGoodsClose,

        /// <summary>
        /// 开门页
        /// </summary>
        DoorOpen,

        /// <summary>
        /// 其他页
        /// </summary>
        Others,
    }
}
