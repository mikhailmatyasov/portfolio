using System;
using WeSafe.DAL.Entities;
using WeSafe.Services.Client.Models;

namespace WeSafe.Services
{
    public class DeviceLogMapper
    {      
        public DeviceLog ToDevice(DeviceLogModel model)
        {
            if (model == null) 
                throw new ArgumentNullException(nameof(model));

            return new DeviceLog
            {
                DeviceId = model.DeviceId,
                CameraId = model.CameraId, 
                DateTime = model.DateTime,
                ErrorMessage = model.ErrorMessage,
                LogLevel = model.LogLevel,    
            };
        }

        public DeviceLogModel ToDeviceLogModel(DeviceLog device)
        {
            if (device == null)
                throw new ArgumentNullException(nameof(device));

            return new DeviceLogModel
            {
                DeviceId = device.DeviceId,
                CameraId = device.CameraId,
                DateTime = device.DateTime,
                ErrorMessage = device.ErrorMessage,
                LogLevel = device.LogLevel
            };
        }       
    }

    
}
