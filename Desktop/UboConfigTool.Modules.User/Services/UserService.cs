using RestSharp;
using RestSharp.Deserializers;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using UboConfigTool.Infrastructure;
using UboConfigTool.Infrastructure.Interfaces;
using UboConfigTool.Infrastructure.Models;
using UboConfigTool.Infrastructure.Services;
using UboConfigTool.Modules.User.Repository;

namespace UboConfigTool.Modules.User.Services
{
    [Export( typeof( IUserService ) )]
    [PartCreationPolicy( CreationPolicy.Shared )]
    public class UserService : IUserService
    {
        public event DataBeginLoading DataBeginLoadingEvent;
        public event DataEndLoading DataEndLoadingEvent;

        protected  Dictionary<object, Model.User> _repository = new Dictionary<object, Model.User>();
        private bool _isLoadingData = false;
        private IEncryptionService encryptionService;

        public UserService()
        {
            // => for Test UI sake
            // _currentUserDescription = new UserDescription( "001", "testUser" );
            // _repository.Add( new Model.User( "testUser", "123", Model.Role.Admin ) );
            //_userRepository

            encryptionService = new EncryptionService();
        }

        private UserDescription _currentUserDescription;
        public UserDescription GetCurrentUser()
        {
            return _currentUserDescription;
        }

        public UserDescription Login( string name, string password )
        {
            string encryptedPsw = this.encryptionService.EncrypteVText( password );
            //_currentUserDescription = Authenticate( name, encryptedPsw );
            bool bSuc = Authenticate( name, encryptedPsw );
            return _currentUserDescription;
        }

        public Model.User GetUser( UserDescription desc )
        {
            return _repository[desc.UserName];
        }

        public bool ChangePassword( UserDescription user, string oldPassword, string newPassword )
        {
            return true;
        }

        public bool Authenticate( string name, string psw )
        {
            bool bSuc = false;

            RestRequest request = new RestRequest( "auth/sign_in", Method.POST );
            request.RequestFormat = DataFormat.Json;

            request.AddHeader( "Content-Type", "application/json" );
            request.AddBody( new
            {
                username = name,
                password = psw
            } );

            IRestResponse response = RestClientInstance.Instance.Client.Execute( request );
            if( response.ErrorException == null && ( response.StatusCode == System.Net.HttpStatusCode.OK
                   || response.StatusCode == System.Net.HttpStatusCode.ResetContent ) )
            {
                if( response.Headers != null )
                {
                    RestClientInstance.Instance.AccessToken = response.Headers.FirstOrDefault( item => item.Name == "access-token" ).Value.ToString();
                    RestClientInstance.Instance.UID = response.Headers.FirstOrDefault( item => item.Name == "uid" ).Value.ToString();
                    RestClientInstance.Instance.TokenType = response.Headers.FirstOrDefault( item => item.Name == "token-type" ).Value.ToString();
                    RestClientInstance.Instance.ClientText = response.Headers.FirstOrDefault( item => item.Name == "client" ).Value.ToString();
                    bSuc = true;
                }
            }

            return bSuc;
        }

        public List<Model.User> GetCurrentManageableUserAccordingRole()
        {
            //List<Model.User> users = new List<Model.User>();

            //Model.User curUser = GetUser( GetCurrentUser() );
            //switch( curUser.UserRole )
            //{
            //    case Model.Role.Super_Admin:
            //        users.AddRange( _repository.FindAll() );
            //        break;
            //    case Model.Role.Admin:
            //        users.Add( curUser );
            //        foreach( Model.User aUser in _repository.FindAll() )
            //        {
            //            if( aUser.UserRole == Model.Role.Log_Checker )
            //                users.Add( aUser );
            //        }
            //        break;
            //    case Model.Role.Log_Checker:
            //        users.Add( curUser );
            //        break;
            //    default:
            //        break;
            //}

            //return users;

            return null;
        }

        public UserDescription CurrentUserCreateNew( string name, string password, Model.Role role )
        {
            if( _repository[name] != null )
                return null;

            //Model.User curUser = GetUser( GetCurrentUser() );
            //if( curUser.UserRole >= role )
            //    return null;

            //_repository.Add( new Model.User( name, password, role ) );

            return new UserDescription( "001", name );
        }

        public bool CurrentUserRemoveUser( UserDescription userDesc )
        {
            if( _repository[userDesc.UserName] == null )
                return false;

            if( _currentUserDescription.UserName == userDesc.UserName )
                return false;

            Model.User willBeRemovedUser = GetUser( userDesc );

            _repository.Remove( willBeRemovedUser );

            return true;
        }

        public void ReloadData()
        {
            if( _isLoadingData )
                return;

            _isLoadingData = true;

            _repository.Clear();

            if( DataBeginLoadingEvent != null )
                DataBeginLoadingEvent();

            RestRequest request = new RestRequest( "admins", Method.GET );
            request.RequestFormat = DataFormat.Json;
            request.AddHeader( "Content-Type", "application/json" );
            RestClientInstance.Instance.InitRequestHeaderInfo( request );
            try
            {
                RestClientInstance.Instance.Client.ExecuteAsync( request, response =>
                {
                    if( response.ErrorException == null && ( response.StatusCode == System.Net.HttpStatusCode.OK
                     || response.StatusCode == System.Net.HttpStatusCode.ResetContent ) )
                    {
                        RestClientInstance.Instance.ResetAccessTokenFrom( response.Headers );
                        JsonDeserializer jDeserializer = new JsonDeserializer();
                        List<DTOUser> deviceLst = jDeserializer.Deserialize<List<DTOUser>>( response );

                        deviceLst.ForEach( ( dtoUser ) =>
                        {
                            Model.User user = new Model.User( dtoUser );
                            _repository.Add( user.Key, user );
                        } );
                    }

                    if( DataEndLoadingEvent != null )
                        DataEndLoadingEvent();
                } );
            }
            catch
            {
            }
            finally
            {
                _isLoadingData = false;
            }
        }

        public IList<Model.User> GetAllUsers()
        {
            return _repository.Values.ToList<Model.User>();
        }

    }
}
