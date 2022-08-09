using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeZoneConverter;
using WeSafe.DAL.Entities;
using WeSafe.DAL.Extensions;
using WeSafe.Services;
using WeSafe.Services.Client;
using WeSafe.Services.Client.Models;
using WeSafe.Shared;
using WeSafe.Shared.Results;
using WeSafe.Shared.Roles;
using WeSafe.Web.Core.Models;

namespace WeSafe.Web.Core.Controllers
{
    /// <summary>
    /// Represents authorized user operations.
    /// </summary>
    [Route("api/v{version:apiVersion}/[controller]")]
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        #region Fields

        private readonly UserManager<User> _userManager;
        private readonly IDeviceService _deviceService;
        private readonly ICameraService _cameraService;
        private readonly CameraMapper _mapper;
        private readonly IClientSubscriberService _clientSubscriberService;
        private readonly IClientService _clientService;
        private readonly IDeviceIndicatorsService _deviceIndicatorsService;
        private readonly IDetectedCameraService _detectedCameraService;

        #endregion

        #region Ctor

        public ClientController(UserManager<User> userManager, IDeviceService deviceService,
            ICameraService cameraService, CameraMapper mapper, IClientSubscriberService clientSubscriberService,
            IClientService clientService, IDeviceIndicatorsService deviceIndicatorsService, IDetectedCameraService detectedCameraService)
        {
            _userManager = userManager;
            _deviceService = deviceService;
            _cameraService = cameraService;
            _mapper = mapper;
            _clientSubscriberService = clientSubscriberService;
            _clientService = clientService;
            _deviceIndicatorsService = deviceIndicatorsService;
            _detectedCameraService = detectedCameraService;
        }

        #endregion

        #region PublicMethods

