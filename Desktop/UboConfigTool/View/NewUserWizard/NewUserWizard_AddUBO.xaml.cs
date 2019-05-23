using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using UboConfigTool.Infrastructure.Events;
using UboConfigTool.Infrastructure.Models;
using UboConfigTool.Infrastructure.UserGuidBase;
using UboConfigTool.ViewModel;

namespace UboConfigTool.View
{
    public class UBOSpecification
    {
        public string UBOName
        {
            get;
            set;
        }

        public string UBOId
        {
            get;
            set;
        }

        public string UBOKey
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Interaction logic for NewUserWizard_AddUBO.xaml
    /// </summary>
    [Export( "NewUserWizard_AddUBO" )]
    [PartCreationPolicy( CreationPolicy.NonShared )]
    public partial class NewUserWizard_AddUBO : UserControl, INavigationAware, IUserGuidValidation
    {
        [Import]
        public IEventAggregator EventAggregator;

        [Import]
        public IRegionManager RegionManager;

        public NewUserWizard_AddUBO()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public bool IsNavigationTarget( NavigationContext navigationContext )
        {
            return true;
        }

        public void OnNavigatedFrom( NavigationContext navigationContext )
        {
            DTOUBOGroup wizardDataObj = ( RegionManager.Regions[RegionNames.SecondaryRegion].Context as NewUserWizardViewModel ).WizardDataObject;

            if( wizardDataObj.ubo_devices == null )
                wizardDataObj.ubo_devices = new List<DTOUBO>();

            wizardDataObj.ubo_devices.Clear();
            foreach( UBOSpecification uboSpec in _ubos )
            {
                wizardDataObj.ubo_devices.Add( new DTOUBO()
                {
                    sid = uboSpec.UBOKey,
                    name = uboSpec.UBOName,
                    bid = uboSpec.UBOId
                } );
            }
        }

        public void OnNavigatedTo( NavigationContext navigationContext )
        {
            DTOUBOGroup wizardDataObj = ( RegionManager.Regions[RegionNames.SecondaryRegion].Context as NewUserWizardViewModel ).WizardDataObject;
            txbTitle.Text = string.Format( "请为群《{0}》添加UBO", wizardDataObj.name );

            _ubos.Clear();
            if( wizardDataObj.ubo_devices != null )
            {
                foreach( DTOUBO ubo in wizardDataObj.ubo_devices )
                {
                    _ubos.Add( new UBOSpecification()
                    {
                        UBOId = ubo.bid,
                        UBOKey = ubo.sid,
                        UBOName = ubo.name
                    } );
                }
            }
        }

        public bool ValidateData()
        {
            bool bSuc = _ubos.Count > 0;
            errorMsg.Text = bSuc ? "" : "请添加UBO设备";
            return bSuc;
        }

        private void addUboBtn_Click( object sender, RoutedEventArgs e )
        {
            UBOSpecification UBODes = new UBOSpecification();
            UBODes.UBOName = txbUBOName.Text;
            UBODes.UBOId = txbUBOId.Text;
            UBODes.UBOKey = txbUBOKey.Text;

            if(string.IsNullOrEmpty(UBODes.UBOName)
                || string.IsNullOrEmpty( UBODes.UBOId )
                || string.IsNullOrEmpty( UBODes.UBOKey ))
            {
                errorMsg.Text = "请输入完整的UBO设备信息";
                return;
            }

            errorMsg.Text = "";
            _ubos.Add( UBODes );
        }

        private ObservableCollection<UBOSpecification> _ubos = new ObservableCollection<UBOSpecification>();
        public ObservableCollection<UBOSpecification> UBOs
        {
            get
            {
                return _ubos;
            }
        }

        private void btnDeleteUBO_Click( object sender, RoutedEventArgs e )
        {
            Button btn = sender as Button;
            if( btn != null && btn.DataContext is UBOSpecification )
            {
                UBOs.Remove( btn.DataContext as UBOSpecification );
            }
        }
    }
}
