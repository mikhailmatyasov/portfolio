using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeSafe.DAL.Entities;
using WeSafe.Services;
using WeSafe.Services.Client;
using WeSafe.Services.Client.Models;
using WeSafe.Services.Extensions;
using WeSafe.Shared;
using WeSafe.Shared.Enumerations;
using WeSafe.Web.Core.Models;

namespace WeSafe.Web.Core.Controllers
{
    /// <summary>
    /// Represents devices operations.
    /// </summary>
    [Route("api/v{version:apiVersion}/[controller]")]
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize("RequireAdministratorsRole")]
    public class DevicesController : ControllerBase
    {
        private readonly IDeviceService _deviceService;
        private readonly UserManager<User> _userManager;
        private readonly ICameraService _cameraService;
        private readonly CameraMapper _mapper;
        private readonly IAuthTokenGenerator _authToken;

        public DevicesController(IDeviceService deviceService, UserManager<User> userManager, ICameraService cameraService,
            CameraMapper mapper, IAuthTokenGenerator generator)
        {
            _deviceService = deviceService;
            _userManager = userManager;
            _cameraService = cameraService;
            _mapper = mapper;
            _authToken = generator;
        }

        /// <summary>
        /// Gets devices.
        /// </summary>
        /// <param name="skip">The number of skipped devices for pagination.</param>
        /// <param name="take">The number of taken devices for pagination.</param>
        /// <param name="sort">The sorted field indicator for filtering devices.</param>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="search">The search text.</param>
        /// <returns>The device collection.</returns>

        [HttpGet]
        public async Task<IActionResult> GetDevicesAsync ([FromQuery] DeviceFilterModel filter)
        {
            var result = await _deviceService.GetDevices(new DeviceRequest
            {
                Skip = filter.Skip,
                Take = filter.Take,
                SortBy = PageRequest.ParseSort(filter.Sort),
                ClientId = filter.ClientId,
                SearchText = filter.Search,
                FilterBy = filter.FilterBy == null ? DeviceRequest.FilterType.None : (DeviceRequest.FilterType)filter.FilterBy
            });

            return Ok(result);
        }

        /// <summary>
        /// Gets device.
        /// </summary>
        /// <param name="id">The device identifier.</param>
        /// <returns>The device.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDeviceByIdAsync(int id)
        {
            var device = await _deviceService.GetDeviceById(id);

            if (device == null) return Ok(null);

            return Ok(device);
        }

        /// <summary>
        /// Gets device cameras.
        /// </summary>
        /// <param name="id">The device identifier.</param>
        /// <param name="active">Include only active cameras.</param>
        /// <returns>The camera collection.</returns>
        [HttpGet("{id}/cameras")]
        public async Task<IActionResult> GetDeviceCamerasAsync(int id, bool active = false)
        {
            var device = await _deviceService.GetDeviceById(id);
            var cameras = await _cameraService.Cameras
                                              .Where(c => c.DeviceId == id && (!active || c.IsActive))
                                              .OrderBy(c => c.CameraName)
                                              //.Select(_mapper.Projection)
                                              .ToListAsync();
            var result = new List<CameraModel>();

            foreach ( var camera in cameras )
            {
                var model = _mapper.ToCameraModel(camera);

                model.IsActiveScheduler = camera.IsActiveSchedule(DateTime.UtcNow, device.TimeZone);

                result.Add(model);
            }

            return Ok(result);
        }

        /// <summary>
        /// Adds device to the storage.
        /// </summary>
        /// <param name="model">The device model.</param>
        /// <returns>The action result.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateDeviceAsync([FromBody] DeviceModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var userId = _userManager.GetUserId(User);

            model.CreatedBy = userId;
            model.ActivationDate = null;

            if (model.AssemblingDate == null)
                model.AssemblingDate = DateTimeOffset.UtcNow;

            var result = await _deviceService.CreateDevice(model);

            return Ok(result);
        }

        /// <summary>
        /// Updates device.
        /// </summary>
        /// <param name="model">The device model.</param>
        /// <returns>The action result.</returns>
        [HttpPut]
        public async Task<IActionResult> UpdateDeviceAsync([FromBody] DeviceModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            var result = await _deviceService.UpdateDevice(model);

            return Ok(result);
        }

        /// <summary>
        /// Deletes device.
        /// </summary>
        /// <param name="deviceId">The device identifier.</param>
        /// <returns>The action result.</returns>
        [HttpDelete("{deviceId}")]
        public async Task<IActionResult> DeleteDeviceAsync(int deviceId)
        {
            var result = await _deviceService.RemoveDevice(deviceId);

            return Ok(result);
        }

        /// <summary>
        /// Deactivates device.
        /// </summary>
        /// <param name="deviceId">The device identifier.</param>
        /// <returns>The action result.</returns>
        [HttpPost("{deviceId}/deactivate")]
        public async Task<IActionResult> DeactivateDeviceAsync(int deviceId)
        {
            var result = await _deviceService.DeactivateDevice(deviceId);

            return Ok(result);
        }

        /// <summary>
        /// Resets device authorization token.
        /// </summary>
        /// <param name="deviceId">The device identifier.</param>
        /// <returns>The action result.</returns>
        [HttpPost("{deviceId}/resetauth")]
        public async Task<IActionResult> ResetAuthAsync(int deviceId)
        {
            await _deviceService.ResetAuthToken(deviceId);

            return Ok();
        }

        [Authorize]
        [AllowAnonymous]
        [HttpGet("deviceTypes")]
        public IActionResult GetDeviceTypes()
        {
            return Ok(Enum.GetValues(typeof(DeviceType))
                .Cast<DeviceType>()
                .ToDictionary(t => (int)t, t => t.ToString()));
        }

        [HttpGet("{deviceId}/create-api-token")]
        public async Task<IActionResult> CreateApiToken(int deviceId)
        {
            var device = await _deviceService.GetDeviceById(deviceId);

            if ( device == null )
            {
                return BadRequest("Device not found");
            }

            var auth = _authToken.CreateToken(new TokenRequest(device.MACAddress, device.Name, DateTime.UtcNow.Add(TimeSpan.FromDays(365)))
            {
                Role = "Devices"
            });

            return Ok(auth);
        }
    }
}