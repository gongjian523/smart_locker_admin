using Microsoft.Practices.Prism.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using UboConfigTool.Infrastructure.Models;
using UboConfigTool.Services;

namespace UboConfigTool.ViewModel
{

    [Export(typeof(DashboardViewModel))]

    public class DashboardViewModel : NotificationObject
    {
        private readonly IDashboardService _dashboardService;

        [ImportingConstructor]
        public DashboardViewModel(IDashboardService dashboardService)
        {
            if (dashboardService == null)
            {
                throw new ArgumentNullException("dashboardService");
            }

            this._dashboardService = dashboardService;
            _dashboardService.DataEndLoadingEvent += _dashboardService_DataEndLoadingEvent;
        }

        void _dashboardService_DataEndLoadingEvent()
        {
            Application.Current.Dispatcher.BeginInvoke( DispatcherPriority.Normal, new Action( () =>
            {
                DashboardSummaryInfo = _dashboardService.GetDashboardSummaryInfo();
                DashboardLogInfoList = _dashboardService.GetLatestLogs();
            } ) );
        }

        private DashboardSummaryInfo _dashboardSummaryInfo;
        public DashboardSummaryInfo DashboardSummaryInfo
        {
            get
            {
                return _dashboardSummaryInfo;
            }
            set
            {
                _dashboardSummaryInfo = value;
                this.RaisePropertyChanged(() => DashboardSummaryInfo);
            }
        }

        private List<DashboardLogInfo> _dashboardLogInfoList;
        public List<DashboardLogInfo> DashboardLogInfoList
        {
            get
            {
                return _dashboardLogInfoList;
            }
            set 
            {
                _dashboardLogInfoList = value;
                this.RaisePropertyChanged(() => DashboardLogInfoList);
            }
        }

    }
}
