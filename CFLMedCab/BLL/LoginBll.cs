using CFLMedCab.DAL;
using CFLMedCab.Infrastructure;
using CFLMedCab.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.BLL
{
    public class LoginBll
    {
        /// <summary>
        /// 获取操作实体
        /// </summary>
        private readonly LoginDal loginDal;

        public LoginBll()
        {
            loginDal = LoginDal.GetInstance();
        }

        public int NewLogin()
        {

            LoginRecord record = new LoginRecord() {
                login_time = System.DateTime.Now,
                user_name = ApplicationState.GetUserInfo().name,
                phone_num = ApplicationState.GetUserInfo().Phone
            };


            //生成记录
            int id = loginDal.NewLoginRecode(record);
            return id;
        }


        public void  UptadeLoingOutInfo(int id, string info)
        {
            LoginRecord record = loginDal.GetLoginRecordById(id);

            record.logout_time = System.DateTime.Now;
            record.logout_info = info;

            loginDal.UpdateLoginRecode(record);
        }


        public List<LoginRecord> GetAllLoginRecord()
        {
            return loginDal.GetAllLoginRecord();
        }

        public List<LoginRecord> GetLoginRecordByUserName(string name)
        {
            return loginDal.GetLoginRecordByUserName(name);
        }
    }
}
