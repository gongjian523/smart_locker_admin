using CFLMedCab.DAL;
using CFLMedCab.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Test
{
    public class TestGoods
    {
        private  readonly GoodsDal GoodsDal;

        public TestGoods()
        {
           GoodsDal = GoodsDal.GetInstance();
        }

        public void AddGoodTest()
        {
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
                    position = "Z1",
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
                    position = "Z1",
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
                    position = "Z1",
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
                    position = "Z1",
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
                    position = "Z1",
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
                    position = "Z1",
                    remarks = ""
                },
                new Goods
                {
                    code = "E20000176012026619504D8D",
                    name = "手术刀片",
                    goods_code = "B30001",
                    batch_number = "D1111",
                    birth_date = new DateTime(2019, 1, 2),
                    valid_period = 30,
                    expire_date = new DateTime(2019, 2, 2),
                    position = "Z1",
                    remarks = ""
                },
                new Goods
                {
                    code = "E20000176012026319404F98",
                    name = "手术刀片",
                    goods_code = "B30001",
                    batch_number = "D1112",
                    birth_date = new DateTime(2019, 1, 12),
                    valid_period = 30,
                    expire_date = new DateTime(2019,2, 12),
                    position = "Z1",
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
                    position = "Z1",
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
                    position = "Z1",
                    remarks = ""
                },
            };

            GoodsDal.InsertGoods(goods);
        }
    }
}
