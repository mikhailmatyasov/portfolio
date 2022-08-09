using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using WeSafe.Services.Client;
using WeSafe.Services.Client.Models;
using Camera = WeSafe.DAL.Entities.Camera;

namespace WeSafe.Web.Core.Controllers
{
    /// <summary>
    /// Represents monitoring operations.
    /// </summary>
    [Route("api/v{version:apiVersion}/[controller]")]
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize("RequireAdministratorsRole")]
    public class MonitoringController : ControllerBase
    {
        private readonly IDeviceService _deviceService;
        private readonly ICameraService _cameraService;
        private readonly IMobileService _mobileService;

        public MonitoringController(IDeviceService deviceService, ICameraService cameraService, IMobileService mobileService)
        {
            _deviceService = deviceService;
            _cameraService = cameraService;
            _mobileService = mobileService;
        }

        /// <summary>
        /// Gets devices.
        /// </summary>
        /// <param name="activatedOnly">The flag that indicates if only activated devices should be included.</param>
        /// <param name="status">The device filter status.</param>
        /// <returns>The device collection.</returns>
        [HttpGet("alldevices")]
        public async Task<IActionResult> GetAllDevices(bool activatedOnly = true, string status = null)
        {
            return Ok(await _deviceService.GetAllDevices(activatedOnly, status));
        }

        /// <summary>
        /// Gets device cameras.
        /// </summary>
        /// <param name="deviceId">The device identifier.</param>
        /// <param name="timeMark">HTe filter that filters cameras by last activity tme.</param>
        /// <param name="activeOnly">The flag that indicates if only active cameras should be included.</param>
        /// <returns>the camera collection.</returns>
        [HttpGet("cameras")]
        public async Task<IActionResult> GetDeviceCameras(int deviceId, DateTimeOffset? timeMark = null, bool activeOnly = false)
        {
            var query = _cameraService.Cameras
                                             .Where(c => c.DeviceId == deviceId && (!activeOnly || c.IsActive))
                                             .OrderBy(c => c.CameraName);

            IEnumerable result;

            if (timeMark.HasValue)
            {
                result = await query.Select(c => new CameraMonitoringModel
                {
                    Id = c.Id,
                    CameraName = c.CameraName,
                    Status = c.Status,
                    NetworkStatus = c.NetworkStatus,
                    HasRecent = c.LastActivityTime > timeMark
                })
                                    .ToListAsync();
            }
            else
            {
                result = await query.Select(c => new CameraMonitoringModel
                {
                    Id = c.Id,
                    CameraName = c.CameraName,
                    Status = c.Status,
                    NetworkStatus = c.NetworkStatus,
                })
                                    .ToListAsync();
            }

            return Ok(result);
        }

        /// <summary>
        /// Updates device status.
        /// </summary>
        /// <param name="model">The update device status model.</param>
        /// <returns>The action result.</returns>
        [HttpPost("devicestatus")]
        public async Task<IActionResult> UpdateDeviceStatus([FromBody] DeviceUpdateStatusModel model)
        {
            await _deviceService.UpdateDeviceStatus(model);

            return Ok();
        }

        /// <summary>
        /// Updates camera status.
        /// </summary>
        /// <param name="model">The update camera status model.</param>
        /// <returns>The action result.</returns>
        [HttpPost("camerastatus")]
        public async Task<IActionResult> UpdateCameraStatus([FromBody] CameraUpdateStatusModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            var camera = await _cameraService.GetByIdAsync(model.Id);

            if (camera == null) return BadRequest("Camera not found");

            camera.Status = model.Status;

            var result = await _cameraService.UpdateAsync(camera);

            if (!result.IsSuccess) return BadRequest(result.ToString());

            return Ok();
        }

        /// <summary>
        /// Sends notification for updated device status.
        /// </summary>
        /// <param name="model">The update device status model.</param>
        /// <returns>The action result.</returns>
        [HttpPost("statuschanged")]
        public async Task<IActionResult> StatusChanged([FromBody] DeviceUpdateStatusModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            await _mobileService.SendStatusChangedNotification(model.Id);

            return Ok();
        }
    }
}