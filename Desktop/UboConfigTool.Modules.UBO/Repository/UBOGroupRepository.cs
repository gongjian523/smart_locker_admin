using Microsoft.Practices.Prism.Events;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Deserializers;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using UboConfigTool.Infrastructure;
using UboConfigTool.Infrastructure.Events;
using UboConfigTool.Infrastructure.Interfaces;
using UboConfigTool.Infrastructure.Models;
using UboConfigTool.Infrastructure.Repository;
using Model = UboConfigTool.Infrastructure.Models;

namespace UboConfigTool.Modules.UBO.Repository
{
    [Export( typeof( IUBODataService ) )]
    [PartCreationPolicy( CreationPolicy.Shared )]
    public class UBOGroupRepository : IRepository<UBOGroup>, IUBODataService
    {
        private readonly IEventAggregator EventAggregator;

        protected  Dictionary<object, UBOGroup> _repository = new Dictionary<object, UBOGroup>();

        [ImportingConstructor]
        public UBOGroupRepository( IEventAggregator eventAggregator )
        {
            if( eventAggregator == null )
            {
                throw new ArgumentNullException( "eventAggregator" );
            }
            EventAggregator = eventAggregator;
            this.EventAggregator.GetEvent<LoadAllDataEvent>().Subscribe( LoadData, ThreadOption.UIThread );
        }

        public event DataBeginLoading DataBeginLoadingEvent;

        public event DataEndLoading DataEndLoadingEvent;

        private bool _isLoadingData = false;

        private readonly object _lockerObj = new object();

        public void LoadData( bool obj )
        {
            if( _isLoadingData )
                return;

            lock( _lockerObj )
            {
                _isLoadingData = true;

                _repository.Clear();

                if( DataBeginLoadingEvent != null )
                    DataBeginLoadingEvent();

                RestRequest request = new RestRequest( "ubo_groups", Method.GET );
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
                            List<DTOUBOGroup> deviceLst = jDeserializer.Deserialize<List<DTOUBOGroup>>( response );
                            if( deviceLst != null )
                            {
                                deviceLst.ForEach( dtoUBOGrp =>
                                {
                                    UBOGroup uboGrp = new UBOGroup( dtoUBOGrp );

                                    //Init USB Access role
                                    if( dtoUBOGrp.ubo_group_usbs != null )
                                    {
                                        dtoUBOGrp.ubo_group_usbs.ForEach( dtoGrpUsb =>
                                        {
                                            USB usb = uboGrp.GetUSB( dtoGrpUsb.usb_id );
                                            if( usb != null )
                                                usb.AccessRole = (USB_Access_Role)dtoGrpUsb.permission_role;
                                        } );
                                    }
                                    _repository.Add( dtoUBOGrp.id, uboGrp );
                                } );
                            }
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
        }

        public UBOGroup FindBy( object key )
        {
            return _repository[key];
        }

        public IList<UBOGroup> FindAll()
        {
            return _repository.Values.ToList<UBOGroup>().AsReadOnly();
        }

        public void Add( UBOGroup item )
        {
            if( !_repository.Keys.Contains( item.Key ) )
            {
                //var request = new RestRequest( RestResource, Method.POST );
                //request.AddJsonBody( item );
                //IRestResponse response = RestClientInstance.Instance.Client.Execute( request );
                //if( response.ResponseStatus == ResponseStatus.Completed )
                //{
                _repository.Add( item.Key, item );
                // }
            }
        }

        public UBOGroup this[object key]
        {
            get
            {
                if( !_repository.Keys.Contains( key ) )
                    return null;

                return _repository[key];
            }
            set
            {
                //var request = new RestRequest( RestResource + '/' + key.ToString(), Method.PUT );
                //request.AddJsonBody( value );
                ////var response = RestClientInstance.Instance.Client.Execute( request );
                ////if( response.ResponseStatus == ResponseStatus.Completed )
                //{
                //    this._repository[key] = value;
                //}
            }
        }

        public void Remove( UBOGroup item )
        {
            //var request = new RestRequest( RestResource + "/" + item.Key, Method.DELETE );
            ////var response = RestClientInstance.Instance.Client.Execute( request );
            ////if( response.ResponseStatus == ResponseStatus.Completed )
            //{
            //    this._repository.Remove( item.Key );
            //}
        }

        public void ClearData()
        {
            _repository.Clear();
        }

        public IList<UBOGroup> GetAllUBOs()
        {
            return this.FindAll();
        }

        public void ReloadData()
        {
            this.LoadData( true );
        }

        public IList<Model.UBOWithUsbInfo> GetUBOsForUSB( int usbId )
        {
            List<Model.UBOWithUsbInfo> ubos = new List<Model.UBOWithUsbInfo>();

            _repository.Values.ToList<UBOGroup>().ForEach( uboGrp =>
            {
                if( uboGrp.GetUSB( usbId ) != null )
                {
                    uboGrp.UBODevices.ToList<Model.UBO>().ForEach( ubo =>
                    {
                        UBOWithUsbInfo uboItem = new UBOWithUsbInfo( ubo.Name, uboGrp.Name, ubo.ConnectionStatus, ubo.Id, uboGrp.GetUSB( usbId ).AccessRole );
                        ubos.Add( uboItem );
                    } );
                }
            } );

            return ubos;
        }

        public bool AddUBOIntoGroup( int groupId, string uboName, string uboSId, string uboBId )
        {
            RestRequest request = new RestRequest( "ubos", Method.POST );
            RestClientInstance.Instance.InitRequestHeaderInfo( request );
            request.AddBody( new
            {
                ubo_params = new
                {
                    name = uboName,
                    sid = uboSId,
                    bid = uboBId,
                    ubo_group_id = groupId
                }
            } );

            IRestResponse response = RestClientInstance.Instance.Client.Execute( request );
            bool rtn = ( response.ErrorException == null && response.StatusCode == System.Net.HttpStatusCode.OK );
            if( rtn )
                RestClientInstance.Instance.ResetAccessTokenFrom( response.Headers );
            return rtn;
        }

        public bool DeleteUBO( int uboId )
        {
            string delResource = "ubos/" + uboId.ToString();
            RestRequest request = new RestRequest( delResource, Method.DELETE );
            RestClientInstance.Instance.InitRequestHeaderInfo( request );
            IRestResponse response = RestClientInstance.Instance.Client.Execute( request );
            bool rtn = ( response.ErrorException == null && response.StatusCode == System.Net.HttpStatusCode.OK );
            if(rtn)
                RestClientInstance.Instance.ResetAccessTokenFrom( response.Headers );
            return rtn;
        }

        public bool RenameUsb( int usbId, string newName )
        {
            string strRequest = "";
            strRequest = "usbs/" + usbId;

            RestRequest request = new RestRequest( strRequest, Method.PUT );
            request.RequestFormat = DataFormat.Json;
            RestClientInstance.Instance.InitRequestHeaderInfo( request );
            request.AddBody( new
            {
                name = newName
            } );

            IRestResponse response = RestClientInstance.Instance.Client.Execute( request );

            bool rtn = ( response.ErrorException == null && response.StatusCode == System.Net.HttpStatusCode.OK );

            if(rtn)
                RestClientInstance.Instance.ResetAccessTokenFrom( response.Headers );

            return rtn;
        }


        
    }
}
