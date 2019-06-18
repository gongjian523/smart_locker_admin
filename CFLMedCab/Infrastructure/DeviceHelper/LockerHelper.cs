using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Speech.Synthesis;
using System.Threading;
using System.Timers;

namespace CFLMedCab.Infrastructure.DeviceHelper
{
    class LockHelper
	{

		/// <summary>
		/// 关门消息超时时间
		/// </summary>
		private static readonly int timeout = 10000;

		

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

			if (isGetSuccess)
			{
				ManualResetEvent manualResetEvent = new ManualResetEvent(false);
				manualEvents.Add(manualResetEvent);
				DelegateGetMsg delegateGetMsg = new DelegateGetMsg(com, clientConn, manualResetEvent);
				// 订阅标签上报事件
				clientConn.DataReceived += new SerialDataReceivedEventHandler(delegateGetMsg.OnDataReceivedLock);
				//返回收集结果
				return delegateGetMsg.GetDelegateMsg();
			}
			else
			{
				return 0;
			}

		}

		/// <summary>
		/// 消息发送和数据处理（发送开门消息）
		/// </summary>
		/// <param name="clientConn"></param>
		/// <param name="com"></param>
		/// <returns></returns>
		private static DelegateGetMsg DealComDataNonblocking(SerialPort clientConn, string com, out bool isGetSuccess)
		{
			isGetSuccess = true;

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

			if (isGetSuccess)
			{
	
				DelegateGetMsg delegateGetMsg = new DelegateGetMsg(com, clientConn);
				// 订阅标签上报事件
				clientConn.DataReceived += new SerialDataReceivedEventHandler(delegateGetMsg.OnDataReceivedLockNonblocking);
				//返回委托类
				return delegateGetMsg;
			}
			else
			{
				return null;
			}

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

		/// <summary>
		/// 获取rfid的Locker数据，目前只有主柜(COM2)和副柜(COM5)信息
		/// </summary>
		public static DelegateGetMsg GetLockerData(string com, out bool isGetSuccess)
		{
            isGetSuccess = true;

			SerialPort comClientConn = CreateClientConn(com, 115200, out bool isCom1Connect);
			if (isCom1Connect)
			{
				DelegateGetMsg delegateGetMsg = DealComDataNonblocking(comClientConn, com, out isGetSuccess);
				if (isGetSuccess)
				{
					return delegateGetMsg;
				}
				else
				{
					return null;
				}
			}
			else
			{
				isGetSuccess = false;

				return null;

			}
		}


		public static void TestGetLockerData(object sender, ElapsedEventArgs elapsed)
		{
			Console.ReadKey();
			System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
			watch.Start();  //开始监视代码运行时间
			GetLockerData("com1", out bool isGetSuccess);
			watch.Stop();  //停止监视
			TimeSpan timespan = watch.Elapsed;  //获取当前实例测量得出的总时间
			System.Diagnostics.Debug.WriteLine("打开窗口代码执行时间：{0}(毫秒)", timespan.TotalMilliseconds);  //总毫秒数
			Console.ReadKey();
		}

	

		/// <summary>
		/// 内部类，区分不同机柜（即不同com串口）的扫描消息
		/// </summary>
		public class DelegateGetMsg
		{
            public object userData { get; set; }
			/// <summary>
			/// 语音对象
			/// </summary>
			private SpeechSynthesizer synth;

			/// <summary>
			/// 记录定时任务开始时的时间毫秒数
			/// </summary>
			private long timerStartTime;

			/// <summary>
			/// 记录定时任务上次时间毫秒数
			/// </summary>
			private long timerLastTime;

			/// <summary>
			/// 语音消息播放间隔
			/// </summary>
			private static readonly int voiceBroadcastTimeInterval = 5000;

			/// <summary>
			/// 用于语音播报的定时任务（如果超时的话）
			/// </summary>
			private readonly System.Timers.Timer voiceBroadcastTimer; 

			/// <summary>
			/// 委托
			/// </summary>
			/// <param name="sender"></param>
			/// <param name="e"></param>
			public delegate void DelegateGetMsgHandler(object sender, bool isClose);

			/// <summary>
			/// 事件
			/// </summary>
			public event DelegateGetMsgHandler DelegateGetMsgEvent;

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
			/// 存储当前收集到的消息;默认以关闭
			/// </summary>
			private int isClose = 1;


			public DelegateGetMsg(string com, SerialPort currentClientConn)
			{
				this.com = com;
				this.currentClientConn = currentClientConn;

				synth = new SpeechSynthesizer
				{
					Rate = 3,
					Volume = 100

				};
				//配置和声音输出  
				synth.SetOutputToDefaultAudioDevice();

				timerStartTime = DateTime.Now.Ticks / 10000;
				timerLastTime = timerStartTime;
				voiceBroadcastTimer = new System.Timers.Timer
				{
					AutoReset = true,
					Enabled = true,
					Interval = 100 //执行间隔时间,单位为毫秒;
				};
				voiceBroadcastTimer.Elapsed += new ElapsedEventHandler(VoiceBroadcastEventHandler);
				voiceBroadcastTimer.Start();


			}


			public DelegateGetMsg(string com, SerialPort currentClientConn, ManualResetEvent manualResetEvent)
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

				//看上报数据格式为：08 81 xx xx(16进制)
				//08 81 表示帧头
				//第一XX: 表示1 - 8号锁的工作状态
				//第二XX: 表示9 - 16号锁的工作状态
				//xx&0x01==0x01   1号锁门关闭，否则1号打开
				if (receivedData[0] != 0x08)
					isClose = 0;

				if (receivedData[1] != 0x81)
					isClose = 0;

				if ((receivedData[2] & 0x01) == 0x01) {
					isClose = 1;
					currentClientConn.Close();
					manualResetEvent.Set();

				}
				else
					isClose = 0;


			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name="sender"></param>
			/// <param name="e"></param>
			public void OnDataReceivedLockNonblocking(object sender, SerialDataReceivedEventArgs e)
			{

				byte[] receivedData = new byte[4];
				currentClientConn.Read(receivedData, 0, 4);

				//看上报数据格式为：08 81 xx xx(16进制)
				//08 81 表示帧头
				//第一XX: 表示1 - 8号锁的工作状态
				//第二XX: 表示9 - 16号锁的工作状态
				//xx&0x01==0x01   1号锁门关闭，否则1号打开
				if (receivedData[0] != 0x08)
					isClose = 0;

				if (receivedData[1] != 0x81)
					isClose = 0;

				if ((receivedData[2] & 0x01) == 0x01)
				{

					if (isClose==0) {
						//检测到已经关门，停止定时任务
						voiceBroadcastTimer.Stop();
						//关闭定时资源
						voiceBroadcastTimer.Close();
						//检测到已经关门，关闭串口连接
						currentClientConn.Close();
						isClose = 1;
						DelegateGetMsgEvent(this, true);
						//释放语音资源
						synth.Dispose();
					}
				}
				else
					isClose = 0;

				Console.WriteLine("{0}机柜当关门状态:{1}", com, isClose);


			}

			/// <summary>
			/// 返回当前消息 回调消息
			/// </summary>
			public int GetDelegateMsg()
			{
				return isClose;
			}

			/// <summary>
			/// 调用语音播报的实现方法
			/// </summary>
			/// <param name="sender"></param>
			/// <param name="e"></param>
			private void VoiceBroadcastEventHandler(object sender, ElapsedEventArgs e)
			{
				long currentTimerTime = DateTime.Now.Ticks / 10000;
				//当时间与开始任务时间差距大于等于规定等待时间时，开始语音播报
				long diff1 = currentTimerTime - timerStartTime;
				if (diff1 >= timeout)
				{
					//当前时间大于等于语音播报间隔，才播报，否则丢弃该次播报
					long diff2 = currentTimerTime - timerLastTime;
					System.Diagnostics.Debug.WriteLine("时间差{0}", diff2);
					if (diff2 >= voiceBroadcastTimeInterval)
					{
						//记录这次时间
						timerLastTime = currentTimerTime;
						//语音播报
						synth.Speak("操作超时！请关门！");
					}
				}
			}
		}
	}
}
