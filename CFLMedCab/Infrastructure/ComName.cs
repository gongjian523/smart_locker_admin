using System;
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
            return(new List<string> {
                "COM2",
                "COM5"
            });
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
    }
}
