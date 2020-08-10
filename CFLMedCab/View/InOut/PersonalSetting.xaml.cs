using CFLMedCab.Controls;
using CFLMedCab.Http.Bll;
using CFLMedCab.Http.Helper;
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

namespace CFLMedCab.View.InOut
{
    /// <summary>
    /// CloseCabinet.xaml 的交互逻辑
    /// </summary>
    public partial class PersonalSetting : UserControl
    {
        List<string> SerialComList { get; set; }

        List<Locations> locationList { get; set; }

        string strEditingLocCode = "";

        public PersonalSetting()
        {
            InitializeComponent();

            UpdateEquipment();

            SerialComList = new List<string>();
            SerialComList.Add(" ");
            SerialComList.Add("COM1");
            SerialComList.Add("COM2");
            SerialComList.Add("COM3");
            SerialComList.Add("COM4");
            SerialComList.Add("COM5");
            SerialComList.Add("COM6");
            SerialComList.Add("COM7");
            SerialComList.Add("COM8");
            SerialComList.Add("COM9");

            locationList = ApplicationState.GetLocations();
            listView.DataContext = locationList;
        }

        private void UpdateEquipment()
        {
            BaseData<StoreHouse> bdSH = ConsumingBll.GetInstance().GetNameById<StoreHouse>(ApplicationState.GetHouseId(), out bool isSuccess);
            lbHouseName.Content = isSuccess ? bdSH.body.objects[0].name : "";
            lbHouseCode.Content = ApplicationState.GetHouseName();
            lbEquipCode.Content = ApplicationState.GetEquipName();
            lbVeinCom.Content = ApplicationState.GetMVeinCOM();
        }

