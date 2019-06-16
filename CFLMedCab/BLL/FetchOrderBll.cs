using CFLMedCab.DAL;
using CFLMedCab.DTO.Goodss;
using CFLMedCab.Infrastructure;
using CFLMedCab.Model;
using CFLMedCab.Model.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.BLL
{

    public class FetchOrderBll
    {
        private readonly FetchOrderDal FetchOrderDal  = FetchOrderDal.GetInstance();

		/// <summary>
		///  获取一般领用操作情况
		/// </summary>
		/// <param name="goodsDtos"></param>
		/// <returns></returns>
		public List<GoodsDto> GetFetchOrderdtlOperateDto(List<GoodsDto> goodsDtos)
		{

			//组装当前状态
			goodsDtos.ForEach(it =>
			{

				//入库
				if (it.operate_type == (int)OperateType.入库)
				{
					it.operate_type_description = OperateType.入库.ToString();
					it.exception_flag = (int)ExceptionFlag.异常;
					it.exception_flag_description = ExceptionFlag.异常.ToString();
					it.exception_description = ExceptionDescription.领用属性与业务类型冲突.ToString();
				}
				//出库
				else if (it.operate_type == (int)OperateType.出库)
				{
					it.operate_type_description = OperateType.出库.ToString();
					it.exception_flag = (int)ExceptionFlag.正常;
					
				}
			});


			//均升序排列
			return goodsDtos.OrderBy(it => it.exception_flag).ThenBy(it => it.expire_date).ToList();
		}


	}
}
