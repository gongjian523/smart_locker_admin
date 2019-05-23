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
using UboConfigTool.Modules.User.ViewModel;

namespace UboConfigTool.Modules.User.View
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
   [Export( "SettingsView" )]
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();
            this.Loaded += SettingsView_Loaded;
        }

        void SettingsView_Loaded( object sender, RoutedEventArgs e )
        {
            ( this.DataContext as SettingsViewModel ).CurrentViewType = SettingViewType.UserSettingView;
        }

        [Import]
        public SettingsViewModel ViewModel
        {
            set
            {
                this.DataContext = value;
            }
        }
    }
}
