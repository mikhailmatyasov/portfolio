using System;
using WeSafe.Services.Client.Models;
using WeSafe.Web.Core.Models;

namespace WeSafe.Web.Core.Mappers
{
    public class DeviceLogFilterMapper
    {
        public ClientDeviceLogRecordQuery ToClientDeviceLogRecordQuery(IDeviceLogFilter deviceLogFilter)
        {
            if (deviceLogFilter == null)
                throw new ArgumentNullException(nameof(deviceLogFilter));

            var recordQuery = new ClientDeviceLogRecordQuery();

            recordQuery.SearchText = deviceLogFilter.SearchText;
            recordQuery.ClientId = deviceLogFilter.ClientId;
            recordQuery.DeviceId = deviceLogFilter.DeviceId;
            recordQuery.CameraId = deviceLogFilter.CameraId;
            recordQuery.FromDate = deviceLogFilter.FromDate;
            recordQuery.ToDate = deviceLogFilter.ToDate;
            recordQuery.LogLevels = deviceLogFilter.LogLevels;

            PageFilter pageFilter = deviceLogFilter as PageFilter;

            if (pageFilter != null)
            {
                recordQuery.Skip = pageFilter.Skip;
                recordQuery.Take = pageFilter.Take;
            }

            return recordQuery;
        }
    }
}
