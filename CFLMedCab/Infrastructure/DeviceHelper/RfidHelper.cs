using CFLMedCab.Http.Model;
using CFLMedCab.Test;
using GDotnet.Reader.Api.DAL;
using GDotnet.Reader.Api.Protocol.Gx;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Timers;
using System.Linq;
using System.Windows;
using Newtonsoft.Json;
using CFLMedCab.Infrastructure.ToolHelper;
using System.Threading.Tasks;

/// <summary>
/// rfid工具类
/// </summary>
namespace CFLMedCab.Infrastructure.DeviceHelper
{
	public class RfidHelper
	{

		/// <summary>
		/// 保持子线程阻塞主线程
		/// </summary>
		private static List<ManualResetEvent> manualEvents = new List<ManualResetEvent>();

		/// <summary>
		/// 创建串口连接
		/// </summary>
		/// <param name="comInfo">串口和波特率信息：如COM1:115200</param>
		/// <param name="isConnect">是否连接串口成功</param>
		/// <returns></returns>
		private static GClient CreateClientConn(string comInfo, out bool isConnect) {
			GClient clientConn = new GClient();
			isConnect = clientConn.OpenSerial(comInfo, 3000, out eConnectionAttemptEventStatusType status);
			return clientConn;
		}

		/// <summary>
		/// 创建串口连接
		/// </summary>
		/// <param name="com">串口</param>
		/// <param name="baudRate">波特率</param>
		/// <param name="isConnect">是否连接串口成功</param>
		/// <returns></returns>
		private static GClient CreateClientConn(string com, string baudRate, out bool isConnect)
		{
			GClient clientConn = new GClient();
			isConnect = clientConn.OpenSerial(com+":"+baudRate, 3000, out eConnectionAttemptEventStatusType status);
			return clientConn;
		}


        /// <summary>
        /// 创建串口连接
        /// </summary>
        /// <param name="com">串口</param>
        /// <param name="baudRate">波特率</param>
        /// <returns></returns>
        private static GClient CreateClientConn(string com, string baudRate)
        {
            GClient clientConn = new GClient();
            bool isConnect = clientConn.OpenSerial(com + ":" + baudRate, 3000, out eConnectionAttemptEventStatusType status);
            return isConnect ? clientConn : null;
        }

        /// <summary>
        /// 消息发送和数据处理
        /// </summary>
        /// <param name="clientConn"></param>
        /// <param name="com"></param>
        /// <returns></returns>
        private static HashSet<string> DealComData(GClient clientConn, string com, out bool isGetSuccess)
		{
			/// mutex互斥锁，用于人为阻塞当前线程
			ManualResetEvent manualResetEvent = new ManualResetEvent(false);
			manualEvents.Add(manualResetEvent);

			DelegateGetMsg delegateGetMsg = new DelegateGetMsg(com, clientConn, manualResetEvent);

			// 订阅标签上报事件
			clientConn.OnEncapedTagEpcLog += new delegateEncapedTagEpcLog(delegateGetMsg.OnEncapedTagEpcLog);
			clientConn.OnEncapedTagEpcOver += new delegateEncapedTagEpcOver(delegateGetMsg.OnEncapedTagEpcOver);
			
			//停止指令，空闲态
			SendSynBaseStopMsg(clientConn, out isGetSuccess);

			//功率配置, 将 4 个天线功率都设置为 30dBm.
			SendSynBaseSetPowerMsg(clientConn, out isGetSuccess);

			//4个天线读卡, 读取EPC数据区以及TID数据区
			SendSynBaseInventoryEpcMsg(clientConn, out isGetSuccess);

			//返回收集结果
			return delegateGetMsg.GetDelegateMsg();

			//停止指令，空闲态
			//SendSynBaseStopMsg(clientConn);

		}

		/// <summary>
		/// 停止指令，空闲态(当读写器处于读卡状态时，所有配置消息将无法发送，必须发送停止指令)
		/// </summary>
		/// <param name="clientConn">连接</param>
		private static void SendSynBaseStopMsg(GClient clientConn, out bool isGetSuccess) {
			// 停止指令，空闲态
			MsgBaseStop msgBaseStop = new MsgBaseStop();
			clientConn.SendSynMsg(msgBaseStop);
			if (0 == msgBaseStop.RtCode)
			{
				isGetSuccess = true;
				LogUtils.Debug("Stop successful.");
			}
			else
			{
				LogUtils.Debug("Stop error.");
				isGetSuccess = false;
			}
	
		}

