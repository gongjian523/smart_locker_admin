using CFLMedCab.Infrastructure.ToolHelper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Speech.Synthesis;
using System.Threading;
using System.Timers;

namespace CFLMedCab.Infrastructure.DeviceHelper
{
	/// <summary>
	/// 用于指静脉相关串口处理的帮助类
	/// </summary>
    public class VeinSerialHelper
	{

		

		#region 发送命令帧的常量定义

		/// <summary>
		/// 帧头
		/// </summary>
		private const byte FRAME_HEADER = 0x40;


		#region 指令码相关

		/// <summary>
		/// 1:N比对
		/// </summary>
		private const byte CMD_ONE_VS_N = 0x00;

		/// <summary>
		/// 1:1比对
		/// </summary>
		private const byte CMD_ONE_VS_ONE = 0x02;

		/// <summary>
		/// 检测手指是否放置在手指检测传感器上
		/// </summary>
		private const byte CMD_CHK_FINGER = 0x0F;

		/// <summary>
		/// 单次采集手指静脉特征
		/// </summary>
		private const byte CMD_REGISTER = 0x03;

		/// <summary>
		/// 删除所有手指 
		/// </summary>
		private const byte CMD_DELETE_ALL = 0x06;

		/// <summary>
		/// 注册结束，用结束多次注册采集次数足够的流程
		/// </summary>
		private const byte CMD_REGISTER_END = 0x04;

		/// <summary>
		/// 设置模块的设备编号 Devid
		/// </summary>
		private const byte CMD_SET_DEVID = 0x12;

		#endregion

		/// <summary>
		/// 设备号（广播地址，也是默认地址）
		/// </summary>
		private const byte DEV_ID = 0xFF;

		/// <summary>
		/// 帧尾
		/// </summary>
		private const byte FRAME_END = 0x0D;


		#endregion

		#region 返回结果帧常量定义

		/// <summary>
		/// 成功
		/// </summary>
		private const byte ERR_SUCCESS = 0x00;

		/// <summary>
		/// 串口连接失败
		/// </summary>
		private const byte ERR_CONNECTION = 0xEE;

		/// <summary>
		/// 串口发送命令
		/// </summary>
		private const byte ERR_CMD_TIMEOUT = 0xED;

		/// <summary>
		/// 数据不符合规定格式
		/// </summary>
		private const byte ERR_CMD_DATA_ERR = 0xEF;

		/// <summary>
		/// 验证失败
		/// </summary>
		private const byte ERR_FALT = 0x01;

		/// <summary>
		///  放置手指超时
		/// </summary>
		private const byte ERR_TIMEOUT = 0x02;

		/// <summary>
		/// 与前一次采集静脉特征差异过大，请重放手指
		/// </summary>
		private const byte ERR_TEMPLATE_OCC = 0x05;

		/// <summary>
		///  与前一次采集手指下发信息不同 
		/// </summary>
		private const byte ERR_FINGERID_OCC = 0x06;

		/// <summary>
		/// 生成不合格模板
		/// </summary>
		private const byte ERR_TEMPLATE = 0x10;

		/// <summary>
		/// 拍照超时
		/// </summary>
		private const byte ERR_CAP = 0x11;

		/// <summary>
		///  传感器未检测到手指 
		/// </summary>
		private const byte ERR_NO_FINGER = 0x0E;

		/// <summary>
		/// 注册模板缓存区满
		/// </summary>
		private const byte ERR_REG_BUFFFULL = 0x0F;

		#endregion

		#region 其他常量定义

		/// <summary>
		///  单指特征采集次数
		/// </summary>
		public const int FEATURE_COLLECT_CNT = 3;

		/// <summary>
		/// 控制是否关闭正在进行的手指检测变量
		/// </summary>
		public volatile static bool isCloseCheckFinger = true;

		

		/// <summary>
		/// 持久连接，不关闭
		/// </summary>
		private static SerialPort preSerialPort;

		/// <summary>
		/// 持久连接，接收参数
		/// </summary>
		private static byte[] preReceivedData = new byte[8];

		/// <summary>
		/// 持久连接标识的uuid
		/// </summary>
		private static string preUUID;

