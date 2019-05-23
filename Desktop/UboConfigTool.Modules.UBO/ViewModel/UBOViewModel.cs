using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows.Input;
using UboConfigTool.Infrastructure;
using UboConfigTool.Infrastructure.Interfaces;
using UboConfigTool.Infrastructure.Models;
using Model = UboConfigTool.Infrastructure.Models;

namespace UboConfigTool.Modules.UBO.ViewModel
{
    public class UBOViewModel : NotificationObject
    {
        private readonly string NORMAL_VM = "NormalViewModel";
        private readonly string ADD_VM = "AddViewModel";
        private Model.UBO _ubo;
        private UBOGroupViewModel _uboGrpBelongsToVM;
        private IMessageService _msgService;

        public UBOViewModel( Model.UBO ubo, UBOGroupViewModel uboGrpVM )
        {
            _ubo = ubo;
            _connetionStatus = _ubo.ConnectionStatus;
            _vmType = NORMAL_VM;
            _uboGrpBelongsToVM = uboGrpVM;
            USBItemBelongsClickedCommand = new DelegateCommand( USBItemBelongsClicked );
            ChangeUBONameCommand = new DelegateCommand( ChangeUBOName );
            ChangeUBOIPAddressCommand = new DelegateCommand( ChangeUBOIPAddress );
            ChangeUBOLocationInfoCommand = new DelegateCommand( ChangeUBOLocationInfo );

            _msgService = ServiceLocator.Current.GetInstance<IMessageService>();
        }

        public UBOViewModel()
        {
            _vmType = ADD_VM;
        }

        private string _vmType;
        public string VMType
        {
            get
            {
                return _vmType;
            }
        }

        public UBOGroupViewModel UBOGroupBelongsToVM
        {
            get
            {
                return _uboGrpBelongsToVM;
            }
        }

        private UBOConnectionType _connetionStatus = UBOConnectionType.Disconnected;
        public UBOConnectionType CurrentConnectionType
        {
            get
            {
                return _connetionStatus;
            }
            set
            {
                _connetionStatus = value;
                this.RaisePropertyChanged( () => this.CurrentConnectionType );
            }
        }

        public string Name
        {
            get
            {
                if( _ubo == null )
                    return string.Empty;

                return this._ubo.Name;
            }
            set
            {
                if( !string.IsNullOrWhiteSpace( value ) )
                {
                    this._ubo.Name = value;
                    this.RaisePropertyChanged( () => this.Name );
                }
            }
        }

        public string SId
        {
            get
            {
                if( _ubo == null )
                    return string.Empty;
                return _ubo.SId;
            }
        }

        public string BId
        {
            get
            {
                if( _ubo == null )
                    return string.Empty;
                return _ubo.BId;
            }
        }

        public string VersionNo
        {
            get
            {
                if( _ubo == null )
                    return string.Empty;
                return _ubo.Version;
            }
        }

        public string IPAddress
        {
            get
            {
                if( _ubo == null )
                    return string.Empty;
                return _ubo.IPAddress;
            }
        }

        public string Location
        {
            get
            {
                if( _ubo == null )
                    return string.Empty;
                return _ubo.Location;
            }
        }

        public int Id
        {
            get
            {
                return _ubo.Id;
            }
        }

        public ICommand USBItemBelongsClickedCommand
        {
            get;
            private set;
        }

        public ICommand ChangeUBONameCommand
        {
            get;
            private set;
        }

        public ICommand ChangeUBOIPAddressCommand
        {
            get;
            private set;
        }

        public ICommand ChangeUBOLocationInfoCommand
        {
            get;
            private set;
        }

        private void USBItemBelongsClicked()
        {
            if( _msgService.ShowMessage( "离开提示", "即将离开 《UBO管理》模块，前往《USB管理》模块" ) != DialogResponse.Ok )
                return;
        }

        private void ChangeUBOLocationInfo()
        {
            string expectedLocaton = string.Empty;
            if( _msgService.ShowTextInputMessage( ref expectedLocaton, "输入UBO位置信息" ) != DialogResponse.Ok )
                return;
        }

        private void ChangeUBOIPAddress()
        {
            string expectedIPAdd = string.Empty;
            if( _msgService.ShowTextInputMessage( ref expectedIPAdd, "输入IP地址,格式如: 192.168.1.1" ) != DialogResponse.Ok )
                return;
        }

        private void ChangeUBOName()
        {
            string expectedName = string.Empty;
            if( _msgService.ShowTextInputMessage( ref expectedName, "输入新的UBO名称" ) != DialogResponse.Ok )
                return;
        }
    }
}
