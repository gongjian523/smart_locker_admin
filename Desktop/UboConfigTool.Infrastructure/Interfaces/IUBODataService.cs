using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UboConfigTool.Infrastructure.Models;

namespace UboConfigTool.Infrastructure.Interfaces
{
    public delegate void DataBeginLoading();
    public delegate void DataEndLoading();

    public interface IUBODataService
    {
        event DataBeginLoading DataBeginLoadingEvent;
        event DataEndLoading DataEndLoadingEvent;

        IList<UBOGroup> GetAllUBOs();

        IList<UBOWithUsbInfo> GetUBOsForUSB(int usbId);

        void ReloadData();

        bool AddUBOIntoGroup( int groupId, string uboName, string uboSId, string uboBId );

        bool DeleteUBO( int uboId );

        bool RenameUsb(int usbId, string newName);
    }
}
