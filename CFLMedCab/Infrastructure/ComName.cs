using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Infrastructure
{
    public class ComName
    {
        public static string GetVeinCom(string pos)
        {
            //主柜指静脉串口"COM9"
            //副柜指静脉串口"COM8"

            if (pos.EndsWith("1"))
                return "COM9";
            else
                return "COM8";
        }

        public static string GetLockerCom(string pos)
        {
            //主柜锁串口 COM2
           //副柜锁串口 COM5

            if (pos.EndsWith("1"))
                return "COM2";
            else
                return "COM5";
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

        public static string GetRfidCom(string pos)
        {
            //COM1  主柜rfid串口
            //COM4  副柜rfid串口 

            if (pos.EndsWith("1"))
                return "COM1";
            else
                return "COM4";
        }

        public static string GetCabNameByRFidCom(string com)
        {
            if (com == ApplicationState.GetValue<string>((int)ApplicationKey.COM_MRFid))
                return ApplicationState.GetValue<string>((int)ApplicationKey.CodeMCab);
            else if (com == ApplicationState.GetValue<string>((int)ApplicationKey.COM_SRFid))
                return ApplicationState.GetValue<string>((int)ApplicationKey.CodeSCab);
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
            if (cabName == ApplicationState.GetValue<string>((int)ApplicationKey.CodeMCab))
                return ApplicationState.GetValue<string>((int)ApplicationKey.COM_MLocker);
            else if (cabName == ApplicationState.GetValue<string>((int)ApplicationKey.CodeSCab))
                return ApplicationState.GetValue<string>((int)ApplicationKey.COM_SLocker);
            else
                return "";
        }

        public static string GetCabNameByCode(string code, Hashtable ht)
        {
            foreach (string key in ht.Keys)
            {
                if(((HashSet<string>) ht[key]).Contains(code))
                    return GetCabNameByRFidCom(key);
            }

            return "";
        }

    }
}
