using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Model
{
    /// <summary>
    /// 货柜信息
    /// </summary>
    public class Locations
    {
        /// <summary>
        /// 货柜编号
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 货柜id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 锁串口号
        /// </summary>
        public string LockerCom { get; set; }

        /// <summary>
        /// Rfid串口号
        /// </summary>
        public string RFCom { get; set; }
    }
}
