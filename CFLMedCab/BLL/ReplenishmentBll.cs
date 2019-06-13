using CFLMedCab.DAL;
using CFLMedCab.Model;
using System.Collections.Generic;

namespace CFLMedCab.BLL
{
	/// <summary>
	/// 补货入库业务层
	/// </summary>
	class ReplenishmentBll
	{
		
		private readonly ReplenishDal replenishDal;

		public ReplenishmentBll()
		{
			replenishDal = ReplenishDal.GetInstance();
		}

		/// <summary>
		/// 获取待完成上架工单
		/// </summary>
		/// <returns></returns>
		public List<ReplenishOrder> GetIncompleteWorkOrder()
		{

			return null;
		}
	}
}
