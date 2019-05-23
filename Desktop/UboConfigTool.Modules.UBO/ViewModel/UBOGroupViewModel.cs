using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using UboConfigTool.Infrastructure;
using UboConfigTool.Infrastructure.Interfaces;
using UboConfigTool.Infrastructure.Models;
using Model = UboConfigTool.Infrastructure.Models;

namespace UboConfigTool.Modules.UBO.ViewModel
{
    public class UBOGroupViewModel : NotificationObject
    {
        private static Uri addUBOViewUri = new Uri( "AddUBOView", UriKind.Relative );
        private static Uri aUBOGroupViewUri = new Uri( "UBOGroupView", UriKind.Relative );
        private static Uri editUBOViewUri = new Uri( "UBOView", UriKind.Relative );
        private static Uri configUBOGroupViewUri = new Uri( "UBOConfigOptionView", UriKind.Relative );
        private static Uri upgradeViewUri = new Uri( "UBOUpgradeView", UriKind.Relative );
        private UBOGroup _uboGrp;
        private UBOGroupListViewModel _uboGrpLstVM;
        private IRegionManager _regionMgr;
        private IMessageService _msgService;

        public UBOGroupViewModel( UBOGroup group, UBOGroupListViewModel grpLstVM )
        {
            _uboGrp = group;
            _info = "有新的修改元素未部署";
            InitData();
            _editUBOGroupCommand = new DelegateCommand( EditUBOGroup );
            _addUBOCommand = new DelegateCommand( AddUBO );
            _editUBOCommand = new DelegateCommand<object>( EditUBO );
            _delUBOCommand = new DelegateCommand<object>( DeleteUBO );
            _configUBOGroupCommand = new DelegateCommand( ConfigUBOGroup );
            _upgradeCommand = new DelegateCommand( UBOUpgrade );
            _editRulesCommand = new DelegateCommand( EditRules );
            _changeGroupNameCommand = new DelegateCommand( ChangeGroupName );
            _uboGrpLstVM = grpLstVM;

            _regionMgr = ServiceLocator.Current.GetInstance<IRegionManager>();
            _msgService = ServiceLocator.Current.GetInstance<IMessageService>();
        }

        private readonly ICommand _changeGroupNameCommand;
        public ICommand ChangeGroupNameCommand
        {
            get
            {
                return _changeGroupNameCommand;
            }
        }

        private readonly ICommand _editRulesCommand;
        public ICommand EditRulesCommand
        {
            get
            {
                return _editRulesCommand;
            }
        }

        private readonly ICommand _upgradeCommand;
        public ICommand UpgradeCommand
        {
            get
            {
                return _upgradeCommand;
            }
        }

        private ICommand _configUBOGroupCommand;
        public ICommand ConfigUBOGroupCommand
        {
            get
            {
                return _configUBOGroupCommand;
            }
        }

        private ICommand _addUBOCommand;
        public ICommand AddUBOCommand
        {
            get
            {
                return _addUBOCommand;
            }
        }

        private ICommand _delUBOCommand;
        public ICommand DeleteUBOCommand
        {
            get
            {
                return _delUBOCommand;
            }
        }

        private readonly ICommand _editUBOGroupCommand;
        public ICommand EditUBOGroupCommand
        {
            get
            {
                return _editUBOGroupCommand;
            }
        }

        private readonly ICommand _editUBOCommand;
        public ICommand EditUBOCommand
        {
            get
            {
                return _editUBOCommand;
            }
        }

        public string GroupName
        {
            get
            {
                return this._uboGrp.Name;
            }

            set
            {
                if( !string.IsNullOrWhiteSpace( value ) )
                {
                    this._uboGrp.Name = value;
                    this.RaisePropertyChanged( () => this.GroupName );
                }
            }
        }

        private string _info;
        public string Info
        {
            get
            {
                return this._info;
            }
            set
            {
                _info = value;
                this.RaisePropertyChanged( () => this.Info );
            }
        }

        private ObservableCollection<UBOViewModel> _uboVMs = new ObservableCollection<UBOViewModel>();
        public ObservableCollection<UBOViewModel> UBOVMs
        {
            get
            {
                return _uboVMs;
            }
        }