		#endregion
		/// <summary>
		/// 检测手指是否放置在手指检测传感器上，该命令最大返回时间<1ms（以上时间均不包括通信时间，仅为模块内 部程序的处理时间）。
		/// </summary>
		/// <returns></returns>
		public static bool CMD_CHK_FINGER_F()
		{
			return CMD_COMMON_F_IS_SUCCESS(() => { return CmdCommunicationFrame(CMD_CHK_FINGER); });
		}

		/// <summary>
		/// 带有超时判断的指纹检测
		/// </summary>
		/// <returns></returns>
		public static bool CMD_CHK_FINGER_TIMEOUT_F(long timeout = long.MaxValue)
		{
			long beforeTime = DateTime.Now.Ticks / 10000;
			bool isCheckFinger = false;
			isCloseCheckFinger = false;
			while (!isCloseCheckFinger)
			{
				isCheckFinger = CMD_CHK_FINGER_F();

				if (isCheckFinger)
				{
					isCloseCheckFinger = true;
				}

				if (timeout != long.MaxValue && ( DateTime.Now.Ticks/10000 - beforeTime >= timeout))
				{
					isCloseCheckFinger = true;
				}

			}

			return isCheckFinger;

		}

		/// <summary>
		/// 采集手指静脉特征与设备模板库进行比对，手指检测的超时时间为 3 秒。 
		/// 100 指以内（每指 3 个模板）平均比对时间<1 秒，最大时间<1.6 秒。
		/// 该命令最大返回时间为 3 秒（未放手指）， 在发送该命令前手指已正常放置，最大返回时间为 150ms + n*15ms(n 为已存在的手指数)。
		///（以上时间均不包括 通信时间，仅为模块内部程序的处理时间）
		/// </summary>
		public static bool CMD_ONE_VS_N_F(out ushort fid, out string errStr)
		{

			fid = CMD_COMMON_F_USHORT(() => { return CmdCommunicationFrame(CMD_ONE_VS_N); }, (receivedData) => {

				if (receivedData[5] == ERR_SUCCESS)
				{
					return CmdParam(receivedData);
	
				}
				return ushort.MaxValue;
			}, out errStr);
			
			return fid != ushort.MaxValue;
		}

		/// <summary>
		/// 采集手指静脉特征与指定 FID 手指的模板进行比对，手指检测的超时时间为 3 秒。
		/// 该命令最大返回时间为 3 秒 （未放手指），在发送该命令前手指已正常放置，最大返回时间为 165ms。
		///（以上时间均不包括通信时间，仅为模块 内部程序的处理时间） 
		/// </summary>
		/// <param name="fid"></param>
		/// <returns></returns>
		public static bool CMD_ONE_VS_ONE_F(ushort fid, out string errStr)
		{
			return CMD_COMMON_F_IS_SUCCESS(() => { return CmdCommunicationFrame(CMD_ONE_VS_ONE, fid);}, out errStr);
		}

		/// <summary>
		/// 删除指定的所有手指 FID 的信息头和所有模板。
		/// 该命令最大返回时间为 50ms*n(n 为已存在的手指数)。（以上时 间均不包括通信时间，仅为模块内部程序的处理时间）。
		/// </summary>
		/// <returns></returns>
		public static bool CMD_DELETE_ALL_F(out string errStr)
		{
			return CMD_COMMON_F_IS_SUCCESS(() => { return CmdCommunicationFrame(CMD_DELETE_ALL); }, out errStr);
		}

		
		/// <summary>
		/// 单次采集手指静脉特征。根据手指注册次数需求，此命令需要重复 3~6 次。
		/// 手指检测的超时时间为 7 秒。该命 令最大返回时间为 7 秒（未放手指），在发送该命令前手指已正常放置，最大返回时间为 150ms。
		/// 以上时间均不包 括通信时间，仅为模块内部程序的处理时间）
		/// </summary>
		/// <returns></returns>
		public static int CMD_REGISTER_F(ushort fid)
		{
			byte gid = 1;
			return CMD_COMMON_F_INT(() => { return CmdCommunicationFrame(CMD_REGISTER, fid, gid); });
		}

