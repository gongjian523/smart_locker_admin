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

namespace UboConfigTool.Modules.UBO.ViewModel
{
    public class AddUBOViewModel : PopupViewModelBase
    {
        private bool _isInAddUBOCommand = false;

        public AddUBOViewModel()
        {
            AddUBOCommand = new DelegateCommand( AddUBO, CanAddUBO );
        }

        private bool CanAddUBO()
        {
            return !_isInAddUBOCommand;
        }

        public ICommand AddUBOCommand
        {
            get;
            private set;
        }

        private string _uboName = string.Empty;
        public string UBOName
        {
            get
            {
                return this._uboName;
            }
            set
            {
                this._uboName = value;
                RaisePropertyChanged( () => UBOName );
            }
        }

        private string _sId = string.Empty;
        public string SId
        {
            get
            {
                return _sId;
            }
            set
            {
                this._sId = value;
                RaisePropertyChanged( () => SId );
            }
        }

        private string _bId = string.Empty;
        public string BId
        {
            get
            {
                return _bId;
            }
            set
            {
                this._bId = value;
                RaisePropertyChanged( () => BId );
            }
        }

        private string _location = string.Empty;
        public string Location
        {
            get
            {
                return _location;
            }
            set
            {
                this._location = value;
                RaisePropertyChanged( () => Location );
            }
        }

        public override bool Validate()
        {
            Error = string.Empty;

            if( string.IsNullOrEmpty( UBOName ) || string.IsNullOrEmpty( SId ) || string.IsNullOrEmpty( BId )
                || string.IsNullOrEmpty( Location ) )
            {
                Error = "请输入完整的UBO信息!";
            }

            return string.IsNullOrEmpty( Error );
        }

        private void AddUBO()
        {
            UBOGroupListViewModel uboGrpLstVM = ServiceLocator.Current.GetInstance<UBOGroupListViewModel>();
            if( uboGrpLstVM == null && uboGrpLstVM.CurrentUBOGroup == null )
                return;

            IUBODataService dataService = ServiceLocator.Current.GetInstance<IUBODataService>();
            if( dataService == null )
                return;

            if( !Validate() )
                return;

            _isInAddUBOCommand = true;

            ( AddUBOCommand as DelegateCommand ).RaiseCanExecuteChanged();

            bool bSuc = dataService.AddUBOIntoGroup( uboGrpLstVM.CurrentUBOGroup.Id, SId, BId, Location );
            if( !bSuc )
            {
                _isInAddUBOCommand = false;
                return;
            }

            dataService.ReloadData();

            ClosePopup();
        }
    }
}
