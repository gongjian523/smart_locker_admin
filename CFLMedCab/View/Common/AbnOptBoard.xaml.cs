using CFLMedCab.Http.Enum;
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

namespace CFLMedCab.View.Common
{
    /// <summary>
    /// AbnormalOptionsBoard.xaml 的交互逻辑
    /// </summary>
    public partial class AbnOptBoard : UserControl
    {
        public AbnOptBoard()
        {
            InitializeComponent();
        }

        public string GetAbnormal()
        {
            if ((bool)bthShortHide.IsChecked)
                return AbnormalCauses.商品缺失.ToString();
            else if ((bool)bthLossHide.IsChecked)
                return AbnormalCauses.商品遗失.ToString();
            else if ((bool)bthBadHide.IsChecked)
                return AbnormalCauses.商品损坏.ToString();
            else if ((bool)bthOtherHide.IsChecked)
                return AbnormalCauses.其他.ToString();
            else
                return "";
        }

        /// <summary>
        /// 操作缺货按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onShowBtnShort(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            bthShortHide.Visibility = (btn.Name == "bthShortShow" ? Visibility.Visible : Visibility.Collapsed);
        }

        /// <summary>
        /// 操作损耗按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onShowBtnLoss(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            bthLossHide.Visibility = (btn.Name == "bthLossShow" ? Visibility.Visible : Visibility.Collapsed);
        }

        /// <summary>
        /// 操作其他按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onShowBtnOther(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            bthOtherHide.Visibility = (btn.Name == "bthOtherShow" ? Visibility.Visible : Visibility.Collapsed);
        }

        /// <summary>
        /// 操作损耗按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onShowBtnBad(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            bthBadHide.Visibility = (btn.Name == "bthBadShow" ? Visibility.Visible : Visibility.Collapsed);
        }
    }
}