        public ObservableCollection<USB> USBs
        {
            get
            {
                return new ObservableCollection<USB>( _uboGrp.USBs );
            }
        }

        public ObservableCollection<Rule> BlackRuleList
        {
            get
            {
                return new ObservableCollection<Rule>( _uboGrp.Rules.Where( item =>
                    item.Type == RuleType.Black_name ) );
            }
        }

        public ObservableCollection<Rule> WhiteRuleList
        {
            get
            {
                return new ObservableCollection<Rule>( _uboGrp.Rules.Where( item =>
                    item.Type == RuleType.White_name ) );
            }
        }

        public int UBONumber
        {
            get
            {
                return _uboGrp.UBODevices.Count;
            }
        }

        public int USBNumber
        {
            get
            {
                return _uboGrp.USBs.Count;
            }
        }

        public int Id
        {
            get
            {
                return _uboGrp.Id;
            }
        }

        private void InitData()
        {
            _uboVMs.Clear();

            _uboGrp.UBODevices.ToList<Model.UBO>().ForEach( ubo =>
                {
                    _uboVMs.Add( new UBOViewModel( ubo, this ) );
                } );

            _uboVMs.Add( new UBOViewModel() );
        }

        private void UBOUpgrade()
        {
            _regionMgr.RequestNavigate( RegionNames.SecondaryRegion, upgradeViewUri );
        }

        private void ConfigUBOGroup()
        {
            _regionMgr.RequestNavigate( RegionNames.SecondaryRegion, configUBOGroupViewUri );
        }

        private void DeleteUBO( object uboVM )
        {
            if( !( uboVM is UBOViewModel ) )
                return;

            IMessageService msgService = ServiceLocator.Current.GetInstance<IMessageService>();
            if( msgService == null )
                return;

            if( msgService.ShowMessage( "删除UBO", "确认删除当前UBO?" ) != DialogResponse.Ok )
                return;

            if( _uboGrpLstVM.DeleteUBO( ( uboVM as UBOViewModel ).Id ) )
            {
                _regionMgr.RequestNavigate( RegionNames.MainContentRegion, aUBOGroupViewUri );
            }
        }

        private void EditUBO( object uboVM )
        {
            if( !( uboVM is UBOViewModel ) )
                return;

            _uboGrpLstVM.CurrentEdittingUBO = uboVM as UBOViewModel;
            _regionMgr.RequestNavigate( RegionNames.MainContentRegion, editUBOViewUri );
        }

        private void AddUBO()
        {
            _uboGrpLstVM.CurrentUBOGroup = this;
            IRegionManager regionMgr = ServiceLocator.Current.GetInstance<IRegionManager>();
            _regionMgr.RequestNavigate( RegionNames.SecondaryRegion, addUBOViewUri );
        }

        private void EditUBOGroup()
        {
            _uboGrpLstVM.CurrentUBOGroup = this;
            IRegionManager regionMgr = ServiceLocator.Current.GetInstance<IRegionManager>();
            _regionMgr.RequestNavigate( RegionNames.MainContentRegion, aUBOGroupViewUri );
        }

        private void EditRules()
        {
            if( _msgService.ShowMessage( "离开提示", "即将离开 《UBO管理》模块，前往《规则管理》模块" ) != DialogResponse.Ok )
                return;
        }

        private void ChangeGroupName()
        {
            string expectedNewGroupName = string.Empty;
            if( _msgService.ShowTextInputMessage( ref expectedNewGroupName, "输入新的群组名称", ValidateNewGroupName ) != DialogResponse.Ok )
                return;

            //To do: to add change the group name logic 
        }

        private void ValidateNewGroupName( TextInputValiateResult Validateobj )
        {
            bool bValid = true;

            _uboGrpLstVM.UBOGroupVMs.ToList<UBOGroupViewModel>().ForEach( item =>
                {
                    if( item.GroupName == Validateobj.TextInput && item.Id != this.Id )
                        bValid = false;
                } );

            Validateobj.IsPass = bValid;

            if( !bValid )
            {
                Validateobj.ExtraInfo = "已存在相同的群组名";
            }
        }
    }
}
