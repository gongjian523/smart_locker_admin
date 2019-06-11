using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Infrastructure.LockHelper
{
    class LockHelper : SerialPort
    {
        public LockHelper(string portName, int baudRate)
        {
            PortName = portName;
            BaudRate = baudRate;
            DataBits = 8;
        }

        public void OpenDoor()
        {

            Console.WriteLine("OpenDoor");
            if (!IsOpen)
                Open();

            Byte[] TxData = { 0xA1 };
            Write(TxData, 0, 1);
        }

        public int IsDoorOpen()
        {
            Byte[] receivedData = new Byte[4];
            Read(receivedData, 0, 4);

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

            return -1;
            //if (receivedData[6] != (byte)Check_Xor(receivedData, 6))
            //    return -1;

        }
    }
}
