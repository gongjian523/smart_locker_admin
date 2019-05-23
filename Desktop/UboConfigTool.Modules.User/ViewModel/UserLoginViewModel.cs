using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows.Input;
using UboConfigTool.Infrastructure.Events;
using UboConfigTool.Infrastructure.Interfaces;
using UboConfigTool.Infrastructure.Models;
using UboConfigTool.Modules.User.Services;

namespace UboConfigTool.Modules.User.ViewModel
{
    [Export( typeof( UserLoginViewModel ) )]
    [PartCreationPolicy( CreationPolicy.NonShared )]
    public class UserLoginViewModel : NotificationObject
    {
        private readonly IUserService _userService;
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;
        [ImportingConstructor]
        public UserLoginViewModel( IUserService userService, IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            if( userService == null )
            {
                throw new ArgumentNullException( "userService" );
            }

            if( eventAggregator == null )
            {
                throw new ArgumentNullException( "eventAggregator" );
            }

            this._userService = userService;
            this._regionManager = regionManager;
            this._eventAggregator = eventAggregator;

            this._userName = "abc";
            this._password = "abc";

            this._userLoginCommand = new DelegateCommand( UserLogin );
        }

        private void UserLogin()
        {
            UserDescription userDesc = this._userService.Login( UserName, Password );
            //bool bSucLogin = ( userDesc != null && userDesc.UserId.Length > 0 );

            this._eventAggregator.GetEvent<UserLoginEvent>().Publish( true );
        }

        private readonly ICommand _userLoginCommand;
        public ICommand UserLoginCommand
        {
            get
            {
                return _userLoginCommand;
            }
        }

        private string _userName;
        public string UserName
        {
            get
            {
                return this._userName;
            }

            set
            {
                if( !string.IsNullOrWhiteSpace( value ) )
                {
                    this._userName = value;
                    this.RaisePropertyChanged( () => this.UserName );
                }
            }
        }

        private string _password;
        public string Password
        {
            get
            {
                return this._password;
            }

            set
            {
                if( !string.IsNullOrWhiteSpace( value ) )
                {
                    this._password = value;
                    this.RaisePropertyChanged( () => this.Password );
                }
            }
        }
    }
}
