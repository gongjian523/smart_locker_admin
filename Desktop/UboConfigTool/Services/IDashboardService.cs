using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UboConfigTool.Infrastructure.Interfaces;
using UboConfigTool.Infrastructure.Models;

namespace UboConfigTool.Services
{
    public interface IDashboardService
    {
        event DataBeginLoading DataBeginLoadingEvent;
        event DataEndLoading DataEndLoadingEvent;

        DashboardSummaryInfo GetDashboardSummaryInfo();
        List<DashboardLogInfo> GetLatestLogs();
        void RefreshData();
    }
}
