
﻿using CFLMedCab.Http.Model;
using CFLMedCab.Model;
﻿using CFLMedCab.Http.Bll;

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
        Goods,     //从主系统中的查询到商品信息
        HouseId,   //库房id
        HouseName, //库房名字
        EquipId,   //设备id
        EquipName, //设备名字
        Location,  //货柜
        COM_MVein,
        AccessToken,
        RefreshToken,
        LoginId
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
        /// 通过货柜编号获取RFID的串口
        /// </summary>
        /// <param></param>
        public static string GetRfidComByLocCode(string locCode)
        {
            List<Locations> listLoc = GetLocations();
            return listLoc.Where(item => item.Code == locCode).First().RFCom;
        }

        /// <summary>
        /// 获取当前的门锁串口
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAllRfidCom()
        {
            List<Locations> listLoc = GetLocations();
            return listLoc.Select(item => item.RFCom).Distinct().ToList();
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

        #region goods info from mian system
        /// <summary>
        /// 保存所有货柜的打过去商品信息
        /// </summary>
        /// <param name="hs"></param>
        public static void SetGoodsInfo(HashSet<CommodityEps> hs)
        {
            SetValue((int)ApplicationKey.Goods, hs);
			//插入数据库，每次开门时，即同步数据到
			CommodityCodeBll.GetInstance().InsertLocalCommodityEpsInfo(hs);
			return;
        }

        /// <summary>
        /// 更新特定货柜的商品
        /// </summary>
        /// <param name="hs"></param>
        /// <param name="locCodes">如果没有整个参数，在一个货柜的商品都全部拿出的情况下，仅从hs中是无法判断这个货柜的商品已经全部取出还是这个货柜没有扫描</param> 
        public static void SetGoodsInfoInSepcLoc(HashSet<CommodityEps> hs, List<string> locCodes)
        {

            HashSet<CommodityEps> all = GetValue<HashSet<CommodityEps>>((int)ApplicationKey.Goods);
            //删除指定货柜的原来的商品
            all.RemoveWhere(item => locCodes.Contains(item.GoodsLocationName));
            //增加相应货柜的现在的商品
            all.UnionWith(hs);

            SetValue((int)ApplicationKey.Goods, all);
			//插入数据库，每次开门时，即同步数据到
			CommodityCodeBll.GetInstance().InsertLocalCommodityEpsInfo(all);
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
            return new HashSet<CommodityEps>(all.Where(item => locCodes.Contains(item.GoodsLocationName)));
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
            List<Locations> listLoc = GetLocations().Where(item => item.Id == id).ToList();

            if (listLoc.Count > 0)
            {
                return listLoc.First().Code;
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 通过Rfid串口查询货柜名字
        /// </summary>
        /// <param name="com"></param>
        /// <returns></returns>
        public static string GetLocCodeByRFidCom(string com)
        {
            List<Locations> listLoc = GetLocations().Where(item => item.RFCom == com).ToList();

            if(listLoc.Count > 0)
            {
                return listLoc.First().Code;
            }
            else
            {
                return "";
            }
        }
               
        /// <summary>
        /// 通过Rfid串口查询货柜id
        /// </summary>
        /// <param name="com"></param>
        /// <returns></returns>
        public static string GetLocIdByRFidCom(string com)
        {
            List<Locations> listLoc = GetLocations().Where(item => item.RFCom == com).ToList();
            if (listLoc.Count > 0)
            {
                return listLoc.First().Id;
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 通过锁串口查询货柜名字
        /// </summary>
        /// <param name="com"></param>
        /// <returns></returns>
        public static string GetLocCodeByLockerCom(string com)
        {
            List<Locations> listLoc = GetLocations().Where(item => item.LockerCom == com).ToList();

            if (listLoc.Count > 0)
            {
                return listLoc.First().Code;
            }
            else
            {
                return "";
            }
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


        /// <summary>
        /// 保存登录Id
        /// </summary>
        /// <param name="loginId"></param>
        public static void SetLoginId(int loginId)
        {
            SetValue((int)ApplicationKey.LoginId, loginId);
            return;
        }

        /// <summary>
        /// 获取登录Id
        /// </summary>
        /// <returns></returns>
        public static int GetLoginId()
        {
            return GetValue<int>((int)ApplicationKey.LoginId);
        }
    }
}
