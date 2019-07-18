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

            tbMCabName.Text = ApplicationState.GetValue<string>((int)ApplicationKey.CodeMCab) ;
            tbSCabName.Text = ApplicationState.GetValue<string>((int)ApplicationKey.CodeSCab);

            MLockerCB.SelectedItem = ApplicationState.GetValue<string>((int)ApplicationKey.COM_MLocker);
            SLockerCB.SelectedItem = ApplicationState.GetValue<string>((int)ApplicationKey.COM_SLocker);

            MrfidCB.SelectedItem = ApplicationState.GetValue<string>((int)ApplicationKey.COM_MRFid);
            SrfidCB.SelectedItem = ApplicationState.GetValue<string>((int)ApplicationKey.COM_SRFid);

            MVeinCB.SelectedItem = ApplicationState.GetValue<string>((int)ApplicationKey.COM_MVein);

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
            ApplicationState.SetValue((int)ApplicationKey.CodeMCab, tbMCabName.Text);
            ApplicationState.SetValue((int)ApplicationKey.CodeSCab, tbSCabName.Text);

            ApplicationState.SetValue((int)ApplicationKey.COM_MLocker, MLockerCB.SelectedItem);
            ApplicationState.SetValue((int)ApplicationKey.COM_SLocker, SLockerCB.SelectedItem);

            ApplicationState.SetValue((int)ApplicationKey.COM_MRFid, MrfidCB.SelectedItem);
            ApplicationState.SetValue((int)ApplicationKey.COM_SRFid, SrfidCB.SelectedItem);

            ApplicationState.SetValue((int)ApplicationKey.COM_MVein, MVeinCB.SelectedItem);
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
