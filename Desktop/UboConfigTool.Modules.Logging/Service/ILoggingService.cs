using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UboConfigTool.Modules.Logging.Model;

namespace UboConfigTool.Modules.Logging.Service
{
    public delegate void LogDataBeginLoading();
    public delegate void LogDataEndLoading();

    public class LogDataParcel<T>
    {
        public LogDataParcel( List<T> logLst, int pageLimit, int totalPages )
        {
            _logs = logLst;
            _page_limit = pageLimit;
            _total_pages = totalPages;
        }

        private List<T> _logs;
        public List<T> logs
        {
            get
            {
                return _logs;
            }
        }

        private int _page_limit = 0;
        public int page_limit
        {
            get
            {
                return _page_limit;
            }
        }

        private int _total_pages = 0;
        public int total_pages
        {
            get
            {
                return _total_pages;
            }
        }
    }

    public interface ILoggingService
    {
        event LogDataBeginLoading LogDataBeginLoadingEvent;
        event LogDataEndLoading LogDataEndLoadingEvent;

        LogDataParcel<DeviceLogItem> DeviceLogs
        {
            get;
        }

        LogDataParcel<SystemLogItem> SystemLogs
        {
            get;
        }

        void LoadDeviceLogData();
        void SearchDeviceLogData( int pageIndex = 1, string stTime = "", string edTime = "", string keyword = "", string strLevel = "", string uboGroup = "", string uboName = "" );

        void LoadSystemLogData();
        void SearchSystemLogData( int pageIndex = 1, string stTime = "", string edTime = "", string keyword = "", string strLevel = "" );
    }
}
