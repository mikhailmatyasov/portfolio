using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeZoneConverter;
using WeSafe.DAL;
using WeSafe.DAL.Entities;
using WeSafe.Services.Client;
using WeSafe.Services.Client.Models;
using Camera = WeSafe.DAL.Entities.Camera;
using Direction = WeSafe.Services.Client.Models.Direction;

namespace WeSafe.Services
{
    public class TrafficService : BaseService, ITrafficService
    {
        private IDeviceService _deviceService;
        private ICameraService _cameraService;

        public TrafficService(WeSafeDbContext context, ILoggerFactory loggerFactory, IDeviceService deviceService, ICameraService cameraService) : base(context, loggerFactory)
        {
            _deviceService = deviceService ?? throw new ArgumentNullException(nameof(deviceService));
            _cameraService = cameraService ?? throw new ArgumentNullException(nameof(cameraService));
        }

        public async Task AddTrafficEvents(IEnumerable<TrafficEventModel> trafficEvents)
        {
            Validate(trafficEvents);

            var devices = await GetDevices(trafficEvents.Select(x => x.DeviceMAC).Distinct());

            var cameras = await GetCameras(devices.Select(x => x.Id).Distinct());

            var events = GetTrafficEvents(trafficEvents, devices, cameras);

            await DbContext.TrafficEvents.AddRangeAsync(events);
            await DbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<TrafficChartModel>> GetTrafficHourlyChart(TrafficHourlyChartRequest request)
        {
            var device = await DbContext.Devices.FindAsync(request.DeviceId);

            if ( device == null )
            {
                throw new InvalidOperationException($"The device with Id {request.DeviceId} not exist in the database.");
            }

            var startDate = request.Date.Date.ToUniversalTime();
            var endDate = startDate.AddDays(1);
            var trafficEvents = await DbContext.TrafficEvents
                                               .Where(c => c.DeviceMAC == device.MACAddress && c.CameraId == request.CameraId &&
                                                           c.UtcDateTime >= startDate && c.UtcDateTime < endDate)
                                               .AsNoTracking()
                                               .Select(c => new
                                               {
                                                   c.UtcDateTime,
                                                   c.Direction
                                               })
                                               .ToListAsync();

            if ( !String.IsNullOrEmpty(device.TimeZone) )
            {
                var timeZoneInfo = TZConvert.GetTimeZoneInfo(device.TimeZone);

                trafficEvents = trafficEvents.Select(c => new
                                             {
                                                 UtcDateTime = TimeZoneInfo.ConvertTimeFromUtc(c.UtcDateTime, timeZoneInfo),
                                                 c.Direction
                                             })
                                             .ToList();
            }

            var chartItems = new List<TrafficChartModel>();

            if ( trafficEvents.Count == 0 )
            {
                return chartItems;
            }

            for ( int hour = 0; hour < 24; hour++ )
            {
                var events = trafficEvents.Where(c => c.UtcDateTime.Hour == hour).ToArray();

                chartItems.Add(new TrafficChartModel
                {
                    CameraId = request.CameraId,
                    Mark = $"{hour:D2}:00",
                    Entered = events.Count(c => c.Direction == DAL.Entities.Direction.In),
                    Exited = events.Count(c => c.Direction == DAL.Entities.Direction.Out)
                });
            }

            return chartItems;
        }

        public async Task<IEnumerable<TrafficCountModel>> GetTraffic(TrafficSearchModel searchModel)
        {
            ValidateSearchModel(searchModel);

            return await GetTrafficCount(searchModel);
        }

        private async Task<IEnumerable<TrafficCountModel>> GetTrafficCount(TrafficSearchModel searchModel)
        {
            var cameras = await GetCameras(new[] { searchModel.DeviceId });
            if (!cameras.Any())
                throw new InvalidOperationException(
                    $"Not found any camera for device with identifier {searchModel.DeviceId}");

            var cameraIds = cameras.Select(x => x.Id).ToArray();
            var utcStartDate = searchModel.StartDateTime?.ToUniversalTime();
            var utcEndDate = searchModel.EndDateTime?.ToUniversalTime();

            // [EM]: We can't use Count() method inside GroupBy method. Entity framework will not transform the query to SQL.
            var trafficCount = await DbContext.TrafficEvents
                .Where(x => cameraIds.Contains(x.CameraId) && (utcStartDate == null || x.UtcDateTime >= utcStartDate) &&
                            (utcEndDate == null || x.UtcDateTime <= utcEndDate)).GroupBy(x => x.CameraId, i => new
                            {
                                Entered = i.Direction == DAL.Entities.Direction.In ? 1 : 0,
                                Exited = i.Direction == DAL.Entities.Direction.Out ? 1 : 0,
                            }).Select(x => new TrafficCountModel
                            {
                                CameraId = x.Key,
                                Entered = x.Sum(e => e.Entered),
                                Exited = x.Sum(e => e.Exited),
                            }).ToListAsync();

            trafficCount.ForEach(t => t.CameraName = cameras.First(c => c.Id == t.CameraId).CameraName);

            return trafficCount;
        }

        private void ValidateSearchModel(TrafficSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (searchModel.StartDateTime > searchModel.EndDateTime)
                throw new InvalidOperationException("Start date time can't be greater that end date time.");
        }


        private IList<TrafficEvent> GetTrafficEvents(IEnumerable<TrafficEventModel> trafficEvents, IList<DeviceModel> deviceModels, IList<Camera> cameras)
        {
            return trafficEvents.Select(x => GetTrafficEvents(x, deviceModels, cameras)).ToList();
        }

        private TrafficEvent GetTrafficEvents(TrafficEventModel trafficEvent, IList<DeviceModel> deviceModels,
            IList<Camera> cameras)
        {
            return new TrafficEvent()
            {
                ObjectId = trafficEvent.ObjectId,
                DeviceMAC = trafficEvent.DeviceMAC,
                UtcDateTime = trafficEvent.UtcDateTime.ToUniversalTime(),
                CameraId = GetCameraId(trafficEvent, deviceModels, cameras),
                Direction = trafficEvent.Direction == Direction.In ? DAL.Entities.Direction.In : DAL.Entities.Direction.Out
            };
        }

        private int GetCameraId(TrafficEventModel trafficEvent, IList<DeviceModel> deviceModels,
            IList<Camera> cameras)
        {
            var device = FindDeviceByMac(deviceModels, trafficEvent.DeviceMAC);

            return FindCamera(cameras, device.Id, trafficEvent.CameraIp).Id;
        }

        private DeviceModel FindDeviceByMac(IEnumerable<DeviceModel> deviceModels, string mac)
        {
            var device = deviceModels.FirstOrDefault(x => x.MACAddress == mac);
            if (device == null)
                throw new InvalidOperationException($"The device address with MAC address {mac} not exist in the database.");

            return device;
        }

        private Camera FindCamera(IEnumerable<Camera> cameras, int deviceId, string ipAddress)
        {
            var camera = cameras.FirstOrDefault(x => x.DeviceId == deviceId && x.Ip == ipAddress);
            if (camera == null)
                throw new InvalidOperationException(
                    $"The camera with IP Address {ipAddress} not exist for device with device identifier {deviceId}");

            return camera;
        }

        private async Task<IList<Camera>> GetCameras(IEnumerable<int> deviceIds)
        {
            return await _cameraService.Cameras.Where(x => deviceIds.Contains(x.DeviceId)).ToListAsync();
        }

        private void Validate(IEnumerable<TrafficEventModel> trafficEvents)
        {
            if (trafficEvents == null)
                throw new ArgumentNullException(nameof(trafficEvents));

            if (trafficEvents.Any(x => string.IsNullOrEmpty(x.CameraIp)))
                throw new InvalidOperationException("CameraIp is required property for traffic event.");

            if (trafficEvents.Any(x => string.IsNullOrEmpty(x.DeviceMAC)))
                throw new InvalidOperationException("DeviceMAC is required property for traffic event.");
        }

        private async Task<IList<DeviceModel>> GetDevices(IEnumerable<string> macAddresses)
        {
            var devicesList = new List<DeviceModel>();
            foreach (var macAddress in macAddresses)
            {
                devicesList.Add(await GetDevice(macAddress));
            }

            return devicesList;
        }

        private async Task<DeviceModel> GetDevice(string mac)
        {
            var device = await _deviceService.GetDeviceByMAC(mac);
            if (device == null)
                throw new InvalidOperationException($"Device with MAC address {mac} not found.");

            return device;
        }
    }
}
