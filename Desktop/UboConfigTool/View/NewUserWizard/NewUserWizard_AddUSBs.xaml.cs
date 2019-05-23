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
using UboConfigTool.Infrastructure.UserGuidBase;

namespace UboConfigTool.View
{
    /// <summary>
    /// Interaction logic for NewUserWizard_AddUSBs.xaml
    /// </summary>
    [Export( "NewUserWizard_AddUSBs" )]
    public partial class NewUserWizard_AddUSBs : UserControl, IUserGuidValidation
    {
        public NewUserWizard_AddUSBs()
        {
            InitializeComponent();
        }

        public bool ValidateData()
        {
            return true;
        }
    }
}