		/// <summary>
		/// 功率配置, 将4个天线功率都设置为30dBm.
		/// </summary>
		/// <param name="clientConn">连接</param>
		private static void SendSynBaseSetPowerMsg(GClient clientConn, out bool isGetSuccess)
		{
			// 功率配置, 将4个天线功率都设置为30dBm.
			MsgBaseSetPower msgBaseSetPower = new MsgBaseSetPower
			{
				DicPower = new Dictionary<byte, byte>()
				{
					{1, 30},
					{2, 30},
					{3, 30},
					{4, 30}
				}
			};
			clientConn.SendSynMsg(msgBaseSetPower);
			if (0 == msgBaseSetPower.RtCode)
			{
				LogUtils.Debug("Power configuration successful.");
				isGetSuccess = true;
			}
			else
			{
				LogUtils.Debug("Power configuration error.");
				isGetSuccess = false;

			}
		
		}

		/// <summary>
		/// 4个天线读卡, 读取EPC数据区以及TID数据区
		/// </summary>
		/// <param name="clientConn">连接</param>
		private static void SendSynBaseInventoryEpcMsg(GClient clientConn, out bool isGetSuccess)
		{
			// 4个天线读卡, 读取EPC数据区以及TID数据区
			MsgBaseInventoryEpc msgBaseInventoryEpc = new MsgBaseInventoryEpc
			{
				AntennaEnable = (eAntennaNo._1 | eAntennaNo._2 | eAntennaNo._3 | eAntennaNo._4),
				InventoryMode = (byte)eInventoryMode.Single,
				// tid参数
				ReadTid = new ParamEpcReadTid()
			};
			msgBaseInventoryEpc.ReadTid.Mode = (byte)eParamTidMode.Auto;
			msgBaseInventoryEpc.ReadTid.Len = 6;
			clientConn.SendSynMsg(msgBaseInventoryEpc);
			if (0 == msgBaseInventoryEpc.RtCode)
			{
				isGetSuccess = true;
				LogUtils.Debug("Inventory epc successful.");
			}
			else
			{
				LogUtils.Debug("Inventory epc error.");
				isGetSuccess = false;
			}

		}

        public static HashSet<CommodityEps> GetEpcDataJsonInventory(out bool isGetSuccess)
        {
            isGetSuccess = true;
            string com1 = ApplicationState.GetMRfidCOM();
            var ret = new HashSet<CommodityEps>()
            {
                new CommodityEps
                {
                    CommodityCodeName = "RF00000327",
                    EquipmentId = ApplicationState.GetEquipId(),
                    EquipmentName = ApplicationState.GetEquipName(),
                    StoreHouseId = ApplicationState.GetHouseId(),
                    StoreHouseName = ApplicationState.GetHouseName(),
                    GoodsLocationName = ApplicationState.GetLocCodeByRFidCom(com1),
                    GoodsLocationId = ApplicationState.GetLocIdByRFidCom(com1)
                },
                new CommodityEps
                {
                    CommodityCodeName = "RF00000333",
                    EquipmentId = ApplicationState.GetEquipId(),
                    EquipmentName = ApplicationState.GetEquipName(),
                    StoreHouseId = ApplicationState.GetHouseId(),
                    StoreHouseName = ApplicationState.GetHouseName(),
                    GoodsLocationName = ApplicationState.GetLocCodeByRFidCom(com1),
                    GoodsLocationId = ApplicationState.GetLocIdByRFidCom(com1)
                },
                new CommodityEps
                {
                    CommodityCodeName = "RF00000324",
                    EquipmentId = ApplicationState.GetEquipId(),
                    EquipmentName = ApplicationState.GetEquipName(),
                    StoreHouseId = ApplicationState.GetHouseId(),
                    StoreHouseName = ApplicationState.GetHouseName(),
                    GoodsLocationName = ApplicationState.GetLocCodeByRFidCom(com1),
                    GoodsLocationId = ApplicationState.GetLocIdByRFidCom(com1)
                }
            };
            return ret;
        }

