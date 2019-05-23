using RestSharp;
using RestSharp.Deserializers;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using UboConfigTool.Domain;
using UboConfigTool.Domain.Models;
using UboConfigTool.Infrastructure;
using UboConfigTool.Infrastructure.Repository;

//namespace UboConfigTool.Repository
//{
    //[Export( typeof( IUBOGroupRepository ) )]
    //[PartCreationPolicy( CreationPolicy.Shared )]
    //public class UBOGroupRepository : IRepository<UBOGroup>, IUBOGroupRepository
    //{
    //    protected  Dictionary<object, UBOGroup> _repository = new Dictionary<object, UBOGroup>();

    //    public UBOGroupRepository()
    //    {

    //    }

    //    public void LoadData()
    //    {
    //        _repository.Clear();

    //        RestRequest request = new RestRequest( "ubo_groups", Method.GET );
    //        request.RequestFormat = DataFormat.Json;

    //        request.AddHeader( "Content-Type", "application/json" );

    //        RestClientInstance.Instance.Client.ExecuteAsync( request, response =>
    //        {
    //            if( response.ErrorException == null && ( response.StatusCode == System.Net.HttpStatusCode.OK
    //                || response.StatusCode == System.Net.HttpStatusCode.ResetContent ) )
    //            {
    //                JsonDeserializer jDeserializer = new JsonDeserializer();
    //                List<DTOUBOGroup> deviceLst = jDeserializer.Deserialize<List<DTOUBOGroup>>( response );
    //                if(deviceLst != null )
    //                {
    //                    deviceLst.ForEach( dtoUBOGrp =>
    //                        {
    //                            _repository.Add( dtoUBOGrp.id, new UBOGroup( dtoUBOGrp ) );

    //                        } );
    //                }
    //            }

    //        } );
    //    }

    //    public UBOGroup FindBy( object key )
    //    {
    //        return _repository[key];
    //    }

    //    public IList<UBOGroup> FindAll()
    //    {
    //        return _repository.Values.ToList<UBOGroup>().AsReadOnly();
    //    }

    //    public void Add( UBOGroup item )
    //    {
    //        if( !_repository.Keys.Contains( item.Key ) )
    //        {
    //            //var request = new RestRequest( RestResource, Method.POST );
    //            //request.AddJsonBody( item );
    //            //IRestResponse response = RestClientInstance.Instance.Client.Execute( request );
    //            //if( response.ResponseStatus == ResponseStatus.Completed )
    //            //{
    //            _repository.Add( item.Key, item );
    //            // }
    //        }
    //    }

    //    public UBOGroup this[object key]
    //    {
    //        get
    //        {
    //            if( !_repository.Keys.Contains( key ) )
    //                return null;

    //            return _repository[key];
    //        }
    //        set
    //        {
    //            //var request = new RestRequest( RestResource + '/' + key.ToString(), Method.PUT );
    //            //request.AddJsonBody( value );
    //            ////var response = RestClientInstance.Instance.Client.Execute( request );
    //            ////if( response.ResponseStatus == ResponseStatus.Completed )
    //            //{
    //            //    this._repository[key] = value;
    //            //}
    //        }
    //    }

    //    public void Remove( UBOGroup item )
    //    {
    //        //var request = new RestRequest( RestResource + "/" + item.Key, Method.DELETE );
    //        ////var response = RestClientInstance.Instance.Client.Execute( request );
    //        ////if( response.ResponseStatus == ResponseStatus.Completed )
    //        //{
    //        //    this._repository.Remove( item.Key );
    //        //}
    //    }

    //    public void ClearData()
    //    {
    //        _repository.Clear();
    //    }

    //    public IList<UBOGroup> GetAllUBOs()
    //    {
    //        LoadData();
    //        return this.FindAll();
    //    }
    //}
//}
