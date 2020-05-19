using CFLMedCab.Http.Model;
using CFLMedCab.Http.Model.Base;
using CFLMedCab.Http.Model.Enum;
using CFLMedCab.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Http.Bll
{
    public class CommodityCodeMockBll
    {
        public static List<CommodityCode> GetSimpleCommodity()
        {
            var commodityCodes = new List<CommodityCode>();
            string com1 = ApplicationState.GetAllRfidCom().First();
            commodityCodes.Add(new CommodityCode
            {
                name = "RF00000730",
                EquipmentId = ApplicationState.GetEquipId(),
                EquipmentName = ApplicationState.GetEquipName(),
                GoodsLocationId = ApplicationState.GetLocIdByRFidCom(com1),
                GoodsLocationName = ApplicationState.GetLocCodeByRFidCom(com1),
                StoreHouseId = ApplicationState.GetHouseId(),
                StoreHouseName = ApplicationState.GetHouseName(),
                operate_type = (int)OperateType.入库
            });
            commodityCodes.Add(new CommodityCode
            {
                name = "RF00000712",
                EquipmentId = ApplicationState.GetEquipId(),
                EquipmentName = ApplicationState.GetEquipName(),
                GoodsLocationId = ApplicationState.GetLocIdByRFidCom(com1),
                GoodsLocationName = ApplicationState.GetLocCodeByRFidCom(com1),
                StoreHouseId = ApplicationState.GetHouseId(),
                StoreHouseName = ApplicationState.GetHouseName(),
                operate_type = (int)OperateType.入库
            });
            commodityCodes.Add(new CommodityCode
            {
                name = "RF00000735",
                EquipmentId = ApplicationState.GetEquipId(),
                EquipmentName = ApplicationState.GetEquipName(),
                GoodsLocationId = ApplicationState.GetLocIdByRFidCom(com1),
                GoodsLocationName = ApplicationState.GetLocCodeByRFidCom(com1),
                StoreHouseId = ApplicationState.GetHouseId(),
                StoreHouseName = ApplicationState.GetHouseName(),
                operate_type = (int)OperateType.入库
            });
            commodityCodes.Add(new CommodityCode
            {
                name = "RF00000725",
                EquipmentId = ApplicationState.GetEquipId(),
                EquipmentName = ApplicationState.GetEquipName(),
                GoodsLocationId = ApplicationState.GetLocIdByRFidCom(com1),
                GoodsLocationName = ApplicationState.GetLocCodeByRFidCom(com1),
                StoreHouseId = ApplicationState.GetHouseId(),
                StoreHouseName = ApplicationState.GetHouseName(),
                operate_type = (int)OperateType.入库
            });
            commodityCodes.Add(new CommodityCode
            {
                name = "RF00000722",
                EquipmentId = ApplicationState.GetEquipId(),
                EquipmentName = ApplicationState.GetEquipName(),
                GoodsLocationId = ApplicationState.GetLocIdByRFidCom(com1),
                GoodsLocationName = ApplicationState.GetLocCodeByRFidCom(com1),
                StoreHouseId = ApplicationState.GetHouseId(),
                StoreHouseName = ApplicationState.GetHouseName(),
                operate_type = (int)OperateType.入库
            });
            commodityCodes.Add(new CommodityCode
            {
                name = "RF00000727",
                EquipmentId = ApplicationState.GetEquipId(),
                EquipmentName = ApplicationState.GetEquipName(),
                GoodsLocationId = ApplicationState.GetLocIdByRFidCom(com1),
                GoodsLocationName = ApplicationState.GetLocCodeByRFidCom(com1),
                StoreHouseId = ApplicationState.GetHouseId(),
                StoreHouseName = ApplicationState.GetHouseName(),
                operate_type = (int)OperateType.入库
            });
            commodityCodes.Add(new CommodityCode
            {
                name = "RF00000736",
                EquipmentId = ApplicationState.GetEquipId(),
                EquipmentName = ApplicationState.GetEquipName(),
                GoodsLocationId = ApplicationState.GetLocIdByRFidCom(com1),
                GoodsLocationName = ApplicationState.GetLocCodeByRFidCom(com1),
                StoreHouseId = ApplicationState.GetHouseId(),
                StoreHouseName = ApplicationState.GetHouseName(),
                operate_type = (int)OperateType.入库
            });
            commodityCodes.Add(new CommodityCode
            {
                name = "RF00000731",
                EquipmentId = ApplicationState.GetEquipId(),
                EquipmentName = ApplicationState.GetEquipName(),
                GoodsLocationId = ApplicationState.GetLocIdByRFidCom(com1),
                GoodsLocationName = ApplicationState.GetLocCodeByRFidCom(com1),
                StoreHouseId = ApplicationState.GetHouseId(),
                StoreHouseName = ApplicationState.GetHouseName(),
                operate_type = (int)OperateType.入库
            });
            commodityCodes.Add(new CommodityCode
            {
                name = "RF00000726",
                EquipmentId = ApplicationState.GetEquipId(),
                EquipmentName = ApplicationState.GetEquipName(),
                GoodsLocationId = ApplicationState.GetLocIdByRFidCom(com1),
                GoodsLocationName = ApplicationState.GetLocCodeByRFidCom(com1),
                StoreHouseId = ApplicationState.GetHouseId(),
                StoreHouseName = ApplicationState.GetHouseName(),
                operate_type = (int)OperateType.入库
            });
            commodityCodes.Add(new CommodityCode
            {
                name = "RF00000729",
                EquipmentId = ApplicationState.GetEquipId(),
                EquipmentName = ApplicationState.GetEquipName(),
                GoodsLocationId = ApplicationState.GetLocIdByRFidCom(com1),
                GoodsLocationName = ApplicationState.GetLocCodeByRFidCom(com1),
                StoreHouseId = ApplicationState.GetHouseId(),
                StoreHouseName = ApplicationState.GetHouseName(),
                operate_type = (int)OperateType.入库
            });
            commodityCodes.Add(new CommodityCode
            {
                name = "RF00000713",
                EquipmentId = ApplicationState.GetEquipId(),
                EquipmentName = ApplicationState.GetEquipName(),
                GoodsLocationId = ApplicationState.GetLocIdByRFidCom(com1),
                GoodsLocationName = ApplicationState.GetLocCodeByRFidCom(com1),
                StoreHouseId = ApplicationState.GetHouseId(),
                StoreHouseName = ApplicationState.GetHouseName(),
                operate_type = (int)OperateType.入库
            });
            commodityCodes.Add(new CommodityCode
            {
                name = "RF00000721",
                EquipmentId = ApplicationState.GetEquipId(),
                EquipmentName = ApplicationState.GetEquipName(),
                GoodsLocationId = ApplicationState.GetLocIdByRFidCom(com1),
                GoodsLocationName = ApplicationState.GetLocCodeByRFidCom(com1),
                StoreHouseId = ApplicationState.GetHouseId(),
                StoreHouseName = ApplicationState.GetHouseName(),
                operate_type = (int)OperateType.入库
            });
            commodityCodes.Add(new CommodityCode
            {
                name = "RF00000719",
                EquipmentId = ApplicationState.GetEquipId(),
                EquipmentName = ApplicationState.GetEquipName(),
                GoodsLocationId = ApplicationState.GetLocIdByRFidCom(com1),
                GoodsLocationName = ApplicationState.GetLocCodeByRFidCom(com1),
                StoreHouseId = ApplicationState.GetHouseId(),
                StoreHouseName = ApplicationState.GetHouseName(),
                operate_type = (int)OperateType.入库
            });
            commodityCodes.Add(new CommodityCode
            {
                name = "RF00000697",
                EquipmentId = ApplicationState.GetEquipId(),
                EquipmentName = ApplicationState.GetEquipName(),
                GoodsLocationId = ApplicationState.GetLocIdByRFidCom(com1),
                GoodsLocationName = ApplicationState.GetLocCodeByRFidCom(com1),
                StoreHouseId = ApplicationState.GetHouseId(),
                StoreHouseName = ApplicationState.GetHouseName(),
                operate_type = (int)OperateType.入库
            });
            commodityCodes.Add(new CommodityCode
            {
                name = "RF00000714",
                EquipmentId = ApplicationState.GetEquipId(),
                EquipmentName = ApplicationState.GetEquipName(),
                GoodsLocationId = ApplicationState.GetLocIdByRFidCom(com1),
                GoodsLocationName = ApplicationState.GetLocCodeByRFidCom(com1),
                StoreHouseId = ApplicationState.GetHouseId(),
                StoreHouseName = ApplicationState.GetHouseName(),
                operate_type = (int)OperateType.入库
            });
            commodityCodes.Add(new CommodityCode
            {
                name = "RF00000695",
                EquipmentId = ApplicationState.GetEquipId(),
                EquipmentName = ApplicationState.GetEquipName(),
                GoodsLocationId = ApplicationState.GetLocIdByRFidCom(com1),
                GoodsLocationName = ApplicationState.GetLocCodeByRFidCom(com1),
                StoreHouseId = ApplicationState.GetHouseId(),
                StoreHouseName = ApplicationState.GetHouseName(),
                operate_type = (int)OperateType.入库
            });
            commodityCodes.Add(new CommodityCode
            {
                name = "RF00000723",
                EquipmentId = ApplicationState.GetEquipId(),
                EquipmentName = ApplicationState.GetEquipName(),
                GoodsLocationId = ApplicationState.GetLocIdByRFidCom(com1),
                GoodsLocationName = ApplicationState.GetLocCodeByRFidCom(com1),
                StoreHouseId = ApplicationState.GetHouseId(),
                StoreHouseName = ApplicationState.GetHouseName(),
                operate_type = (int)OperateType.入库
            });
            commodityCodes.Add(new CommodityCode
            {
                name = "RF00000717",
                EquipmentId = ApplicationState.GetEquipId(),
                EquipmentName = ApplicationState.GetEquipName(),
                GoodsLocationId = ApplicationState.GetLocIdByRFidCom(com1),
                GoodsLocationName = ApplicationState.GetLocCodeByRFidCom(com1),
                StoreHouseId = ApplicationState.GetHouseId(),
                StoreHouseName = ApplicationState.GetHouseName(),
                operate_type = (int)OperateType.入库
            });
            commodityCodes.Add(new CommodityCode
            {
                name = "RF00000702",
                EquipmentId = ApplicationState.GetEquipId(),
                EquipmentName = ApplicationState.GetEquipName(),
                GoodsLocationId = ApplicationState.GetLocIdByRFidCom(com1),
                GoodsLocationName = ApplicationState.GetLocCodeByRFidCom(com1),
                StoreHouseId = ApplicationState.GetHouseId(),
                StoreHouseName = ApplicationState.GetHouseName(),
                operate_type = (int)OperateType.入库
            });
            commodityCodes.Add(new CommodityCode
            {
                name = "RF00000734",
                EquipmentId = ApplicationState.GetEquipId(),
                EquipmentName = ApplicationState.GetEquipName(),
                GoodsLocationId = ApplicationState.GetLocIdByRFidCom(com1),
                GoodsLocationName = ApplicationState.GetLocCodeByRFidCom(com1),
                StoreHouseId = ApplicationState.GetHouseId(),
                StoreHouseName = ApplicationState.GetHouseName(),
                operate_type = (int)OperateType.入库
            });
            commodityCodes.Add(new CommodityCode
            {
                name = "RF00000733",
                EquipmentId = ApplicationState.GetEquipId(),
                EquipmentName = ApplicationState.GetEquipName(),
                GoodsLocationId = ApplicationState.GetLocIdByRFidCom(com1),
                GoodsLocationName = ApplicationState.GetLocCodeByRFidCom(com1),
                StoreHouseId = ApplicationState.GetHouseId(),
                StoreHouseName = ApplicationState.GetHouseName(),
                operate_type = (int)OperateType.入库
            });
            commodityCodes.Add(new CommodityCode
            {
                name = "RF00000724",
                EquipmentId = ApplicationState.GetEquipId(),
                EquipmentName = ApplicationState.GetEquipName(),
                GoodsLocationId = ApplicationState.GetLocIdByRFidCom(com1),
                GoodsLocationName = ApplicationState.GetLocCodeByRFidCom(com1),
                StoreHouseId = ApplicationState.GetHouseId(),
                StoreHouseName = ApplicationState.GetHouseName(),
                operate_type = (int)OperateType.入库
            });
            commodityCodes.Add(new CommodityCode
            {
                name = "RF00000698",
                EquipmentId = ApplicationState.GetEquipId(),
                EquipmentName = ApplicationState.GetEquipName(),
                GoodsLocationId = ApplicationState.GetLocIdByRFidCom(com1),
                GoodsLocationName = ApplicationState.GetLocCodeByRFidCom(com1),
                StoreHouseId = ApplicationState.GetHouseId(),
                StoreHouseName = ApplicationState.GetHouseName(),
                operate_type = (int)OperateType.入库
            });
            commodityCodes.Add(new CommodityCode
            {
                name = "RF00000728",
                EquipmentId = ApplicationState.GetEquipId(),
                EquipmentName = ApplicationState.GetEquipName(),
                GoodsLocationId = ApplicationState.GetLocIdByRFidCom(com1),
                GoodsLocationName = ApplicationState.GetLocCodeByRFidCom(com1),
                StoreHouseId = ApplicationState.GetHouseId(),
                StoreHouseName = ApplicationState.GetHouseName(),
                operate_type = (int)OperateType.入库
            });
            commodityCodes.Add(new CommodityCode
            {
                name = "RF00000699",
                EquipmentId = ApplicationState.GetEquipId(),
                EquipmentName = ApplicationState.GetEquipName(),
                GoodsLocationId = ApplicationState.GetLocIdByRFidCom(com1),
                GoodsLocationName = ApplicationState.GetLocCodeByRFidCom(com1),
                StoreHouseId = ApplicationState.GetHouseId(),
                StoreHouseName = ApplicationState.GetHouseName(),
                operate_type = (int)OperateType.入库
            });
            commodityCodes.Add(new CommodityCode
            {
                name = "RF00000745",
                EquipmentId = ApplicationState.GetEquipId(),
                EquipmentName = ApplicationState.GetEquipName(),
                GoodsLocationId = ApplicationState.GetLocIdByRFidCom(com1),
                GoodsLocationName = ApplicationState.GetLocCodeByRFidCom(com1),
                StoreHouseId = ApplicationState.GetHouseId(),
                StoreHouseName = ApplicationState.GetHouseName(),
                operate_type = (int)OperateType.入库
            });
            commodityCodes.Add(new CommodityCode
            {
                name = "RF00000743",
                EquipmentId = ApplicationState.GetEquipId(),
                EquipmentName = ApplicationState.GetEquipName(),
                GoodsLocationId = ApplicationState.GetLocIdByRFidCom(com1),
                GoodsLocationName = ApplicationState.GetLocCodeByRFidCom(com1),
                StoreHouseId = ApplicationState.GetHouseId(),
                StoreHouseName = ApplicationState.GetHouseName(),
                operate_type = (int)OperateType.入库
            });
            commodityCodes.Add(new CommodityCode
            {
                name = "RF00000711",
                EquipmentId = ApplicationState.GetEquipId(),
                EquipmentName = ApplicationState.GetEquipName(),
                GoodsLocationId = ApplicationState.GetLocIdByRFidCom(com1),
                GoodsLocationName = ApplicationState.GetLocCodeByRFidCom(com1),
                StoreHouseId = ApplicationState.GetHouseId(),
                StoreHouseName = ApplicationState.GetHouseName(),
                operate_type = (int)OperateType.入库
            });
            commodityCodes.Add(new CommodityCode
            {
                name = "RF00000732",
                EquipmentId = ApplicationState.GetEquipId(),
                EquipmentName = ApplicationState.GetEquipName(),
                GoodsLocationId = ApplicationState.GetLocIdByRFidCom(com1),
                GoodsLocationName = ApplicationState.GetLocCodeByRFidCom(com1),
                StoreHouseId = ApplicationState.GetHouseId(),
                StoreHouseName = ApplicationState.GetHouseName(),
                operate_type = (int)OperateType.入库
            });
            commodityCodes.Add(new CommodityCode
            {
                name = "RF00000744",
                EquipmentId = ApplicationState.GetEquipId(),
                EquipmentName = ApplicationState.GetEquipName(),
                GoodsLocationId = ApplicationState.GetLocIdByRFidCom(com1),
                GoodsLocationName = ApplicationState.GetLocCodeByRFidCom(com1),
                StoreHouseId = ApplicationState.GetHouseId(),
                StoreHouseName = ApplicationState.GetHouseName(),
                operate_type = (int)OperateType.入库
            });
            commodityCodes.Add(new CommodityCode
            {
                name = "RF00000716",
                EquipmentId = ApplicationState.GetEquipId(),
                EquipmentName = ApplicationState.GetEquipName(),
                GoodsLocationId = ApplicationState.GetLocIdByRFidCom(com1),
                GoodsLocationName = ApplicationState.GetLocCodeByRFidCom(com1),
                StoreHouseId = ApplicationState.GetHouseId(),
                StoreHouseName = ApplicationState.GetHouseName(),
                operate_type = (int)OperateType.入库
            });
            commodityCodes.Add(new CommodityCode
            {
                name = "RF00000700",
                EquipmentId = ApplicationState.GetEquipId(),
                EquipmentName = ApplicationState.GetEquipName(),
                GoodsLocationId = ApplicationState.GetLocIdByRFidCom(com1),
                GoodsLocationName = ApplicationState.GetLocCodeByRFidCom(com1),
                StoreHouseId = ApplicationState.GetHouseId(),
                StoreHouseName = ApplicationState.GetHouseName(),
                operate_type = (int)OperateType.入库
            });
            commodityCodes.Add(new CommodityCode
            {
                name = "RF00000715",
                EquipmentId = ApplicationState.GetEquipId(),
                EquipmentName = ApplicationState.GetEquipName(),
                GoodsLocationId = ApplicationState.GetLocIdByRFidCom(com1),
                GoodsLocationName = ApplicationState.GetLocCodeByRFidCom(com1),
                StoreHouseId = ApplicationState.GetHouseId(),
                StoreHouseName = ApplicationState.GetHouseName(),
                operate_type = (int)OperateType.入库
            });
            commodityCodes.Add(new CommodityCode
            {
                name = "RF00000696",
                EquipmentId = ApplicationState.GetEquipId(),
                EquipmentName = ApplicationState.GetEquipName(),
                GoodsLocationId = ApplicationState.GetLocIdByRFidCom(com1),
                GoodsLocationName = ApplicationState.GetLocCodeByRFidCom(com1),
                StoreHouseId = ApplicationState.GetHouseId(),
                StoreHouseName = ApplicationState.GetHouseName(),
                operate_type = (int)OperateType.入库
            });
            commodityCodes.Add(new CommodityCode
            {
                name = "RF00000701",
                EquipmentId = ApplicationState.GetEquipId(),
                EquipmentName = ApplicationState.GetEquipName(),
                GoodsLocationId = ApplicationState.GetLocIdByRFidCom(com1),
                GoodsLocationName = ApplicationState.GetLocCodeByRFidCom(com1),
                StoreHouseId = ApplicationState.GetHouseId(),
                StoreHouseName = ApplicationState.GetHouseName(),
                operate_type = (int)OperateType.入库
            });
            commodityCodes.Add(new CommodityCode
            {
                name = "RF00000720",
                EquipmentId = ApplicationState.GetEquipId(),
                EquipmentName = ApplicationState.GetEquipName(),
                GoodsLocationId = ApplicationState.GetLocIdByRFidCom(com1),
                GoodsLocationName = ApplicationState.GetLocCodeByRFidCom(com1),
                StoreHouseId = ApplicationState.GetHouseId(),
                StoreHouseName = ApplicationState.GetHouseName(),
                operate_type = (int)OperateType.入库
            });
            return commodityCodes;
        }

        public static BaseData<CommodityCode> GetCommodityCode()
        {
            string str = Read("D:\\1\\1.txt");
            return Deserialize<BaseData<CommodityCode>>(str);
        }

        public static BaseData<Commodity> GetCommodity()
        {
            string str = Read("D:\\1\\2.txt");
            return Deserialize<BaseData<Commodity>>(str);
        }

        public static BaseData<CommodityInventoryDetail> GetCommodityInventoryDetail()
        {
            string str = Read("D:\\1\\3.txt");
            return Deserialize<BaseData<CommodityInventoryDetail>>(str);
        }

        public static BaseData<CommodityInventoryGoods> GetCommodityInventoryGoods()
        {
            string str = Read("D:\\1\\4.txt");
            return Deserialize<BaseData<CommodityInventoryGoods>>(str);
        }

        public static BaseData<BatchNumber> GetBatchNumber()
        {
            string str = Read("D:\\1\\5.txt");
            return Deserialize<BaseData<BatchNumber>>(str);
        }

        public static BaseData<HospitalGoods> GetHospitalGoods()
        {
            string str = Read("D:\\1\\6.txt");
            return Deserialize<BaseData<HospitalGoods>>(str);
        }

        public static BaseData<CommodityCatalogue> GetCommodityCatalogue()
        {
            string str = Read("D:\\1\\7.txt");
            return Deserialize<BaseData<CommodityCatalogue>>(str);
        }

        public static BaseData<Equipment> GetEquipment()
        {
            string str = Read("D:\\1\\8.txt");
            return Deserialize<BaseData<Equipment>>(str);
        }


        private static string Read(string path)
        {
            System.IO.StreamReader file = new System.IO.StreamReader(path);
            string str = file.ReadToEnd();
            file.Close();
            return str;
        }

        private static T Deserialize<T>(string json)
        {
            T obj = Activator.CreateInstance<T>();
            MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(json));
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            obj = (T)serializer.ReadObject(ms);
            ms.Close();
            return obj;
        }
    }
}
