using System;
using WeSafe.Services.Client.Models;
using WeSafe.Web.Core.Models;

namespace WeSafe.Web.Core.Mappers
{
    public class DeviceLogPresentationMapper
    {
        public DeviceLogModel ToDeviceLogModel(DeviceLogPresentationModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            return new DeviceLogModel
            {
                DeviceId = model.DeviceId,
                CameraId = model.CameraId,
                DateTime = model.DateTime,
                ErrorMessage = model.ErrorMessage,
                LogLevel = model.LogLevel,
            };
        }
    }
}
