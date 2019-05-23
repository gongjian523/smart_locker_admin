using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UboConfigTool.Infrastructure.DomainBase;

namespace UboConfigTool.Infrastructure.Repository
{
    public abstract class RepositoryBase<T> : IRepository<T> where T : class, IAggregateRoot
    {
        protected RepositoryBase()
        {

        }

        protected  Dictionary<object, T> _repository = new Dictionary<object, T>();

        public T FindBy( object key )
        {
            return _repository[key];
        }

        public IList<T> FindAll()
        {
            return _repository.Values.ToList<T>().AsReadOnly();
        }

        public void Add( T item )
        {
            if( !_repository.Keys.Contains( item.Key ) )
            {
                var request = new RestRequest( RestResource, Method.POST );
                request.AddJsonBody( item );
                //IRestResponse response = RestClientInstance.Instance.Client.Execute( request );
                //if( response.ResponseStatus == ResponseStatus.Completed )
                //{
                    _repository.Add( item.Key, item );
               // }
            }
        }

        public T this[object key]
        {
            get
            {
                if( !_repository.Keys.Contains( key ) )
                    return null;

                return _repository[key];
            }
            set
            {
                var request = new RestRequest( RestResource + '/' + key.ToString(), Method.PUT );
                request.AddJsonBody( value );
                //var response = RestClientInstance.Instance.Client.Execute( request );
                //if( response.ResponseStatus == ResponseStatus.Completed )
                {
                    this._repository[key] = value;
                }
            }
        }

        public void Remove( T item )
        {
            var request = new RestRequest( RestResource + "/" + item.Key, Method.DELETE );
            //var response = RestClientInstance.Instance.Client.Execute( request );
            //if( response.ResponseStatus == ResponseStatus.Completed )
            {
                this._repository.Remove( item.Key );
            }
        }

        public string RestResource
        {
            get;
            set;
        }

        public void LoadData()
        {
            _repository.Clear();

            RestRequest request = new RestRequest( RestResource, Method.GET );
            //IRestResponse<List<T>> response = RestClientInstance.Instance.Client.Execute<List<T>>( request );
            //if( response.ResponseStatus == ResponseStatus.Completed )
            //{
            //    foreach( T item in response.Data )
            //    {
            //        _repository.Add( item.Key, item );
            //    }
            //}
        }

        public void ClearData()
        {
            _repository.Clear();
        }
    }
}
