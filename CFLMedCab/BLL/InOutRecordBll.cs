using CFLMedCab.DAL;
using CFLMedCab.Http.Model;
using CFLMedCab.Infrastructure;
using CFLMedCab.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.BLL
{
    public class InOutRecordBll
    {
        /// <summary>
        /// 获取操作实体
        /// </summary>
        private readonly InOutRecordeDal inOutRecordeDal;

        public InOutRecordBll()
        {
            inOutRecordeDal = InOutRecordeDal.GetInstance();
        }

        public int NewInOutRecord()
        {
            int id = inOutRecordeDal.NewInOutRecord(new InOutRecord
            {
                login_id = ApplicationState.GetLoginId(),
                open_time = DateTime.Now,
                user_name = ApplicationState.GetUserInfo().name,
            });

            return id;
        }

        public void UpdateInOutRecord(List<CommodityCode> goods = null, string business = "")
        {

            int openDoorId = ApplicationState.GetOpenDoorId();

            InOutRecord record = inOutRecordeDal.GetInOutRecordById(openDoorId);

            record.close_time = DateTime.Now;
            record.operate = business;

            inOutRecordeDal.UpdateInOutRecord(record);

            if (goods == null)
                return;

            List<InOutDetail> details = new List<InOutDetail>();

            foreach(var item in  goods)
            {
                details.Add(new InOutDetail()
                {
                    in_out_id = openDoorId,
                    login_id = ApplicationState.GetLoginId(),
                    create_time = DateTime.Now,
                    operate = business,
                    user_name = ApplicationState.GetUserInfo().name,
                    ctalogue_name = item.CommodityName,
                    position = item.GoodsLocationName,
                    in_out = item.operate_type == 0 ? "出库" : "入库",
                    manufactor_name = item.ManufactorName,
                    specifications = item.Spec,
                    model = item.Model,
                }) ; 
            }

            inOutRecordeDal.InsertInOutDetails(details);
        }

        public List<InOutRecord> GetInOutRecordByName(string name)
        {
            return inOutRecordeDal.GetInOutRecordByUserName(name);
        }

        public InOutRecord GetInOutRecordById(int id)
        {
            return inOutRecordeDal.GetInOutRecordById(id);
        }

        public List<InOutRecord> GetAllInOutRecord()
        {
            return inOutRecordeDal.GetAllInOutRecord();
        }

        public List<InOutDetail> GetInOutDetails(int recordId)
        {
            return inOutRecordeDal.GetInOutDetailByRecordId(recordId);
        }


    }
}