		/// <summary>
		/// 该命令作用为把注册生成的信息头和模板的进行处理。有三种方式： 
		/// 1. 取消注册操作，每次采集时都可以使用此命令来结束注册手指流程。 
		/// 2. 注册结果写入特征库，若已经存在相同 FID 的用户将会被覆盖； 
		/// 3. 注册结果回传上位机。 在使用方式 2，注册结果写入特征库的情况下该命令最大返回时间为 160m。其他两种在方式在模块内的等待时 间<1ms。（以上时间均不包括通信时间，仅为模块内部程序的处理时间）
		/// </summary>
		/// <param name="errStr"></param>
		/// <param name="p3"></param>
		/// <returns></returns>
		public static bool CMD_REGISTER_END_F(out string errStr, byte p3 = 0x01)
		{
			int errInt = CMD_COMMON_F_INT(() => { return CmdCommunicationFrame(CMD_REGISTER_END, p3); });
			errStr = GetErrDescribe(errInt);
			return errInt == ERR_SUCCESS;
		}

		/// <summary>
		/// 单次采集手指静脉特征。根据手指注册次数需求，此命令需要重复 3~6 次。
		/// 手指检测的超时时间为 7 秒。该命 令最大返回时间为 7 秒（未放手指），在发送该命令前手指已正常放置，最大返回时间为 150ms。
		/// 以上时间均不包 括通信时间，仅为模块内部程序的处理时间）
		/// </summary>
		/// <param name="fid"></param>
		/// <param name="errStr"></param>
		/// <param name="gid"></param>
		/// <returns></returns>
		public static bool CMD_REGISTER_F(ushort fid, out string errStr, byte gid = 1)
		{
			int errInt = CMD_COMMON_F_INT(() => { return CmdCommunicationFrame(CMD_REGISTER, fid, gid);});
			errStr = GetErrDescribe(errInt);
			return errInt == ERR_SUCCESS;
		}

		/// <summary>
		/// 设置模块的设备编号 Devid，用于设备通信。该命令最大返回时间<1ms（以上时间均不包括通信时间，仅为模 块内部程序的处理时间）。默认设置为0xFF
		/// </summary>
		/// <returns></returns>
		public static bool CMD_SET_DEVID_F()
		{
			return CMD_COMMON_F_IS_SUCCESS(() => { return CmdCommunicationFrame(CMD_SET_DEVID, 0x00, 0x00, DEV_ID); });
		}

		
		/// <summary>
		/// 通过命令生成完整通信命令帧
		/// </summary>
		/// <param name="cmd"></param>
		/// <returns></returns>
		private static byte[] CmdCommunicationFrame(byte cmd)
		{
			return CmdCommunicationFrame(cmd, 0x00, 0x00, 0x00);
		}

		/// <summary>
		/// 通过命令生成完整通信命令帧
		/// </summary>
		/// <param name="cmd"></param>
		/// <returns></returns>
		private static byte[] CmdCommunicationFrame(byte cmd, byte p1, byte p2)
		{
			return CmdCommunicationFrame(cmd, p1, p2, 0x00);
		}

		/// <summary>
		/// 通过命令生成完整通信命令帧
		/// </summary>
		/// <param name="cmd"></param>
		/// <returns></returns>
		private static byte[] CmdCommunicationFrame(byte cmd, ushort fid)
		{
			var fids = CmdParam(fid);
			return CmdCommunicationFrame(cmd, fids[0], fids[1], 0x00);
		}

		/// <summary>
		/// 通过命令生成完整通信命令帧
		/// </summary>
		/// <param name="cmd"></param>
		/// <returns></returns>
		private static byte[] CmdCommunicationFrame(byte cmd, byte p3)
		{
			return CmdCommunicationFrame(cmd, 0x00, 0x00, p3);
		}

		/// <summary>
		/// 通过命令生成完整通信命令帧
		/// </summary>
		/// <param name="cmd"></param>
		/// <returns></returns>
		private static byte[] CmdCommunicationFrame(byte cmd, ushort fid, byte gid)
		{
			var fids = CmdParam(fid);
			return CmdCommunicationFrame(cmd, fids[0], fids[1], gid);
		}

