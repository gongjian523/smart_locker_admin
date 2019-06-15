using CFLMedCab.APO;
using CFLMedCab.DAL;
using CFLMedCab.DTO;
using CFLMedCab.DTO.Goodss;
using CFLMedCab.DTO.Replenish;
using CFLMedCab.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.BLL
{
    /// <summary>
	/// 盘点业务层
	/// </summary>
    class InventoryBll
    {
        /// <summary>
		/// 获取操作实体
		/// </summary>
        private readonly InventoryDal inventoryDal;

        public InventoryBll()
            {
            inventoryDal= InventoryDal.GetInstance();
            }

        /// <summary>
		/// 手动新增盘点记录
		/// </summary>
        public void addInventory(InventoryOrder inventoryOrder)
        {
            inventoryDal.addInventory(inventoryOrder);
        }

        /// <summary>
		/// 更新盘点记录
		/// </summary>
        public void confirmInventory(InventoryOrder inventoryOrder)
        {
            inventoryDal.confirmInventory(inventoryOrder);
        }


        /// <summary>
        /// 获取所有库存商品
        /// </summary>
        /// <param name="basePageDataApo"></param>
        /// <returns></returns>
        public BasePageDataDto<GoodsDto> GetPageGoods(BasePageDataApo basePageDataApo)
        {
            return new BasePageDataDto<GoodsDto>()
            {
                PageIndex = basePageDataApo.PageIndex,
                PageSize = basePageDataApo.PageSize,
                Data = inventoryDal.GetGoods(basePageDataApo, out int totalCount),
                TotalCount = totalCount
            };

        }
    }
}
