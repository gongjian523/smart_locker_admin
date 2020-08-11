using AutoMapper;
using CFLMedCab.BLL;
using CFLMedCab.DTO.Goodss;
using CFLMedCab.DTO.Inventory;
using CFLMedCab.Http.Model;
using CFLMedCab.Infrastructure.DeviceHelper;
using CFLMedCab.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CFLMedCab.View.InOut
{
    /// <summary>
    /// InOut.xaml 的交互逻辑
    /// </summary>
    public partial class InOutDetailWin : UserControl
    {
        public delegate void BackPersonalSettingHandler(object sender, RoutedEventArgs e);
        public event BackPersonalSettingHandler BackPersonalSettingEvent;

        InOutRecordBll inOutRecordBll = new InOutRecordBll();

        public string Operater { get; set; }
        public string Department { get; set; }
        public DateTime CreateTime { get; set; }
        public string BusinessType { get; set; }


        public InOutDetailWin(int inventoryId)
        {
            InitializeComponent();

            DataContext = this;

            InOutRecord record = inOutRecordBll.GetInOutRecordById(inventoryId);

            CreateTime = record.create_time;
            BusinessType = record.operate;
            Operater = record.user_name;
            Department = record.department;

            List<InOutDetail> detals = inOutRecordBll.GetInOutDetails(inventoryId);

            listView1.DataContext = detals;

        }

        /// <summary>
        /// 返回的个人设置界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onBackword(object sender, RoutedEventArgs e)
        {
            BackPersonalSettingEvent(this, null);
        }
    }
}
