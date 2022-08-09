using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WeSafe.DAL;
using WeSafe.DAL.Entities;
using WeSafe.DAL.Extensions;
using WeSafe.Services.Client;
using WeSafe.Services.Client.Models;
using WeSafe.Shared;
using WeSafe.Shared.Enumerations;

namespace WeSafe.Services
{
    public class PlateEventService : BaseService, IPlateEventService
    {
        private readonly IDeviceService _deviceService;
        private readonly ICameraService _cameraService; 
        private readonly ICloudStorage _cloudStorage;

        public PlateEventService(WeSafeDbContext context, ILoggerFactory loggerFactory,
            IDeviceService deviceService, ICameraService cameraService, ICloudStorage cloudStorage) : base(context, loggerFactory)
        {
            _deviceService = deviceService ?? throw new ArgumentNullException(nameof(deviceService));
            _cameraService = cameraService ?? throw new ArgumentNullException(nameof(cameraService));
            _cloudStorage = cloudStorage ?? throw new ArgumentNullException(nameof(cloudStorage));
        }

        public async Task AddPlateEventAsync(PlateEventModel eventModel)
        {
            Validate(eventModel);

            var device = await GetDeviceAsync(eventModel.DeviceMac);
            var camera = GetCamera(device.Id, eventModel.CameraIp);

            var plateEvent = await GetPlateEventAsync(eventModel, device, camera);

            await DbContext.PlateEvents.AddAsync(plateEvent);
            await DbContext.SaveChangesAsync();
        }

        public async Task<PageResponse<PlateEventDisplayModel>> GetPlateEventsAsync(int deviceId, PlateEventSearchModel searchModel)
        {
            int total = 0;
            var events = DbContext.PlateEvents
                                  .Where(e => e.DeviceId == deviceId)
                                  .Include(e => e.Frames)
                                  .Include(e => e.Camera)
                                  .AsQueryable();
            var licensePlateRestrictions = await DbContext.LicensePlateRestrictions.Where(l => l.DeviceId == deviceId).ToListAsync();

            if (searchModel != null)
            {
                searchModel.Skip ??= 0;
                searchModel.Take ??= 25;

                ValidateSearchModel(searchModel);
                (events, total) = await GetFilteredEvents(events, searchModel);
            }

            var eventList = await events.ToListAsync();
            var displayEventModels = eventList.Select(e => GetPlateEventDisplayModel(e, licensePlateRestrictions)).ToList();

            return new PageResponse<PlateEventDisplayModel>(displayEventModels, total);
        }

        private void Validate(PlateEventModel eventModel)
        {
            if (eventModel == null)
                throw new ArgumentNullException(nameof(eventModel));

            if (string.IsNullOrEmpty(eventModel.CameraIp))
                throw new InvalidOperationException("CameraIp is required property for plate event.");

            if (string.IsNullOrEmpty(eventModel.DeviceMac))
                throw new InvalidOperationException("DeviceMac is required property for plate event.");

            if (string.IsNullOrEmpty(eventModel.PlateNumber))
                throw new InvalidOperationException("PlateNumber is required property for plate event.");
        }

        private async Task<DeviceModel> GetDeviceAsync(string mac)
        {
            var device = await _deviceService.GetDeviceByMAC(mac);
            if (device == null)
                throw new InvalidOperationException($"Device with MAC address {mac} is not found.");

            return device;
        }

        private Camera GetCamera(int deviceId, string cameraIp)
        {
            var camera = _cameraService.Cameras.FirstOrDefault(c => c.DeviceId == deviceId && c.Ip == cameraIp);
            if (camera == null)
                throw new InvalidOperationException($"Camera with ip {cameraIp} is not found.");

            return camera;
        }

        private async Task<PlateEvent> GetPlateEventAsync(PlateEventModel eventModel, DeviceModel device, Camera camera)
        {
            var frames = await GetFramesAsync(eventModel.FrameImage, eventModel.PlateImage);
            var plateEventStates = await GetPlateEventStateAsync(eventModel.PlateNumber);

            return new PlateEvent()
            {
                DeviceId = device.Id,
                CameraId = camera.Id,
                Frames = frames,
                PlateNumber = eventModel.PlateNumber,
                PlateEventState = plateEventStates,
                DateTime = eventModel.DateTime
            };
        }

