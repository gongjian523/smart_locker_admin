using Microsoft.Practices.Prism.Regions;
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
using UboConfigTool.Infrastructure;
using UboConfigTool.Infrastructure.Models;
using UboConfigTool.Infrastructure.UserGuidBase;
using UboConfigTool.ViewModel;

namespace UboConfigTool.View
{
    /// <summary>
    /// Interaction logic for NewUserWizard_CreateUBOGroup.xaml
    /// </summary>
    [Export( "NewUserWizard_CreateUBOGroup" )]
    [PartCreationPolicy( CreationPolicy.NonShared )]
    public partial class NewUserWizard_CreateUBOGroup : UserControl, INavigationAware, IUserGuidValidation
    {
        [Import]
        public IRegionManager RegionManager;

        public NewUserWizard_CreateUBOGroup()
        {
            InitializeComponent();
        }

        public bool IsNavigationTarget( NavigationContext navigationContext )
        {
            return true;
        }

        public void OnNavigatedFrom( NavigationContext navigationContext )
        {
            DTOUBOGroup wizardDataObj = ( RegionManager.Regions[RegionNames.SecondaryRegion].Context as NewUserWizardViewModel ).WizardDataObject;
            wizardDataObj.name = txbGroupName.Text;
        }

        public void OnNavigatedTo( NavigationContext navigationContext )
        {
            DTOUBOGroup wizardDataObj = (RegionManager.Regions[RegionNames.SecondaryRegion].Context as NewUserWizardViewModel).WizardDataObject;
            txbGroupName.Text = wizardDataObj.name;
        }

        public bool ValidateData()
        {
            if( string.IsNullOrEmpty( txbGroupName.Text ) )
            {
                errorMsg.Text = "UBO群组名不能为空!";
                return false;
            }
            else
                errorMsg.Text = "";

            return true;
        }
    }
}