        /// <summary>
        /// 点击设备信息中的编辑按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEditEquip(object sender, RoutedEventArgs e)
        {
            tbEquipCode.Text = lbEquipCode.Content.ToString();
            tbHouseCode.Text = lbHouseCode.Content.ToString();
            cbVein.SelectedItem = lbVeinCom.Content.ToString();

            equipView.Visibility = Visibility.Collapsed;
            eidtEquipView.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// 保存新的设备信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onSaveEditEquip(object sender, RoutedEventArgs e)
        {
            BaseData<string> bdEquip = ConsumingBll.GetInstance().GetIdByName<Equipment>(tbEquipCode.Text.ToString());
            BaseData<string> bdHouse = ConsumingBll.GetInstance().GetIdByStoreHouseCode<StoreHouse>(tbHouseCode.Text.ToString());

            if (bdEquip.code == 0 && bdHouse.code == 0)
            {
                ApplicationState.SetEquipName(tbEquipCode.Text.ToString());
                ApplicationState.SetEquipId(bdEquip.body.objects[0]);

                ApplicationState.SetHouseName(tbHouseCode.Text.ToString());
                ApplicationState.SetHouseId(bdHouse.body.objects[0]);

                ApplicationState.SetMVeinCOM(cbVein.SelectedItem.ToString());

                UpdateXMLEquip(tbEquipCode.Text.ToString(), bdEquip.body.objects[0], tbHouseCode.Text.ToString(),
                    bdHouse.body.objects[0], cbVein.SelectedItem.ToString());

                UpdateEquipment();

                MessageBox.Show("保存成功!", "温馨提示", MessageBoxButton.OK);
            }
            else
            {
                string err = "无法获取";

                if(bdHouse.code != 0)
                {
                    err += "库房ID、";
                }

                if (bdEquip.code != 0)
                {
                    err += "设备ID、";
                }

                MessageBox.Show(err.Remove(err.Length - 1) + "!", "温馨提示", MessageBoxButton.OK);
            }

            equipView.Visibility = Visibility.Visible;
            eidtEquipView.Visibility = Visibility.Collapsed;
        }

        private void onCancelEditEquip(object sender, RoutedEventArgs e)
        {
            equipView.Visibility = Visibility.Visible;
            eidtEquipView.Visibility = Visibility.Collapsed;
        }

        private void onAddGoodsLoc(object sender, RoutedEventArgs e)
        {
            if((sender as Button).Name == "btnAddLoc")
            {
                strEditingLocCode = "";

                tbLocationCode.Text = "";
                cbLocker.SelectedItem = " ";
                cbRFid.SelectedItem = " ";
            }
            else
            {
                strEditingLocCode = (string)(sender as Button).CommandParameter;

                Locations loc = locationList.Where(item => item.Code == strEditingLocCode).ToList().First();

                if(loc == null)
                {
                    MessageBox.Show("获取数据错误", "温馨提示", MessageBoxButton.OK);
                    return;
                }

                tbLocationCode.Text = loc.Code;
                cbLocker.SelectedItem = loc.LockerCom;
                cbRFid.SelectedItem = loc.RFCom;
            }

            eidtLocationView.Visibility = Visibility.Visible;
        }

        private void onSaveEditLocation(object sender, RoutedEventArgs e)
        {
            //新增
            if(strEditingLocCode == "")
            {
                if (tbLocationCode.Text == "")
                {
                    MessageBox.Show("请填写货柜编号！", "温馨提示", MessageBoxButton.OK);
                    return;
                }

                if (cbLocker.SelectedItem.ToString() == "" || cbLocker.SelectedItem.ToString() == " ")
                {
                    MessageBox.Show("请选择货柜锁串口！", "温馨提示", MessageBoxButton.OK);
                    return;
                }

                if (cbLocker.SelectedItem.ToString() == "" || cbLocker.SelectedItem.ToString() == " ")
                {
                    MessageBox.Show("请选择RF扫描仪串口！", "温馨提示", MessageBoxButton.OK);
                    return;
                }

                if (locationList.Where(item => item.Code == tbLocationCode.Text).Count() > 0)
                {
                    MessageBox.Show("此货柜已经添加！", "温馨提示", MessageBoxButton.OK);
                    return;
                }

                BaseData<string> bdLoc = ConsumingBll.GetInstance().GetIdByName<GoodsLocation>(tbLocationCode.Text.ToString());

                if(bdLoc.code != 0)
                {
                    MessageBox.Show("获取货柜ID失败！", "温馨提示", MessageBoxButton.OK);
                    return;
                }
                else
                {
                    Locations newLoc = new Locations
                    {
                        Code = tbLocationCode.Text,
                        Id = bdLoc.body.objects[0],
                        LockerCom = cbLocker.SelectedItem.ToString(),
                        RFCom = cbRFid.SelectedItem.ToString()
                    };

                    locationList.Add(newLoc);
                    ApplicationState.SetLocations(locationList);

                    AddXMLLoc(newLoc);

                    MessageBox.Show("保存成功!", "温馨提示", MessageBoxButton.OK);
                    listView.Items.Refresh();
                }
            }
            else
            //编辑
            {
                if (locationList.Where(item => item.Code == tbLocationCode.Text).Count() > 0 && strEditingLocCode != tbLocationCode.Text)
                {
                    MessageBox.Show("此货柜已经添加！", "温馨提示", MessageBoxButton.OK);
                    return;
                }

                BaseData<string> bdLoc = ConsumingBll.GetInstance().GetIdByName<GoodsLocation>(tbLocationCode.Text.ToString());

                if (bdLoc.code != 0)
                {
                    MessageBox.Show("获取货柜ID失败！", "温馨提示", MessageBoxButton.OK);
                    return;
                }

                Locations loc = new Locations {
                    Code = tbLocationCode.Text,
                    Id = bdLoc.body.objects[0],
                    LockerCom = cbLocker.SelectedItem.ToString(),
                    RFCom = cbRFid.SelectedItem.ToString()
                };

                locationList.ForEach(item => {
                    if (item.Code == strEditingLocCode)
                    {
                        item.Code = loc.Code;
                        item.Id = loc.Id;
                        item.LockerCom = loc.LockerCom;
                        item.RFCom = loc.RFCom;
                    }
                 });
                ApplicationState.SetLocations(locationList);

                EditXMLLoc(loc);

                MessageBox.Show("保存成功!", "温馨提示", MessageBoxButton.OK);
                listView.Items.Refresh();
            }

            eidtLocationView.Visibility = Visibility.Collapsed;
        }

        private void onCancelEditLocation(object sender, RoutedEventArgs e)
        {
            eidtLocationView.Visibility = Visibility.Collapsed;
        }

        private void onDelGoodsLoc(object sender, RoutedEventArgs e)
        {
            strEditingLocCode = (string)(sender as Button).CommandParameter;

            Locations loc = locationList.Where(item => item.Code == strEditingLocCode).ToList().First();

            if (loc == null)
            {
                MessageBox.Show("获取数据错误", "温馨提示", MessageBoxButton.OK);
                return;
            }

            locationList.Remove(loc);

            DeleteXMLLoc(loc);
            ApplicationState.SetLocations(locationList);

            listView.Items.Refresh();
        }

        private void UpdateXMLEquip(string equipName, string equipId, string houseName, string houseId, string veinCom)
        {
            //xml文件回写
            XmlDocument xmlDoc = new XmlDocument();
            string xmlPath = $"{ApplicationState.GetProjectRootPath()}/MyProject.xml";
            xmlDoc.Load(xmlPath);
            XmlNode root = xmlDoc.SelectSingleNode("config");//指向根节点
            XmlNode device = root.SelectSingleNode("device");//指向设备节点

            device.SelectSingleNode("equip_name").InnerText = equipName;
            device.SelectSingleNode("equip_id").InnerText = equipId;

            device.SelectSingleNode("house_name").InnerText = houseName;
            device.SelectSingleNode("house_id").InnerText = houseId;

            device.SelectSingleNode("mvein_com").InnerText = veinCom;

            //节点修改值保存
            xmlDoc.Save(xmlPath);
        }

        private void DeleteXMLLoc(Locations locations)
        {
            //xml文件回写
            XmlDocument xmlDoc = new XmlDocument();
            string xmlPath = $"{ApplicationState.GetProjectRootPath()}/MyProject.xml";
            xmlDoc.Load(xmlPath);
            XmlNode root = xmlDoc.SelectSingleNode("config");//指向根节点
            XmlNode device = root.SelectSingleNode("device");//指向设备节点

            XmlNode goodsLocaiton = device.SelectSingleNode("goods_loction");//指向货柜节点

            if (goodsLocaiton != null)
            {
                XmlNodeList locationNodeList = goodsLocaiton.ChildNodes;//获取货柜节点下的所有location子节点
                foreach (XmlNode node in locationNodeList)
                {
                    if(locations.Code == node.SelectSingleNode("location_name").InnerText)
                    {
                        goodsLocaiton.RemoveChild(node);
                    }
                }
            }

            //节点修改值保存
            xmlDoc.Save(xmlPath);
        }

        private void EditXMLLoc(Locations locations)
        {
            //xml文件回写
            XmlDocument xmlDoc = new XmlDocument();
            string xmlPath = $"{ApplicationState.GetProjectRootPath()}/MyProject.xml";
            xmlDoc.Load(xmlPath);
            XmlNode root = xmlDoc.SelectSingleNode("config");//指向根节点
            XmlNode device = root.SelectSingleNode("device");//指向设备节点

            XmlNode goodsLocaiton = device.SelectSingleNode("goods_loction");//指向货柜节点

            if (goodsLocaiton != null)
            {
                XmlNodeList locationNodeList = goodsLocaiton.ChildNodes;//获取货柜节点下的所有location子节点
                foreach (XmlNode node in locationNodeList)
                {
                    if (strEditingLocCode == node.SelectSingleNode("location_name").InnerText)
                    {
                        node.SelectSingleNode("location_name").InnerText = locations.Code;
                        node.SelectSingleNode("location_id").InnerText = locations.Id;
                        node.SelectSingleNode("locker_com").InnerText = locations.LockerCom;
                        node.SelectSingleNode("rfid_com").InnerText = locations.RFCom;
                    }
                }
            }
            //节点修改值保存
            xmlDoc.Save(xmlPath);
        }

        private void AddXMLLoc(Locations locations)
        {
            //xml文件回写
            XmlDocument xmlDoc = new XmlDocument();
            string xmlPath = $"{ApplicationState.GetProjectRootPath()}/MyProject.xml";
            xmlDoc.Load(xmlPath);
            XmlNode root = xmlDoc.SelectSingleNode("config");//指向根节点
            XmlNode device = root.SelectSingleNode("device");//指向设备节点

            XmlNode goodsLocaiton = device.SelectSingleNode("goods_loction");//指向货柜节点

            XmlNode locNode = xmlDoc.CreateNode("element", "location", "");
            goodsLocaiton.AppendChild(locNode);

            XmlNode codeNode = xmlDoc.CreateNode("element", "location_name", "");
            codeNode.InnerText = locations.Code;
            locNode.AppendChild(codeNode);

            XmlNode idNode = xmlDoc.CreateNode("element", "location_id", "");
            idNode.InnerText = locations.Id;
            locNode.AppendChild(idNode);

            XmlNode lockerNode = xmlDoc.CreateNode("element", "locker_com", "");
            lockerNode.InnerText = locations.LockerCom;
            locNode.AppendChild(lockerNode);

            XmlNode rfNode = xmlDoc.CreateNode("element", "rfid_com", "");
            rfNode.InnerText = locations.RFCom;
            locNode.AppendChild(rfNode);

            //节点修改值保存
            xmlDoc.Save(xmlPath);
        }

        private void comboBoxSizeType_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBox box = sender as ComboBox;
            box.ItemsSource = SerialComList;
        }
    }
}
