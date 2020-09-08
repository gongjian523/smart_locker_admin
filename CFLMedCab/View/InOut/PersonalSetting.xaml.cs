using CFLMedCab.BLL;
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

        public delegate void EnterInOutDetailHandler(object sender, int e);
        public event EnterInOutDetailHandler EnterInOutDetailEvent;

        List<LoginRecord> loginList { get; set; }

        List<InOutRecord> inOutList { get; set; }

        public PersonalSetting()
        {
            InitializeComponent();

            User user = ApplicationState.GetUserInfo();

            lbName.Content = user.name;
            lbPhone.Content = user.MobilePhone.Substring(4);
            lbDepartment.Content = user.DepartmentInUse;

            LoginBll loginBll = new LoginBll();
            InOutRecordBll inOutRecordBll = new InOutRecordBll();

            if (user.Role == "医护人员")
            {
                loginList = loginBll.GetLoginRecordByUserName(user.name);
                inOutList = inOutRecordBll.GetInOutRecordByName(user.name);
            }
            else
            {
                loginList = loginBll.GetAllLoginRecord();
                inOutList = inOutRecordBll.GetAllInOutRecord();
            }

            listView.DataContext = loginList;
            listView1.DataContext = inOutList;

        }

        /// <summary>
        /// 查看登录日志时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onChooseLoginRecord(object sender, RoutedEventArgs e)
        {
            Content0.Visibility = Visibility.Visible;
            Content1.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// 查看开关门日志时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onChooseInOutRecord(object sender, RoutedEventArgs e)
        {
            Content0.Visibility = Visibility.Collapsed;
            Content1.Visibility = Visibility.Visible;
        }


        /// <summary>
        /// 进入开关门详情事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterInOutDetail(object sender, RoutedEventArgs e)
        {
            Button btnItem = sender as Button;
            int  id = (int)btnItem.CommandParameter;
            EnterInOutDetailEvent(this, id);
        }
    }
}