        /// <summary>
        /// Gets authorized user devices.
        /// </summary>
        /// <returns>The device collection.</returns>
        [HttpGet("devices")]
        [Authorize("RequireUsersRole")]
        public async Task<IActionResult> GetDevicesAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user.ClientId == null) return BadRequest();

            var devices = await _deviceService.GetClientDevices(user.ClientId.Value);

            return Ok(devices);
        }

        /// <summary>
        /// Gets authorized user system status.
        /// </summary>
        /// <returns>The system status.</returns>
        [HttpGet("systemstatus")]
        [Authorize("RequireUsersRole")]
        public async Task<IActionResult> GetSystemStatusAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user.ClientId == null) return BadRequest();

            var systemStatus = await _clientService.GetSystemSettings(user.ClientId.Value);

            return Ok(systemStatus);
        }

        /// <summary>
        /// Gets authorized user events.
        /// </summary>
        /// <param name="skip">The number of skipped events for pagination.</param>
        /// <param name="take">The number of taken events for pagination.</param>
        /// <param name="sort">The sorted field indicator for filtering events.</param>
        /// <param name="deviceId">The device identifier for filtering events.</param>
        /// <param name="cameraId">The camera identifier for filtering events.</param>
        /// <param name="fromDate">The date from which events should be selected.</param>
        /// <param name="toDate">The date to which events should be selected.</param>
        /// <returns>The filtered event collection.</returns>
        [HttpGet("events")]
        [Authorize("RequireUsersRole")]
        public async Task<IActionResult> GetEvents(int? skip = null, int? take = null, string sort = null,
            int? deviceId = null, int? cameraId = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user.ClientId == null) return BadRequest();

            if (fromDate != null) fromDate = DateTime.SpecifyKind(fromDate.Value, DateTimeKind.Utc);
            if (toDate != null) toDate = DateTime.SpecifyKind(toDate.Value, DateTimeKind.Utc);

            return Ok(await _clientService.GetEvents(user.ClientId.Value, new EventRequest
            {
                DeviceId = deviceId,
                CameraId = cameraId,
                Skip = skip,
                Take = take ?? 50,
                SortBy = PageRequest.ParseSort(sort),
                FromDate = fromDate,
                ToDate = toDate
            }));
        }

        /// <summary>
        /// Gets user's filtered events.
        /// </summary>
        /// <param name="query">The filtering query.</param>
        /// <returns>The filtered event collection.</returns>
        [HttpGet("search_events")]
        [Authorize("RequireUsersRole")]
        public async Task<IActionResult> SearchEvents([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest($"Parameter {nameof(query)} cannot be empty.");

            var searchRequest = JsonConvert.DeserializeObject<EventSearchRequest>(query);

            var user = await _userManager.GetUserAsync(User);

            return Ok(await _clientService.GetEvents(user.ClientId.Value, searchRequest));
        }

        /// <summary>
        /// Gets device.
        /// </summary>
        /// <param name="deviceId">The device identifier.</param>
        /// <returns>The device.</returns>
        [HttpGet("devices/{deviceId}")]
        [Authorize("RequireUsersRole")]
        public async Task<IActionResult> GetDeviceByIdAsync(int deviceId)
        {
            var device = await _deviceService.GetDeviceById(deviceId);

            if (device == null) return Ok(null);

            return Ok(device);
        }

        /// <summary>
        /// Gets device indicators.
        /// </summary>
        /// <param name="deviceId">The device identifier.</param>
        /// <param name="from">The start time to get indicators.</param>
        /// <param name="to">The end time to get indicators.</param>
        /// <returns>The device.</returns>
        [HttpGet("devices/{deviceId}/indicators")]
        [Authorize("RequireUsersRole")]
        public async Task<IActionResult> GetDeviceIndicatorsAsync(int deviceId, DateTimeOffset? from = null, DateTimeOffset? to = null)
        {
            var device = await _deviceService.GetDeviceById(deviceId);

            if (device == null)
            {
                return BadRequest("Device is not found");
            }

            return Ok(await _deviceIndicatorsService.GetDeviceIndicators(deviceId, from, to));
        }

        /// <summary>
        /// Gets device cameras.
        /// </summary>
        /// <param name="deviceId">The device identifier.</param>
        /// <returns>The device camera collection.</returns>
        [HttpGet("devices/{deviceId}/cameras")]
        [Authorize]
        public async Task<IActionResult> GetDeviceCamerasAsync([FromRoute] int deviceId, [FromQuery] PageRequest pageRequest)
        {
            var user = await _userManager.GetUserAsync(User);
            var role = (await _userManager.GetRolesAsync(user)).Single();

            if (role != UserRoles.Administrators)
                if (!await CheckClientDevice(deviceId))
                    return BadRequest();


            var page = await _cameraService.Cameras
                .Where(c => c.DeviceId == deviceId)
                .ApplyPageRequest(pageRequest);


            var cameras = await page.Query.OrderBy(c => c.CameraName).Select(_mapper.Projection).ToListAsync();

            return Ok(new PageResponse<CameraModel>()
            {
                Items = cameras,
                Total = page.Total
            });
        }

        /// <summary>
        /// Gets device's camera status.
        /// </summary>
        /// <param name="deviceId">The device identifier.</param>
        /// <returns>The device's camera status.</returns>
        [HttpGet("devices/{deviceId}/cameras-stat")]
        [Authorize("RequireUsersRole")]
        public async Task<IActionResult> GetDeviceCamerasStatAsync(int deviceId)
        {
            if (!await CheckClientDevice(deviceId))
            {
                return BadRequest();
            }

            var device = await _deviceService.GetDeviceById(deviceId);
            var cameras = await _cameraService.Cameras
                                              .Where(c => c.DeviceId == deviceId)
                                              .Select(c => c.IsActive)
                                              .ToListAsync();

            var result = new CamerasStatModel
            {
                Count = cameras.Count,
                ActiveCount = cameras.Count(c => c),
                MaxActiveCameras = device.MaxActiveCameras
            };

            return Ok(result);
        }

        /// <summary>
        /// Gets camera.
        /// </summary>
        /// <param name="deviceId">The device identifier.</param>
        /// <param name="id">The camera identifier.</param>
        /// <returns>The camera.</returns>
        [HttpGet("devices/{deviceId}/cameras/{id}")]
        [Authorize("RequireUsersRole")]
        public async Task<IActionResult> GetCameraByIdAsync(int deviceId, int id)
        {
            if (!await CheckClientDevice(deviceId)) return BadRequest();

            var camera = await _cameraService.GetByIdAsync(id);

            if (camera == null) return Ok(null);

            return Ok(_mapper.ToCameraModel(camera));
        }

        /// <summary>
        /// Adds camera to the storage.
        /// </summary>
        /// <param name="deviceId">The device identifier.</param>
        /// <param name="model">The camera model.</param>
        /// <returns>The action result.</returns>
        [HttpPost("devices/{deviceId}/cameras")]
        [Authorize("RequireUsersRole")]
        public async Task<IActionResult> CreateCameraAsync(int deviceId, [FromBody] CameraModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (!await CheckClientDevice(deviceId))
            {
                return BadRequest();
            }

            await CheckActiveCamerasCount(deviceId, model);

            model.DeviceId = deviceId;
            var result = await _cameraService.CreateAsync(_mapper.ToCamera(model));

            if (!result.IsSuccess)
                throw new InvalidOperationException(result.ToString(), null);

            return Ok(new { Id = ((PayloadExecutionResult<int>)result).Payload });
        }

        /// <summary>
        /// Updates camera.
        /// </summary>
        /// <param name="deviceId">The device identifier.</param>
        /// <param name="model">The camera model.</param>
        /// <returns>The updated camera identifier.</returns>
        [HttpPut("devices/{deviceId}/cameras")]
        [Authorize("RequireUsersRole")]
        public async Task<IActionResult> UpdateCameraAsync(int deviceId, [FromBody] CameraModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (!await CheckClientDevice(deviceId))
            {
                return BadRequest();
            }

            model.DeviceId = deviceId;
            var camera = await _cameraService.GetByIdAsync(model.Id);

            if (camera == null || camera.DeviceId != deviceId) throw new InvalidOperationException("Camera not found", null);

            var result = await _cameraService.UpdateAsync(_mapper.ToCamera(camera, model));

            if (!result.IsSuccess) throw new InvalidOperationException(result.ToString(), null);

            return Ok(new { Id = model.Id });
        }

        /// <summary>
        /// Deletes camera.
        /// </summary>
        /// <param name="deviceId">The device identifier.</param>
        /// <param name="id">The camera identifier.</param>
        /// <returns>The deleted camera identifier.</returns>
        [HttpDelete("devices/{deviceId}/cameras/{id}")]
        [Authorize("RequireUsersRole")]
        public async Task<IActionResult> DeleteCameraAsync(int deviceId, int id)
        {
            if (!await CheckClientDevice(deviceId)) return BadRequest();

            var camera = await _cameraService.GetByIdAsync(id);

            if (camera == null || camera.DeviceId != deviceId) throw new InvalidOperationException("Camera not found", null);

            var result = await _cameraService.DeleteAsync(id);

            if (!result.IsSuccess) throw new InvalidOperationException(result.ToString(), null);

            return Ok(new { Id = id });
        }

        /// <summary>
        /// Gets device detected cameras.
        /// </summary>
        /// <param name="deviceId">The device identifier.</param>
        /// <returns>The device camera collection.</returns>
        [HttpGet("devices/{deviceId}/detectedcameras")]
        [Authorize]
        public async Task<IActionResult> GetDeviceDetectedCamerasAsync([FromRoute] int deviceId, [FromQuery] PageRequest pageRequest)
        {
            var user = await _userManager.GetUserAsync(User);
            var role = (await _userManager.GetRolesAsync(user)).Single();

            if (role != UserRoles.Administrators)
                if (!await CheckClientDevice(deviceId))
                    return BadRequest();

            var (Query, Total) = await _detectedCameraService.GetDetectedCamerasAsync(deviceId, pageRequest);

            return Ok(new PageResponse<DetectedCameraModel>()
            {
                Items = Query,
                Total = Total
            });
        }

        /// <summary>
        /// Gets detected camera.
        /// </summary>
        /// <param name="deviceId">The device identifier.</param>
        /// <param name="id">The camera identifier.</param>
        /// <returns>The camera.</returns>
        [HttpGet("devices/{deviceId}/detectedcameras/{id}")]
        [Authorize("RequireUsersRole")]
        public async Task<IActionResult> GetDetectedCameraByIdAsync(int deviceId, int id)
        {
            if (!await CheckClientDevice(deviceId)) return BadRequest();

            var camera = await _detectedCameraService.GetDetectedCameraByIdAsync(id);

            return Ok(camera);
        }

        [HttpPost("devices/{deviceId}/detectedcameras/{id}/connecting")]
        [Authorize("RequireUsersRole")]
        public async Task<IActionResult> ConnectingDetectedCameraAsync(int deviceId, int id, [FromBody] ConnectingDetectedCameraModel model)
        {
            if (!await CheckClientDevice(deviceId)) return BadRequest();

            model.Id = id;

            await _detectedCameraService.ConnectingDetectedCameraAsync(model);

            return Ok();
        }

        /// <summary>
        /// Deletes detected camera.
        /// </summary>
        /// <param name="deviceId">The device identifier.</param>
        /// <param name="id">The camera identifier.</param>
        /// <returns>The deleted camera identifier.</returns>
        [HttpDelete("devices/{deviceId}/detectedcameras/{id}")]
        [Authorize("RequireUsersRole")]
        public async Task<IActionResult> DeleteDetectedCameraAsync(int deviceId, int id)
        {
            if (!await CheckClientDevice(deviceId)) return BadRequest();

            var camera = await _detectedCameraService.GetDetectedCameraByIdAsync(id);

            if (camera == null || camera.DeviceId != deviceId) throw new InvalidOperationException("Camera not found", null);

            await _detectedCameraService.RemoveDetectedCameraAsync(id);

            return Ok();
        }

        /// <summary>
        /// Gets user subscriber.
        /// </summary>
        /// <returns>The subscriber collection.</returns>
        [HttpGet("subscribers")]
        [Authorize("RequireUsersRole")]
        public async Task<IActionResult> GetClientSubscribersAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user.ClientId == null) return BadRequest();

            var result = await _clientSubscriberService.GetClientSubscribers(user.ClientId.Value);

            return Ok(result);
        }

        /// <summary>
        /// Adds subscriber to the storage.
        /// </summary>
        /// <param name="model">The subscriber model.</param>
        /// <returns>The action result.</returns>
        [HttpPost("subscribers")]
        [Authorize("RequireUsersRole")]
        public async Task<IActionResult> CreateClientSubscriberAsync([FromBody] ClientSubscriberModel model)
        {
            if (model == null || String.IsNullOrWhiteSpace(model.Phone)) return BadRequest();

            var user = await _userManager.GetUserAsync(User);

            if (user.ClientId == null) return BadRequest();

            model.ClientId = user.ClientId.Value;

            var result = await _clientSubscriberService.CreateClientSubscriber(user.ClientId.Value, model);

            return Ok(result);
        }

        /// <summary>
        /// Deletes subscriber.
        /// </summary>
        /// <param name="subscriberId">The subscriber identifier.</param>
        /// <returns>The action result.</returns>
        [HttpDelete("subscribers/{subscriberId}")]
        [Authorize("RequireUsersRole")]
        public async Task<IActionResult> DeleteClientSubscriberAsync(int subscriberId)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user.ClientId == null) return BadRequest();

            var result = await _clientSubscriberService.RemoveClientSubscriber(user.ClientId.Value, subscriberId);

            return Ok(result);
        }

        /// <summary>
        /// Gets subscriber assignments.
        /// </summary>
        /// <param name="subscriberId">The subscriber identifier.</param>
        /// <returns>The subscriber assignment collection.</returns>
        [HttpGet("subscribers/{subscriberId}/assignments")]
        [Authorize("RequireUsersRole")]
        public async Task<IActionResult> GetSubscriberAssignmentsAsync(int subscriberId)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user.ClientId == null) return BadRequest();

            var assignments =
                await _clientSubscriberService.GetSubscriberAssignments(user.ClientId.Value, subscriberId);

            return Ok(assignments);
        }

        /// <summary>
        /// Adds assignment to the storage.
        /// </summary>
        /// <param name="subscriberId">The subscriber identifier.</param>
        /// <param name="model">The assignment model.</param>
        /// <returns>The action result.</returns>
        [HttpPost("subscribers/{subscriberId}/assignments")]
        public async Task<IActionResult> SaveSubscriberAssignmentsAsync(int subscriberId, [FromBody] IEnumerable<AssignmentModel> model)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user.ClientId == null) return BadRequest();

            await _clientSubscriberService.SaveSubscriberAssignments(user.ClientId.Value, subscriberId, model);

            return Ok();
        }

        /// <summary>
        /// Binds device to the user.
        /// </summary>
        /// <param name="token">The device token.</param>
        /// <param name="timeZone"></param>
        /// <returns>The action result.</returns>
        [HttpPost("devices/{token}")]
        [Authorize("RequireUsersRole")]
        public async Task<IExecutionResult> BindDeviceToClientAsync([FromRoute] string token, string timeZone)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentNullException(nameof(token));

            var user = await _userManager.GetUserAsync(User);

            if (user.ClientId == null)
                ExecutionResult.Failed("Authorization error");

            var result = await _deviceService.BindDeviceToClient(token, user.ClientId.Value, timeZone);

            return (result);
        }

        /// <summary>
        /// Updates device name.
        /// </summary>
        /// <param name="deviceId">The device identifier.</param>
        /// <param name="deviceEditNameModel">The device name model.</param>
        /// <returns>The action result.</returns>
        [HttpPost("devices/{deviceId}/editname")]
        [Authorize("RequireUsersRole")]
        public async Task<IActionResult> UpdateDeviceNameAsync([FromRoute] int deviceId, [FromBody] DeviceEditNameModel deviceEditNameModel)
        {
            if (string.IsNullOrWhiteSpace(deviceEditNameModel.NewDeviceName))
                throw new ArgumentNullException(nameof(deviceEditNameModel.NewDeviceName));

            if (!await CheckClientDevice(deviceId))
                return BadRequest();

            var result = await _deviceService.UpdateDeviceName(deviceId, deviceEditNameModel.NewDeviceName);

            return Ok(result);
        }

        /// <summary>
        /// Updates device type.
        /// </summary>
        /// <param name="deviceId">The device identifier.</param>
        /// <param name="deviceEditNameModel">The device type model.</param>
        /// <returns>The action result.</returns>
        [HttpPost("devices/{deviceId}/changetype")]
        [Authorize("RequireUsersRole")]
        public async Task<IActionResult> ChangeDeviceTypeAsync([FromRoute] int deviceId, [FromBody] ChangeDeviceTypeModel changeDeviceTypeModel)
        {
            if (changeDeviceTypeModel == null)
                throw new ArgumentNullException(nameof(changeDeviceTypeModel));

            if (!await CheckClientDevice(deviceId))
                return BadRequest();

            var result = await _deviceService.ChangeDeviceType(deviceId, changeDeviceTypeModel.DeviceType);

            return Ok(result);
        }

        /// <summary>
        /// Updates device time zone.
        /// </summary>
        /// <param name="deviceId">The device identifier.</param>
        /// <param name="model">The device time zone model.</param>
        /// <returns>The action result.</returns>
        [HttpPost("devices/{deviceId}/changetimezone")]
        [Authorize("RequireUsersRole")]
        public async Task<IActionResult> ChangeDeviceTimeZoneAsync([FromRoute] int deviceId, [FromBody] ChangeDeviceTimeZoneModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (!await CheckClientDevice(deviceId))
                return BadRequest();

            await _deviceService.ChangeTimeZone(deviceId, model.TimeZone);

            return Ok();
        }

        [HttpGet("timezones")]
        public IActionResult GetTimeZones()
        {
            var zones = TZConvert.KnownIanaTimeZoneNames
                                 .Where(c => TZConvert.TryGetTimeZoneInfo(c, out _))
                                 .Select(c =>
                                 {
                                     var tz = TZConvert.GetTimeZoneInfo(c);
                                     var s = (tz.BaseUtcOffset >= TimeSpan.Zero ? "+" : "-") + tz.BaseUtcOffset.ToString(@"hh\:mm");
                                     return new { Id = c, Name = $"{c} (UTC{s})" };
                                 })
                                 .OrderBy(c => c.Name)
                                 .ToList();

            return Ok(zones);
        }

        #endregion

        #region PrivateMethods

        private async Task CheckActiveCamerasCount(int deviceId, CameraModel model)
        {
            var device = await _deviceService.GetDeviceById(deviceId);

            if (device.MaxActiveCameras.HasValue && model.IsActive &&
                 await GetActiveCameraCount(deviceId, null) >= device.MaxActiveCameras)
            {
                throw new InvalidOperationException($"Only {device.MaxActiveCameras.Value} active cameras are enabled", null);
            }
        }

        private async Task<bool> CheckClientDevice(int deviceId)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user.ClientId == null && !user.Demo)
                return false;

            var device = await _deviceService.GetDeviceById(deviceId);

            return device != null && (user.Demo || device.ClientId == user.ClientId);
        }

        private Task<int> GetActiveCameraCount(int deviceId, int? id)
        {
            return _cameraService.Cameras.CountAsync(c => c.DeviceId == deviceId && c.IsActive && (id == null || c.Id != id));
        }

        #endregion
    }
}