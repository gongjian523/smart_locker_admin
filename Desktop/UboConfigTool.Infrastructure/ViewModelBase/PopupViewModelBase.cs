using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace UboConfigTool.Infrastructure
{
    public abstract class PopupViewModelBase : NotificationObject
    {
        public PopupViewModelBase()
        {
            ClosePopupCommand = new DelegateCommand( ClosePopup );
        }

        public ICommand ClosePopupCommand
        {
            get;
            private set;
        }

        private string _error = string.Empty;
        public string Error
        {
            get
            {
                return _error;
            }
            set
            {
                this._error = value;
                RaisePropertyChanged( () => Error );
            }
        }

        public void ClosePopup()
        {
            IRegionManager regionMgr = ServiceLocator.Current.GetInstance<IRegionManager>();
            if( regionMgr == null )
                return;

            foreach( object viewObj in regionMgr.Regions[RegionNames.SecondaryRegion].ActiveViews )
                regionMgr.Regions[RegionNames.SecondaryRegion].Remove( viewObj );
        }

        public abstract bool Validate();
    }
}
