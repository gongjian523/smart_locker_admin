using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UboConfigTool.Infrastructure;
using UboConfigTool.Infrastructure.Interfaces;
using UboConfigTool.Infrastructure.Models;

namespace UboConfigTool.Services
{

    [Export( typeof( IDashboardService ) )]
    [PartCreationPolicy( CreationPolicy.Shared)]
    public class DashboardService : IDashboardService
    {
        public event DataBeginLoading DataBeginLoadingEvent;
        public event DataEndLoading DataEndLoadingEvent;

        private DashboardSummaryInfo _dashboardSummaryInfo;
        private List<DashboardLogInfo> _dashboardLogInfoList = new List<DashboardLogInfo>();

        public DashboardSummaryInfo GetDashboardSummaryInfo()
        {
            return _dashboardSummaryInfo;
        }

        public List<DashboardLogInfo> GetLatestLogs()
        {
            return _dashboardLogInfoList;
        }

        public void RefreshData()
        {
            if( DataBeginLoadingEvent != null )
                DataBeginLoadingEvent();

            RestRequest request = new RestRequest( "dashboard/index", Method.GET );
            RestClientInstance.Instance.InitRequestHeaderInfo( request );
            Task<IRestResponse<DashboardSummaryInfo>> summaryInfoTask = RestClientInstance.Instance.Client
                        .ExecuteGetTaskAsync<DashboardSummaryInfo>( request );

            summaryInfoTask.ContinueWith
                ( ( ctx ) =>
                {
                    RestClientInstance.Instance.ResetAccessTokenFrom( summaryInfoTask.Result.Headers );
                    _dashboardSummaryInfo = summaryInfoTask.Result.Data;
                    _dashboardLogInfoList.Clear();
                    RestRequest logRequest = new RestRequest( "dashboard/latest_logs/10", Method.GET );
                    RestClientInstance.Instance.InitRequestHeaderInfo( logRequest );
                    IRestResponse<List<DashboardLogInfo>> response = RestClientInstance.Instance.Client.Execute<List<DashboardLogInfo>>( request );
                    RestClientInstance.Instance.ResetAccessTokenFrom( response.Headers );
                    if(response.Data != null)
                        _dashboardLogInfoList.AddRange( response.Data );

                    if( DataEndLoadingEvent != null )
                        DataEndLoadingEvent();
                }
                );

        }
    }
}