        private async Task<ICollection<PlateEventState>> GetPlateEventStateAsync(string licensePlate)
        {
            var licensePlateRestrictions = await DbContext.LicensePlateRestrictions
                .Where(l => l.LicensePlate == licensePlate).Select(l => l.LicensePlateType).ToListAsync();
            
            return licensePlateRestrictions.Select(GetEventState).ToList();
        }

        private PlateEventState GetEventState(LicensePlateType licensePlateType)
        {
            return new PlateEventState()
            {
                State = licensePlateType
            };
        }

        private async Task<List<Frame>> GetFramesAsync(IFormFile frameImage, IFormFile plateImage)
        {
            return new List<Frame>()
            {
                new Frame()
                {
                    ImageUrl = await GetImageUrlAsync(frameImage),
                    ImageType = ImageType.PlateFrame
                },
                new Frame()
                {
                    ImageUrl = await GetImageUrlAsync(plateImage),
                    ImageType = ImageType.PlateNumber
                },
            };
        }

        private async Task<string> GetImageUrlAsync(IFormFile image)
        {
            var fileName = _cloudStorage.GetRandomFileName("jpg");
            string url;

            using (var imageStream = image.OpenReadStream())
            {
                url = await _cloudStorage.CreateFileAsync(fileName, image.ContentType, imageStream);
            }

            return url;

        }

        private void ValidateSearchModel(PlateEventSearchModel searchModel)
        {
            if (searchModel.StartDateTime > searchModel.EndDateTime)
                throw new InvalidOperationException("Start date time can't be greater that end date time.");

            if (searchModel.Skip < 0)
                throw new ArgumentOutOfRangeException($"The value {searchModel.Skip} can not be less than 0");

            if (searchModel.Take <= 0)
                throw new ArgumentOutOfRangeException($"The value {searchModel.Take} can not be 0");

            if (searchModel.Take > 100)
                throw new ArgumentOutOfRangeException(nameof(searchModel.Take),
                    $"You cannot take more 100 events per request");
        }

        private Task<(IQueryable<PlateEvent> Query, int Total)> GetFilteredEvents(IQueryable<PlateEvent> events, PlateEventSearchModel searchModel)
        {
            if (searchModel.StartDateTime != null)
            {
                var utcStartDate = searchModel.StartDateTime?.ToUniversalTime();
                events = events.Where(e => e.DateTime > utcStartDate);
            }

            if (searchModel.EndDateTime != null)
            {
                var utcEndDate = searchModel.EndDateTime?.ToUniversalTime();
                events = events.Where(e => e.DateTime < utcEndDate);
            }

            if (!string.IsNullOrEmpty(searchModel.PlateNumber))
            {
                events = events.Where(e => e.PlateNumber.ToLower().Contains(searchModel.PlateNumber.ToLower()));
            }

            return events.ApplyPageRequest(searchModel);
        }

        private PlateEventDisplayModel GetPlateEventDisplayModel(PlateEvent plateEvent, IEnumerable<LicensePlateRestriction> licensePlateRestrictions)
        {
            return new PlateEventDisplayModel()
            {
                CameraName = plateEvent.Camera.CameraName,
                DateTime = plateEvent.DateTime,
                FrameImageUrl = plateEvent.Frames.FirstOrDefault(f => f.ImageType == ImageType.PlateFrame)?.ImageUrl,
                PlateNumberImageUrl = plateEvent.Frames.FirstOrDefault(f => f.ImageType == ImageType.PlateNumber)?.ImageUrl,
                PlateNumberString = plateEvent.PlateNumber,
                LicensePlateType = licensePlateRestrictions.FirstOrDefault(l => l.LicensePlate == plateEvent.PlateNumber)?.LicensePlateType
            };
        }
    }
}
