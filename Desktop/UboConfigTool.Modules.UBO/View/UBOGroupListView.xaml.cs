using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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
using UboConfigTool.Infrastructure;
using UboConfigTool.Infrastructure.Interfaces;
using UboConfigTool.Modules.UBO.ViewModel;

namespace UboConfigTool.Modules.UBO
{
    /// <summary>
    /// UBOGroupView.xaml 的交互逻辑
    /// </summary>
    [Export( "UBOGroupListView" )]
    public partial class UBOGroupListView : UserControl
    {
        public UBOGroupListView()
        {
            InitializeComponent();
        }

        [Import]
        public UBOGroupListViewModel ViewModel
        {
            set
            {
                this.DataContext = value;
            }
        }
    }
}