		/// <summary>
		/// 根据eps json获取eps对象数据
		/// </summary>
		/// <param name="isGetSuccess"></param>
		/// <returns></returns>
		public static HashSet<CommodityEps> GetEpcDataJson(out bool isGetSuccess)
		{

			isGetSuccess = true;

			//string com1 = "COM1";
            string com1 = ApplicationState.GetMRfidCOM();
            HashSet<string> com1HashSet = new HashSet<string>();

            string log = "";

#if DUALCAB
            //string com4 = "COM4";
            string com4 = ApplicationState.GetSRfidCOM();
            HashSet<string> com4HashSet = new HashSet<string>();
#endif
            HashSet<CommodityEps> currentEpcDataHs = new HashSet<CommodityEps>();

            //TODO:需要补充id
            GClient com1ClientConn = CreateClientConn(com1, "115200", out bool isCom1Connect);
			if (isCom1Connect)
			{
				com1HashSet = DealComData(com1ClientConn, com1, out isGetSuccess);
			}
			else
			{
				isGetSuccess = false;
			}

#if DUALCAB
            GClient com4ClientConn = CreateClientConn(com4, "115200", out bool isCom4Connect);
			if (isCom4Connect)
			{
				com4HashSet = DealComData(com4ClientConn, com4, out isGetSuccess);
			}
			else
			{
				isGetSuccess = false;
			}
#endif

            WaitHandle.WaitAll(manualEvents.ToArray());
			manualEvents.Clear();

            //提取com1的标签epc，并组装
            foreach (string rfid in com1HashSet)
            {
                CommodityEps commodityEps = new CommodityEps
                {
                    CommodityCodeName = $"RF{rfid.Substring(rfid.Length - 8)}",
                    EquipmentId = ApplicationState.GetEquipId(),
                    EquipmentName = ApplicationState.GetEquipName(),
                    StoreHouseId = ApplicationState.GetHouseId(),
                    StoreHouseName = ApplicationState.GetHouseName(),
                    GoodsLocationName = ApplicationState.GetLocCodeByRFidCom(com1),
                    GoodsLocationId = ApplicationState.GetLocIdByRFidCom(com1)
                };

                currentEpcDataHs.Add(commodityEps);
                LogUtils.Debug(commodityEps.CommodityCodeName + commodityEps.CommodityName);
                log += commodityEps.CommodityCodeName + " ";
            }

#if DUALCAB
            //提取com4的标签epc，并组装
            foreach (string rfid in com4HashSet)
            {
                CommodityEps commodityEps = new CommodityEps
                {
                    CommodityCodeName = $"RF{rfid.Substring(rfid.Length - 8)}",
                    EquipmentId = ApplicationState.GetEquipId(),
                    EquipmentName = ApplicationState.GetEquipName(),
                    StoreHouseId = ApplicationState.GetHouseId(),
                    StoreHouseName = ApplicationState.GetHouseName(),
                    GoodsLocationName = ApplicationState.GetLocCodeByRFidCom(com1),
                    GoodsLocationId = ApplicationState.GetLocIdByRFidCom(com1)
                };

                currentEpcDataHs.Add(commodityEps);
                LogUtils.Debug(commodityEps.CommodityCodeName + commodityEps.CommodityName);
                log += commodityEps.CommodityCodeName + " ";
            }
#endif

            Task.Factory.StartNew(a =>
            {
                LogUtils.Debug(log);
            }, log);

            LogUtils.Debug("RFID NUM:" + currentEpcDataHs.Count());
            return currentEpcDataHs;
		}

        /// <summary>
        /// 多柜模式的扫描函数，只扫描特定货柜的商品
        /// 根据eps json获取eps对象数据
        /// </summary>
        /// <param name="isGetSuccess"></param>
        /// <param name="listCom"></param>
        /// <returns></returns>
        public static HashSet<CommodityEps> GetEpcDataJson(out bool isGetSuccess, List<string> listCom)
        {
            isGetSuccess = true;

            List<GClient> listGClient = new List<GClient>();
            List<bool> listIsConnect = new List<bool>();
            List<HashSet<string>> listComHashSet = new List<HashSet<string>>();

            HashSet<CommodityEps> currentEpcDataHs = new HashSet<CommodityEps>();
            string log = "";

            for (int i = 0; i < listCom.Count(); i++)
            {
                listComHashSet.Add(new HashSet<string>());

                GClient clientConn = CreateClientConn(listCom[i], "115200");
                listGClient.Add(clientConn);

                if (listGClient[i] != null)
                {
                    listComHashSet[i] = DealComData(listGClient[i], listCom[i], out isGetSuccess);
                }
                else
                {
                    isGetSuccess = false;
                }
            }

            WaitHandle.WaitAll(manualEvents.ToArray());
            manualEvents.Clear();

            for (int i = 0; i < listCom.Count(); i++)
            {
                //提取标签epc，并组装
                foreach (string rfid in listComHashSet[i])
                {
                    CommodityEps commodityEps = new CommodityEps
                    {
                        CommodityCodeName = $"RF{rfid.Substring(rfid.Length - 8)}",
                        EquipmentId = ApplicationState.GetEquipId(),
                        EquipmentName = ApplicationState.GetEquipName(),
                        StoreHouseId = ApplicationState.GetHouseId(),
                        StoreHouseName = ApplicationState.GetHouseName(),
                        GoodsLocationName = ApplicationState.GetLocCodeByRFidCom(listCom[i]),
                        GoodsLocationId = ApplicationState.GetLocIdByRFidCom(listCom[i])
                    };

                    currentEpcDataHs.Add(commodityEps);
                    LogUtils.Debug(commodityEps.CommodityCodeName + commodityEps.CommodityName);
                    log += commodityEps.CommodityCodeName + " ";
                }
            }

            Task.Factory.StartNew(a =>
            {
                LogUtils.Debug(log);
            }, log);

            LogUtils.Debug("RFID NUM:" + currentEpcDataHs.Count());
            return currentEpcDataHs;
        }


