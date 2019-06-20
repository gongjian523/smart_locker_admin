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

namespace CFLMedCab.View.Fetch
{
    /// <summary>
    /// GerFetchState.xaml 的交互逻辑
    /// </summary>
    public partial class GerFetchState : UserControl
    {
        public GerFetchState(int e)
        {
            InitializeComponent();
            if (e == 1)
                attention.Content = "请拿取您需要的耗材，拿取完毕请关闭柜门";
            else if (e == 2)
                attention.Content = "请放入您需要回退的的耗材，放回完毕请关闭柜门";
            else
                attention.Content = "还有柜门未关，操作完毕请关门";
        }
    }
}
