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

            //tbHouseName.Text = ApplicationState.GetValue<string>((int)ApplicationKey.HouseId);
            //tbEquipName.Text = ApplicationState.GetValue<string>((int)ApplicationKey.EquipId);
            tbHouseName.Text = ApplicationState.GetHouseName();
            tbEquipName.Text = ApplicationState.GetEquipName();

            //tbMCabName.Text = ApplicationState.GetValue<string>((int)ApplicationKey.MCabName) ;
            //tbSCabName.Text = ApplicationState.GetValue<string>((int)ApplicationKey.SCabName);
            tbMCabName.Text = ApplicationState.GetMCabName();
#if DUALCAB
            tbSCabName.Text = ApplicationState.GetSCabName();
#endif

            //MLockerCB.SelectedItem = ApplicationState.GetValue<string>((int)ApplicationKey.COM_MLocker);
            //SLockerCB.SelectedItem = ApplicationState.GetValue<string>((int)ApplicationKey.COM_SLocker);
            MLockerCB.SelectedItem = ApplicationState.GetMLockerCOM();
#if DUALCAB
            SLockerCB.SelectedItem = ApplicationState.GetSLockerCOM();
#endif

            //MrfidCB.SelectedItem = ApplicationState.GetValue<string>((int)ApplicationKey.COM_MRFid);
            //SrfidCB.SelectedItem = ApplicationState.GetValue<string>((int)ApplicationKey.COM_SRFid);
            MrfidCB.SelectedItem = ApplicationState.GetMRfidCOM();
#if DUALCAB
            SrfidCB.SelectedItem = ApplicationState.GetSRfidCOM();
#endif

            //MVeinCB.SelectedItem = ApplicationState.GetValue<string>((int)ApplicationKey.COM_MVein);
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
            //ApplicationState.SetValue((int)ApplicationKey.EquipName, tbEquipName.Text);
            //ApplicationState.SetValue((int)ApplicationKey.HouseName, tbHouseName.Text);
            ApplicationState.SetEquipName(tbEquipName.Text.ToString());
            ApplicationState.SetHouseName(tbHouseName.Text.ToString());

            //ApplicationState.SetValue((int)ApplicationKey.MCabName, tbMCabName.Text);
            //ApplicationState.SetValue((int)ApplicationKey.SCabName, tbSCabName.Text);
            ApplicationState.SetMCabName(tbMCabName.Text.ToString());
#if DUALCAB
            ApplicationState.SetSCabName(tbSCabName.Text.ToString());
#endif

            //ApplicationState.SetValue((int)ApplicationKey.COM_MLocker, MLockerCB.SelectedItem);
            //ApplicationState.SetValue((int)ApplicationKey.COM_SLocker, SLockerCB.SelectedItem);
            ApplicationState.SetMLockerCOM(MLockerCB.SelectedItem.ToString());
#if DUALCAB
            ApplicationState.SetSLockerCOM(SLockerCB.SelectedItem.ToString());
#endif
            //ApplicationState.SetValue((int)ApplicationKey.COM_MRFid, MrfidCB.SelectedItem);
            //ApplicationState.SetValue((int)ApplicationKey.COM_SRFid, SrfidCB.SelectedItem);
            ApplicationState.SetMRfidCOM(MrfidCB.SelectedItem.ToString());
#if DUALCAB
            ApplicationState.SetSRfidCOM(SrfidCB.SelectedItem.ToString());
#endif

            //ApplicationState.SetValue((int)ApplicationKey.COM_MVein, MVeinCB.SelectedItem);
            ApplicationState.SetMVeinCOM(MVeinCB.SelectedItem.ToString());

            BaseData<string> bdEquip =  ConsumingBll.GetInstance().GetIdByName<Equipment>(tbEquipName.Text.ToString());
            BaseData<string> bdHouse = ConsumingBll.GetInstance().GetIdByName<StoreHouse>(tbHouseName.Text.ToString());
            BaseData<string> bdMCab = ConsumingBll.GetInstance().GetIdByName<Equipment>(tbMCabName.Text.ToString());
#if DUALCAB
            BaseData<string> bdSCab = ConsumingBll.GetInstance().GetIdByName<Equipment>(tbSCabName.Text.ToString());
#endif

            string err = "无法获取";

            if(bdEquip.code == 0)
            {
                ApplicationState.SetEquipId(bdEquip.body.objects[0]);
            }
            else
            {
                err += "设备ID、";
            }

            if (bdHouse.code == 0)
            {
                ApplicationState.SetHouseId(bdHouse.body.objects[0]);
            }
            else
            {
                err += "库房ID、";
            }

            if (bdMCab.code == 0)
            {
                ApplicationState.SetMCabId(bdMCab.body.objects[0]);
            }
            else
            {
                err += "主货架ID、";
            }

#if DUALCAB
            if (bdSCab.code == 0)
            {
                ApplicationState.SetSCabId(bdSCab.body.objects[0]);
            }
            else
            {
                err += "副货架ID、";
            }
#endif
            if(err != "无法获取")
            {
                MessageBox.Show(err.Remove(err.Length - 1) + "!", "温馨提示", MessageBoxButton.OK);
            }
            return;
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
