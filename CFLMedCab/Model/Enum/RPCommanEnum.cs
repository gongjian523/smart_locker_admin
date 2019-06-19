using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace CFLMedCab.Model.Enum
{
    class RPCommanEnum
    {
    }

    /// <summary>
    /// 上架或拣货单完成状态
    /// </summary>
    public enum RPOStatusType
    {
        /// <summary>
        /// 出库
        /// </summary>
        [Description("待完成")]
        待完成 = 0,

        /// <summary>
        /// 入库
        /// </summary>
        [Description("已完成")]
        已完成 = 1
    }

    /// <summary>
    /// 上架工单完成状态
    /// </summary>
    public enum RSOStatusType
    {
        /// <summary>
        /// 待上架
        /// </summary>
        [Description("待上架")]
        待上架 = 0,

        /// <summary>
        /// 已上架
        /// </summary>
        [Description("已上架")]
        已上架 = 1,

        /// <summary>
        /// 部分上架
        /// </summary>
        [Description("部分上架")]
        部分上架 = 2
    }


    /// <summary>
    /// 拣货工单完成状态
    /// </summary>
    public enum PSOStatusType
    {
        /// <summary>
        /// 待拣货
        /// </summary>
        [Description("待拣货")]
        待拣货 = 0,

        /// <summary>
        ///  已拣货
        /// </summary>
        [Description(" 已拣货")]
        已拣货 = 1,

        /// <summary>
        /// 部分拣货
        /// </summary>
        [Description("部分拣货")]
        部分拣货 = 2
    }

}