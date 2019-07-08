using CFLMedCab.BLL;
using CFLMedCab.Infrastructure;
using CFLMedCab.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using static CFLMedCab.Model.Enum.UserIdEnum;

namespace CFLMedCab.Test
{
    public class TestGoods
    {
        private GoodsBll goodsBll = new GoodsBll();
        private UserBll userBll = new UserBll();
        private ReplenishBll replenishBll = new ReplenishBll();
        private PickingBll pickingBll = new PickingBll();
        private FetchOrderBll fetchOrderBll = new FetchOrderBll();

        public Hashtable  GetCurrentRFid()
        {
            Hashtable ht = new Hashtable();
            HashSet<string> hs1 = new HashSet<string> { "E20000176012027919504D98", "E20000176012025319504D67", "E20000176012025619504D70", "E20000176012028119504DA5", "E20000176012023919504D48" };
            ht.Add("COM1", hs1);
            HashSet<string> hs4 = new HashSet<string> { "E20000176012028219504DAD", "E20000176012026619504D8D", "E20000176012026319404F98", "E20000176012028019504DA0", "E20000176012026519504D85" };
            ht.Add("COM4", hs4);
            ApplicationState.SetValue((int)ApplicationKey.CurGoods, ht);
            return ht;
        }


        public void InitUsersInfo()
        {
            if (userBll.GetUserNum() > 0)
                return;

            List<User> users = new List<User> {
                new User{
                    name = "Alex",
                    role = (int)UserIdType.SPD交收员,
                    vein_id = 58268,
                },
                new User{
                    name = "郭颖Doc",
                    role = (int)UserIdType.医生,
                    vein_id = 57688,
                },
                new User{
                    name = "郭颖SPD",
                    role = (int)UserIdType.SPD交收员,
                    vein_id = 61886,
                },
                new User{
                    name = "Nathan",
                    role = (int)UserIdType.医生,
                    vein_id = 58046,
                },
                new User{
                    name = "Kate",
                    role = (int)UserIdType.医生,
                    vein_id = 12630,
                },
                new User{
                    name = "ZHIWEN",
                    role = (int)UserIdType.医生,
                    vein_id = 62800,
                },
            };
            userBll.InsetUsers(users);
        }

        public void InitGoodsInfo()
        {
            if (goodsBll.GetGoodsTypeNum() > 0)
                return;

            List<Goods> goods = new List<Goods>{
                new Goods
                {
                    code = "E20000176012027919504D98",
                    name = "一次性输液器",
                    goods_code = "B20001",
                    batch_number = "C1111",
                    birth_date = new DateTime(2019, 1, 2),
                    valid_period = 180,
                    expire_date = new DateTime(2019, 7, 2),
                    position = "CAB1-1",
                    fetch_type = 1,
                    remarks = ""
                },
                new Goods
                {
                    code = "E20000176012025319504D67",
                    name = "一次性输液器",
                    goods_code = "B20001",
                    batch_number = "C1112",
                    birth_date = new DateTime(2019, 2, 2),
                    valid_period = 180,
                    expire_date = new DateTime(2019, 8, 2),
                    position = "CAB1-1",
                    fetch_type = 1,
                    remarks = ""
                },
                new Goods
                {
                    code = "E20000176012025619504D70",
                    name = "一次性输液器",
                    goods_code = "B20001",
                    batch_number = "C1113",
                    birth_date = new DateTime(2019, 3, 2),
                    valid_period = 180,
                    expire_date = new DateTime(2019, 9, 2),
                    position = "CAB1-1",
                    fetch_type = 2,
                    remarks = ""
                },
                new Goods
                {
                    code = "E20000176012028119504DA5",
                    name = "医用胶布卷",
                    goods_code = "B30001",
                    batch_number = "C2111",
                    birth_date = new DateTime(2019, 2, 12),
                    valid_period = 90,
                    expire_date = new DateTime(2019, 5, 12),
                    position = "CAB1-2",
                    fetch_type = 1,
                    remarks = ""
                },
                new Goods
                {
                    code = "E20000176012023919504D48",
                    name = "医用胶布卷",
                    goods_code = "B30001",
                    batch_number = "C2112",
                    birth_date = new DateTime(2019, 3, 12),
                    valid_period = 90,
                    expire_date = new DateTime(2019, 6, 12),
                    position = "CAB1-2",
                    fetch_type = 1,
                    remarks = ""
                },
                new Goods
                {
                    code = "E20000176012028219504DAD",
                    name = "医用胶布卷",
                    goods_code = "B30001",
                    batch_number = "C2113",
                    birth_date = new DateTime(2019, 4, 12),
                    valid_period = 90,
                    expire_date = new DateTime(2019, 7, 12),
                    position = "CAB1-2",
                    fetch_type = 2,
                    remarks = ""
                },
                new Goods
                {
                    code = "E20000176012026619504D8D",
                    name = "手术刀片",
                    goods_code = "B30002",
                    batch_number = "D1111",
                    birth_date = new DateTime(2019, 1, 2),
                    valid_period = 30,
                    expire_date = new DateTime(2019, 2, 2),
                    position = "CAB2-1",
                    fetch_type = 1,
                    remarks = ""
                },
                new Goods
                {
                    code = "E20000176012026319404F98",
                    name = "手术刀片",
                    goods_code = "B30002",
                    batch_number = "D1112",
                    birth_date = new DateTime(2019, 1, 12),
                    valid_period = 30,
                    expire_date = new DateTime(2019,2, 12),
                    position = "CAB2-1",
                    fetch_type = 2,
                    remarks = ""
                },
                new Goods
                {
                    code = "E20000176012028019504DA0",
                    name = "碘伏",
                    goods_code = "B40001",
                    batch_number = "E1111",
                    birth_date = new DateTime(2019, 2, 2),
                    valid_period = 60,
                    expire_date = new DateTime(2019, 4, 2),
                    position = "CAB2-2",
                    fetch_type = 1,
                    remarks = ""
                },
                 new Goods
                {
                    code = "E20000176012026519504D85",
                    name = "碘伏",
                    goods_code = "B40001",
                    batch_number = "E1113",
                    birth_date = new DateTime(2019, 3, 2),
                    valid_period = 60,
                    expire_date = new DateTime(2019, 4, 2),
                    position = "CAB2-2",
                    fetch_type = 2,
                    remarks = ""
                },
            };

            goodsBll.InsertGood(goods);
        }

