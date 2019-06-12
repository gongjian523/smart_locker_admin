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

            //发送指令：A1（16进制 ）打开1号锁门
            //A2—A8 打开2--8号锁门
            //B1—B8 打开9--16号锁门

            Byte[] TxData = { 0xA1 };
            Write(TxData, 0, 1);
        }

        public int IsDoorOpen()
        {
            Byte[] receivedData = new Byte[4];
            Read(receivedData, 0, 4);

            string strRcv = null;
            for (int i = 0; i < 4; i++) 
                strRcv += receivedData[i].ToString("X2");  //16进制显示
            WriteLine(strRcv);

            //看上报数据格式为：08 81 xx xx(16进制)
            //08 81 表示帧头
            //第一XX: 表示1 - 8号锁的工作状态
            //第二XX: 表示9 - 16号锁的工作状态
            //xx&0x01==0x01   1号锁门关闭，否则1号打开
            if (receivedData[0] != 0x08)
                return 0;

            if (receivedData[1] != 0x81)
                return 0;

            if ((receivedData[0] & 0x01) == 0x01)
                return 1;
            else
                return 0;
        }

    }
}
