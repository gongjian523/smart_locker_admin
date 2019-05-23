using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UboConfigTool.Modules.UBO.ViewModel;

namespace UboConfigTool.Modules.UBO.View
{
    /// <summary>
    /// Interaction logic for UBOConfigOptionView.xaml
    /// </summary>
    [Export( "UBOConfigOptionView" )]
    public partial class UBOConfigOptionView : UserControl
    {
        public UBOConfigOptionView()
        {
            InitializeComponent();
        }

        [Import]
        public UBOConfigOptionViewModel ViewModel
        {
            set
            {
                this.DataContext = value;
            }
        }
    }
}