        /// <summary>
        /// 新版epc数据获取，规则rf码扫描到的字段的后八位，前加RF。例：2019072800000001 =》 RF00000001
        /// </summary>
        /// <param name="isGetSuccess"></param>
        /// <returns></returns>
        public static Hashtable GetEpcDataNew(out bool isGetSuccess)
		{
			isGetSuccess = true;

			//string com1 = "COM1";
            string com1 = ApplicationState.GetMRfidCOM();
#if DUALCAB
            //string com4 = "COM4";
            string com4 = ApplicationState.GetSRfidCOM();
#endif

            Hashtable currentEpcDataHt = new Hashtable();

			GClient com1ClientConn = CreateClientConn(com1, "115200", out bool isCom1Connect);
			if (isCom1Connect)
			{
				currentEpcDataHt.Add(com1, DealComData(com1ClientConn, com1, out isGetSuccess));
			}
			else
			{
				isGetSuccess = false;
			}

#if DUALCAB
            GClient com4ClientConn = CreateClientConn(com4, "115200", out bool isCom4Connect);
			if (isCom4Connect)
			{
				currentEpcDataHt.Add(com4, DealComData(com4ClientConn, com4, out isGetSuccess));
			}
			else
			{
				isGetSuccess = false;
			}
#endif
            WaitHandle.WaitAll(manualEvents.ToArray());
			manualEvents.Clear();

			return currentEpcDataHt;

		}
#region
		public static void TestRFID(object sender, ElapsedEventArgs elapsed)
		{
			GClient clientConn = new GClient();
			//COM1  主柜rfid串口
			//COM4  副柜rfid串口  
			if (clientConn.OpenSerial("COM1:115200", 3000, out eConnectionAttemptEventStatusType status))
			//if (clientConn.OpenTcp("192.168.1.168:8160", 3000, out status))
			{
				// 订阅标签上报事件
				clientConn.OnEncapedTagEpcLog += new delegateEncapedTagEpcLog(OnEncapedTagEpcLog);
				clientConn.OnEncapedTagEpcOver += new delegateEncapedTagEpcOver(OnEncapedTagEpcOver);

				// 停止指令，空闲态
				MsgBaseStop msgBaseStop = new MsgBaseStop();
				clientConn.SendSynMsg(msgBaseStop);
				if (0 == msgBaseStop.RtCode)
				{
					LogUtils.Debug("Stop successful.");
				}
				else { LogUtils.Debug("Stop error."); }

				// 功率配置, 将4个天线功率都设置为30dBm.
				MsgBaseSetPower msgBaseSetPower = new MsgBaseSetPower
				{
					DicPower = new Dictionary<byte, byte>()
				{
					{1, 30},
					{2, 30},
					{3, 30},
					{4, 30}
				}
				};
				clientConn.SendSynMsg(msgBaseSetPower);
				if (0 == msgBaseSetPower.RtCode)
				{
					LogUtils.Debug("Power configuration successful.");
				}
				else { LogUtils.Debug("Power configuration error."); }
				LogUtils.Debug("Enter any character to start reading the tag.");
				Console.ReadKey();

				// 4个天线读卡, 读取EPC数据区以及TID数据区
				MsgBaseInventoryEpc msgBaseInventoryEpc = new MsgBaseInventoryEpc
				{
					AntennaEnable = (uint)(eAntennaNo._1 | eAntennaNo._2 | eAntennaNo._3 | eAntennaNo._4),
					InventoryMode = (byte)eInventoryMode.Inventory,
					// tid参数
					ReadTid = new ParamEpcReadTid()
				};
				msgBaseInventoryEpc.ReadTid.Mode = (byte)eParamTidMode.Auto;
				msgBaseInventoryEpc.ReadTid.Len = 6;
				clientConn.SendSynMsg(msgBaseInventoryEpc);
				if (0 == msgBaseInventoryEpc.RtCode)
				{
					LogUtils.Debug("Inventory epc successful.");
				}
				else { LogUtils.Debug("Inventory epc error."); }
				Console.ReadKey();

				// 停止读卡，空闲态
				clientConn.SendSynMsg(msgBaseStop);
				if (0 == msgBaseStop.RtCode)
				{
					LogUtils.Debug("Stop successful.");
				}
				else { LogUtils.Debug("Stop error."); }
			}
			else
			{
				LogUtils.Debug("Connect failure.");
			}
			Console.ReadKey();
			clientConn.Close();
		}

