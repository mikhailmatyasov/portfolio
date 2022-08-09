using System;
using WeSafe.Services.Client.Models;
using WeSafe.Web.Core.Models;

namespace WeSafe.Web.Core.Mappers
{
    public class ClientDeviceLogPresentationMapper
    {
        public ClientDeviceLogPresentationModel ToClientDeviceLogPresentationModel(ClientDeviceLogModel clientDeviceLogModel)
        {
            if (clientDeviceLogModel == null)
                throw new ArgumentNullException(nameof(clientDeviceLogModel));

            return new ClientDeviceLogPresentationModel
            {
                ClientName = clientDeviceLogModel.ClientName,
                DeviceName = clientDeviceLogModel.DeviceName,
                CameraName = clientDeviceLogModel.CameraName,
                LogLevel = clientDeviceLogModel.LogLevel.ToString("g"),
                ErrorMessage = clientDeviceLogModel.ErrorMessage,
                DateTime = clientDeviceLogModel.DateTime.ToUniversalTime()
            };
        }
    }
}
