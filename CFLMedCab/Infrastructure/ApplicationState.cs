using CFLMedCab.Http.Model;
using CFLMedCab.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Infrastructure
{
    enum ApplicationKey{
        User,      //从主系统查询到的用户信息
        CurGoods,  //本地数据库中的查询到商品信息
        Goods,     //从主系统中的查询到商品信息
        HouseId,   //库房id
        HouseName, //库房名字
        EquipId,   //设备id
        EquipName, //设备名字
        Location,  //货柜
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
        //默认配置
        //主柜锁串口 COM2
        //副柜锁串口 COM5

        /// <summary>
        /// 设置主柜门锁的串口
        /// </summary>
        /// <param name="com"></param>
        [Obsolete]
        public static void SetMLockerCOM(string com)
        {
            SetValue((int)ApplicationKey.COM_MLocker, com);
            return;
        }

        /// <summary>
        /// 获取主柜门锁的串口
        /// </summary>
        /// <param></param>
        [Obsolete]
        public static string GetMLockerCOM()
        {
            return GetValue<string>((int)ApplicationKey.COM_MLocker);
        }

        /// <summary>
        /// 设置副柜门锁的串口
        /// </summary>
        /// <param name="com"></param>
        [Obsolete]
        public static void SetSLockerCOM(string com)
        {
            SetValue((int)ApplicationKey.COM_SLocker, com);
            return;
        }

        /// <summary>
        /// 获取副柜门锁的串口
        /// </summary>
        /// <param></param>
        [Obsolete]
        public static string GetSLockerCOM()
        {
            return GetValue<string>((int)ApplicationKey.COM_SLocker);
        }

        /// <summary>
        /// 通过货柜的名字查询门锁的串口
        /// </summary>
        /// <param name="cabName"></param>
        /// <returns></returns>
        public static string GetLockerComByLocCode(string locCode)
        {
            List<Locations> listLoc = GetLocations();
            return listLoc.Where(item => item.Code == locCode).First().LockerCom;
        }

        /// <summary>
        /// 获取当前的门锁串口
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAllLockerCom()
        {
            List<Locations> listLoc = GetLocations();
            return listLoc.Select(item => item.LockerCom).Distinct().ToList();
        }
        #endregion

        #region the serial name of the RFid
        //默认配置
        //COM1  主柜rfid串口
        //COM4  副柜rfid串口 

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
        /// 获取主柜RF扫描仪的串口
        /// </summary>
        /// <param></param>
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

        /// <summary>
        /// 获取副柜RF扫描仪的串口
        /// </summary>
        /// <param></param>
        public static string GetSRfidCOM()
        {
            return GetValue<string>((int)ApplicationKey.COM_SRFid);
        }
        #endregion

        #region the serial name of the Vein
        //默认配置
        //主柜指静脉串口"COM9"
        //副柜指静脉串口"COM8"
        
        /// <summary>
        /// 设置主柜指静脉的串口
        /// </summary>
        /// <param name="com"></param>
        public static void SetMVeinCOM(string com)
        {
            SetValue((int)ApplicationKey.COM_MVein, com);
            return;
        }

        /// <summary>
        /// 获取主柜指静脉的串口
        /// </summary>
        /// <param></param>
        public static string GetMVeinCOM()
        {
            return GetValue<string>((int)ApplicationKey.COM_MVein);
        }
        #endregion

        #region Token

        /// <summary>
        ///设置发送请求的Token
        /// </summary>
        /// <param name="accessToken"></param>
        public static void SetAccessToken(string accessToken)
        {
            SetValue((int)ApplicationKey.AccessToken, accessToken);
            return;
        }


        /// <summary>
        ///获取发送请求的Token
        /// </summary>
        /// <param></param>
        public static string  GetAccessToken()
        {
            return GetValue<string>((int)ApplicationKey.AccessToken);
        }

        /// <summary>
        /// 设置刷新发送请求Token的Token
        /// </summary>
        /// <param name="token"></param>
        public static void SetRefreshToken(string token)
        {
            SetValue((int)ApplicationKey.RefreshToken, token);
            return;
        }

        /// <summary>
        /// 获取刷新发送请求Token的Token
        /// </summary>
        /// <returns></returns>
        public static string GetRefreshToken()
        {
            return GetValue<string>((int)ApplicationKey.RefreshToken);
        }
        #endregion

        #region user info from mian system
        /// <summary>
        /// 保存主系统传来的用户信息
        /// </summary>
        /// <param name="user"></param>
        public static void SetUserInfo(User user)
        {
            SetValue((int)ApplicationKey.User, user);
            return;
        }

        /// <summary>
        /// 获取主系统传来的用户信息
        /// </summary>
        /// <returns></returns>
        public static User GetUserInfo()
        {
            return GetValue<User>((int)ApplicationKey.User);
        }
        #endregion

        #region user info from mian system
        /// <summary>
        /// 保存所有货柜的打过去商品信息
        /// </summary>
        /// <param name="hs"></param>
        public static void SetGoodsInfo(HashSet<CommodityEps> hs)
        {
            SetValue((int)ApplicationKey.Goods, hs);
            return;
        }

        /// <summary>
        /// 保存某些货柜的当前商品信息
        /// </summary>
        /// <param name="hs"></param>
        public static void SetGoodsInfoInSepcLoc(HashSet<CommodityEps> hs)
        {
            var locCodes = hs.Select(item => item.CommodityCodeName).Distinct().ToList();

            HashSet<CommodityEps> all = GetValue<HashSet<CommodityEps>>((int)ApplicationKey.Goods);
            //删除指定货柜的原来的商品
            all.RemoveWhere(item => locCodes.Contains(item.CommodityCodeName));
            //增加相应货柜的现在的商品
            all.UnionWith(hs);

            SetValue((int)ApplicationKey.Goods, all);
            return;
        }

        /// <summary>
        /// 获取所有货柜的商品信息
        /// </summary>
        /// <returns></returns>
        public static HashSet<CommodityEps> GetGoodsInfo()
        {
            return GetValue<HashSet<CommodityEps>>((int)ApplicationKey.Goods);
        }

        /// <summary>
        /// 获取指定货柜的商品
        /// </summary>
        /// <returns></returns>
        public static HashSet<CommodityEps> GetGoodsInfo(List<string> locCodes)
        {
            HashSet<CommodityEps> all =  GetValue<HashSet<CommodityEps>>((int)ApplicationKey.Goods);
            return new HashSet<CommodityEps>(all.Where(item => locCodes.Contains(item.CommodityCodeName)));
        }

        #endregion

#region equipment
        /// <summary>
        /// 保存设备ID
        /// </summary>
        /// <param name="equipId"></param>
        public static void SetEquipId(string  equipId)
        {
            SetValue((int)ApplicationKey.EquipId, equipId);
            return;
        }

        /// <summary>
        /// 获取设备ID
        /// </summary>
        /// <returns></returns>
        public static string GetEquipId()
        {
            return GetValue<string>((int)ApplicationKey.EquipId);
        }

        /// <summary>
        /// 保存设备名字
        /// </summary>
        /// <param name="equipName"></param>
        public static void SetEquipName(string equipName)
        {
            SetValue((int)ApplicationKey.EquipName, equipName);
            return;
        }

        /// <summary>
        /// 获取设备名字
        /// </summary>
        /// <returns></returns>
        public static string GetEquipName()
        {
            return GetValue<string>((int)ApplicationKey.EquipName);
        }
#endregion

        #region house
        /// <summary>
        /// 保存库房ID
        /// </summary>
        /// <param name="houseId"></param>
        public static void SetHouseId(string houseId)
        {
            SetValue((int)ApplicationKey.HouseId, houseId);
            return;
        }

        /// <summary>
        /// 获取库房ID
        /// </summary>
        /// <returns></returns>
        public static string GetHouseId()
        {
            return GetValue<string>((int)ApplicationKey.HouseId);
        }

        /// <summary>
        /// 保存库房名字
        /// </summary>
        /// <param name="houseName"></param>
        public static void SetHouseName(string houseName)
        {
            SetValue((int)ApplicationKey.HouseName, houseName);
            return;
        }

        /// <summary>
        /// 获取库房名字
        /// </summary>
        /// <returns></returns>
        public static string GetHouseName()
        {
            return GetValue<string>((int)ApplicationKey.HouseName);
        }
        #endregion

        #region location
        public static void SetLocations(List<Locations> locations)
        {
            SetValue((int)ApplicationKey.Location, locations);
        }

        public static List<Locations> GetLocations()
        {
            return GetValue<List<Locations>>((int)ApplicationKey.Location);
        }

        /// <summary>
        /// 获取所有货柜Id
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAllLocIds()
        {
            List<Locations> listLoc = GetLocations();
            return listLoc.Select(item => item.Id).Distinct().ToList(); 
        }


        /// <summary>
        /// 通过货柜id查询货柜编号
        /// </summary>
        /// <param name="com"></param>
        /// <returns></returns>
        public static string GetLocCodeById(string id)
        {
            List<Locations> listLoc = GetLocations();
            return listLoc.Where(item => item.Id == id).First().Code;
        }


        /// <summary>
        /// 通过Rfid串口查询货柜名字
        /// </summary>
        /// <param name="com"></param>
        /// <returns></returns>
        public static string GetLocCodeByRFidCom(string com)
        {
            List<Locations> listLoc = GetLocations();
            return listLoc.Where(item => item.RFCom == com).First().Code;
        }

        /// <summary>
        /// 通过Rfid串口查询货柜id
        /// </summary>
        /// <param name="com"></param>
        /// <returns></returns>
        public static string GetLocIdByRFidCom(string com)
        {
            List<Locations> listLoc = GetLocations();
            return listLoc.Where(item => item.RFCom == com).First().Id;
        }

        #endregion

		/// <summary>
		/// 获得项目的根路径
		/// </summary>
		/// <returns></returns>
		public static string GetProjectRootPath()
		{
			string BaseDirectoryPath = Directory.GetCurrentDirectory(); 
			return BaseDirectoryPath;
		}
	}
}
