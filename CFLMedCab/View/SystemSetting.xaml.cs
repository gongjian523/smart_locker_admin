using CFLMedCab.Controls;
using CFLMedCab.Http.Bll;
using CFLMedCab.Http.Model;
using CFLMedCab.Http.Model.Base;
using CFLMedCab.Infrastructure;
using CFLMedCab.Infrastructure.DeviceHelper;
using CFLMedCab.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Xml;

namespace CFLMedCab.View
{
    /// <summary>
    /// CloseCabinet.xaml 的交互逻辑
    /// </summary>
    public partial class SystemSetting : UserControl
    {
        List<string> SerialComList { get; set; }

        public SystemSetting()
        {
            InitializeComponent();

            SerialComList = new List<string>();
            SerialComList.Add("COM1");
            SerialComList.Add("COM2");
            SerialComList.Add("COM3");
            SerialComList.Add("COM4");
            SerialComList.Add("COM5");
            SerialComList.Add("COM6");
            SerialComList.Add("COM7");
            SerialComList.Add("COM8");
            SerialComList.Add("COM9");

            tbHouseName.Text = ApplicationState.GetHouseName();
            tbEquipName.Text = ApplicationState.GetEquipName();

            tbMCabName.Text = ApplicationState.GetMCabName();
#if DUALCAB
            tbSCabName.Text = ApplicationState.GetSCabName();
#endif

            MLockerCB.SelectedItem = ApplicationState.GetMLockerCOM();
#if DUALCAB
            SLockerCB.SelectedItem = ApplicationState.GetSLockerCOM();
#endif

            MrfidCB.SelectedItem = ApplicationState.GetMRfidCOM();
#if DUALCAB
            SrfidCB.SelectedItem = ApplicationState.GetSRfidCOM();
#endif

            MVeinCB.SelectedItem = ApplicationState.GetMVeinCOM();

#if DUALCAB
#else
            lbSCabName.Visibility = Visibility.Hidden;
            tbSCabName.Visibility = Visibility.Hidden;

            tbSrfid.Visibility = Visibility.Hidden;
            SrfidCB.Visibility = Visibility.Hidden;

            tbSLocker.Visibility = Visibility.Hidden;
            SLockerCB.Visibility = Visibility.Hidden;
#endif
        }

        private void onSave(object sender, RoutedEventArgs e)
        {
			ApplicationState.SetEquipName(tbEquipName.Text.ToString());
            ApplicationState.SetHouseName(tbHouseName.Text.ToString());
			
			ApplicationState.SetMCabName(tbMCabName.Text.ToString());
#if DUALCAB
            ApplicationState.SetSCabName(tbSCabName.Text.ToString());
#endif

            ApplicationState.SetMLockerCOM(MLockerCB.SelectedItem.ToString());
#if DUALCAB
            ApplicationState.SetSLockerCOM(SLockerCB.SelectedItem.ToString());
#endif

            ApplicationState.SetMRfidCOM(MrfidCB.SelectedItem.ToString());
#if DUALCAB
            ApplicationState.SetSRfidCOM(SrfidCB.SelectedItem.ToString());
#endif

            ApplicationState.SetMVeinCOM(MVeinCB.SelectedItem.ToString());

			//xml文件回写
			XmlDocument xmlDoc = new XmlDocument();
			string xmlPath = $"{ApplicationState.GetProjectRootPath()}/MyProject.xml";
			xmlDoc.Load(xmlPath);
			XmlNode root = xmlDoc.SelectSingleNode("config");//指向根节点
			XmlNode device = root.SelectSingleNode("device");//指向设备节点

			device.SelectSingleNode("equip_name").InnerText = tbEquipName.Text.ToString();
			device.SelectSingleNode("house_name").InnerText = tbHouseName.Text.ToString();
			device.SelectSingleNode("mcab_name").InnerText = tbMCabName.Text.ToString();
#if DUALCAB
			device.SelectSingleNode("scab_name").InnerText = tbSCabName.Text.ToString();
#endif
			device.SelectSingleNode("mlocker_com").InnerText = MLockerCB.SelectedItem.ToString();
#if DUALCAB
			device.SelectSingleNode("slocker_com").InnerText = SLockerCB.SelectedItem.ToString();
#endif
			device.SelectSingleNode("mrfid_com").InnerText = MrfidCB.SelectedItem.ToString();
#if DUALCAB
            device.SelectSingleNode("srfid_com").InnerText = SrfidCB.SelectedItem.ToString();
#endif
			device.SelectSingleNode("mvein_com").InnerText = MVeinCB.SelectedItem.ToString();
	
			//获取线上id
			BaseData<string> bdEquip =  ConsumingBll.GetInstance().GetIdByName<Equipment>(tbEquipName.Text.ToString());
            BaseData<string> bdHouse = ConsumingBll.GetInstance().GetIdByStoreHouseCode<StoreHouse>(tbHouseName.Text.ToString());
            BaseData<string> bdMCab = ConsumingBll.GetInstance().GetIdByName<GoodsLocation>(tbMCabName.Text.ToString());
#if DUALCAB
            BaseData<string> bdSCab = ConsumingBll.GetInstance().GetIdByName<Equipment>(tbSCabName.Text.ToString());
#endif

            string err = "无法获取";


            if(bdEquip.code == 0)
            {
                ApplicationState.SetEquipId(bdEquip.body.objects[0]);
				device.SelectSingleNode("equip_id").InnerText = bdEquip.body.objects[0];
			}
			else
            {
                err += "设备ID、"+ bdEquip.message;
            }

            if (bdHouse.code == 0)
            {
                ApplicationState.SetHouseId(bdHouse.body.objects[0]);
				device.SelectSingleNode("house_id").InnerText = bdHouse.body.objects[0];
			}
            else
            {
                err += "库房ID、" + bdHouse.message;
            }

            if (bdMCab.code == 0)
            {
                ApplicationState.SetMCabId(bdMCab.body.objects[0]);
				device.SelectSingleNode("mcab_id").InnerText = bdMCab.body.objects[0];
			}
            else
            {
                err += "主货架ID、" + bdMCab.message;
            }

#if DUALCAB
            if (bdSCab.code == 0)
            {
                ApplicationState.SetSCabId(bdSCab.body.objects[0]);
				device.SelectSingleNode("scab_id").InnerText = bdSCab.body.objects[0];
			}
            else
            {
                err += "副货架ID、" + bdSCab.message;
            }
#endif
            if(err != "无法获取")
            {
                MessageBox.Show(err.Remove(err.Length - 1) + "!", "温馨提示", MessageBoxButton.OK);
            }

			//节点修改值保存
			xmlDoc.Save(xmlPath);


		}

        private void onItemChanged(object sender, RoutedEventArgs e)
        {
            
        }

        private void comboBoxSizeType_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBox box = sender as ComboBox;
            box.ItemsSource = SerialComList;
        }

        private void onItemChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
