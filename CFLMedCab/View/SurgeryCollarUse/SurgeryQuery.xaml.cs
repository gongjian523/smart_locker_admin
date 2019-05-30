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

namespace CFLMedCab.View.SurgeryCollarUse
{
    /// <summary>
    /// Query.xaml 的交互逻辑
    /// </summary>
    public partial class SurgeryQuery : UserControl
    {
        public SurgeryQuery()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 手术单号查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Query(object sender, RoutedEventArgs e)
        {
            SurgeryNumQuery surgeryNumQuery=new SurgeryNumQuery(tbOddNumbers.Text);
            ContentFrame.Navigate(surgeryNumQuery);
        }

        /// <summary>
        /// 暂无手术单号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NoNum(object sender, RoutedEventArgs e)
        {
            NoSurgeryNumClose noSurgeryNumClose = new NoSurgeryNumClose(); 
            ContentFrame.Navigate(noSurgeryNumClose);
        }
    }
}