        public void InitReplenishOrder()
        {
            if (replenishBll.GettReplenishOrderNum() > 0)
                return;

            Hashtable ro1 = new Hashtable();
            HashSet<string> hs1 = new HashSet<string> { "E20000176012027919504D98", "E20000176012025319504D67" };
            HashSet<string> hs2 = new HashSet<string> { "E20000176012028119504DA5", "E20000176012023919504D48" };
            ro1.Add("hs1", hs1);
            ro1.Add("hs2", hs2);
            replenishBll.InitReplenshOrder("RO-TEST-001", "RO001-RSO-TEST-00", ro1, 1);

            Hashtable ro2 = new Hashtable();
            HashSet<string> hs3 = new HashSet<string> { "E20000176012026619504D8D" };
            HashSet<string> hs4 = new HashSet<string> { "E20000176012028019504DA0" };

            ro2.Add("hs3", hs3);
            ro2.Add("hs4", hs4);
            replenishBll.InitReplenshOrder("RO-TEST-002", "RO002-RSO-TEST-00", ro2, 1);

            Hashtable ro3 = new Hashtable();
            ro3.Add("hs1", hs1);
            ro3.Add("hs4", hs4);
            replenishBll.InitReplenshOrder("RO-TEST-003", "RO003-RSO-TEST-00", ro3, 3);
        }

        public void InitPickingOrder()
        {
            if (pickingBll.GettPickingOrderNum() > 0)
                return;

            Hashtable ro1 = new Hashtable();
            HashSet<string> hs1 = new HashSet<string> { "E20000176012027919504D98", "E20000176012025319504D67" };
            HashSet<string> hs2 = new HashSet<string> { "E20000176012028119504DA5", "E20000176012023919504D48" };
            ro1.Add("hs1", hs1);
            ro1.Add("hs2", hs2);

            pickingBll.InitPickingOrder("PO-TEST-001", "PO001-PSO-TEST-00", ro1, 1);

            Hashtable ro2 = new Hashtable();
            HashSet<string> hs3 = new HashSet<string> { "E20000176012026619504D8D"};
            HashSet<string> hs4 = new HashSet<string> { "E20000176012028019504DA0"};

            ro2.Add("hs3", hs3);
            ro2.Add("hs4", hs4);
            pickingBll.InitPickingOrder("PO-TEST-002", "PO002-PSO-TEST-00", ro2, 1);

            Hashtable ro3 = new Hashtable();
            ro3.Add("hs2", hs2);
            ro3.Add("hs3", hs3);
            pickingBll.InitPickingOrder("PO-TEST-003", "PO003-PSO-TEST-00", ro3, 3);
        }

        public void InitSurgerOrder()
        {
            if (fetchOrderBll.GettSurgerOrderNum() > 0)
                return;

            List<SurgeryOrderdtl> surgeryOrders = new List<SurgeryOrderdtl>
            {
                new SurgeryOrderdtl
                {
                    surgery_order_code="sh12",
                    goods_code="B20001",
                    name="一次性输液器",
                    fetch_type=1,
                    fetch_num=2,
                    already_fetch_num=0,
                    not_fetch_num=2,
                }  
                ,
                new SurgeryOrderdtl
                {
                    surgery_order_code="sh12",
                    goods_code="B30001",
                    name="医用胶布卷",
                    fetch_type=1,
                    fetch_num=1,
                    already_fetch_num=0,
                    not_fetch_num=1,
                }
            };
            fetchOrderBll.InitSurgerOrder(surgeryOrders);

            List<SurgeryOrderdtl> surgeryOrdersG = new List<SurgeryOrderdtl>
            {
                new SurgeryOrderdtl
                {
                    surgery_order_code="GUOYTEST001",
                    goods_code="B20001",
                    name="一次性输液器",
                    fetch_type=1,
                    fetch_num=2,
                    already_fetch_num=1,
                    not_fetch_num=1,
                },
                new SurgeryOrderdtl
                {
                    surgery_order_code="GUOYTEST001",
                    goods_code="B30001",
                    name="医用胶布卷",
                    fetch_type=1,
                    fetch_num=1,
                    already_fetch_num=0,
                    not_fetch_num=1,
                },
                new SurgeryOrderdtl
                {
                    surgery_order_code="GUOYTEST001",
                    goods_code="B30002",
                    name="手术刀片",
                    fetch_type=1,
                    fetch_num=1,
                    already_fetch_num=0,
                    not_fetch_num=1,
                },
            };
            fetchOrderBll.InitSurgerOrder(surgeryOrdersG);
        }
    }
}
