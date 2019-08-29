﻿using CFLMedCab.Controls;
using CFLMedCab.Http.Model;
using CFLMedCab.Infrastructure;
using CFLMedCab.Infrastructure.BootUpHelper;
using CFLMedCab.Infrastructure.ToolHelper;
using CFLMedCab.Model;
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
			ApplicationState.SetGoodsInfo(new HashSet<CommodityEps>());

			ApplicationState.SetEquipName(device.SelectSingleNode("equip_name").InnerText);
			ApplicationState.SetEquipId(device.SelectSingleNode("equip_id").InnerText);

			ApplicationState.SetHouseName(device.SelectSingleNode("house_name").InnerText);
			ApplicationState.SetHouseId(device.SelectSingleNode("house_id").InnerText);

			ApplicationState.SetMVeinCOM(device.SelectSingleNode("mvein_com").InnerText); //"COM9"

            XmlNode goodsLocaiton = device.SelectSingleNode("goods_loction");//指向货柜节点

            List<Locations> list = new List<Locations>();
            if(goodsLocaiton != null)
            {
                XmlNodeList locationNodeList = goodsLocaiton.ChildNodes;//获取货柜节点下的所有location子节点
                foreach (XmlNode node in locationNodeList)
                {
                    list.Add(new Locations
                    {
                        Code = node.SelectSingleNode("location_name").InnerText,
                        Id = node.SelectSingleNode("location_id").InnerText,
                        LockerCom = node.SelectSingleNode("locker_com").InnerText,
                        RFCom = node.SelectSingleNode("rfid_com").InnerText
                    });
                }
            }
            ApplicationState.SetLocations(list);

            LogUtils.Debug("App config initial...");
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
