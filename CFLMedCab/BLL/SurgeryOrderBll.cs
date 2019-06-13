using CFLMedCab.DAL;
using CFLMedCab.Model;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.BLL
{
    public class SurgeryOrderBll
    {
        private SurgeryOrderDal surgeryOrderDal = new SurgeryOrderDal();

        public SurgeryOrder GetById(int id)
        {
            return surgeryOrderDal.CurrentDb.GetById(id);
        }
    }
}
