using AutoMapper;
using CFLMedCab.APO;
using CFLMedCab.APO.Inventory;
using CFLMedCab.DAL;
using CFLMedCab.DTO;
using CFLMedCab.DTO.Goodss;
using CFLMedCab.DTO.Inventory;
using CFLMedCab.DTO.Replenish;
using CFLMedCab.Infrastructure;
using CFLMedCab.Infrastructure.ToolHelper;
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
		/// 新增盘点记录
		/// </summary>
        public int NewInventory(List<GoodsDto> list,  InventoryType type)
        {

            InventoryOrderLDB inventoryOrder = new InventoryOrderLDB();
            var ran = new Random();

            inventoryOrder.create_time = System.DateTime.Now;
            inventoryOrder.type = (int)type;

            //设置状态为待确认
            inventoryOrder.status = (int)InventoryStatus.Unconfirm;

            if (type == InventoryType.Manual)
            {
                //获取当前用户
                inventoryOrder.operator_id = ApplicationState.GetValue<CurrentUser>((int)ApplicationKey.CurUser).id;
                inventoryOrder.operator_name = ApplicationState.GetValue<CurrentUser>((int)ApplicationKey.CurUser).name;
                inventoryOrder.type = (int)InventoryType.Manual;
            }
            else
            {
                inventoryOrder.type = (int)InventoryType.Auto;
            }

            inventoryOrder.code = "INV" + DateTime.Now.ToString("yyyyMMddHHmm") + ran.Next(9999);

            //生成记录

            int id =  inventoryDal.NewInventory(inventoryOrder);

            InsertInventoryDetails(list, id);

            return id;
        }

        /// <summary>
        /// 更新盘点记录
        /// </summary>
        public void ConfirmInventory(InventoryOrderLDB inventoryOrder)
        {
            //设置状态为已确认
            inventoryOrder.status = (int)InventoryStatus.Confirm;
            //设置当前时间
            inventoryOrder.confirm_time = DateTime.Now;
            //设置当前盘点确认人姓名和id
            inventoryOrder.inspector_id = ApplicationState.GetValue<CurrentUser>((int)ApplicationKey.CurUser).id;
            inventoryOrder.inspector_name = ApplicationState.GetValue<CurrentUser>((int)ApplicationKey.CurUser).name;

            inventoryDal.ConfirmInventory(inventoryOrder);
        }


        /// <summary>
        /// 获取盘点记录
        /// </summary>
        /// <returns></returns>
        public BasePageDataDto<InventoryOrderDto> GetInventoryOrder(InventoryOrderApo pageDataApo)
        {
            return new BasePageDataDto<InventoryOrderDto>()
            {
                PageIndex = pageDataApo.PageIndex,
                PageSize = pageDataApo.PageSize,
                Data = inventoryDal.GetInventoryOrder(pageDataApo, out int totalCount),
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 插入盘点记录详情
        /// </summary>
        /// <returns></returns>
        public void InsertInventoryDetails(List<GoodsDto> list, int invertoryOrderId)
        {
            var config = new MapperConfiguration(x => x.CreateMap<GoodsDto, InventoryOrderdtl>()
                                                        .ForMember(d => d.goods_id, o => o.MapFrom(s => s.id) ));
            IMapper mapper = new Mapper(config);
            var listDtl =  mapper.Map<List<InventoryOrderdtl>>(list);

            foreach (InventoryOrderdtl item in listDtl)
            {
                item.inventory_order_id = invertoryOrderId;
                item.goods_type = (int)GoodsInventoryStatus.Auto;
                item.book_inventory = 1;
                item.actual_inventory = 1;
                item.num_differences = 0;
            }

            inventoryDal.InsertInventoryDetails(listDtl);
        }

        /// <summary>
        /// 通过盘点单号获取盘点记录详情
        /// </summary>
        /// <returns></returns>
        public List<InventoryOrderdtl> GetInventoryDetailsByInventoryId(int invertoryOrderId)
        {
            return inventoryDal.GetInventoryDetailsByInventoryId(invertoryOrderId);
        }

        /// <summary>
        /// 通过盘点单号获取盘点记录
        /// </summary>
        /// <returns></returns>
        public List<InventoryOrderDto> GetInventoryOrdersByInventoryId(int invertoryOrderId)
        {
            return inventoryDal.GetInventoryOrdersByInventoryId(invertoryOrderId);
        }


        /// <summary>
        /// 查询单品码是否在一个库存单中
        /// </summary>
        /// <returns></returns>
        public bool IsGoodsInInventoryOrder(int invertoryOrderId, string code)
        {
            var list = inventoryDal.GetInventoryDetailsByInventoryId(invertoryOrderId);
            return (list.Where(item => item.code == code).ToList().Count > 0);
        }

        /// <summary>
        /// 更新盘点记录详情
        /// </summary>
        /// <returns></returns>
        public void UpdateInventoryDetails(List<InventoryOrderdtl> list)
        {
            //更新已有的计划
            var oldlist = list.Where(item => item.id != 0).ToList();
            if (oldlist.Count != 0)
                inventoryDal.UpdateInventoryDetails(oldlist);

            //新增计划
            var newlist = list.Where(item => item.id == 0).ToList();
            if (newlist.Count != 0)
                inventoryDal.InsertInventoryDetails(newlist);
        }

        /// <summary>
        /// 新建盘点计划
        /// </summary>
        /// <returns></returns>
        public void InventoryPlan(List<InventoryPlanLDB> list)
        {
            inventoryDal.InsertInventoryPlan(list);
        }

        /// <summary>
        /// 更新盘点计划
        /// </summary>
        /// <returns></returns>
        public void UpdateInventoryPlan(List<InventoryPlanLDB> list)
        {
            //更新已有的计划
            var oldlist = list.Where(item => item.id != 0).ToList();
            if(oldlist.Count != 0)
                inventoryDal.UpdateInventoryPlan(oldlist);

            //新增计划
            var newlist = list.Where(item => item.id == 0).ToList();
            if (newlist.Count != 0)
                inventoryDal.InsertInventoryPlan(newlist);
        }

        public List<InventoryPlanLDB> GetInventoryPlan()
        {
            return inventoryDal.GetInventoryPlan();
        }
    }
}
