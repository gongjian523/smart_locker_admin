using RestSharp;
using RestSharp.Deserializers;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UboConfigTool.Infrastructure;
using UboConfigTool.Modules.Logging.Model;

namespace UboConfigTool.Modules.Logging.Service
{
    [Export( typeof( ILoggingService ) )]
    [PartCreationPolicy( CreationPolicy.Shared )]
    public class LoggingService : ILoggingService
    {
        public event LogDataBeginLoading LogDataBeginLoadingEvent;
        public event LogDataEndLoading LogDataEndLoadingEvent;

        public LoggingService()
        {

        }

        private LogDataParcel<DeviceLogItem> _deviceLogs = new LogDataParcel<DeviceLogItem>( new List<DeviceLogItem>(), 0, 0 );
        public LogDataParcel<DeviceLogItem> DeviceLogs
        {
            get
            {
                return _deviceLogs;
            }
        }

        private LogDataParcel<SystemLogItem> _systemLogs = new LogDataParcel<SystemLogItem>( new List<SystemLogItem>(), 0, 0 );
        public LogDataParcel<SystemLogItem> SystemLogs
        {
            get
            {
                return _systemLogs;
            }
        }

        public void LoadDeviceLogData()
        {
            SearchDeviceLogData();
        }

        public void SearchDeviceLogData( int pageIndex = 1, string stTime = "", string edTime = "", string keyword = "", string strLevel = "", string uboGroup = "", string uboName = "" )
        {
            RestRequest request = new RestRequest( "device_logs/search", Method.POST );
            RestClientInstance.Instance.InitRequestHeaderInfo( request );
            request.AddBody( new
            {
                searchparms = new
                {
                    page = pageIndex,
                    start_time = stTime,
                    end_time = edTime,
                    keywords = keyword,
                    level = strLevel,
                    ubo_group = uboGroup,
                    ubo = uboName
                }
            } );

            if( LogDataBeginLoadingEvent != null )
                LogDataBeginLoadingEvent();

            _deviceLogs = new LogDataParcel<DeviceLogItem>( new List<DeviceLogItem>(), 0, 0 );

            RestClientInstance.Instance.Client.ExecuteAsync( request, response =>
                {
                    if( response.ErrorException == null && (response.StatusCode == System.Net.HttpStatusCode.OK
                        || response.StatusCode == System.Net.HttpStatusCode.ResetContent))
                    {
                        RestClientInstance.Instance.ResetAccessTokenFrom( response.Headers );
                        JsonDeserializer jDeserializer = new JsonDeserializer();
                        jDeserializer.RootElement = "device_logs";
                        List<DeviceLogItem> deviceLst = jDeserializer.Deserialize<List<DeviceLogItem>>( response );
                        jDeserializer.RootElement = "page_limit";
                        int pageLimit = jDeserializer.Deserialize<int>( response );
                        jDeserializer.RootElement = "total_records";
                        int totalPage = jDeserializer.Deserialize<int>( response );

                        _deviceLogs = new LogDataParcel<DeviceLogItem>( deviceLst, pageLimit, totalPage );
                    }

                    if( LogDataEndLoadingEvent != null )
                        LogDataEndLoadingEvent();
                } );
        }

        public void LoadSystemLogData()
        {
            SearchSystemLogData();
        }

        public void SearchSystemLogData( int pageIndex = 1, string stTime = "", string edTime = "", string keyword = "", string strLevel = "" )
        {
            RestRequest request = new RestRequest( "system_logs/search", Method.POST );
            RestClientInstance.Instance.InitRequestHeaderInfo( request );
            request.AddBody( new
            {
                searchparms = new
                {
                    page = pageIndex,
                    start_time = stTime,
                    end_time = edTime,
                    keywords = keyword,
                    level = strLevel
                }
            } );

            if( LogDataBeginLoadingEvent != null )
                LogDataBeginLoadingEvent();

            RestClientInstance.Instance.Client.ExecuteAsync( request, response =>
            {
                if( response.ErrorException == null && ( response.StatusCode == System.Net.HttpStatusCode.OK
                    || response.StatusCode == System.Net.HttpStatusCode.ResetContent ) )
                {
                    RestClientInstance.Instance.ResetAccessTokenFrom( response.Headers );

                    JsonDeserializer jDeserializer = new JsonDeserializer();
                    jDeserializer.RootElement = "system_logs";
                    List<SystemLogItem> systemLogLst = jDeserializer.Deserialize<List<SystemLogItem>>( response );
                    jDeserializer.RootElement = "page_limit";
                    int pageLimit = jDeserializer.Deserialize<int>( response );
                    jDeserializer.RootElement = "total_records";
                    int totalPage = jDeserializer.Deserialize<int>( response );

                    _systemLogs = new LogDataParcel<SystemLogItem>( systemLogLst, pageLimit, totalPage );
                }

                if( LogDataEndLoadingEvent != null )
                    LogDataEndLoadingEvent();
            } );
        }
    }
}
