using CFLMedCab.APO.GoodsChange;
using CFLMedCab.DTO.Goodss;
using CFLMedCab.Infrastructure;
using CFLMedCab.Infrastructure.DbHelper;
using CFLMedCab.Infrastructure.ToolHelper;
using CFLMedCab.Model;
using CFLMedCab.Model.Enum;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.DAL
{
    /// <summary>
    /// 盘点库存dao层
    /// </summary>
    public class GoodsChangeOrderDal
    {
        //Db
        public SqlSugarClient Db = SqlSugarHelper.GetInstance().Db;

        // 定义一个静态变量来保存类的实例
        private static GoodsChangeOrderDal singleton;
        // 定义一个标识确保线程同步
        private static readonly object locker = new object();


        //定义公有方法提供一个全局访问点。
        public static GoodsChangeOrderDal GetInstance()
        {
            //这里的lock其实使用的原理可以用一个词语来概括“互斥”这个概念也是操作系统的精髓
            //其实就是当一个进程进来访问的时候，其他进程便先挂起状态
            if (singleton == null)
            {
                lock (locker)
                {
                    // 如果类的实例不存在则创建，否则直接返回
                    if (singleton == null)
                    {
                        singleton = new GoodsChangeOrderDal();
                    }
                }
            }
            return singleton;
        }


        /// <summary>
        /// 获取出入库记录
        /// </summary>
        /// <returns></returns>
        public List<GoodsChangeDto> GetGoodsChange(GoodsChangeApo pageDataApo, out int totalCount)
        {
            totalCount = 0;
            List<GoodsChangeDto> data;

            //查询语句
            var queryable = Db.Queryable<GoodsChageOrderdtl, GoodsChageOrder>((ordtl, orl) => new object[] { JoinType.Left, ordtl.related_order_id == orl.id })
                .Where((ordtl, orl) => ordtl.operate_type == pageDataApo.operate_type)
                .WhereIF(pageDataApo.startTime.HasValue, (ordtl, orl) => orl.create_time >= pageDataApo.startTime)
                .WhereIF(pageDataApo.endTime.HasValue, (ordtl, orl) => orl.create_time <= pageDataApo.endTime)
                .WhereIF((!string.IsNullOrEmpty(pageDataApo.name) && !string.IsNullOrWhiteSpace(pageDataApo.name)), (ordtl, orl) => ordtl.name.Contains(pageDataApo.name))

                 .OrderBy((ordtl, orl) => orl.create_time, OrderByType.Desc)
                .Select((ordtl, orl) => new GoodsChangeDto
                {
                    id = ordtl.id,
					related_order_id = ordtl.related_order_id,
                    goods_id = ordtl.goods_id,
                    name = ordtl.name,
                    goods_code = ordtl.goods_code,
                    code = ordtl.code,
                    batch_number = ordtl.batch_number,
                    birth_date = ordtl.birth_date,
                    expire_date = ordtl.expire_date,
                    create_time = orl.create_time,
                    business_type=orl.business_type,
                    operate_type= ordtl.operate_type
                });


            //如果小于0，默认查全部
            if (pageDataApo.PageSize > 0)
            {
                data = queryable.ToPageList(pageDataApo.PageIndex, pageDataApo.PageSize, ref totalCount);
            }
            else
            {
                data = queryable.ToList();
                totalCount = data.Count();
            }
            return data;
        }


		/// <summary>
		///  插入变化后的商品信息
		/// </summary>
		/// <param name="goodsDtos">所有数据</param>
		/// <param name="businessOrderCode">关联业务单号</param>
		/// <param name="requisitionType">业务类型</param>
		/// <returns></returns>
		public bool InsertGoodsChageOrderInfo(List<GoodsDto> goodsDtos, RequisitionType requisitionType)
		{

			if (goodsDtos == null || goodsDtos.Count <= 0) return false;
			
            //事务防止多插入产生脏数据
            var result = Db.Ado.UseTran(() =>
            {
				//库存变化单id
				int goodsChageOrderId = Db.Insertable(new GoodsChageOrder
				{
					//用uuid当作新生成的领用单单号
					code = Guid.NewGuid().ToString("N"),
					create_time = DateTime.Now,
					operator_id = ApplicationState.GetValue<CurrentUser>((int)ApplicationKey.CurUser).id,
					business_type = (int)requisitionType
                }).ExecuteReturnIdentity();

                List<GoodsChageOrderdtl> goodsChageOrderdtls = goodsDtos.MapToListIgnoreId<GoodsDto, GoodsChageOrderdtl>();

				goodsChageOrderdtls.ForEach(it =>
				{
					it.related_order_id = goodsChageOrderId;
				
				});

                Db.Insertable(goodsChageOrderdtls).ExecuteCommand();

            });

            return result.IsSuccess;
        }

		/// <summary>
		///  插入变化后的商品信息
		/// </summary>
		/// <param name="goodsDtos">所有数据</param>
		/// <param name="businessOrderCode">关联业务单号</param>
		/// <param name="requisitionType">业务类型</param>
		/// <returns></returns>
		public bool InsertGoodsChageOrderInfo(List<GoodsDto> goodsDtos, string businessOrderCode, RequisitionType requisitionType)
		{

			if (goodsDtos == null || goodsDtos.Count <= 0) return false;

			//若有业务单号，则一次操作商品集，拼接该业务单号
			goodsDtos.ForEach(it=>{
				it.business_order_code = businessOrderCode;
			});

			return InsertGoodsChageOrderInfo(goodsDtos, requisitionType);

		}

	}

}
