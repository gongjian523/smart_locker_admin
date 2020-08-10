using CFLMedCab.Infrastructure.DbHelper;
using CFLMedCab.Model;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.DAL
{
    class InOutRecordeDal
    {
        //Db
        public SqlSugarClient Db = SqlSugarHelper.GetInstance().Db;

        // 定义一个静态变量来保存类的实例
        private static InOutRecordeDal singleton;
        // 定义一个标识确保线程同步
        private static readonly object locker = new object();


        //定义公有方法提供一个全局访问点。
        public static InOutRecordeDal GetInstance()
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
                        singleton = new InOutRecordeDal();
                    }
                }
            }
            return singleton;
        }

        /// <summary>
        /// 新建出入库记录
        /// </summary>
        /// <returns></returns>
        public int NewInOutRecord(InOutRecord inOutRecord)
        {
            return Db.Insertable<InOutRecord>(inOutRecord).ExecuteReturnEntity().id;
        }


        public List<InOutRecord> GetInOutRecordByUserName(string userName)
        {
            var list = Db.Queryable<InOutRecord>()
                .Where(it => it.user_name == userName)
                .OrderBy(it => it.create_time, OrderByType.Desc)
                .Select<InOutRecord>()
                .ToList();
            return list;
        }


        public List<InOutRecord> GetAllInOutRecord()
        {
            var list = Db.Queryable<InOutRecord>()
                .OrderBy(it => it.create_time, OrderByType.Desc)
                .Select<InOutRecord>()
                .ToList();
            return list;
        }

        /// <summary>
        /// 插入出入记录详情（列表）
        /// </summary>
        /// <returns></returns>
        public void InsertInOutDetails(List<InOutDetail> list)
        {
            Db.Insertable<InOutDetail>(list).ExecuteCommand();
        }


        public List<InOutDetail> GetInOutDetailByRecordId(int id)
        {
            var list = Db.Queryable<InOutDetail>()
                .Where(it => it.in_out_id == id)
                .OrderBy(it => it.create_time, OrderByType.Desc)
                .Select<InOutDetail>()
                .ToList();
            return list;
        }

    }
}