		/// <summary>
		/// 通过命令生成完整通信命令帧
		/// </summary>
		/// <param name="cmd"></param>
		/// <returns></returns>
		private static byte[] CmdCommunicationFrame(byte cmd, byte p1, byte p2, byte p3)
		{

			byte[] cmdCommunicationFrame = new byte[8];
			cmdCommunicationFrame[0] = FRAME_HEADER;
			cmdCommunicationFrame[1] = cmd;
			cmdCommunicationFrame[2] = DEV_ID;
			cmdCommunicationFrame[3] = p1;
			cmdCommunicationFrame[4] = p2;
			cmdCommunicationFrame[5] = p3;
			cmdCommunicationFrame[6] = Check_Xor(cmdCommunicationFrame, 6);
			cmdCommunicationFrame[7] = FRAME_END;
			return cmdCommunicationFrame;

		}

	

		/// <summary>
		/// 获取对应描述
		/// </summary>
		/// <param name="err"></param>
		/// <returns></returns>
		public static string GetErrDescribe(int err)
		{
			string errEscribe;

			switch (err)
			{
			
				case ERR_SUCCESS:
					errEscribe = "成功！";
					break;
				case ERR_FALT:
					errEscribe = "验证失败！";
					break;
				case ERR_TIMEOUT:
					errEscribe = "指静脉匹配超时！";
					break;
				case ERR_TEMPLATE_OCC:
					errEscribe = "与前一次采集静脉特征差异过大，请重放手指！";
					break;
				case ERR_FINGERID_OCC:
					errEscribe = "与前一次采集手指下发信息不同 ！";
					break;
				case ERR_TEMPLATE:
					errEscribe = "生成不合格模板！";
					break;
				case ERR_CAP:
					errEscribe = "拍照超时！";
					break;
				case ERR_NO_FINGER:
					errEscribe = "传感器未检测到手指 ！";
					break;
				case ERR_REG_BUFFFULL:
					errEscribe = "注册模板缓存区满！";
					break;
				case ERR_CONNECTION:
					errEscribe = "串口连接失败！";
					break;
				case ERR_CMD_DATA_ERR:
					errEscribe = "返回串口数据异常！";
					break;
				default:
					errEscribe = "非法状态";
					break;
			}

			return errEscribe;

		}

		/// <summary>
		/// 计算命令帧校验和
		/// </summary>
		/// <param name="arrary"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		private static byte Check_Xor(byte[] arrary, int length)
		{
			byte chk = arrary[0];
			for (int i = 1; i < length; i++)
			{
				chk ^= arrary[i];
			}
			return chk;
		}

		/// <summary>
		/// 参数处理 （字符串解析成字节）
		/// </summary>
		/// <param name="fid">fid字符串</param>
		/// <returns></returns>
		private static byte[] CmdParam(ushort fid) {
			return BitConverter.GetBytes(fid);
		}

		/// <summary>
		/// 参数处理 （组合成字符串）
		/// </summary>
		/// <param name="fidl"></param>
		/// <param name="fidh"></param>
		/// <returns></returns>
		private static ushort CmdParam(byte fidl, byte fidh)
		{
			return BitConverter.ToUInt16(new byte[]{ fidl, fidh }, 0);
		}

		/// <summary>
		/// 参数处理
		/// </summary>
		/// <param name="resposeByte">相应结果提取fid</param>
		/// <returns></returns>
		private static ushort CmdParam(byte[] resposeByte)
		{
			return CmdParam(resposeByte[3], resposeByte[4]);
		}

