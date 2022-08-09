using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WeSafe.DAL;
using WeSafe.DAL.Entities;
using WeSafe.DAL.Extensions;
using WeSafe.Services.Client;
using WeSafe.Services.Client.Models;
using WeSafe.Shared;
using WeSafe.Shared.Enumerations;
using WeSafe.Shared.Extensions;
using WeSmart.Alpr.Core.Scheduler;
using SchedulerSerializer = WeSmart.Alpr.Core.Scheduler.SchedulerSerializer;

namespace WeSafe.Services
{
    public class DetectedCameraService : BaseService, IDetectedCameraService
    {
        private readonly IFileStorage _fileStorage;

        public DetectedCameraService(WeSafeDbContext context, ILoggerFactory loggerFactory, IFileStorage fileStorage) : base(context,
            loggerFactory)
        {
            _fileStorage = fileStorage;
        }

        public async Task<IReadOnlyCollection<DetectedCameraModel>> GetDetectedCamerasAsync(int deviceId)
        {
            return await BuildQuery()
                         .Where(c => c.DeviceId == deviceId)
                         .OrderBy(c => c.Name)
                         .ToListAsync();
        }

        public async Task<(IQueryable<DetectedCameraModel> Query, int Total)> GetDetectedCamerasAsync(int deviceId, PageRequest request)
        {
            return await BuildQuery()
                .Where(c => c.DeviceId == deviceId)
                .OrderBy(c => c.Name).ApplyPageRequest(request);
        }

        public async Task<DetectedCameraModel> GetDetectedCameraByIdAsync(int id)
        {
            return await BuildQuery().FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task CreateDetectedCameraAsync(string macAddress, CreateDetectedCameraModel model)
        {
            if (String.IsNullOrEmpty(macAddress))
            {
                throw new ArgumentNullException(nameof(macAddress));
            }

            var device = await DbContext.Devices.FirstOrDefaultAsync(c => c.MACAddress == macAddress);

            if (device == null)
            {
                throw new InvalidOperationException($"Device with {macAddress} MAC address is not found.");
            }

            Validate(device.Id, model);

            var camera = new DetectedCamera
            {
                DeviceId = device.Id,
                Name = model.Name ?? "Unnamed",
                Ip = model.Ip,
                Port = model.Port,
                State = CameraState.Detected,
                DetectingMethod = model.DetectingMethod
            };

            DbContext.DetectedCameras.Add(camera);

            await SaveChangesAsync();
        }

        public async Task ConnectingDetectedCameraAsync(ConnectingDetectedCameraModel model)
        {
            Validate(model);

            var camera = await DbContext.DetectedCameras.FindAsync(model.Id);

            if (camera == null)
            {
                throw new InvalidOperationException("Camera is not found.");
            }

            camera.Login = model.Login;
            camera.Password = model.Password;
            camera.State = CameraState.Connecting;
            camera.ConnectFailureText = null;

            await SaveChangesAsync();
        }

        public async Task ConnectDetectedCameraAsync(ConnectDetectingCameraModel model)
        {
            Validate(model);

            var detectedCamera = await DbContext.DetectedCameras.FirstOrDefaultAsync(c => c.Id == model.Id && c.DeviceId == model.DeviceId);

            if (detectedCamera == null)
            {
                throw new InvalidOperationException("Detected camera is not found.");
            }

            var device = await DbContext.Devices.FindAsync(detectedCamera.DeviceId);
            int activeCameras = await DbContext.Cameras.CountAsync(c => c.DeviceId == detectedCamera.DeviceId && c.IsActive);
            bool active = device.MaxActiveCameras == null || activeCameras < device.MaxActiveCameras.Value;
            var fileName = _fileStorage.GetRandomFileName("jpg");

            await _fileStorage.CreateFileAsync(fileName, model.Frame.OpenReadStream());

            var camera = new Camera
            {
                CameraName = detectedCamera.Name,
                Port = detectedCamera.Port,
                Ip = detectedCamera.Ip,
                IsActive = active,
                Login = detectedCamera.Login,
                Password = detectedCamera.Password.Encrypt(),
                SpecificRtcpConnectionString = model.RtspConnection.Encrypt(),
                DeviceId = detectedCamera.DeviceId,
                LastImagePath = _fileStorage.GetFileUrl(fileName),
                LastActivityTime = DateTimeOffset.UtcNow,
                Schedule = SchedulerSerializer.Serialize(DateTimeScheduler.DefaultWeekDaysHourScheduler())
            };

            DbContext.Cameras.Add(camera);

            await SaveChangesAsync();

            DbContext.DetectedCameras.Remove(detectedCamera);

            await SaveChangesAsync();
        }

        public async Task FailureDetectedCameraAsync(FailureDetectingCameraModel model)
        {
            Validate(model);

            var detectedCamera = await DbContext.DetectedCameras.FirstOrDefaultAsync(c => c.Id == model.Id && c.DeviceId == model.DeviceId);

            if (detectedCamera == null)
            {
                throw new InvalidOperationException("Detected camera is not found.");
            }

            detectedCamera.ConnectFailureText = model.FailureText;
            detectedCamera.State = CameraState.Failure;

            await SaveChangesAsync();
        }

        public async Task RemoveDetectedCameraAsync(int id)
        {
            var detectedCamera = await DbContext.DetectedCameras.FindAsync(id);

            if (detectedCamera == null)
            {
                throw new InvalidOperationException("Detected camera is not found.");
            }

            DbContext.DetectedCameras.Remove(detectedCamera);

            await SaveChangesAsync();
        }

        private IQueryable<DetectedCameraModel> BuildQuery()
        {
            return DbContext.DetectedCameras
                            .Select(c => new DetectedCameraModel
                            {
                                Id = c.Id,
                                Ip = c.Ip,
                                Port = c.Port,
                                Name = c.Name,
                                Login = c.Login,
                                Password = c.Password,
                                State = c.State,
                                DetectingMethod = c.DetectingMethod,
                                ConnectFailureText = c.ConnectFailureText,
                                DeviceId = c.DeviceId
                            });
        }

        private void Validate(int deviceId, CreateDetectedCameraModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (String.IsNullOrEmpty(model.Ip))
            {
                throw new ValidationException("IP is required.");
            }

            if (String.IsNullOrEmpty(model.Port))
            {
                throw new ValidationException("Port is required.");
            }
        }

        private void Validate(ConnectingDetectedCameraModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (String.IsNullOrEmpty(model.Login))
            {
                throw new ValidationException("Login is required.");
            }

            if (String.IsNullOrEmpty(model.Password))
            {
                throw new ValidationException("Password is required.");
            }
        }

        private void Validate(ConnectDetectingCameraModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (String.IsNullOrEmpty(model.RtspConnection))
            {
                throw new ValidationException("RTSP connection string is required.");
            }

            if (model.Frame == null)
            {
                throw new ValidationException("Frame is required.");
            }
        }

        private static void Validate(FailureDetectingCameraModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (String.IsNullOrEmpty(model.FailureText))
            {
                throw new ValidationException("FailureText is required.");
            }
        }
    }
}