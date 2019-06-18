using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using System.Threading.Tasks;


namespace CFLMedCab.Infrastructure.DeviceHelper
{
    public class VeinHelper : SerialPort
    {

        public VeinHelper(string portName, int baudRate)
        {
            //主柜指静脉串口"COM9"
            //副柜指静脉串口"COM8"

            PortName = portName;
            BaudRate = baudRate;
            DataBits = 8;
        }

        public void ChekVein()
        {
            return;
            Console.WriteLine("checkVein");
            if (!IsOpen)
                Open();

            //字节：内容
            //1：Start 固定为Ox40
            //2: 命令 CMD_ONE_VS_N Ox00
            //3: deviceId  固定为OxFF
            //4: 0x00
            //5: 0x00
            //6: 0x00
            //7: chk
            //8: End 固定为0x0D

            Byte[] TxData = new Byte[8];
            TxData[0] = 0x40;
            TxData[1] = 0x00;
            TxData[2] = 0xFF;
            TxData[3] = 0x00;
            TxData[4] = 0x00;
            TxData[5] = 0x00;

            //char chk = (char)TxData[0];
            //for (int i = 1; i < 6; i++)
            //{
            //    chk ^= (char)TxData[i];
            //}
            //TxData[6] = (Byte)chk;
            TxData[6] = (Byte)Check_Xor(TxData, 6);
            TxData[7] = 0x0D;

            //string strRcv = null;
            //for (int i = 0; i < 8; i++) //窗体显示
            //    strRcv += TxData[i].ToString("X2");  //16进制显示

            Write(TxData, 0, 8);
        }

        public int GetVeinId()
        {
            Byte[] receivedData = new Byte[8];
            Read(receivedData, 0, 8);

            string strRcv = null;
            for (int i = 0; i < 8; i++) //窗体显示
                strRcv += receivedData[i].ToString("X2");  //16进制显示
            WriteLine(strRcv);

            //字节：内容
            //1：Start 固定为Ox40
            //2: 命令 CMD_ONE_VS_N Ox00
            //3: deviceId  固定为OxFF
            //4: 正确的时候为指静脉id低字节 FID(L); 错误的时候为0x00; 
            //5: 正确的时候为指静脉id高字节 FID(H); 错误的时候为0x00; 
            //6: result 0x00验证成功; 0x01验证失败; 0x02放置手指超时 
            //   0x07存储空间为空（没有注册或者下载模板）
            //   0x0E传感器未检测到手指  0x10生成不合格模板 0x11拍照超时
            //7: chk
            //8: End 固定为0x0D

            if (receivedData[0] != 0x40)
                return -1;

            if (receivedData[7] != 0x0d)
                return -1;

            if (receivedData[6] != (byte)Check_Xor(receivedData, 6))
                return -1;

            //验证成功
            if (receivedData[5] == 0x00)
                return receivedData[3] + receivedData[4] * 256;
            //验证失败
            else if (receivedData[5] == 0x01)
                return 0;
            else
                return -1;
        }

        private char Check_Xor(Byte[] arrary, int length)
        {
            char chk = (char)arrary[0];
            for (int i = 1; i < length; i++)
            {
                chk ^= (char)arrary[i];
            }
            return chk;
        }
    }
}
