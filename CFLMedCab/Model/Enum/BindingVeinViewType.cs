using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Model.Enum
{
    //进入绑定指静脉的种类
    public enum BindingVeinViewType
    {
        /// <summary>
        /// 绑定指静脉
        /// </summary>
        VeinBinding=0,

        /// <summary>
        /// 指静脉登录
        /// </summary>
        VeinLogin=1,

        /// <summary>
        /// 用户名密码登录
        /// </summary>
        PswLogin=2
    }
}
