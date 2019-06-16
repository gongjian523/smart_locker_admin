using CFLMedCab.Infrastructure.DbHelper;
using CFLMedCab.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.DAL
{



    //public class InventoryPlanDal : SqlSugarContext<InventoryPlan>
    //{
        ///// <summary>
        ///// 数据库没有表时创建
        ///// </summary>
        //public void CreateTable_InventoryPlan()
        //{
        //    string commandText = @"CREATE TABLE if not exists inventory_plan ('id' INTEGER PRIMARY KEY AUTOINCREMENT, 
        //                                                           'inventory_time' not null default (datetime('localtime')),
        //                                                           'status' INTEGER);";
        //    SqliteHelper.Instance.ExecuteNonQuery(commandText);
        //    return;
        //}

        ///// <summary>
        ///// 新增盘点计划
        ///// </summary>
        ///// <param name="pickingOrder"></param>
        ///// <returns></returns>
        //public int InsertNewInventoryPlan(InventoryPlan inventoryPlan)
        //{
        //    string commandText = string.Format(@"INSERT INTO inventory_plan (inventory_time, status) VALUES 
        //                                        ('{0}', '{1}')",
        //                                        inventoryPlan.inventory_time, inventoryPlan.status);

        //    if (!SqliteHelper.Instance.ExecuteNonQuery(commandText))
        //        return 0;


        //    return LastInsertRowId();
        //}

        ///// <summary>
        ///// 查找所有的盘点计划
        ///// </summary>
        ///// <returns></returns>
        //public DataTable GetAllInventoryPlan()
        //{
        //    DataTable dataList = new DataTable("Table_New");
        //    dataList.Columns.Add("id", Type.GetType("System.String"));
        //    dataList.Columns.Add("inventory_time", Type.GetType("System.String"));
        //    dataList.Columns.Add("status", Type.GetType("System.String"));
        //    IDataReader data = SqliteHelper.Instance.ExecuteReader(string.Format(@"SELECT * FROM inventory_plan "));
        //    if (data == null)
        //        return dataList;
        //    while (data.Read())
        //    {
        //        DataRow entity = dataList.NewRow();
        //        entity[0] =Convert.ToInt32(data["id"].ToString());
        //        entity[1] = Convert.ToDateTime(data["inventory_time"]);
        //        entity[2] = Convert.ToInt32(data["status"]);
        //        dataList.Rows.Add(entity);
        //    }
        //    return dataList;
        //}

        //private int LastInsertRowId()
        //{
        //    return Convert.ToInt16(SqliteHelper.Instance.ExecuteScalar("SELECT last_insert_rowid();"));
        //}
    //}
}
