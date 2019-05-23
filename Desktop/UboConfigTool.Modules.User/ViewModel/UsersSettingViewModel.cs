using Microsoft.Practices.Prism.ViewModel;
using RestSharp;
using RestSharp.Deserializers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using UboConfigTool.Infrastructure;
using UboConfigTool.Infrastructure.Models;
using UboConfigTool.Modules.User.Services;

namespace UboConfigTool.Modules.User.ViewModel
{
    [Export( typeof( UsersSettingViewModel ) )]
    [PartCreationPolicy( CreationPolicy.NonShared )]
    public class UsersSettingViewModel : NotificationObject
    {
        private readonly IUserService _userService;

        [ImportingConstructor]
        public UsersSettingViewModel( IUserService userService )
        {
            if( userService == null )
            {
                throw new ArgumentNullException( "userService" );
            }
            this._userService = userService;
            this._userService.DataBeginLoadingEvent += _userService_DataBeginLoadingEvent;
            this._userService.DataEndLoadingEvent += _userService_DataEndLoadingEvent;
        }

        void _userService_DataEndLoadingEvent()
        {
            Application.Current.Dispatcher.BeginInvoke( DispatcherPriority.Normal, new Action( () =>
            {
                UserGroups.Clear();
                List<Model.User> users = this._userService.GetAllUsers().ToList<Model.User>();

                List<Model.User> sysAdminUsers = users.Where( aUser => aUser.IsSysAdminUser() ).ToList<Model.User>();
                UserGroups.Add( new UserGroupViewModel( UserGroupViewModel.GROUP_SYS_ADMIN, sysAdminUsers ) );

                List<Model.User> logUsers = users.Where( aUser => aUser.IsLogUser() ).ToList<Model.User>();
                UserGroups.Add( new UserGroupViewModel( UserGroupViewModel.GROUP_LOG_USER, logUsers ) );
            } ) );

        }

        void _userService_DataBeginLoadingEvent()
        {

        }

        private ObservableCollection<UserGroupViewModel> _userGroups = new ObservableCollection<UserGroupViewModel>();
        public ObservableCollection<UserGroupViewModel> UserGroups
        {
            get
            {
                return _userGroups;
            }
        }

    }
}
