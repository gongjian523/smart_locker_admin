using CFLMedCab.Http.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Infrastructure
{
    enum ApplicationKey{
        CurUser,   //本地数据库中的查询到用户信息
        User,      //从主系统查询到的用户信息
        CurGoods,  //本地数据库中的查询到商品信息
        Goods,     //从主系统中的查询到商品信息
        HouseId,   //库房id
        HouseName, //库房名字
        EquipId,   //设备id
        EquipName, //设备名字
        CodeMCab,  //主柜编码(名字)
        CodeSCab,  //副柜编码(名字)
        IdMCab,    //主柜id
        IdSCab,    //副柜id
        COM_MLocker,
        COM_SLocker,
        COM_MRFid,
        COM_SRFid,
        COM_MVein,
        AccessToken,
        RefreshToken,
        InvPlan,
         };


    public static class ApplicationState
    {
        private static Dictionary<int, object> _values =
            new Dictionary<int, object>();

        public static void SetValue(int key, object value)
        {
            object sValue;

            if (_values.TryGetValue(key, out sValue))
                _values.Remove(key);

            _values.Add(key, value);
        }

        public static T GetValue<T>(int key)
        {
            return (T)_values[key];
        }

        public static void SetAccessToken(string accessToken)
        {
            SetValue((int)ApplicationKey.AccessToken, accessToken);
            return;
        }

        public static string  GetAccessToken()
        {
            return GetValue<string>((int)ApplicationKey.AccessToken);
        }


        public static void SetRefreshToken(string token)
        {
            SetValue((int)ApplicationKey.RefreshToken, token);
            return;
        }

        public static string GetRefreshToken()
        {
            return GetValue<string>((int)ApplicationKey.RefreshToken);
        }


        public static void SetUserInfo(User user)
        {
            SetValue((int)ApplicationKey.User, user);
            return;
        }

        public static User GetUserInfo()
        {
            return GetValue<User>((int)ApplicationKey.User);
        }


        public static void SetGoodsInfo(HashSet<CommodityEps> hs)
        {
            SetValue((int)ApplicationKey.Goods, hs);
            return;
        }

        public static HashSet<CommodityEps> GetGoodsInfo()
        {
            return GetValue<HashSet<CommodityEps>>((int)ApplicationKey.Goods);
        }

        public static void SetInvPlan(InventoryPlan inventoryPlan)
        {
            SetValue((int)ApplicationKey.InvPlan, inventoryPlan);
            return;
        }

        public static InventoryPlan GetInvPlan()
        {
            return GetValue<InventoryPlan>((int)ApplicationKey.InvPlan);
        }

        public static void SetEquipId(string  equipId)
        {
            SetValue((int)ApplicationKey.EquipId, equipId);
            return;
        }

        public static string GetEquipId()
        {
            return GetValue<string>((int)ApplicationKey.EquipId);
        }

        public static void SetEquipName(string equipName)
        {
            SetValue((int)ApplicationKey.EquipName, equipName);
            return;
        }

        public static string GetEquipName()
        {
            return GetValue<string>((int)ApplicationKey.EquipName);
        }

        public static void SetHouseId(string houseId)
        {
            SetValue((int)ApplicationKey.EquipId, houseId);
            return;
        }

        public static string GetHouseId()
        {
            return GetValue<string>((int)ApplicationKey.HouseId);
        }

        public static void SetHouseName(string houseName)
        {
            SetValue((int)ApplicationKey.HouseName, houseName);
            return;
        }

        public static string GetHouseName()
        {
            return GetValue<string>((int)ApplicationKey.HouseName);
        }

    }
}
