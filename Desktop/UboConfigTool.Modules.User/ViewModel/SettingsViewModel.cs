using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using UboConfigTool.Infrastructure;

namespace UboConfigTool.Modules.User.ViewModel
{
    public enum SettingViewType
    {
        None = 0,
        UserSettingView,
        VersionSettingView,
        CloudSettingView
    }

    [Export( typeof( SettingsViewModel ) )]
    [PartCreationPolicy( CreationPolicy.NonShared )]
    public class SettingsViewModel : NotificationObject
    {
        private static Uri _userSettingUri = new Uri( "UsersSettingView", UriKind.Relative );
        private static Uri _versionSettingUri = new Uri( "VersionsSettingView", UriKind.Relative );
        private static Uri _cloudSettingUri = new Uri( "CloudSettingView", UriKind.Relative );

        private readonly IRegionManager _regionManager;

        [ImportingConstructor]
        public SettingsViewModel( IRegionManager regionManager )
        {
            if( regionManager == null )
            {
                throw new ArgumentNullException( "regionManager" );
            }

            _regionManager = regionManager;
        }

        private SettingViewType _currentViewType = SettingViewType.None;
        public SettingViewType CurrentViewType
        {
            get
            {
                return _currentViewType;
            }
            set
            {
                this._currentViewType = value;
                this.RaisePropertyChanged( () => CurrentViewType );
                NavigateToView();
            }
        }

        private void NavigateToView()
        {
            switch( _currentViewType )
            {
                case SettingViewType.UserSettingView:
                    _regionManager.RequestNavigate( RegionNames.SettingContent, _userSettingUri );
                    break;
                case SettingViewType.VersionSettingView:
                    _regionManager.RequestNavigate( RegionNames.SettingContent, _versionSettingUri );
                    break;
                case SettingViewType.CloudSettingView:
                    _regionManager.RequestNavigate( RegionNames.SettingContent, _cloudSettingUri );
                    break;
                default:
                    break;
            }
        }

    }
}