		/// <summary>
		/// 公共方法，利用Func对象做委托(处理结果是否正确，带校验)
		/// </summary>
		/// <param name="cmdFunc">命令参数</param>
		/// <returns></returns>
		private static bool CMD_COMMON_F_IS_SUCCESS(Func<byte[]> cmdFunc)
		{
		  return CMD_COMMON_F_INT(cmdFunc, (receivedData) => {
			  return receivedData[5];
		  }) == ERR_SUCCESS;
		}
		/// <summary>
		/// 公共方法，利用Func对象做委托(处理结果是否正确，带校验)
		/// </summary>
		/// <param name="cmdFunc">命令参数</param>
		/// <returns></returns>
		private static bool CMD_COMMON_F_IS_SUCCESS(Func<byte[]> cmdFunc, out string errStr)
		{
			var receivedDataInt = CMD_COMMON_F_INT(cmdFunc, (receivedData) => {
				return receivedData[5];
			});
			errStr = GetErrDescribe(receivedDataInt);
			return receivedDataInt == ERR_SUCCESS;
		}


		/// <summary>
		/// 公共方法，利用Func对象做委托(返回处理后的INT)
		/// </summary>
		/// <param name="cmdFunc">命令参数</param>
		/// <returns></returns>
		private static int CMD_COMMON_F_INT(Func<byte[]> cmdFunc, Func<byte[], int> intFunc)
		{
			//返回收集结果
			return intFunc.Invoke(CMD_COMMON_F(cmdFunc));
		}

		/// <summary>
		/// 公共方法，利用Func对象做委托(返回处理后的INT)
		/// </summary>
		/// <param name="cmdFunc">命令参数</param>
		/// <returns></returns>
		private static int CMD_COMMON_F_INT(Func<byte[]> cmdFunc)
		{

			var receivedData = CMD_COMMON_F(cmdFunc);
			if (receivedData == null)
			{
				//返回null，如果串口连接失败
				return ERR_CONNECTION;
			}
			return receivedData[5];
		}

		/// <summary>
		/// 公共串口方法，利用Func对象做委托(返回处理后的ushort)
		/// </summary>
		/// <param name="cmdFunc"></param>
		/// <param name="ushortFunc"></param>
		/// <returns></returns>
		private static ushort CMD_COMMON_F_USHORT(Func<byte[]> cmdFunc, Func<byte[], ushort> ushortFunc)
		{
			return ushortFunc.Invoke(CMD_COMMON_F(cmdFunc));
		}

		/// <summary>
		/// 公共串口方法，利用Func对象做委托(返回处理后的ushort)
		/// </summary>
		/// <param name="cmdFunc"></param>
		/// <param name="ushortFunc"></param>
		/// <returns></returns>
		private static ushort CMD_COMMON_F_USHORT(Func<byte[]> cmdFunc, Func<byte[], ushort> ushortFunc, out string errStr)
		{
			var receivedData = CMD_COMMON_F(cmdFunc);
			errStr = GetErrDescribe(receivedData[5]);
			return ushortFunc.Invoke(receivedData);
		}

		/// <summary>
		/// 公共方法，利用Func对象做委托
		/// </summary>
		/// <param name="cmdFunc">命令参数</param>
		/// <returns></returns>
		private static byte[] CMD_COMMON_F(Func<byte[]> cmdFunc)
		{
			return CMD_COMMON_SLEEP_F(cmdFunc, 50);
		}

