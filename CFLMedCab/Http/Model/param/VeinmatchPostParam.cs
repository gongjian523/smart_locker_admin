using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Http.Model.param
{

    /// <summary>
    /// 指静脉注册,参数
    /// </summary>
    public class VeinregisterPostParam
    {
        /// <summary>
        /// 本地设备的签名 
        /// </summary>
        public string devsign { get; set; }

    }

    /// <summary>
    /// 指静脉绑定,参数
    /// </summary>
    public class VeinbindingPostParam
	{
		/// <summary>
		///  3 个指静脉特征 
		/// </summary>
		public string regfeature { get; set; }

		/// <summary>
		/// 手指名， 非必需，默认"finger_1" 
		/// </summary>
		public string finger_name { get; set; }

	}


    /// <summary>
    /// 指静脉绑定,参数
    /// </summary>
    public class VeinmatchPostParam
    {
        /// <summary>
        ///  3 个指静脉特征 
        /// </summary>
        public string regfeature { get; set; }

    }
}
