﻿using CFLMedCab.Controls;
using CFLMedCab.Http.Bll;
using CFLMedCab.Http.Helper;
using CFLMedCab.Http.Model;
using CFLMedCab.Infrastructure;
using CFLMedCab.Infrastructure.BootUpHelper;
using CFLMedCab.Infrastructure.DeviceHelper;
using CFLMedCab.Infrastructure.ToolHelper;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using System.Xml;

namespace CFLMedCab
{
	/// <summary>
	/// App.xaml 的交互逻辑
	/// </summary>
	public partial class App : Application
	{
		public EventWaitHandle ProgramStarted { get; set; }

		protected override void OnStartup(StartupEventArgs e)
		{
			ProgramStarted = new EventWaitHandle(false, EventResetMode.AutoReset, "乘法云链", out bool createNew);

			if (!createNew)
			{
				MessageBox.Show("已有一个程序实例运行");
				Current.Shutdown();
				Environment.Exit(0);
			}

			base.OnStartup(e);

			//控制台信息展示
			ConsoleManager.Show();

			Console.ReadKey();
            
			// 注册Application_Error（全局捕捉异常）
			DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(App_DispatcherUnhandledException);

			//初始化日志
			LogUtils.InitLog4Net();

			//隐藏工具类
			Taskbar.HideTask(true);

			//开启启动
			BootUpHelper.GetInstance().SetMeAutoStart();

			//加载系统配置文件
			//Console.ReadKey();
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load($"{ApplicationState.GetProjectRootPath()}/MyProject.xml");
			XmlNode root = xmlDoc.SelectSingleNode("config");//指向根节点
			XmlNode device = root.SelectSingleNode("device");//指向设备节点

			ApplicationState.SetUserInfo(new User());
			//设置初始化goodsInfo
			ApplicationState.SetInitGoodsInfo(new HashSet<CommodityEps>());

			ApplicationState.SetEquipName(device.SelectSingleNode("equip_name").InnerText);
			ApplicationState.SetEquipId(device.SelectSingleNode("equip_id").InnerText);

			ApplicationState.SetHouseName(device.SelectSingleNode("house_name").InnerText);
			ApplicationState.SetHouseId(device.SelectSingleNode("house_id").InnerText);

			ApplicationState.SetMCabName(device.SelectSingleNode("mcab_name").InnerText);
			ApplicationState.SetMCabId(device.SelectSingleNode("mcab_id").InnerText);
#if DUALCAB
            ApplicationState.SetSCabName(device.SelectSingleNode("scab_name").InnerText);
            ApplicationState.SetSCabId(device.SelectSingleNode("scab_id").InnerText);
#endif
			ApplicationState.SetMLockerCOM(device.SelectSingleNode("mlocker_com").InnerText); //"COM2"
			ApplicationState.SetSLockerCOM(device.SelectSingleNode("slocker_com").InnerText); //"COM5"

			ApplicationState.SetMRfidCOM(device.SelectSingleNode("mrfid_com").InnerText); //"COM1"
			ApplicationState.SetSRfidCOM(device.SelectSingleNode("srfid_com").InnerText); //"COM4"

			ApplicationState.SetMVeinCOM(device.SelectSingleNode("mvein_com").InnerText); //"COM9"

            LogUtils.Debug("App config initial...");

			#region 处理开机（即应用启动时）需要对比库存变化上传的逻辑

			//获取当前机柜所有商品数据
			HashSet<CommodityEps> currentCommodityEps = RfidHelper.GetEpcDataJson(out bool isGetSuccess);

			//判断是否是初次使用本地库存上次，如果是则不上传
			bool isInitLocalCommodityEpsInfo = CommodityCodeBll.GetInstance().isInitLocalCommodityEpsInfo();

			if (isGetSuccess && !isInitLocalCommodityEpsInfo) {

				//获取数据库记录的以前所有商品数据
				HashSet<CommodityEps> lastCommodityEps = CommodityCodeBll.GetInstance().GetLastLocalCommodityEpsInfo();

				//比对
				List<CommodityCode> commodityCodeList = CommodityCodeBll.GetInstance().GetCompareSimpleCommodity(lastCommodityEps, currentCommodityEps);

				//有不同的情况，则需要处理上传逻辑
				if (commodityCodeList != null && commodityCodeList.Count > 0)
				{
					//根据商品码集合获取完整商品属性集合(已对比后结果)
					var bdCommodityCodeList = CommodityCodeBll.GetInstance().GetCommodityCode(commodityCodeList);

					var checkbdCommodityCodeList = HttpHelper.GetInstance().ResultCheck(bdCommodityCodeList, out bool isSuccess);

					if (isSuccess)
					{
						//按照类似无单一般领用的方式（故障领用）
						var bdBasePostData = ConsumingBll.GetInstance().SubmitConsumingChangeWithoutOrder(bdCommodityCodeList, ConsumingOrderType.故障领用);

						//校验是否含有数据
						HttpHelper.GetInstance().ResultCheck(bdBasePostData, out bool isSuccess1);
						if (!isSuccess1)
						{
							LogUtils.Error("提交故障领用结果失败！" + bdBasePostData.message);
						}
						else
						{
							LogUtils.Info("提交故障领用结果成功！" + bdBasePostData.message);
						}
					}
					else
					{
						LogUtils.Info("提交故障领用结果成功！");
					}
				}

			}

			#endregion

		}

		//全局异常处理逻辑
		void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			//处理完后，我们需要将Handler=true表示已此异常已处理过
			LogUtils.Error(e.Exception.Message + "\r\n" + e.Exception.StackTrace);
			e.Handled = true;
			MessageBox.Show(e.Exception.Message + "\r\n" + e.Exception.StackTrace, "系统信息");
		}

	}
}