		/// <summary>
		/// 公共方法，利用Func对象做委托（延时处理）
		/// </summary>
		/// <param name="cmdFunc">命令参数</param>
		/// <returns></returns>
		private static byte[] CMD_COMMON_SLEEP_F(Func<byte[]> cmdFunc, int sleeptime = 100)
		{
			if(sleeptime != 0)
				Thread.Sleep(sleeptime);

			var clientConn = CreateClientConn(ApplicationState.GetMVeinCOM(), out bool isConnect);

			byte[] receivedData = new byte[8];

			//返回串口连接错误
			if (!isConnect)
			{
				receivedData[5] = ERR_CONNECTION;
				return receivedData;
			}

			//加锁阻塞机制
			var m = new ManualResetEvent(false);

		
			//uuid作为消息互动标识
			var cmdUUID = Guid.NewGuid().ToString("N");

			

			// 订阅串口收到
			clientConn.DataReceived += new SerialDataReceivedEventHandler((object sender, SerialDataReceivedEventArgs e) => {

			
				try
				{

					if (clientConn.IsOpen)
					{
                        Thread.Sleep(50);

						clientConn.Read(receivedData, 0, receivedData.Length);

                        #if SERIALPORTLOG
						LogUtils.Debug($"【线程名:{Thread.CurrentThread.ManagedThreadId.ToString()},命令uuid：{cmdUUID}】 串口数据已接收：{HexHelper.ByteToHexStr(",", receivedData)}");
                        #endif

						bool isDataReceivedCheckSuccess = receivedData[0] == 0x40 && receivedData[7] == 0x0d && receivedData[6] == Check_Xor(receivedData, 6);
						if (!isDataReceivedCheckSuccess)
						{
							receivedData[5] = ERR_CMD_DATA_ERR;
						}
					}

				}
				catch (Exception ex)
				{

					LogUtils.Error($"【线程名:{Thread.CurrentThread.ManagedThreadId.ToString()},命令uuid：{cmdUUID}】串口接收时异常：{ex.Message}");

				}
				finally
				{
					//接收到返回信息，最后关闭串口
					CloseClientConn(clientConn);

					//解锁
					m.Set();
				}

			});


			//发送命令
			var sendData = cmdFunc.Invoke();

			try {
				clientConn.Write(sendData, 0, sendData.Length);
			} catch (Exception ex)
			{
				LogUtils.Error($"【线程名:{Thread.CurrentThread.ManagedThreadId.ToString()},写入异常：{ex.ToString()}");
			}

#if SERIALPORTLOG
            LogUtils.Debug($"【线程名:{Thread.CurrentThread.ManagedThreadId.ToString()},命令uuid：{cmdUUID}】 串口数据已发送：{HexHelper.ByteToHexStr(",", sendData)}");
#endif
			
			//阻塞当前线程，最长5秒
			var isTimeout = !m.WaitOne(new TimeSpan(0, 0, 5));
			if (isTimeout)
			{
				//超时关闭当前串口
				CloseClientConn(clientConn);
				receivedData[5] = ERR_CMD_TIMEOUT;
				LogUtils.Error($"【线程名:{Thread.CurrentThread.ManagedThreadId.ToString()},命令uuid：{cmdUUID}】串口接收时超时");
			}
			
			//返回收集结果
			return receivedData;
		}

		/// <summary>
		/// 公共方法，利用Func对象做委托（持久连接）
		/// </summary>
		/// <param name="cmdFunc">命令参数</param>
		/// <returns></returns>
		private static byte[] CMD_COMMON_PER_F(Func<byte[]> cmdFunc)
		{

			var clientConn = InitClientConn(out bool isConnect);

			

			//返回串口连接错误
			if (!isConnect)
			{
				preReceivedData[5] = ERR_CONNECTION;
				return preReceivedData;
			}

			//加锁阻塞机制
			var m = new ManualResetEvent(false);


			//uuid作为消息互动标识
			preUUID = Guid.NewGuid().ToString("N");



			// 订阅串口收到
			clientConn.DataReceived += new SerialDataReceivedEventHandler((object sender, SerialDataReceivedEventArgs e) => {


				try
				{

					if (clientConn.IsOpen)
					{
                        Thread.Sleep(50);

                        clientConn.Read(preReceivedData, 0, preReceivedData.Length);

                        #if SERIALPORTLOG
                        LogUtils.Debug($"【线程名:{Thread.CurrentThread.ManagedThreadId.ToString()},命令uuid：{preUUID}】 串口数据已接收：{HexHelper.ByteToHexStr(",", preReceivedData)}");
                        #endif

                        bool isDataReceivedCheckSuccess = preReceivedData[0] == 0x40 && preReceivedData[7] == 0x0d || preReceivedData[6] == Check_Xor(preReceivedData, 6);
						if (!isDataReceivedCheckSuccess)
						{
							preReceivedData[5] = ERR_CMD_DATA_ERR;
						}
					}

				}
				catch (Exception ex)
				{

					LogUtils.Error($"【线程名:{Thread.CurrentThread.ManagedThreadId.ToString()},命令uuid：{preUUID}】串口接收时异常：{ex.Message}");

				}
				finally
				{
					//接收到返回信息，最后关闭串口
					OnlyCloseClientConn(clientConn);

					//解锁
					m.Set();
				}

			});


			//发送命令
			var sendData = cmdFunc.Invoke();
			clientConn.Write(sendData, 0, sendData.Length);

#if SERIALPORTLOG
            LogUtils.Debug($"【线程名:{Thread.CurrentThread.ManagedThreadId.ToString()},命令uuid：{preUUID}】 串口数据已发送：{HexHelper.ByteToHexStr(",", sendData)}");
#endif

            //阻塞当前线程，最长5秒
            var isTimeout = !m.WaitOne(new TimeSpan(0, 0, 5));
			if (isTimeout)
			{
				//超时关闭当前串口
				OnlyCloseClientConn(clientConn);
				preReceivedData[5] = ERR_CMD_TIMEOUT;
				LogUtils.Error($"【线程名:{Thread.CurrentThread.ManagedThreadId.ToString()},命令uuid：{preUUID}】串口接收时超时");


			}

			//返回收集结果
			return preReceivedData;
		}

