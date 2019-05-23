using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace UboConfigTool.Infrastructure
{
    public class RestClientInstance
    {
        private RestClient  _restClient;

        private RestClientInstance()
        {
            BaseUri = "http://192.168.28.213:4000";
            _restClient = new RestClient( BaseUri );
        }

        private static readonly RestClientInstance _instance = new RestClientInstance();

        public static RestClientInstance Instance
        {
            get
            {
                return _instance;
            }
        }

        public string AccessToken
        {
            get;
            set;
        }

        public string UID
        {
            get;
            set;
        }

        public string TokenType
        {
            get;
            set;
        }

        public string ClientText
        {
            get;
            set;
        }

        public string Expiry
        {
            get;
            set;
        }

        public string IfModifiedSince
        {
            get;
            set;
        }

        public string BaseUri
        {
            get;
            set;
        }

        public RestClient Client
        {
            get
            {
                return _restClient;
            }
        }

        public static string guidTest = Guid.NewGuid().ToString();

        public void InitRequestHeaderInfo(RestRequest req)
        {
            Debug.WriteLine( guidTest );
            req.RequestFormat = DataFormat.Json;
            req.AddParameter( "access-token", RestClientInstance.Instance.AccessToken );
            req.AddParameter( "uid", RestClientInstance.Instance.UID );
            req.AddParameter( "token-type", RestClientInstance.Instance.TokenType );
            req.AddParameter( "client", RestClientInstance.Instance.ClientText );
        }

        public void ResetAccessTokenFrom(IList<Parameter> paras)
        {
            if( paras == null )
                return;

            Parameter accssPara = paras.FirstOrDefault( item => item.Name == "access-token" );
            if( accssPara == null )
                return;

            AccessToken = accssPara.Value.ToString();
        }
    }
}
