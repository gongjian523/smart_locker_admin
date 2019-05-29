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

namespace CFLMedCab.View
{
    /// <summary>
    /// OperationDetails.xaml 的交互逻辑
    /// </summary>
    public partial class OperationDetails : UserControl
    {
        public OperationDetails()
        {
            InitializeComponent();
        }
        
        /// <summary>
        /// 返回领用界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OperationCollarUsed operationCollarUsed = new OperationCollarUsed();
            ContentFrame.Navigate(operationCollarUsed);
        }
    }
}