		/// <summary>
		/// 初始化持久的串口连接
		/// </summary>
		/// <param name="com">串口信息：如COM2</param>
		/// <param name="isConnect">是否连接串口成功</param>
		/// <returns></returns>
		private static SerialPort InitClientConn(out bool isConnect)
		{
			isConnect = true;

			if (preSerialPort == null)
			{
				preSerialPort = new SerialPort
				{
					BaudRate = 9600,
					PortName = ApplicationState.GetMVeinCOM(), //静脉串口
					DataBits = 8,
					WriteBufferSize = 4096,
					ReadBufferSize = 8192
				};
			}

			try
			{
				if (!preSerialPort.IsOpen)
				{
					preSerialPort.Open();
				}
			}
			catch (Exception ex)
			{
				LogUtils.Error("串口创建错误：" + ex.ToString());
				isConnect = false;
			}

			isConnect = preSerialPort.IsOpen;

			return preSerialPort;
		}

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
				BaudRate = 9600,
				PortName = com, //静脉串口
				DataBits = 8
			};
			try
			{
				comSerialPort.Open();
			}
			catch (Exception ex)
			{
				LogUtils.Error("串口创建错误：" + ex.ToString());
				isConnect = false;
			}

			isConnect = comSerialPort.IsOpen;

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
				PortName = com, //静脉串口
				DataBits = 8,
			};
			try
			{
				comSerialPort.Open();
			}
			catch (Exception ex)
			{
				LogUtils.Error("串口创建错误："+ex.ToString());
				isConnect = false;
			}

			return comSerialPort;
		}

		/// <summary>
		/// 创建串口连接
		/// </summary>
		/// <param name="serialPort">串口对象</param>
		/// <returns></returns>
		private static bool CloseClientConn(SerialPort serialPort)
		{

			bool isClose = true;
			if (serialPort != null && serialPort.IsOpen)
			{
				try
				{
					serialPort.Close();
					serialPort.Dispose();
				}
				catch (Exception ex)
				{
					LogUtils.Error("串口关闭错误：" + ex.ToString());
					isClose = false;
				}
			}
			return isClose;
		}


		/// <summary>
		/// 创建串口连接
		/// </summary>
		/// <param name="serialPort">串口对象</param>
		/// <returns></returns>
		private static bool OnlyCloseClientConn(SerialPort serialPort)
		{

			bool isClose = true;
			if (serialPort != null && serialPort.IsOpen)
			{
				try
				{
					serialPort.Close();
				}
				catch (Exception ex)
				{
					LogUtils.Error("串口关闭错误：" + ex.ToString());
					isClose = false;
				}
			}
			return isClose;
		}

		



		public static void TestGetLockerData(object sender, ElapsedEventArgs elapsed)
		{
			Console.ReadKey();
			System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
			watch.Start();  //开始监视代码运行时间
			
			watch.Stop();  //停止监视
			TimeSpan timespan = watch.Elapsed;  //获取当前实例测量得出的总时间
			LogUtils.Debug($"打开窗口代码执行时间：{timespan.TotalMilliseconds}(毫秒)");  //总毫秒数
			Console.ReadKey();
		}

	
	
	}
}
