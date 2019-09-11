using CFLMedCab.Http.Model;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Model
{
    /// <summary>
    /// 医疗柜使用者人员信息
    /// </summary>
    public class CurrentUser
    {
		public CurrentUser()
		{

		}

		public CurrentUser(User user, byte[] regfeature)
		{
			reg_feature = Convert.ToBase64String(regfeature);
			username = user.MobilePhone;
		}


		public CurrentUser(User user, string regfeature)
		{
			reg_feature = regfeature;
			username = user.MobilePhone;
		}


		/// <summary>
		/// 用户ID
		/// </summary>
		[SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int id { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string name { get; set; }

		/// <summary>
		/// 用户名,手机号码，带有+86
		/// </summary>
		[SugarColumn(IsNullable = true)]
		public string username { get; set; }

		/// <summary>
		/// 密码
		/// </summary>
		[SugarColumn(IsNullable = true)]
		public string password { get; set; }

		/// <summary>
		/// 角色  
		/// </summary>
		[SugarColumn(IsNullable = true)]
        public string role { get; set; }

        /// <summary>
        /// 指静脉数值图像
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int vein_id { get; set; }

		/// <summary>
		/// 指静脉数值图像(某根手指的注册特征)
		/// </summary>
		[SugarColumn(IsNullable = true, Length = 2048)]
        public string reg_feature { get; set; }

		/// <summary>
		/// 指静脉数值图像(某根手指的动态特征)
		/// </summary>
		[SugarColumn(IsNullable = true, Length = 2048)]
        public string ai_feature { get; set; }

    }
}
