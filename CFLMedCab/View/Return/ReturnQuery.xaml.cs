using CFLMedCab.APO.Surgery;
using CFLMedCab.BLL;
using CFLMedCab.Controls;
using CFLMedCab.DAL;
using CFLMedCab.DTO.Surgery;
using CFLMedCab.Http.Bll;
using CFLMedCab.Http.Helper;
using CFLMedCab.Http.Model;
using CFLMedCab.Http.Model.Base;
using CFLMedCab.Http.Model.Common;
using CFLMedCab.Infrastructure;
using CFLMedCab.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CFLMedCab.View.Return
{
    /// <summary>
    /// Query.xaml 的交互逻辑
    /// </summary>
    public partial class ReturnQuery : UserControl
    {
        public delegate void EnterReturnOpenHandler(object sender, CommodityRecovery e);
        public event EnterReturnOpenHandler EnterReturnOpenEvent;

        //显示加载数据的进度条
        public delegate void LoadingDataHandler(object sender, bool e);
        public event LoadingDataHandler LoadingDataEvent;

        //KeyboardView kbHandler = new KeyboardView();

        public ReturnQuery()
        {
            InitializeComponent();

            tbInputCode.Focus();
        }
    
        /// <summary>
        /// 查看详情
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnterDetail_Click(object sender, RoutedEventArgs e)
        {
            var inputStr = tbInputCode.Text;
            if (string.IsNullOrWhiteSpace(inputStr))
            {
                MessageBox.Show("单号不可以为空！", "温馨提示", MessageBoxButton.OK);
                return;
            }

            TaskOrder taskOrder;
            string name;
            try
            {
                taskOrder = JsonConvert.DeserializeObject<TaskOrder>(inputStr);
                name = taskOrder.name;
            }
            catch
            {
                name = inputStr;
            }

            LoadingDataEvent(this, true);
            BaseData<CommodityRecovery> bdCommodityRecovery = CommodityRecoveryBll.GetInstance().GetCommodityRecovery(name);
            LoadingDataEvent(this, false);

            HttpHelper.GetInstance().ResultCheck(bdCommodityRecovery, out bool isSuccess);

			if (!isSuccess)
			{
				MessageBox.Show("无法获取回收下架单详情！" + bdCommodityRecovery.message, "温馨提示", MessageBoxButton.OK);
				return;
			}
            EnterReturnOpenEvent(this, bdCommodityRecovery.body.objects[0]);
        }
        
        /// <summary>
        /// 扫码查询事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                EnterDetail_Click(this, null);
            }
        }

        /// <summary>
        /// 获得焦点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void onGotFocus(object sender, RoutedEventArgs e)
        //{
        //    //kbHandler = new KeyboardView();
        //    kbHandler.SetCurrentControl((Control)sender);
        //    kbHandler.Topmost = true;
        //    kbHandler.Show();
        //}

        /// <summary>
        /// 失去焦点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void onLostFocus(object sender, RoutedEventArgs e)
        //{
        //    kbHandler.Hide();
        //}
    }
}
