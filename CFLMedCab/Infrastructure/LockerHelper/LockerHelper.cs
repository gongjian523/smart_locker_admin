using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace CFLMedCab.Infrastructure.LockHelper
{
    class LockHelper
	{

		/// <summary>
		/// 保持子线程阻塞主线程
		/// </summary>
		private static List<ManualResetEvent> manualEvents = new List<ManualResetEvent>();

		/// <summary>
		/// 创建串口连接
		/// </summary>
		/// <param name="com">串口信息：如COM2</param>
		/// <param name="isConnect">是否连接串口成功</param>
		/// <returns></returns>
		private static SerialPort CreateClientConn(string com, out bool isConnect)
		{
			isConnect = true;
			SerialPort comSerialPort = new SerialPort
			{
				BaudRate = 115200,
				PortName = com, //主柜锁串口
				//com.PortName = "COM5";   //副柜锁串口
				DataBits = 8
			};
			try
			{
				comSerialPort.Open();
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex);
				isConnect = false;
			}
		
			return comSerialPort;
		}

		/// <summary>
		/// 创建串口连接
		/// </summary>
		/// <param name="com">串口</param>
		/// <param name="baudRate">波特率</param>
		/// <param name="isConnect">是否连接串口成功</param>
		/// <returns></returns>
		private static SerialPort CreateClientConn(string com, int baudRate, out bool isConnect)
		{
			isConnect = true;
			SerialPort comSerialPort = new SerialPort
			{
				BaudRate = baudRate,
				PortName = com, //主柜锁串口
								//com.PortName = "COM5";   //副柜锁串口
				DataBits = 8
			};
			try
			{
				comSerialPort.Open();
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex);
				isConnect = false;
			}

			return comSerialPort;
		}

		/// <summary>
		/// 消息发送和数据处理
		/// </summary>
		/// <param name="clientConn"></param>
		/// <param name="com"></param>
		/// <returns></returns>
		private static int DealComData(SerialPort clientConn, string com, out bool isGetSuccess)
		{
			isGetSuccess = true;
			ManualResetEvent manualResetEvent = new ManualResetEvent(false);
			manualEvents.Add(manualResetEvent);

			DelegateGetMsg delegateGetMsg = new DelegateGetMsg(com, clientConn, manualResetEvent);

			try
			{
				byte[] TxData = { 0xA1 };
				clientConn.Write(TxData, 0, 1);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex);
				isGetSuccess = false;
			}
			
			// 订阅标签上报事件
			clientConn.DataReceived += new SerialDataReceivedEventHandler(delegateGetMsg.OnDataReceivedLock);
		
			//返回收集结果
			return delegateGetMsg.GetDelegateMsg();

		}


		/// <summary>
		/// 获取rfid的Locker数据，目前只有主柜(COM2)和副柜(COM5)信息
		/// </summary>
		public static Hashtable GetLockerData(out bool isGetSuccess)
		{

			isGetSuccess = true;

			string com2 = "COM2";
			string com5 = "COM5";

			Hashtable currentLockerDataHt = new Hashtable();

			SerialPort com1ClientConn = CreateClientConn(com2, 115200, out bool isCom1Connect);
			if (isCom1Connect)
			{
				currentLockerDataHt.Add(com2, DealComData(com1ClientConn, com2, out isGetSuccess));
			}
			else
			{
				isGetSuccess = false;
			}

			SerialPort com4ClientConn = CreateClientConn(com5, 115200, out bool isCom4Connect);
			if (isCom4Connect)
			{
				currentLockerDataHt.Add(com5, DealComData(com4ClientConn, com5, out isGetSuccess));
			}
			else
			{
				isGetSuccess = false;
			}

			WaitHandle.WaitAll(manualEvents.ToArray());

			return currentLockerDataHt;

		}


		public static void TestGetLockerData(object sender, ElapsedEventArgs elapsed)
		{
			Console.ReadKey();
			System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
			watch.Start();  //开始监视代码运行时间
			GetLockerData(out bool isGetSuccess);
			watch.Stop();  //停止监视
			TimeSpan timespan = watch.Elapsed;  //获取当前实例测量得出的总时间
			System.Diagnostics.Debug.WriteLine("打开窗口代码执行时间：{0}(毫秒)", timespan.TotalMilliseconds);  //总毫秒数
			Console.ReadKey();
		}

	

		/// <summary>
		/// 内部类，区分不同机柜（即不同com串口）的扫描消息
		/// </summary>
		class DelegateGetMsg
		{
			/// <summary>
			/// 串口号，标识机柜
			/// </summary>
			private readonly string com;

			/// <summary>
			/// 当前串口连接对象
			/// </summary>
			private readonly SerialPort currentClientConn;

			/// <summary>
			/// ManualResetEvent，用于人为阻塞当前线程
			/// </summary>
			private readonly ManualResetEvent manualResetEvent;

			/// <summary>
			/// 存储当前收集到的消息
			/// </summary>
			private int isOpen;


			public DelegateGetMsg(String com, SerialPort currentClientConn, ManualResetEvent manualResetEvent)
			{
				this.com = com;
				this.currentClientConn = currentClientConn;
				this.manualResetEvent = manualResetEvent;
			}


			/// <summary>
			/// 
			/// </summary>
			/// <param name="sender"></param>
			/// <param name="e"></param>
			public void OnDataReceivedLock(object sender, SerialDataReceivedEventArgs e)
			{
				
				byte[] receivedData = new byte[4];
				currentClientConn.Read(receivedData, 0, 4);

				string strRcv = null;

				for (int i = 0; i < 4; i++) //窗体显示
				{

					strRcv += receivedData[i].ToString("X2");  //16进制显示
				}

				//看上报数据格式为：08 81 xx xx(16进制)
				//08 81 表示帧头
				//第一XX: 表示1 - 8号锁的工作状态
				//第二XX: 表示9 - 16号锁的工作状态
				//xx&0x01==0x01   1号锁门关闭，否则1号打开
				if (receivedData[0] != 0x08)
					isOpen = 0;

				if (receivedData[1] != 0x81)
					isOpen = 0;

				if ((receivedData[0] & 0x01) == 0x01)
					isOpen = 1;
				else
					isOpen = 0;

				currentClientConn.Close();
				manualResetEvent.Set();

			}

		

			/// <summary>
			/// 返回当前消息 回调消息
			/// </summary>
			public int GetDelegateMsg()
			{
				return isOpen;
			}

		}

	}



}
