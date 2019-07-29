using CFLMedCab.Http.Model;
using System;
using System.Collections;
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
        MCabName,  //主柜编码(名字)
        MCabId,    //主柜id
        SCabName,  //副柜编码(名字)
        SCabId,    //副柜id
        COM_MLocker,
        COM_SLocker,
        COM_MRFid,
        COM_SRFid,
        COM_MVein,
        AccessToken,
        RefreshToken,
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

        #region the serial name of the lockers

        /// <summary>
        /// 设置主柜门锁的串口
        /// </summary>
        /// <param name="com"></param>
        public static void SetMLockerCOM(string com)
        {
            SetValue((int)ApplicationKey.COM_MLocker, com);
            return;
        }

        /// <summary>
        /// 获取主柜门锁的串口
        /// </summary>
        /// <param name="com"></param>
        public static string GetMLockerCOM()
        {
            return GetValue<string>((int)ApplicationKey.COM_MLocker);
        }

        /// <summary>
        /// 设置副柜门锁的串口
        /// </summary>
        /// <param name="com"></param>
        public static void SetSLockerCOM(string com)
        {
            SetValue((int)ApplicationKey.COM_SLocker, com);
            return;
        }

        /// <summary>
        /// 获取副柜门锁的串口
        /// </summary>
        /// <param name="com"></param>
        public static string GetSLockerCOM()
        {
            return GetValue<string>((int)ApplicationKey.COM_SLocker);
        }
        #endregion

        #region the serial name of the RFid

        /// <summary>
        /// 设置主柜RF扫描仪的串口
        /// </summary>
        /// <param name="com"></param>
        public static void SetMRfidCOM(string com)
        {
            SetValue((int)ApplicationKey.COM_MRFid, com);
            return;
        }

        /// <summary>
        /// 设置主柜RF扫描仪的串口
        /// </summary>
        /// <param name="com"></param>
        public static string GetMRfidCOM()
        {
            return GetValue<string>((int)ApplicationKey.COM_MRFid);
        }

        /// <summary>
        /// 设置副柜RF扫描仪的串口
        /// </summary>
        /// <param name="com"></param>
        public static void SetSRfidCOM(string com)
        {
            SetValue((int)ApplicationKey.COM_SRFid, com);
            return;
        }

        public static string GetSRfidCOM()
        {
            return GetValue<string>((int)ApplicationKey.COM_SRFid);
        }
        #endregion

        #region the serial name of the Vein
        public static void SetMVeinCOM(string com)
        {
            SetValue((int)ApplicationKey.COM_MVein, com);
            return;
        }

        public static string GetMVeinCOM()
        {
            return GetValue<string>((int)ApplicationKey.COM_MVein);
        }
        #endregion

        #region Token
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
        #endregion

        #region user info from mian system
        public static void SetUserInfo(User user)
        {
            SetValue((int)ApplicationKey.User, user);
            return;
        }

        public static User GetUserInfo()
        {
            return GetValue<User>((int)ApplicationKey.User);
        }
        #endregion

        #region user info from mian system
        public static void SetGoodsInfo(HashSet<CommodityEps> hs)
        {
            SetValue((int)ApplicationKey.Goods, hs);
            return;
        }

        public static HashSet<CommodityEps> GetGoodsInfo()
        {
            return GetValue<HashSet<CommodityEps>>((int)ApplicationKey.Goods);
        }
        #endregion

        #region equipment
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
        #endregion

        #region house
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
        #endregion

        #region cab
        public static void SetMCabId(string cabId)
        {
            SetValue((int)ApplicationKey.MCabId, cabId);
            return;
        }

        public static string GetMCabId()
        {
            return GetValue<string>((int)ApplicationKey.MCabId);
        }

        public static void SetMCabName(string cabName)
        {
            SetValue((int)ApplicationKey.MCabName, cabName);
            return;
        }

        public static string GetMCabName()
        {
            return GetValue<string>((int)ApplicationKey.MCabName);
        }

        public static void SetSCabId(string cabId)
        {
            SetValue((int)ApplicationKey.SCabId, cabId);
            return;
        }

        public static string GetSCabId()
        {
            return GetValue<string>((int)ApplicationKey.SCabId);
        }

        public static void SetSCabName(string cabName)
        {
            SetValue((int)ApplicationKey.SCabName, cabName);
            return;
        }

        public static string GetSCabName()
        {
            return GetValue<string>((int)ApplicationKey.SCabName);
        }
        #endregion


        public static string GetCabNameByRFidCom(string com)
        {
            if (com == ApplicationState.GetValue<string>((int)ApplicationKey.COM_MRFid))
                return ApplicationState.GetValue<string>((int)ApplicationKey.MCabName);
            else if (com == ApplicationState.GetValue<string>((int)ApplicationKey.COM_SRFid))
                return ApplicationState.GetValue<string>((int)ApplicationKey.SCabName);
            else
                return "";
        }

        public static string GetCabIdByRFidCom(string com)
        {
            if (com == ApplicationState.GetValue<string>((int)ApplicationKey.COM_MRFid))
                return ApplicationState.GetValue<string>((int)ApplicationKey.MCabId);
            else if (com == ApplicationState.GetValue<string>((int)ApplicationKey.COM_SRFid))
                return ApplicationState.GetValue<string>((int)ApplicationKey.SCabId);
            else
                return "";
        }

        public static string GetLockerComByRfidCom(string com)
        {
            if (com == ApplicationState.GetValue<string>((int)ApplicationKey.COM_MRFid))
                return ApplicationState.GetValue<string>((int)ApplicationKey.COM_MLocker);
            else if (com == ApplicationState.GetValue<string>((int)ApplicationKey.COM_SRFid))
                return ApplicationState.GetValue<string>((int)ApplicationKey.COM_SLocker);
            else
                return "";
        }

        public static string GetLockerComByCabName(string cabName)
        {
            if (cabName == ApplicationState.GetValue<string>((int)ApplicationKey.MCabName))
                return ApplicationState.GetValue<string>((int)ApplicationKey.COM_MLocker);
            else if (cabName == ApplicationState.GetValue<string>((int)ApplicationKey.SCabName))
                return ApplicationState.GetValue<string>((int)ApplicationKey.COM_SLocker);
            else
                return "";
        }

        public static string GetCabNameByCode(string code, Hashtable ht)
        {
            foreach (string key in ht.Keys)
            {
                if (((HashSet<string>)ht[key]).Contains(code))
                    return GetCabNameByRFidCom(key);
            }

            return "";
        }

        public static List<string> GetAllLockerCom()
        {
            //return(new List<string> {
            //    "COM2",
            //    "COM5"
            //});

            List<string> list = new List<string>();

            list.Add(ApplicationState.GetValue<string>((int)ApplicationKey.COM_MLocker));
#if DUALCAB
            list.Add(ApplicationState.GetValue<string>((int)ApplicationKey.COM_SLocker));
#endif
            return list;
        }

    }
}
