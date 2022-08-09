using System;
using System.Linq.Expressions;
using WeSafe.DAL.Entities;
using WeSafe.Services.Client.Models;

namespace WeSafe.Services
{
    public class DeviceMapper
    {
        private readonly Func<Device, DeviceModel> _compiledProjection;

        public DeviceMapper()
        {
            _compiledProjection = Projection.Compile();
        }

        public Expression<Func<Device, DeviceModel>> Projection => device => new DeviceModel
        {
            Id = device.Id,
            Token = device.Token,
            ClientId = device.ClientId,
            ActivationDate = device.ActivationDate,
            AssemblingDate = device.AssemblingDate,
            ClientName = device.Client != null ? device.Client.Name : null,
            ClientNetworkIp = device.ClientNetworkIp,
            CreatedBy = device.CreatedBy,
            HWVersion = device.HWVersion,
            Info = device.Info,
            MACAddress = device.MACAddress,
            NVIDIASn = device.NVIDIASn,
            SWVersion = device.SWVersion,
            SerialNumber = device.SerialNumber,
            Status = device.Status,
            NetworkStatus = device.NetworkStatus,
            IsArmed = device.IsArmed,
            CamerasNumber = device.Cameras != null ? device.Cameras.Count : 0,
            Name = device.Name,
            CurrentSshPassword = device.CurrentSshPassword,
            MaxActiveCameras = device.MaxActiveCameras,
            DeviceType = device.DeviceType,
            TimeZone = device.TimeZone
        };

        public DeviceModel ToDeviceModel(Device device)
        {
            if ( device == null ) throw new ArgumentNullException(nameof(device));

            return _compiledProjection(device);
        }

        public Device ToDevice(DeviceModel model)
        {
            if ( model == null ) throw new ArgumentNullException(nameof(model));

            return new Device
            {
                Id = model.Id,
                Token = model.Token,
                ClientId = model.ClientId,
                ActivationDate = model.ActivationDate,
                AssemblingDate = model.AssemblingDate,
                ClientNetworkIp = model.ClientNetworkIp,
                CreatedBy = model.CreatedBy,
                HWVersion = model.HWVersion,
                Info = model.Info,
                MACAddress = model.MACAddress,
                NVIDIASn = model.NVIDIASn,
                SWVersion = model.SWVersion,
                SerialNumber = model.SerialNumber,
                Status = model.Status,
                NetworkStatus = model.NetworkStatus,
                IsArmed = model.IsArmed,
                Name = model.Name,
                CurrentSshPassword = model.CurrentSshPassword,
                MaxActiveCameras = model.MaxActiveCameras,
                DeviceType = model.DeviceType
            };
        }

        public Device ToDevice(Device device, DeviceModel model)
        {
            if ( model == null ) throw new ArgumentNullException(nameof(model));
            if ( device == null ) throw new ArgumentNullException(nameof(device));

            device.ClientId = model.ClientId;
            device.AssemblingDate = model.AssemblingDate;
            device.ClientNetworkIp = model.ClientNetworkIp;
            device.HWVersion = model.HWVersion;
            device.Info = model.Info;
            device.MACAddress = model.MACAddress;
            device.NVIDIASn = model.NVIDIASn;
            device.SWVersion = model.SWVersion;
            device.SerialNumber = model.SerialNumber;
            device.Status = model.Status;
            device.NetworkStatus = model.NetworkStatus;
            device.IsArmed = model.IsArmed;
            device.Name = model.Name;
            device.CurrentSshPassword = model.CurrentSshPassword;
            device.MaxActiveCameras = model.MaxActiveCameras;
            device.DeviceType = model.DeviceType;

            return device;
        }
    }
}