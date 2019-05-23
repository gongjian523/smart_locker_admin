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
using UboConfigTool.Infrastructure.UserGuidBase;

namespace UboConfigTool.View
{
    /// <summary>
    /// Interaction logic for NewUserWizardOverall.xaml
    /// </summary>
    [Export( "NewUserWizard_Overall" )]
    [PartCreationPolicy( CreationPolicy.NonShared )]
    public partial class NewUserWizard_Overall : UserControl, INavigationAware, IUserGuidValidation
    {
        public NewUserWizard_Overall()
        {
            InitializeComponent();

            for( int i=1; i < 6; i++ )
                _ubos.Add( "UBO" + i.ToString() );

            for( int i=1; i < 6; i++ )
                _usbs.Add( "USBKINGSTOND89DD20(只读)" + i.ToString() );

            for( int i=1; i < 6; i++ )
                _blackRuleList.Add( ".BCK文件" + i.ToString() );

            for( int i=1; i < 6; i++ )
                _whiteRuleList.Add( ".WHT文件" + i.ToString() );

            this.DataContext = this;
        }

        public bool IsNavigationTarget( NavigationContext navigationContext )
        {
            return true;
        }

        public void OnNavigatedFrom( NavigationContext navigationContext )
        {

        }

        public void OnNavigatedTo( NavigationContext navigationContext )
        {
            finishRoot.Visibility = System.Windows.Visibility.Visible;
            ruleRunningRoot.Visibility = System.Windows.Visibility.Collapsed;
        }

        public bool ValidateData()
        {
            return true;
        }

        private List<string> _ubos = new List<string>();
        public List<string> UBOs
        {
            get
            {
                return _ubos;
            }
        }

        private List<string> _usbs = new List<string>();
        public List<string> USBs
        {
            get
            {
                return _usbs;
            }
        }

        private List<string> _blackRuleList = new List<string>();
        public List<string> BlackRuleList
        {
            get
            {
                return _blackRuleList;
            }
        }

        private List<string> _whiteRuleList = new List<string>();
        public List<string> WhiteRuleList
        {
            get
            {
                return _whiteRuleList;
            }
        }

        private void btnRunRule_Click( object sender, RoutedEventArgs e )
        {
            finishRoot.Visibility = System.Windows.Visibility.Collapsed; 
            ruleRunningRoot.Visibility = System.Windows.Visibility.Visible;
        }
    }
}