		public static void OnEncapedTagEpcLog(EncapedLogBaseEpcInfo msg)
		{
			// 回调内部如有阻塞，会影响API正常使用
			// 标签回调数量较多，请将标签数据先缓存起来再作业务处理
			if (null != msg && 0 == msg.logBaseEpcInfo.Result)
			{
				LogUtils.Debug(msg.logBaseEpcInfo.ToString());
			}
		}
		

		public static void OnEncapedTagEpcOver(EncapedLogBaseEpcOver msg)
		{
			if (null != msg)
			{
				LogUtils.Debug("Epc log over.");
			}
		}

#endregion

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
			private readonly GClient currentClientConn;

			/// <summary>
			/// ManualResetEvent，用于人为阻塞当前线程
			/// </summary>
			private readonly ManualResetEvent manualResetEvent;

			/// <summary>
			/// 存储当前收集到的消息
			/// </summary>
			private readonly HashSet<string> msgHs = new HashSet<string>();
 

			public DelegateGetMsg(String com, GClient currentClientConn, ManualResetEvent manualResetEvent) 
			{
				this.com = com;
				this.currentClientConn = currentClientConn;
				this.manualResetEvent = manualResetEvent;
			}

			/// <summary>
			/// EncapedLogBaseEpcInfo 回调消息
			/// </summary>
			public void OnEncapedTagEpcLog(EncapedLogBaseEpcInfo msg)
			{
			// 回调内部如有阻塞，会影响API正常使用
			// 标签回调数量较多，请将标签数据先缓存起来再作业务处理
				if (null != msg && 0 == msg.logBaseEpcInfo.Result)
				{
					LogUtils.Debug(msg.logBaseEpcInfo.ToString());
					msgHs.Add(msg.logBaseEpcInfo.Epc);

				}
			}

			/// <summary>
			/// EncapedLogBaseEpcOver 回调消息
			/// </summary>
			public void OnEncapedTagEpcOver(EncapedLogBaseEpcOver msg)
			{
				if (null != msg)
				{
					LogUtils.Debug("Epc log over.");
					//发送停止状态；并停止当前连接
					//SendSynBaseStopMsg(currentClientConn);
					currentClientConn.Close();
					manualResetEvent.Set();
				}	
			}

			/// <summary>
			/// EncapedLogBaseEpcInfo 回调消息
			/// </summary>
			public void OnTagEpcLog(LogBaseEpcInfo msg)
			{
				// 回调内部如有阻塞，会影响API正常使用
				// 标签回调数量较多，请将标签数据先缓存起来再作业务处理
				if (null != msg && 0 == msg.Result)
				{
					LogUtils.Debug(msg.ToString());
					msgHs.Add(msg.Epc);

				}
			}

			/// <summary>
			/// EncapedLogBaseEpcOver 回调消息
			/// </summary>
			public void OnEpcOver(LogBaseEpcOver msg)
			{
				if (null != msg)
				{
					LogUtils.Debug("Epc log over.");
					//发送停止状态；并停止当前连接
					//SendSynBaseStopMsg(currentClientConn);
					currentClientConn.Close();
					manualResetEvent.Set();
				}
			}

			/// <summary>
			/// 返回当前消息 回调消息
			/// </summary>
			public HashSet<string> GetDelegateMsg()
			{
				return msgHs;
			}
		}
	}
}
