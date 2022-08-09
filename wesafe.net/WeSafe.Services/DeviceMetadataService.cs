using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WeSafe.DAL;
using WeSafe.DAL.Entities;
using WeSafe.Services.Client;
using WeSafe.Services.Client.Models;

namespace WeSafe.Services
{
    public class DeviceMetadataService : BaseService, IDeviceMetadataService
    {
        public DeviceMetadataService(WeSafeDbContext context, ILoggerFactory loggerFactory) : base(context, loggerFactory)
        {
        }

        public async Task<string> GetDeviceMetadataAsync(int deviceId)
        {
            var device = await TryGetDevice(deviceId);

            return device.Metadata;
        }

        public async Task UpdateDeviceMetadataAsync(int deviceId, MetadataModel metadata)
        {
            var device = await TryGetDevice(deviceId);

            if ( metadata == null )
            {
                throw new ArgumentNullException(nameof(metadata));
            }

            device.Metadata = metadata.Metadata;

            await SaveChangesAsync();
        }

        public async Task UpdateCamerasMetadataAsync(int deviceId, IEnumerable<CameraMetadataModel> metadata)
        {
            await TryGetDevice(deviceId);

            if ( metadata == null )
            {
                throw new ArgumentNullException(nameof(metadata));
            }

            foreach ( var item in metadata )
            {
                var camera = await DbContext.Cameras.FirstOrDefaultAsync(c => c.DeviceId == deviceId && c.Id == item.Id);

                if ( camera == null )
                {
                    throw new InvalidOperationException("Camera not found");
                }

                camera.Metadata = item.Metadata;
            }

            await SaveChangesAsync();
        }

        private async Task<Device> TryGetDevice(int deviceId)
        {
            var device = await DbContext.Devices.FindAsync(deviceId);

            if ( device == null )
            {
                throw new InvalidOperationException("Device not found");
            }

            return device;
        }
    }
}