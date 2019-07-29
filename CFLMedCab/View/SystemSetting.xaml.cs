using CFLMedCab.Controls;
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

            tbHouseName.Text = ApplicationState.GetValue<string>((int)ApplicationKey.HouseId);
            tbEquipName.Text = ApplicationState.GetValue<string>((int)ApplicationKey.EquipId);

            tbMCabName.Text = ApplicationState.GetValue<string>((int)ApplicationKey.CodeMCab) ;
            tbSCabName.Text = ApplicationState.GetValue<string>((int)ApplicationKey.CodeSCab);

            //MLockerCB.SelectedItem = ApplicationState.GetValue<string>((int)ApplicationKey.COM_MLocker);
            //SLockerCB.SelectedItem = ApplicationState.GetValue<string>((int)ApplicationKey.COM_SLocker);
            MLockerCB.SelectedItem = ApplicationState.GetMLockerCOM();
            SLockerCB.SelectedItem = ApplicationState.GetSLockerCOM();

            //MrfidCB.SelectedItem = ApplicationState.GetValue<string>((int)ApplicationKey.COM_MRFid);
            //SrfidCB.SelectedItem = ApplicationState.GetValue<string>((int)ApplicationKey.COM_SRFid);
            MrfidCB.SelectedItem = ApplicationState.GetMRfidCOM();
            SrfidCB.SelectedItem = ApplicationState.GetSRfidCOM();

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
            ApplicationState.SetValue((int)ApplicationKey.EquipName, tbEquipName.Text);
            ApplicationState.SetValue((int)ApplicationKey.HouseName, tbHouseName.Text);

            ApplicationState.SetValue((int)ApplicationKey.CodeMCab, tbMCabName.Text);
            ApplicationState.SetValue((int)ApplicationKey.CodeSCab, tbSCabName.Text);

            //ApplicationState.SetValue((int)ApplicationKey.COM_MLocker, MLockerCB.SelectedItem);
            //ApplicationState.SetValue((int)ApplicationKey.COM_SLocker, SLockerCB.SelectedItem);
            ApplicationState.SetMLockerCOM(MLockerCB.SelectedItem.ToString());
            ApplicationState.SetSLockerCOM(SLockerCB.SelectedItem.ToString());

            //ApplicationState.SetValue((int)ApplicationKey.COM_MRFid, MrfidCB.SelectedItem);
            //ApplicationState.SetValue((int)ApplicationKey.COM_SRFid, SrfidCB.SelectedItem);
            ApplicationState.SetMRfidCOM(MrfidCB.SelectedItem.ToString());
            ApplicationState.SetSRfidCOM(SrfidCB.SelectedItem.ToString());

            //ApplicationState.SetValue((int)ApplicationKey.COM_MVein, MVeinCB.SelectedItem);
            ApplicationState.SetMVeinCOM(MVeinCB.SelectedItem.ToString());
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
